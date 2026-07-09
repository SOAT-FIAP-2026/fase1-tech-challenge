# Infraestrutura — Terraform (AWS)

Provisiona o ambiente cloud do TechChallenge na AWS (`sa-east-1`) com:
- **VPC** pública com 2 subnets em AZs distintas
- **EKS** (Kubernetes 1.35) com node group `t3.micro`
- **RDS** PostgreSQL 16 Free Tier (`db.t3.micro`, 20 GB, single-AZ)
- **IAM** Roles para cluster e node group
- **S3** backend para estado remoto do Terraform

## Estrutura

```
infra/
├── main.tf                  # Root: chama os módulos
├── variables.tf             # Variáveis do root
├── outputs.tf               # Outputs agregados
├── providers.tf             # Provider AWS + backend S3
├── terraform.tfvars.example # Template de variáveis sensíveis
└── modules/
    ├── networking/          # VPC, subnets, IGW, route table, SG
    ├── iam/                 # IAM roles e policies (cluster + node group)
    ├── eks/                 # EKS cluster, node group, access entries
    └── rds/                 # RDS PostgreSQL, subnet group, SG
```

## Pré-requisitos

- [Terraform](https://developer.hashicorp.com/terraform/install) >= 1.5.0
- AWS CLI configurado (`aws configure`)
- IAM User `terraform-user` existente na conta AWS
- Bucket S3 `fiap-soat-techchallenge-backend` criado na `sa-east-1`

## Uso

```bash
# 1. Copie e preencha as variáveis sensíveis
cp terraform.tfvars.example terraform.tfvars
# edite terraform.tfvars com a senha do RDS

# Alternativa: exporte via variável de ambiente
export TF_VAR_rds_password="minha_senha_segura"

# 2. Inicialize (baixa providers e configura backend S3)
terraform init

# 3. Visualize o plano de execução
terraform plan

# 4. Aplique a infraestrutura
terraform apply

# 5. Destrua toda a infraestrutura
terraform destroy
```

## Comandos úteis

```bash
# Reconfigurar backend (ex: mudança de bucket)
terraform init -reconfigure

# Destruir apenas um recurso específico
terraform apply -destroy -target="module.rds"

# Ver outputs (incluindo connection string)
terraform output rds_connection_string

# Validar sintaxe dos arquivos
terraform validate
```