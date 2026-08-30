# Diagramas de Arquitetura — Tech Challenge Fase 3

## 1. Diagrama de Componentes (Visão de Nuvem)

Contempla a visão completa da infraestrutura AWS: API Gateway, Lambda Authorizer, EKS, RDS e Monitoramento.

```mermaid
graph TB
    subgraph Internet
        CLIENT["🌐 Cliente / Browser"]
    end

    subgraph AWS["☁️ AWS Cloud (sa-east-1)"]
        subgraph Edge["Edge Layer"]
            APIGW["API Gateway (HTTP API)"]
        end

        subgraph Serverless["Serverless"]
            LAMBDA["Lambda Authorizer<br/>(Valida CPF → JWT)"]
        end

        subgraph VPC["VPC 10.0.0.0/16"]
            subgraph PublicSubnets["Subnets Públicas"]
                NLB["Network Load Balancer<br/>(Internal)"]
            end

            subgraph EKS["Amazon EKS (eks-techchallenge)"]
                subgraph NS["Namespace: techchallenge"]
                    SVC["Service<br/>(api-service)"]
                    DEP["Deployment<br/>(api — .NET 8)"]
                    HPA["HPA<br/>(1-5 réplicas)"]
                    CM["ConfigMap<br/>(api-config)"]
                    SEC["Secret<br/>(api-secret)"]
                end
                METRICS["Metrics Server"]
            end

            subgraph DBLayer["Banco de Dados Gerenciado"]
                RDS["Amazon RDS<br/>(PostgreSQL 16)"]
            end
        end

        subgraph Monitoring["Observabilidade"]
            DD["Datadog / New Relic<br/>(APM + Logs + Métricas)"]
        end

        subgraph CICD["CI/CD (GitHub Actions)"]
            GH_APP["CI/CD App<br/>(ci.yml + cd.yaml)"]
            GH_K8S["CI/CD Infra K8s<br/>(pr.yml + deploy.yml)"]
            GH_DB["CI/CD Infra DB"]
            GH_LAMBDA["CI/CD Lambda"]
        end

        DHUB["Docker Hub<br/>(techchallenge-api)"]
    end

    CLIENT -->|"HTTPS"| APIGW
    APIGW -->|"Authorize"| LAMBDA
    LAMBDA -->|"Consulta CPF"| RDS
    APIGW -->|"VPC Link"| NLB
    NLB --> SVC
    SVC --> DEP
    HPA -.->|"escala"| DEP
    METRICS -.->|"métricas CPU/Mem"| HPA
    DEP -->|"ConnectionString"| RDS
    DEP -.->|"envFrom"| CM
    DEP -.->|"envFrom"| SEC
    DD -.->|"coleta traces/logs/métricas"| DEP
    DD -.->|"coleta métricas K8s"| EKS
    GH_APP -->|"push image"| DHUB
    GH_APP -->|"kubectl apply"| EKS

    classDef aws fill:#FF9900,stroke:#232F3E,color:#232F3E
    classDef k8s fill:#326CE5,stroke:#fff,color:#fff
    classDef serverless fill:#D86613,stroke:#232F3E,color:#fff
    classDef db fill:#3B48CC,stroke:#fff,color:#fff
    classDef monitor fill:#632CA6,stroke:#fff,color:#fff
    classDef cicd fill:#24292E,stroke:#fff,color:#fff

    class APIGW aws
    class LAMBDA serverless
    class NLB aws
    class RDS db
    class SVC,DEP,HPA,CM,SEC,METRICS k8s
    class DD monitor
    class GH_APP,GH_K8S,GH_DB,GH_LAMBDA,DHUB cicd
```

### Legenda de Repositórios

| Repositório | Responsabilidade | Componentes Provisionados |
|---|---|---|
| `tech-challenge-infra-k8s` | IaC Kubernetes (Terraform) | VPC, IAM, EKS, API Gateway, VPC Link |
| `tech-challenge-infra-db` | IaC Banco de Dados (Terraform) | RDS PostgreSQL |
| `tech-challenge-auth-serverless` | Lambda Serverless | Lambda Authorizer |
| `fase1-tech-challenge` | Aplicação .NET + Manifests K8s | Deployment, Service, HPA, ConfigMap, Secret, CI/CD |

---

## 2. Diagrama de Sequência — Autenticação (CPF → JWT)

Fluxo completo de quando um cliente se autentica com CPF via API Gateway e Lambda Authorizer.

```mermaid
sequenceDiagram
    autonumber
    participant C as 🌐 Cliente
    participant GW as API Gateway
    participant L as Lambda Authorizer
    participant DB as RDS PostgreSQL
    participant API as API .NET (EKS)

    Note over C,API: Fluxo de Autenticação com CPF

    C->>GW: POST /api/v1/autenticacao/login<br/>{ cpf: "123.456.789-00" }
    GW->>L: Invoca Lambda Authorizer
    L->>DB: SELECT * FROM clientes WHERE cpf = ?
    
    alt CPF encontrado e ativo
        DB-->>L: Cliente válido
        L-->>L: Gera token JWT<br/>(claims: cpf, role, exp)
        L-->>GW: Allow + JWT token
        GW-->>C: 200 OK { token: "eyJhbG..." }
    else CPF não encontrado
        DB-->>L: Nenhum registro
        L-->>GW: Deny (401)
        GW-->>C: 401 Unauthorized
    end

    Note over C,API: Consumo de Rota Protegida

    C->>GW: GET /api/v1/ordens-servico<br/>Authorization: Bearer eyJhbG...
    GW->>L: Valida JWT
    L-->>GW: Allow (token válido)
    GW->>API: Forward request via VPC Link → NLB
    API->>DB: SELECT * FROM ordens_servico
    DB-->>API: Dados das OS
    API-->>GW: 200 OK [{ id, status, ... }]
    GW-->>C: 200 OK [{ id, status, ... }]
```

---

## 3. Diagrama de Sequência — Abertura de Ordem de Serviço

Fluxo completo desde a autenticação até a criação da OS e suas transições de estado.

```mermaid
sequenceDiagram
    autonumber
    participant C as 🌐 Cliente (Autenticado)
    participant GW as API Gateway
    participant L as Lambda Authorizer
    participant API as API .NET (EKS)
    participant SVC as OrdemServicoService
    participant DB as RDS PostgreSQL

    Note over C,DB: Pré-condição: Cliente já possui JWT válido

    C->>GW: POST /api/v1/ordens-servico<br/>Authorization: Bearer {jwt}<br/>{ clienteId, veiculoId, ... }
    GW->>L: Valida JWT
    L-->>GW: Allow (Role: Administrador)
    GW->>API: Forward via VPC Link

    API->>SVC: Criar(request)
    SVC->>DB: INSERT INTO ordens_servico<br/>(status: "Aberta")
    DB-->>SVC: OS criada (id: guid)
    SVC-->>API: { id, clienteNotificado }
    API-->>GW: 201 Created
    GW-->>C: 201 Created { id, clienteNotificado }

    Note over C,DB: Fluxo de estados da OS

    C->>GW: PATCH /api/v1/ordens-servico/{id}/iniciar-diagnostico
    GW->>API: Forward (JWT válido)
    API->>SVC: IniciarDiagnostico(id)
    SVC->>DB: UPDATE status = "EmDiagnostico"
    DB-->>SVC: OK
    SVC-->>API: OS atualizada
    API-->>GW: 200 OK
    GW-->>C: 200 OK

    C->>GW: PATCH /{id}/finalizar-diagnostico
    GW->>API: Forward
    API->>DB: UPDATE status = "DiagnosticoConcluido"
    API-->>C: 200 OK

    C->>GW: PATCH /{id}/aprovar-orcamento?aprovado=true
    GW->>API: Forward
    API->>DB: UPDATE status = "EmExecucao"
    API-->>C: 200 OK

    C->>GW: PATCH /{id}/servicos/{svcId}/finalizar
    GW->>API: Forward
    API->>DB: UPDATE status = "Finalizado"
    API-->>C: 204 No Content

    C->>GW: PATCH /{id}/confirmar-entrega
    GW->>API: Forward
    API->>DB: UPDATE status = "Entregue"
    API-->>C: 200 OK
```

---

## 4. Mapa de Componentes por Repositório

```mermaid
graph LR
    subgraph R1["📦 tech-challenge-infra-k8s"]
        M1["modules/networking<br/>(VPC, Subnets, SG, IGW)"]
        M2["modules/iam<br/>(Roles, Policies)"]
        M3["modules/eks<br/>(Cluster, Node Group)"]
        M4["modules/api-gateway<br/>(HTTP API, VPC Link, Authorizer)"]
    end

    subgraph R2["📦 tech-challenge-infra-db"]
        M5["modules/rds<br/>(PostgreSQL 16)"]
    end

    subgraph R3["📦 tech-challenge-auth-serverless"]
        M6["Lambda Function<br/>(Valida CPF, Gera JWT)"]
    end

    subgraph R4["📦 fase1-tech-challenge"]
        M7["src/ — API .NET 8<br/>(Controllers, Services, Domain)"]
        M8["k8s/ — Manifests<br/>(Deployment, Service, HPA)"]
        M9[".github/workflows<br/>(CI: build+test, CD: deploy)"]
    end

    M1 -->|"vpc_id, subnet_ids"| M3
    M2 -->|"role ARNs"| M3
    M1 -->|"vpc_id, subnet_ids"| M4
    M1 -->|"vpc_id, sg_id"| M5
    M6 -->|"lambda_arn"| M4
    M3 -->|"EKS endpoint"| M8
    M5 -->|"RDS endpoint"| M8

    classDef infra fill:#FF9900,stroke:#232F3E,color:#232F3E
    classDef db fill:#3B48CC,stroke:#fff,color:#fff
    classDef lambda fill:#D86613,stroke:#232F3E,color:#fff
    classDef app fill:#326CE5,stroke:#fff,color:#fff

    class M1,M2,M3,M4 infra
    class M5 db
    class M6 lambda
    class M7,M8,M9 app
```