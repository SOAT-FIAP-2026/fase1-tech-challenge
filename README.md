# Sistema Integrado de Atendimento e Execução de Serviços - Oficina Mecânica

Esta é a aplicação backend principal desenvolvida em **.NET 8** (C# 12), seguindo os princípios da **Clean Architecture** e **Domain-Driven Design (DDD)**, com foco em gestão de ordens de serviço, clientes, veículos, estoque e serviços para uma oficina mecânica de médio/grande porte.

A aplicação executa em contêineres orquestrados por **Kubernetes (AWS EKS)** com dimensionamento horizontal automático (**HPA**), integrada a uma arquitetura moderna em nuvem com **autenticação serverless por CPF**, **banco de dados gerenciado (AWS RDS PostgreSQL 16)** e **observabilidade ponta a ponta**.

---

## 🎯 Tech Challenge - Fase 3

Na **Fase 3**, o sistema foi elevado a um patamar de **operação corporativa**, garantindo alta disponibilidade, segurança de ponta a ponta, isolamento de responsabilidades, automação completa de infraestrutura e visibilidade operacional em tempo real:

- 🔐 **Autenticação Serverless na Borda (Edge Layer):**
  - Implementação de **AWS API Gateway** para roteamento e controle de acesso.
  - Criação de **Function Serverless (AWS Lambda)** em .NET 8 para validação algorítmica de CPF, consulta de existência/status do cliente no PostgreSQL (RDS) e emissão de tokens **JWT (HMAC-SHA256)** para acesso às rotas protegidas.
- 📦 **Segregação em 4 Repositórios Independentes:**
  - Separação estrita de domínios com pipelines dedicadas de CI/CD, branch protection na `main` e permissão concedida ao usuário auditor `soat-architecture`.
- ☁️ **Infraestrutura como Código (Terraform) na AWS:**
  - Provisionamento 100% automatizado de VPC multi-AZ, cluster **AWS EKS**, **Application Load Balancer (ALB)** público integrado via Target Groups e NodePorts, e banco de dados **AWS RDS PostgreSQL 16**.
- 🔭 **Monitoramento e Observabilidade Corporativa:**
  - Instrumentação nativa com **OpenTelemetry (OTLP)** exportando traces para o **Grafana Tempo**.
  - Logs estruturados em formato **JSON** com correlação ponta a ponta via `X-Correlation-ID` propagado e consultável no **Grafana Loki**.
  - Coleta contínua de métricas com **Prometheus**, sondas de uptime com **Blackbox Exporter** e alertas automatizados (**PrometheusRule** / **Alertmanager** / **CloudWatch Alarms**).
  - Dashboards executivos no **Grafana** cobrindo volume diário de OS, tempo médio por etapa (Diagnóstico, Execução, Finalização) e falhas de integrações.
- 🗃️ **Modelagem de Dados e Consistência Relacional:**
  - Banco relacional com 11 entidades, integridade referencial ACID, diagrama ER completo e máquina de estados da Ordem de Serviço com 9 status estritos.

---

## 📦 Matriz de Repositórios da Solução (Fase 3)

O projeto foi desacoplado em **quatro repositórios independentes**, cada um com seu pipeline de CI/CD automatizado:

| Repositório | Responsabilidade | Tecnologias Principais | CI/CD |
|---|---|---|---|
| 🔐 [**lambda-auth-function**](https://github.com/SOAT-FIAP-2026/lambda-auth-function) | Função Serverless de Autenticação por CPF, consulta ao banco e emissão de JWT | .NET 8, C#, Dapper, Npgsql, JWT, CloudWatch Logs | GitHub Actions (Build, Testes, SonarQube e Terraform Apply) |
| ☸️ [**tech-challenge-infra-k8s**](https://github.com/SOAT-FIAP-2026/soat-infra) | Infraestrutura de Cluster EKS, Rede VPC, Application Load Balancer (ALB) e Observabilidade | Terraform, AWS EKS, ALB, Helm (Prometheus, Grafana, Loki, Tempo, OTel) | GitHub Actions (Terraform Plan, Validations e Apply) |
| 🗄️ [**tech-challenge-infra-db**](https://github.com/SOAT-FIAP-2026/soat-db) | Infraestrutura do Banco de Dados Gerenciado desacoplado e alarmes | Terraform, AWS RDS PostgreSQL 16, Security Groups, CloudWatch Alarms | GitHub Actions (Terraform Plan e Apply) |
| 🚀 [**fase1-tech-challenge**](https://github.com/SOAT-FIAP-2026/fase1-tech-challenge) *(Este repo)* | Aplicação Principal Backend, Manifestos Kubernetes (HPA, NodePort), Testes e Observabilidade | .NET 8, Clean Arch, DDD, EF Core, OpenTelemetry, K8s Manifests | GitHub Actions (Build, Testes Unitários + Testcontainers, SonarCloud e Deploy EKS) |

> 🛡️ **Políticas de Governança e Proteção:**
> - Branch `main` protegida em todos os repositórios (sem commits diretos, merge exclusivamente via Pull Request com validação do pipeline).
> - Usuário `soat-architecture` adicionado com permissão de colaborador em todos os 4 repositórios.

---

## 🔗 Entregáveis Oficiais

- 🎥 **Vídeo de Demonstração (YouTube):** [Assista à demonstração completa](https://www.youtube.com/watch?v=kkmQnF3PrXk) *(Autenticação por CPF, execução do CI/CD com deploy automatizado, consumo de APIs protegidas, dashboards de observabilidade ao vivo, logs JSON correlacionados e distributed traces no Tempo)*.
- 📚 **Swagger UI da API:**
  - **AWS Produção (ALB):** [http://fiap-soat-terraform-alb-847091771.sa-east-1.elb.amazonaws.com/swagger](http://fiap-soat-terraform-alb-847091771.sa-east-1.elb.amazonaws.com/swagger)
  - **Ambiente Local:** [http://localhost:8080/swagger](http://localhost:8080/swagger)
- 📮 **Collections Postman:**
  - API Principal: [`docs/postman/techchallenge-api.postman_collection.json`](./docs/postman/techchallenge-api.postman_collection.json) *(Fluxo completo da Ordem de Serviço, clientes, veículos, peças e estoque)*.
  - Lambda Auth: [`lambda-auth-function postman collection`](https://github.com/SOAT-FIAP-2026/lambda-auth-function/blob/main/docs/postman/lambda-auth.postman_collection.json).
- 📊 **Painel de Observabilidade (Grafana UI):**
  - **AWS Produção:** [http://fiap-soat-terraform-alb-847091771.sa-east-1.elb.amazonaws.com:3000](http://fiap-soat-terraform-alb-847091771.sa-east-1.elb.amazonaws.com:3000) *(Usuário: `admin` / Senha: `admin`)*
  - **Ambiente Local:** [http://localhost:3000](http://localhost:3000)
- 📖 **Central de Documentação de Arquitetura:**
  - 🧩 [Diagrama de Componentes na Nuvem (AWS)](./docs/component-diagram.md)
  - 🔄 [Diagrama de Sequência (Autenticação CPF + Ciclo da OS)](./docs/sequence-diagram.md)
  - 🗃️ [Modelo de Dados Relacional e Diagrama ER](./docs/database-model.md) *(11 entidades, máquina de estados da OS e justificativa técnica)*
  - 📜 [RFC-001: Escolha do Provedor de Nuvem (AWS)](./docs/rfcs/RFC-001-cloud-provider-choice.md)
  - 📜 [RFC-002: Escolha do Banco de Dados Gerenciado (PostgreSQL 16)](./docs/rfcs/RFC-002-managed-database-choice.md)
  - 📜 [RFC-003: Estratégia de Autenticação Serverless (Lambda + JWT)](./docs/rfcs/RFC-003-serverless-authentication-strategy.md)
  - 🏛️ [ADR-001: Padrão de Comunicação entre Componentes (REST/HTTP)](./docs/adrs/ADR-001-communication-pattern.md)
  - 🏛️ [ADR-002: Auto-scaling de Pods via Kubernetes HPA](./docs/adrs/ADR-002-kubernetes-hpa-autoscaling.md)
  - 🏛️ [ADR-003: Stack Corporativa de Observabilidade](./docs/adrs/ADR-003-observability-stack.md)
  - 🔭 [Guia Operacional de Observabilidade e Métricas](./docs/observability.md)
  - 📋 [Especificação Oficial do Tech Challenge Fase 3](./docs/tech-challenge-spec/TC_FASE3.md)

---

## 🏗️ Arquitetura e Infraestrutura

Abaixo está o desenho da arquitetura integrada implementada na nuvem AWS para a Fase 3:

```mermaid
graph TB
    subgraph Clients["🌐 Clientes & Operação"]
        USER["Cliente / Mecânico / Postman"]
    end

    subgraph Edge["🔐 Borda & Autenticação (Edge Layer)"]
        APIGW["AWS API Gateway<br/>(HTTP API)"]
        LAMBDA["AWS Lambda Auth<br/>(.NET 8 Serverless)<br/>Valida CPF → Emite JWT"]
    end

    subgraph AWS["☁️ AWS Cloud (sa-east-1)"]
        subgraph Net["VPC & Roteamento (Terraform)"]
            ALB["AWS Application Load Balancer (ALB)<br/>fiap-soat-terraform-alb-*.elb.amazonaws.com"]
        end

        subgraph EKS["Cluster Kubernetes (AWS EKS - 3 nós t3.small)"]
            subgraph NS_App["Namespace: techchallenge"]
                SVC_API["Service: api-service<br/>NodePort: 30080"]
                POD1["API Pod 1 (.NET 8)"]
                POD2["API Pod 2 (.NET 8)"]
                HPA["HPA (1 a 5 pods)<br/>CPU 70% / Memória 80%"]
                HPA -.-> |Auto-scaling| POD1
            end

            subgraph NS_Mon["Namespace: monitoring (Observabilidade)"]
                SVC_GRAF["Service: grafana<br/>NodePort: 30300"]
                GRAFANA["Grafana (:3000)<br/>Dashboards & Alertas"]
                PROM["Prometheus (:9090)<br/>Métricas & Scraping"]
                OTEL["OpenTelemetry Collector (:4318)<br/>Recepção OTLP HTTP"]
                TEMPO["Grafana Tempo (:3200)<br/>Distributed Traces"]
                LOKI["Grafana Loki (:3100)<br/>Logs Estruturados JSON"]
                BBOX["Blackbox Exporter<br/>Probes de Uptime"]
            end
        end

        subgraph DB["🗄️ Dados Gerenciados"]
            RDS[("AWS RDS PostgreSQL 16<br/>techchallengedb<br/>Multi-AZ / Subnets Privadas")]
        end
    end

    subgraph CICD["🤖 Pipelines CI/CD (GitHub Actions)"]
        GHA_APP["fase1-tech-challenge:<br/>Build + Testes + Testcontainers + SonarCloud + EKS Deploy"]
        GHA_LAMBDA["lambda-auth-function:<br/>Build + Testes + Deploy Terraform"]
        GHA_K8S["tech-challenge-infra-k8s:<br/>Terraform EKS + ALB + Monitoring"]
        GHA_DB["tech-challenge-infra-db:<br/>Terraform RDS + CloudWatch"]
    end

    %% Fluxo de Autenticação
    USER -->|1. POST /auth (CPF)| APIGW
    APIGW -->|2. Invoca| LAMBDA
    LAMBDA -->|3. Valida cliente ativo| RDS
    LAMBDA -->|4. Retorna JWT Bearer| USER

    %% Fluxo de Acesso à Aplicação
    USER -->|5. HTTP com JWT Bearer e X-Correlation-ID| ALB
    ALB -->|Roteia :80 para NodePort 30080| SVC_API
    ALB -->|Roteia :3000 para NodePort 30300| SVC_GRAF
    SVC_API --> POD1
    SVC_API --> POD2
    SVC_GRAF --> GRAFANA

    %% Comunicação Pods com Banco e Observabilidade
    POD1 -->|Leitura / Escrita (EF Core)| RDS
    POD2 -->|Leitura / Escrita (EF Core)| RDS
    POD1 -.->|Traces OTLP /v1/traces| OTEL
    POD1 -.->|Logs JSON stdout| LOKI
    PROM -.->|Scrape /metrics| POD1

    %% Observabilidade interna
    OTEL --> TEMPO
    GRAFANA --> PROM
    GRAFANA --> LOKI
    GRAFANA --> TEMPO
    BBOX -.->|Health probe /health/ready| SVC_API
```

### Arquitetura de Software (.NET 8 — Clean Architecture & DDD)

O repositório principal está estritamente estruturado em camadas desacopladas:

- **`Fiap.TechChallenge.Domain`**: Camada central e agnóstica a frameworks. Contém as entidades de negócio (`Cliente`, `Veiculo`, `OrdemServico`, `ItemServico`, `ItemPeca`, `PecaInsumo`, etc.), Value Objects, enums de status, regras de negócio e interfaces de repositório.
- **`Fiap.TechChallenge.Application`**: Casos de uso (Use Cases) do sistema, comandos, consultas (CQRS pattern), DTOs e mapeamentos.
- **`Fiap.TechChallenge.Infrastructure`**: Implementação da persistência com **Entity Framework Core**, mapeamentos relacionais, migrations, seeds de banco e repositórios.
- **`Fiap.TechChallenge.External`**: Gateways de comunicação com serviços e provedores externos.
- **`Fiap.TechChallenge.Api`**: Ponto de entrada REST contendo Controllers, Middlewares de autenticação JWT, Middleware de correlação (`X-Correlation-ID`), documentação Swagger OpenAPI e exportação de telemetria OpenTelemetry.

---

## 🛠️ Stack Tecnológica

| Camada | Tecnologias |
|---|---|
| **Backend & Core** | .NET 8 (C# 12), ASP.NET Core Web API, Clean Architecture, Domain-Driven Design (DDD) |
| **Persistência & Dados** | Entity Framework Core 8, PostgreSQL 16, Dapper (na Lambda) |
| **Autenticação & Segurança** | JWT (JSON Web Tokens HMAC-SHA256), AWS API Gateway, AWS Lambda Serverless |
| **Conteinerização & K8s** | Docker, Kubernetes (AWS EKS v1.35), HPA (Horizontal Pod Autoscaler), NodePort |
| **Infraestrutura como Código** | Terraform (>= 1.5.0), AWS Provider, Helm Provider |
| **Nuvem (AWS)** | Amazon EKS, AWS RDS PostgreSQL, Application Load Balancer (ALB), VPC, IAM, CloudWatch |
| **Observabilidade** | OpenTelemetry .NET SDK, Prometheus, Grafana, Grafana Loki, Grafana Tempo, Blackbox Exporter |
| **Testes & Qualidade** | xUnit, Moq, FluentAssertions, Testcontainers PostgreSQL, Coverlet, ReportGenerator, SonarCloud |
| **CI/CD** | GitHub Actions com deploys automatizados e proteção de branches |

---

## 🔐 Autenticação, Rotas Protegidas e Consumo via CPF

Conforme os requisitos da Fase 3, as rotas sensíveis do sistema (gestão de clientes, abertura e alteração de ordens de serviço, aprovação de orçamentos) exigem autenticação baseada em token JWT emitido após validação do CPF do cliente.

### 1. Fluxo de Autenticação Serverless

1. O cliente envia uma requisição `POST` para o endpoint de autenticação com o CPF cadastrado:
   - **Endpoint AWS (API Gateway):** `https://mwmjdq6xbe.execute-api.sa-east-1.amazonaws.com/auth` (ou rota do gateway `/auth/token`)
   - **Payload:**
     ```json
     {
       "cpf": "282.027.830-20"
     }
     ```
2. A **Function Serverless (AWS Lambda)** valida o formato e dígitos verificadores do CPF, realiza a consulta no PostgreSQL RDS para assegurar que o cliente existe e não está inativo/bloqueado, e devolve o token JWT:
   ```json
   {
     "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "token_type": "Bearer",
     "expires_in": 3600
   }
   ```
3. O cliente consome as rotas protegidas da API passando o token no header `Authorization: Bearer <token>` e opcionalmente o identificador de correlação `X-Correlation-ID`.

### 2. Clientes Disponíveis para Testes (Seed Inicial)

O banco de dados já inicializa com clientes válidos para testes de autenticação:

| Cliente | CPF | Status no Banco | Acesso |
|---|---|---|---|
| **João Silva** | `282.027.830-20` | Ativo | Autorizado (Gera JWT) |
| **Maria Oliveira** | `312.408.510-81` | Ativo | Autorizado (Gera JWT) |

> 🔑 **Usuário Administrador (Fallback):**
> - **E-mail:** `admin@techchallenge.com`
> - **Senha:** `Admin@123`

---

## 🔭 Monitoramento e Observabilidade

O ambiente conta com uma stack corporativa de observabilidade unificada integrada via OpenTelemetry:

### 1. Métricas de Negócio e Operacionais

| Métrica | Tipo | Descrição |
|---|---|---|
| `techchallenge.orders.created` | Counter | Volume diário de ordens de serviço geradas |
| `techchallenge.orders.status.duration` | Histogram | Tempo médio de execução por status (*Diagnóstico*, *Execução*, *Finalização*) |
| `techchallenge.orders.processing.failures` | Counter | Falhas no processamento de ordens de serviço (dispara alertas) |
| `techchallenge.integrations.requests` | Counter | Volume de chamadas a integrações externas |
| `techchallenge.integrations.errors` | Counter | Taxa de falhas de comunicação externa |
| `techchallenge.http.request.duration` | Histogram | Latência das requisições HTTP da API |
| `container_cpu_usage_seconds_total` | Counter | Consumo de CPU dos pods no cluster EKS |
| `container_memory_working_set_bytes` | Gauge | Consumo de memória dos pods no cluster EKS |
| `probe_success` | Gauge | Uptime e health checks monitorados via Blackbox Exporter |

### 2. Logs Estruturados em JSON & Rastreabilidade Ponta a Ponta

- Todas as saídas de log são serializadas em formato **JSON** estruturado (`timestamp`, `level`, `message`, `correlation_id`, `trace_id`, `span_id`).
- O header `X-Correlation-ID` é recebido no API Gateway / Lambda e propagado pela API .NET em todas as camadas e chamadas externas.
- No **Grafana**, o painel de logs do **Loki** possui a variável de filtro **Correlation ID** no topo: ao filtrar por um ID, visualizam-se todas as linhas do fluxo daquela requisição.
- Informações sensíveis (como dígitos completos de CPF e senhas) **nunca** são expostas em texto puro nos logs.

### 3. Distributed Tracing com OpenTelemetry e Grafana Tempo

- A API .NET exporta traces distribuídos via protocolo **OTLP/HTTP** (`http://opentelemetry-collector.monitoring.svc.cluster.local:4318/v1/traces`).
- O **Grafana Tempo** ingere os spans permitindo inspeção detalhada do tempo gasto em middlewares, chamadas de banco (Entity Framework Core) e integrações.

### 4. Dashboards do Grafana e Alertas

O Grafana já possui dashboards provisionados para visualização imediata:
1. **Tech Challenge - Executivo de Ordens de Serviço:** Volume diário de ordens criadas, status atual e taxas de conclusão.
2. **Tech Challenge - Tempos de Ciclo:** Tempo médio gasto nas etapas de Diagnóstico, Execução e Entrega.
3. **Tech Challenge - Saúde da Aplicação e Falhas:** Taxas de erro HTTP 5xx, falhas de processamento e latência p95/p99.
4. **Infraestrutura Kubernetes:** Consumo de CPU/Memória por pod, nós ativos e métricas do HPA.
5. **Alertas Configurados:** Regras no PrometheusRule / Alertmanager disparadas em caso de indisponibilidade da API (`probe_success == 0`) ou pico de falhas de processamento de OS.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop) e [Docker Compose](https://docs.docker.com/compose/install/)
- [AWS CLI v2](https://aws.amazon.com/cli/) autenticado (para ambiente AWS)
- [kubectl](https://kubernetes.io/docs/tasks/tools/) e [Terraform >= 1.5](https://www.terraform.io/)

---

### Opção 1: Execução Local (.NET CLI)

1. Crie o arquivo `.env` na raiz:
   ```bash
   cp .env.example .env
   ```
2. Restaure as dependências e compile:
   ```bash
   dotnet restore fase1-tech-challenge.sln
   dotnet build fase1-tech-challenge.sln
   ```
3. Execute a API:
   ```bash
   dotnet run --project src/Fiap.TechChallenge.Api/Fiap.TechChallenge.Api.csproj
   ```
   Acesse a documentação Swagger em: `http://localhost:8080/swagger`.

---

### Opção 2: Execução com Docker Compose (Ambiente Completo Local)

O `docker-compose.yml` sobe a aplicação, PostgreSQL e toda a stack local de observabilidade (Prometheus, Grafana, Loki, Tempo e Alertmanager):

```bash
docker compose up -d
```

| Serviço | URL | Credenciais |
|---|---|---|
| **Swagger UI (API)** | `http://localhost:8080/swagger` | — |
| **Health Check API** | `http://localhost:8080/health` | — |
| **Métricas Prometheus** | `http://localhost:8080/metrics` | — |
| **Grafana UI** | `http://localhost:3000` | `admin` / `admin` |
| **Prometheus UI** | `http://localhost:9090` | — |
| **Alertmanager UI** | `http://localhost:9093` | — |

---

### Opção 3: Execução em Kubernetes Local (Kind / Minikube)

Para testar os manifestos Kubernetes, HPA e ServiceMonitors em cluster local:

```bash
./k8s/overlays/local/deploy.sh
```

Consulte as instruções detalhadas em [**k8s/README.md**](./k8s/README.md).

---

### Opção 4: Deploy no Ambiente de Produção AWS (EKS + ALB + RDS)

A infraestrutura completa na AWS é orquestrada pelos repositórios de Terraform e pelos manifestos do EKS:

1. **Provisionar Cluster EKS, VPC, ALB e Observabilidade:**
   ```bash
   cd "../tech-challenge-infra-k8s/environments/prod"
   terraform init && terraform apply -auto-approve
   ```
2. **Atualizar kubeconfig do EKS:**
   ```bash
   aws eks update-kubeconfig --name eks-fiap-soat-terraform --region sa-east-1
   ```
3. **Provisionar o Banco de Dados RDS PostgreSQL:**
   ```bash
   cd "../tech-challenge-infra-db/environments/prod"
   terraform init && terraform apply -auto-approve
   ```
4. **Instalar Recursos de Observabilidade no Cluster:**
   ```bash
   cd "../fase1-tech-challenge"
   ./k8s/observability/install.sh
   ```
5. **Realizar o Deploy da Aplicação Backend:**
   ```bash
   ./k8s/overlays/aws/deploy.sh
   ```

O script automatizado lê o endpoint do RDS via AWS CLI, cria os Secrets de conexão e credenciais do Docker Hub, aplica os manifestos do Kubernetes e aguarda os pods ficarem em estado `Ready`.

---

## 🌐 Endpoints do Ambiente de Produção (AWS)

| Serviço | URL Pública / Endpoint | Descrição |
|---|---|---|
| 📖 **Swagger UI** | [http://fiap-soat-terraform-alb-847091771.sa-east-1.elb.amazonaws.com/swagger](http://fiap-soat-terraform-alb-847091771.sa-east-1.elb.amazonaws.com/swagger) | Documentação interativa das APIs da Oficina |
| 🚀 **API Base URL** | `http://fiap-soat-terraform-alb-847091771.sa-east-1.elb.amazonaws.com` | Ponto de entrada das rotas da aplicação |
| 🔐 **API Gateway Auth** | `https://mwmjdq6xbe.execute-api.sa-east-1.amazonaws.com/auth` | Rota serverless para emissão de JWT com CPF |
| 📊 **Grafana UI** | [http://fiap-soat-terraform-alb-847091771.sa-east-1.elb.amazonaws.com:3000](http://fiap-soat-terraform-alb-847091771.sa-east-1.elb.amazonaws.com:3000) | Dashboards de negócio, métricas e traces |
| ❤️ **Health Check** | `http://fiap-soat-terraform-alb-847091771.sa-east-1.elb.amazonaws.com/health/live` | Sonda de integridade da API |

---

## 🧪 Testes, Cobertura e Qualidade de Código

A suíte de testes inclui testes unitários para a camada de domínio e aplicação, além de **testes de integração E2E** executados contra uma instância real de PostgreSQL através de **Testcontainers**.

### 1. Executar a Suíte de Testes

```bash
# Restaurar ferramentas locais (primeira execução)
dotnet tool restore

# Executar todos os testes da solução
dotnet test fase1-tech-challenge.sln
```

### 2. Gerar Relatório de Cobertura de Código (HTML)

```bash
# Executar testes coletando métricas de cobertura Coverlet
dotnet test fase1-tech-challenge.sln --configuration Release --settings coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Gerar relatório HTML amigável
dotnet reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;HtmlSummary"
```

Abra o arquivo `TestResults/CoverageReport/index.html` no navegador para visualizar o mapa de calor da cobertura de código.

---

## 📁 Estrutura do Repositório

```
fase1-tech-challenge/
├── .github/
│   └── workflows/
│       ├── ci.yml                    # Pipeline de CI (Build, Testes, Cobertura)
│       ├── cd.yaml                   # Pipeline de CD (Deploy automatizado no EKS)
│       └── sonar.yaml                # Análise estática de código com SonarCloud
├── docs/                             # Central de Documentação Técnica
│   ├── adrs/                         # Architecture Decision Records (ADR-001 a ADR-003)
│   ├── rfcs/                         # Requests for Comments (RFC-001 a RFC-003)
│   ├── postman/                      # Collections Postman completas da API
│   ├── tech-challenge-spec/          # Especificações oficiais da Fase 3
│   ├── component-diagram.md          # Diagrama de componentes da arquitetura em nuvem
│   ├── sequence-diagram.md           # Diagrama de sequência de auth e ordem de serviço
│   ├── database-model.md             # Diagrama ER e máquina de estados da OS
│   └── observability.md              # Documentação detalhada da stack de observabilidade
├── k8s/                              # Manifestos Kubernetes
│   ├── base/                         # ConfigMaps, Secrets, Deployment, Service, HPA
│   ├── overlays/                     # Ambientes (local e AWS prod com scripts de deploy)
│   └── observability/                # ServiceMonitor, PrometheusRule e Dashboards Grafana
├── src/
│   ├── Fiap.TechChallenge.Domain/    # Entidades de Domínio, Interfaces e Regras DDD
│   ├── Fiap.TechChallenge.Application/ # Casos de Uso, Handlers e DTOs
│   ├── Fiap.TechChallenge.Infrastructure/# EF Core, Repositórios, Migrations e Seeds
│   ├── Fiap.TechChallenge.External/  # Gateways de integração externa
│   └── Fiap.TechChallenge.Api/       # Controllers, Swagger, Middlewares e OpenTelemetry
├── tests/
│   └── Fiap.TechChallenge.Tests/     # Testes Unitários e Integração E2E (Testcontainers)
├── Dockerfile                        # Multi-stage Dockerfile otimizado para .NET 8
├── docker-compose.yml                # Ambiente local completo (API + BD + Observabilidade)
├── dotnet-tools.json                 # Ferramentas locais .NET (ReportGenerator)
└── README.md                         # Documentação principal da aplicação
```

---

## 👥 Autores — Grupo SOAT FIAP

Projeto desenvolvido como parte dos requisitos do **Tech Challenge - Fase 3** da Pós-Graduação em Software Architecture da **FIAP**.
