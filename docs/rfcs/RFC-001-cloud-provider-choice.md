# RFC-001: Escolha do Provedor de Nuvem (Cloud Provider)

* **Status**: Aprovado
* **Data**: 2026-08-31
* **Autor**: Equipe de Arquitetura (SOAT - Tech Challenge)
* **Domínio**: Infraestrutura & Cloud

---

## 1. Resumo Executivo

Este Request for Comments (RFC) analisa e justifica formalmente a escolha da **Amazon Web Services (AWS)** como o provedor de nuvem principal para a hospedagem e orquestração de todos os componentes da solução da Oficina Mecânica (Tech Challenge - Fase 3).

---

## 2. Contexto e Problema

Com a expansão da oficina para múltiplas unidades e o crescimento constante da base de clientes, a infraestrutura precisa atender aos seguintes requisitos fundamentais:
* **Alta Disponibilidade e Escalabilidade**: Capacidade de absorver picos de acesso no cadastro e acompanhamento de Ordens de Serviço (OS).
* **Segurança e Isolamento**: Restrição de acessos via VPC, Subnets Privadas e Security Groups.
* **Automação via IaC**: Provisionamento 100% declarativo e reprodutível utilizando Terraform.
* **Orquestração de Containers e Serverless**: Suporte nativo a Kubernetes gerenciado e funções Serverless escaláveis.

---

## 3. Opções Avaliadas

Foram comparados os três principais provedores de nuvem pública do mercado:

| Critério | AWS (Amazon Web Services) | GCP (Google Cloud Platform) | Azure (Microsoft Azure) |
|---|---|---|---|
| **Kubernetes Gerenciado** | **EKS** (Excelente estabilidade, suporte robusto a VPC CNI e IAM Roles for Service Accounts - IRSA). | **GKE** (Altamente automatizado, porém com custos ligeiramente superiores no plano padrão). | **AKS** (Boa integração com ecossistema Microsoft, mas menor flexibilidade de networking). |
| **Banco de Dados Gerenciado** | **RDS PostgreSQL** (Multi-AZ maduro, backups automatizados e fácil ajuste de I/O). | **Cloud SQL** (Excelente desempenho, mas ecossistema Terraform levemente menos flexível). | **Azure Database for PostgreSQL** (Bom suporte, porém com latências regionais maiores para o Brasil). |
| **Serverless & Edge** | **AWS Lambda + API Gateway** (Integração nativa de Custom Authorizer com altíssimo desempenho e escalabilidade a zero). | **Cloud Functions + Apigee** (Apigee possui alto custo para o cenário do projeto). | **Azure Functions + API Management** (API Management possui modelo de cobrança por hora elevado). |
| **Suporte IaC (Terraform)** | **Líder absoluto** em maturidade e módulos da comunidade (`terraform-aws-modules`). | Muito bom, porém menos módulos padronizados pela comunidade. | Bom, mas com frequentes *breaking changes* nos providers. |

---

## 4. Análise Técnica e Justificativa da Escolha

A **AWS** foi selecionada como a plataforma oficial da solução com base nas seguintes razões:

1. **Maturidade do Amazon EKS**:
   O EKS fornece um plano de controle gerenciado altamente resiliente com distribuição Multi-AZ. A integração nativa com o **AWS VPC CNI** permite que Pods recebam IPs reais da VPC, simplificando o roteamento interno para o banco de dados RDS.

2. **Integração Nativa API Gateway + Lambda Authorizer**:
   A camada Serverless de autenticação (Lambda) integra-se nativamente com o AWS HTTP API Gateway, proporcionando autorização na borda (Edge Layer) com baixíssima latência e custo por requisição.

3. **Padronização com Terraform**:
   A vasta disponibilidade de módulos Terraform mantidos pela comunidade (`terraform-aws-modules/vpc`, `terraform-aws-modules/eks`, `terraform-aws-modules/rds`) permitiu uma estrutura de código limpa e desacoplada em repositórios independentes.

4. **Emulação Local e Testes (Floci / LocalStack)**:
   A AWS possui o ecossistema mais maduro de emulação local para desenvolvimento e testes automatizados em ambientes CI/CD desprovidos de credenciais reais de nuvem.

---

## 5. Arquitetura de Nuvem Adotada

A infraestrutura na AWS está distribuída na região `sa-east-1` (São Paulo) em conformidade com o seguinte modelo:

```
┌────────────────────────────────────────────────────────────────────────┐
│                         AWS Cloud (sa-east-1)                          │
│                                                                        │
│  ┌───────────────────────┐              ┌───────────────────────────┐  │
│  │   AWS API Gateway     │───Authorize─>│  AWS Lambda Authorizer    │  │
│  │   (HTTP API - Edge)   │              │  (Valida CPF / Gera JWT)  │  │
│  └───────────┬───────────┘              └─────────────┬─────────────┘  │
│              │                                        │                │
│           VPC Link                                 Consulta            │
│              │                                        │                │
│  ┌───────────▼────────────────────────────────────────▼─────────────┐  │
│  │ VPC (10.0.0.0/16)                                                │  │
│  │                                                                  │  │
│  │  ┌───────────────────────────┐      ┌─────────────────────────┐  │  │
│  │  │ Subnets Públicas          │      │ Subnets Privadas        │  │  │
│  │  │  - Internal NLB           │      │  - Amazon EKS Cluster   │  │  │
│  │  └───────────────────────────┘      │  - Amazon RDS (Postgres)│  │  │
│  │                                     └─────────────────────────┘  │  │
│  └──────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 6. Considerações de Custo e Riscos

* **Lock-in do Provedor**: Mitigado através da conteinerização total da aplicação (.NET 8 executando em Kubernetes padrão) e uso de banco PostgreSQL relacional padrão.
* **Otimização de Custos**:
  * Ambientes de Desenvolvimento/Homologação utilizam instâncias `t3.small` / `t4g.small` e RDS Single-AZ.
  * O API Gateway e Lambda cobram exclusivamente por execução (Pay-as-you-go).

---

## 7. Decisão Final

* **Provedor Escolhido**: Amazon Web Services (AWS)
* **Status**: Aprovado para execução no Tech Challenge Fase 3.
