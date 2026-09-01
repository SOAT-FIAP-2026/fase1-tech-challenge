# Diagrama de Componentes — Visão de Nuvem (Tech Challenge Fase 3)

Contempla a visão completa de infraestrutura AWS e local: API Gateway, Lambda Authorizer, EKS, RDS, CI/CD e Monitoramento.

> **Requisito**: TC Fase 3 — *"Diagrama de Componentes (com a visão de nuvem, APIs, banco e monitoramento)."*

---

## 1. Diagrama de Componentes — Infraestrutura AWS

```mermaid
graph TB
    subgraph Internet
        CLIENT["🌐 Cliente / Browser / Postman"]
    end

    subgraph AWS["☁️ AWS Cloud (sa-east-1)"]
        subgraph Edge["Edge Layer"]
            APIGW["API Gateway<br/>(HTTP API)"]
        end

        subgraph Serverless["Serverless"]
            LAMBDA["Lambda Authorizer<br/>(Valida CPF → Gera JWT)"]
        end

        subgraph VPC["VPC 10.0.0.0/16"]
            subgraph PublicSubnets["Subnets Públicas (2 AZs)"]
                LB["AWS Load Balancer<br/>(provisionado pelo EKS<br/>via Service type: LoadBalancer)"]
            end

            subgraph EKS["Amazon EKS (eks-techchallenge)"]
                subgraph NS["Namespace: techchallenge"]
                    ING["Ingress<br/>(api-ingress — Nginx)"]
                    SVC["Service<br/>(api-service — port 80)"]
                    DEP["Deployment<br/>(api — .NET 8<br/>gabrielnetto94/techchallenge-api)"]
                    HPA["HPA<br/>(api-hpa — 1 a 5 réplicas)"]
                    CM["ConfigMap<br/>(api-config)"]
                    SEC["Secret<br/>(api-secret)"]
                end
                METRICS["Metrics Server"]
            end

            subgraph DBLayer["Banco de Dados Gerenciado"]
                RDS["Amazon RDS<br/>(PostgreSQL 16<br/>Subnet Group Multi-AZ)"]
            end
        end

        subgraph Monitoring["Observabilidade"]
            DD["Datadog / New Relic<br/>(APM + Logs + Métricas K8s)"]
        end

        subgraph CICD["CI/CD (GitHub Actions)"]
            GH_APP["CI/CD App<br/>(ci.yml + cd.yaml)"]
            GH_K8S["CI/CD Infra K8s<br/>(pr.yml + deploy.yml)"]
            GH_DB["CI/CD Infra DB<br/>(pr.yml + deploy.yml)"]
            GH_LAMBDA["CI/CD Lambda"]
        end

        DHUB["Docker Hub<br/>(gabrielnetto94/techchallenge-api)"]
    end

    CLIENT -->|"HTTPS"| APIGW
    APIGW -->|"Authorize"| LAMBDA
    LAMBDA -->|"Consulta CPF (port 5432)"| RDS
    APIGW -->|"VPC Link"| LB
    LB --> ING
    ING --> SVC
    SVC --> DEP
    HPA -.->|"escala réplicas"| DEP
    METRICS -.->|"métricas CPU/Mem"| HPA
    DEP -->|"ConnectionString (port 5432)"| RDS
    DEP -.->|"envFrom"| CM
    DEP -.->|"envFrom"| SEC
    DD -.->|"coleta traces/logs/métricas"| DEP
    DD -.->|"coleta métricas K8s"| EKS
    GH_APP -->|"docker push"| DHUB
    GH_APP -->|"kubectl apply"| EKS

    classDef aws fill:#FF9900,stroke:#232F3E,color:#232F3E
    classDef k8s fill:#326CE5,stroke:#fff,color:#fff
    classDef serverless fill:#D86613,stroke:#232F3E,color:#fff
    classDef db fill:#3B48CC,stroke:#fff,color:#fff
    classDef monitor fill:#632CA6,stroke:#fff,color:#fff
    classDef cicd fill:#24292E,stroke:#fff,color:#fff

    class APIGW,LB aws
    class LAMBDA serverless
    class RDS db
    class ING,SVC,DEP,HPA,CM,SEC,METRICS k8s
    class DD monitor
    class GH_APP,GH_K8S,GH_DB,GH_LAMBDA,DHUB cicd
```

---

## 2. Legenda de Repositórios

| Repositório | Responsabilidade | Componentes Provisionados | Pipelines CI/CD |
|---|---|---|---|
| `tech-challenge-infra-k8s` | IaC Kubernetes (Terraform) | VPC, Subnets, SG, IGW, IAM Roles, EKS Cluster, Node Group, API Gateway, VPC Link | `pr.yml` + `deploy.yml` |
| `tech-challenge-infra-db` | IaC Banco de Dados (Terraform) | RDS PostgreSQL 16, DB Subnet Group, Security Group RDS | `pr.yml` + `deploy.yml` |
| `tech-challenge-auth-serverless` | Função Serverless de Autenticação | Lambda Function (Valida CPF, Gera JWT) | CI/CD Lambda |
| `fase1-tech-challenge` | Aplicação .NET 8 + Manifestos K8s | Deployment, Service, HPA, Ingress, ConfigMap, Secret, Namespace | `ci.yml` + `cd.yaml` + `sonar.yaml` |

---

## 3. Detalhamento dos Componentes Kubernetes (Namespace `techchallenge`)

| Recurso | Arquivo | Descrição |
|---|---|---|
| `Namespace` | `k8s/base/namespace.yaml` | Namespace isolado `techchallenge` para todos os recursos da aplicação |
| `Deployment` | `k8s/base/deployment.yaml` | Pod `.NET 8` com imagem `gabrielnetto94/techchallenge-api`, probes de liveness/readiness/startup, resource limits (100m-500m CPU, 128Mi-512Mi Mem) |
| `Service` | `k8s/overlays/aws/service.yaml` | `type: LoadBalancer` na AWS (provisiona ALB/NLB automaticamente via AWS Cloud Controller) |
| `Ingress` | `k8s/base/ingress.yaml` | Roteamento via Nginx Ingress Controller para `api.techchallenge.local` |
| `HPA` | `k8s/base/hpa.yaml` | Auto-scaling de 1 a 5 réplicas (CPU ≥ 50%, Memória ≥ 95%) |
| `ConfigMap` | `k8s/base/configmap.yaml` | Variáveis não sensíveis: `ASPNETCORE_URLS`, `Jwt__Issuer`, `Jwt__Audience` |
| `Secret` | `k8s/overlays/aws/secrets.yaml` | ConnectionString e credenciais (valores específicos por ambiente) |
| `Metrics Server` | `k8s/metrics-server/` | Coleta de métricas de CPU e Memória dos Pods para o HPA |

---

## 4. Fluxo de Dados Resumido

```
Cliente → API Gateway → Lambda Authorizer → (Valida CPF no RDS) → JWT
          ↓
     VPC Link → Load Balancer → Ingress → Service → Deployment (Pods .NET 8) → RDS PostgreSQL
                                                         ↑
                                                    HPA ← Metrics Server
                                                         ↑
                                                  Datadog/New Relic (APM)
```

---