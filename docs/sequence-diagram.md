# Diagramas de Arquitetura — Tech Challenge Fase 3

## 1. Diagrama de Sequência — Autenticação (CPF → JWT)

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

    C->>GW: POST /api/v1/autenticacao/Login<br/>{ cpf: "123.456.789-00" }
    GW->>L: Invoca Lambda Authorizer
    L->>DB: SELECT * FROM clientes WHERE cpf_cnpj = ?
    
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

## 2. Diagrama de Sequência — Abertura e Ciclo de Vida da Ordem de Serviço

Fluxo completo desde a criação da OS até a entrega, passando por todas as transições de estado reais do sistema.

> **Referência**: Os status abaixo correspondem exatamente aos códigos definidos em `StatusOS` (`src/Fiap.TechChallenge.Domain/Entities/StatusOrdemServico.cs`).

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
    SVC->>DB: INSERT INTO ordens_servico<br/>(status: RECEBIDA)
    DB-->>SVC: OS criada (id: guid)
    SVC-->>API: { id, status }
    API-->>GW: 201 Created
    GW-->>C: 201 Created { id, status: "Recebida" }

    Note over C,DB: Fluxo de Diagnóstico

    C->>GW: PATCH /api/v1/ordens-servico/{id}/iniciar-diagnostico
    GW->>API: Forward (JWT válido)
    API->>SVC: IniciarDiagnostico(id)
    SVC->>DB: UPDATE status = EM_DIAGNOSTICO
    DB-->>SVC: OK
    SVC-->>API: OS atualizada
    API-->>GW: 200 OK
    GW-->>C: 200 OK

    C->>GW: PATCH /api/v1/ordens-servico/{id}/finalizar-diagnostico
    GW->>API: Forward
    API->>SVC: FinalizarDiagnostico(id)
    SVC->>DB: UPDATE status = AGUARDANDO_APROVACAO
    DB-->>SVC: OK
    API-->>GW: 200 OK
    GW-->>C: 200 OK

    Note over C,DB: Fluxo de Aprovação do Orçamento

    C->>GW: PATCH /api/v1/ordens-servico/{id}/aprovar-orcamento
    GW->>API: Forward

    alt Orçamento aprovado
        API->>SVC: AprovarOrcamento(id)
        SVC->>DB: UPDATE status = ORCAMENTO_APROVADO
        DB-->>SVC: OK
        API-->>GW: 200 OK
        GW-->>C: 200 OK { status: "Orcamento Aprovado" }
    else Orçamento reprovado
        API->>SVC: ReprovarOrcamento(id)
        SVC->>DB: UPDATE status = ORCAMENTO_REPROVADO
        DB-->>SVC: OK
        API-->>GW: 200 OK
        GW-->>C: 200 OK { status: "Orcamento Reprovado" }
    end

    Note over C,DB: Fluxo de Execução e Entrega

    C->>GW: PATCH /api/v1/ordens-servico/{id}/servicos/{svcId}/iniciar
    GW->>API: Forward
    API->>DB: UPDATE item_servico SET data_hora_inicio = NOW()
    API-->>GW: 200 OK
    GW-->>C: 200 OK

    C->>GW: PATCH /api/v1/ordens-servico/{id}/servicos/{svcId}/finalizar
    GW->>API: Forward
    API->>DB: UPDATE item_servico SET data_hora_fim = NOW()
    Note right of API: Quando todos os serviços<br/>estão finalizados,<br/>status → FINALIZADA
    API-->>GW: 200 OK
    GW-->>C: 200 OK

    C->>GW: PATCH /api/v1/ordens-servico/{id}/confirmar-entrega
    GW->>API: Forward
    API->>SVC: ConfirmarEntrega(id)
    SVC->>DB: UPDATE status = ENTREGUE,<br/>data_conclusao = NOW()
    DB-->>SVC: OK
    API-->>GW: 200 OK
    GW-->>C: 200 OK { status: "Entregue" }
```

---

## 3. Mapa de Componentes por Repositório

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