# Infraestrutura — Terraform (AWS)

Provisiona o ambiente cloud na AWS (`sa-east-1`) usando módulos Terraform:

| Módulo | Recursos |
|--------|----------|
| **networking** | VPC (`10.0.0.0/16`), 2 subnets públicas (AZs distintas), Internet Gateway, Route Table, Security Group |
| **iam** | IAM Roles e policies para EKS cluster e node group |
| **eks** | EKS cluster (K8s 1.35), node group (`t3.micro`), access entries |
| **rds** | RDS PostgreSQL 16 Free Tier (`db.t3.micro`, 20 GB, single-AZ), subnet group, Security Group |

Adicionalmente, o root module cria um **S3 bucket** para backend remoto do estado do Terraform.

## Estrutura

```
infra/
├── main.tf                  # Orquestra os módulos
├── variables.tf             # Variáveis de entrada
├── outputs.tf               # Outputs agregados (endpoint EKS, RDS host, etc.)
├── providers.tf             # Provider AWS + backend S3
├── terraform.tfvars.example # Template de variáveis sensíveis
└── modules/
    ├── networking/          # VPC, subnets, IGW, route table, SG
    ├── iam/                 # IAM roles e policies
    ├── eks/                 # EKS cluster + node group
    └── rds/                 # RDS PostgreSQL
```

## Pré-requisitos

- [Terraform](https://developer.hashicorp.com/terraform/install) >= 1.5.0
- [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html) configurado (`aws configure`)
- IAM User `terraform-user` existente na conta AWS
- Bucket S3 `fiap-soat-techchallenge-backend` criado na `sa-east-1`

## Deploy

```bash
cd infra/

# 1. Configurar variáveis sensíveis
cp terraform.tfvars.example terraform.tfvars
# edite terraform.tfvars com a senha do RDS

# 2. Inicializar (baixa providers + configura backend S3)
terraform init

# 3. Visualizar plano de execução
terraform plan

# 4. Aplicar (cria tudo na AWS — ~15 min)
terraform apply
```

> [!TIP]
> Alternativa ao `terraform.tfvars`: exporte a senha via variável de ambiente:
> ```bash
> export TF_VAR_rds_password="minha_senha_segura"
> ```

## Outputs

Após o `apply`, os seguintes outputs ficam disponíveis:

| Output | Descrição | Uso |
|--------|-----------|-----|
| `eks_cluster_name` | Nome do cluster EKS | `aws eks update-kubeconfig --name <valor>` |
| `eks_cluster_endpoint` | Endpoint do API server | Configuração do kubectl |
| `rds_host` | Hostname do RDS (sem porta) | Connection string nos Secrets K8s |
| `rds_endpoint` | Endpoint completo (`host:port`) | Referência |
| `rds_connection_string` | Connection string .NET completa | Codificar em base64 para o Secret K8s |
| `rds_db_name` | Nome do banco | Referência |

```bash
# Ver todos os outputs
terraform output

# Connection string para usar no K8s Secret
terraform output -raw rds_connection_string
```

## Comandos Úteis

```bash
# Validar sintaxe
terraform validate

# Reconfigurar backend (ex: mudança de bucket)
terraform init -reconfigure

# Destruir recurso específico
terraform apply -destroy -target="module.rds"

# Listar recursos gerenciados
terraform state list

# Destruir TUDO (⚠️ irreversível)
terraform destroy
```

## Variáveis

| Variável | Default | Descrição |
|----------|---------|-----------|
| `default_region` | `sa-east-1` | Região AWS |
| `project_name` | `fiap-soat-terraform` | Prefixo de todos os recursos |
| `cidr_block` | `10.0.0.0/16` | CIDR da VPC |
| `availability_zones` | `[sa-east-1a, sa-east-1b, sa-east-1c]` | AZs disponíveis |
| `instance_types` | `[t3.micro]` | Tipo de instância dos nodes EKS |
| `rds_db_name` | `techchallengedb` | Nome do banco |
| `rds_username` | `postgres` | Usuário master do RDS |
| `rds_password` | — (**obrigatório**) | Senha master do RDS |

## Fluxo Infra → K8s

```
terraform apply
    ↓
terraform output -raw rds_connection_string
    ↓
echo -n "<connection_string>" | base64 -w 0
    ↓
Colar no k8s/overlays/aws/secrets.yaml
    ↓
./k8s/overlays/aws/deploy.sh
```