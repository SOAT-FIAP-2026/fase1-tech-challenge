# Central de Documentação de Arquitetura — Tech Challenge Fase 3

Esta pasta contém toda a documentação arquitetural, especificações técnicas, diagramas, solicitações de comentários (RFCs) e registros de decisões de arquitetura (ADRs) do projeto da **Oficina Mecânica**.

---

## 🗺️ Índice Geral de Documentação

### 1. 📊 Diagramas de Arquitetura
* 🧩 [**Diagrama de Componentes (Visão de Nuvem)**](./component-diagram.md)  
  *Visão completa da infraestrutura na AWS: API Gateway, Lambda Authorizer, EKS, RDS PostgreSQL, VPC, Subnets e Observabilidade (Datadog/New Relic).*
* 🔄 [**Diagrama de Sequência**](./sequence-diagram.md)  
  *Fluxo detalhado passo-a-passo da Autenticação via CPF (emissão de JWT) e do ciclo de vida das Ordens de Serviço (abertura, diagnóstico, aprovação de orçamento, execução e entrega).*
* 🗃️ [**Modelo de Dados Relacional e Diagrama ER**](./database-model.md)  
  *Diagrama ER completo com todas as 11 entidades do domínio, explicação dos relacionamentos, justificativa formal da escolha do banco e máquina de estados da Ordem de Serviço.*

---

### 2. 📄 Request for Comments (RFCs) — Decisões Técnicas Relevantes
* ☁️ [**RFC-001: Escolha do Provedor de Nuvem (AWS)**](./rfcs/RFC-001-cloud-provider-choice.md)  
  *Avaliação comparativa entre AWS, GCP e Azure. Justificativa para a escolha da AWS (EKS, RDS, API Gateway, Lambda).*
* 🗄️ [**RFC-002: Escolha do Banco de Dados Gerenciado (AWS RDS PostgreSQL 16)**](./rfcs/RFC-002-managed-database-choice.md)  
  *Análise de consistência ACID, garantias relacionais, alta disponibilidade e comparação entre bancos relacionais e NoSQL.*
* 🔐 [**RFC-003: Estratégia de Autenticação Serverless (AWS Lambda + JWT + API Gateway)**](./rfcs/RFC-003-serverless-authentication-strategy.md)  
  *Proposta de arquitetura de autenticação desacoplada por CPF na borda (Edge Layer) via AWS Lambda Authorizer e emissão de JWT.*

---

### 3. 🏛️ Architecture Decision Records (ADRs) — Decisões Arquiteturais Permanentes
* 🔌 [**ADR-001: Padrão de Comunicação entre Componentes (Síncrono REST/HTTP)**](./adrs/ADR-001-communication-pattern.md)  
  *Definição da comunicação síncrona REST/HTTP via API Gateway e VPC Link para APIs da aplicação e preparação para eventos assíncronos.*
* 📈 [**ADR-002: Estratégia de Auto-scaling da Aplicação via Kubernetes HPA**](./adrs/ADR-002-kubernetes-hpa-autoscaling.md)  
  *Regras de dimensionamento automático de 1 a 5 réplicas baseado em utilização de CPU (70%) e Memória (80%).*

---

### 4. 📋 Especificação Oficial do Projeto
* 📌 [**Especificação do Tech Challenge - Fase 3**](./tech-challenge-spec/TC_FASE3.md)  
  *Requisitos obrigatórios, objetivos, regras dos 4 repositórios, observabilidade e critérios de entrega.*

---

## 🏗️ Matriz de Repositórios da Solução

| Repositório | Responsabilidade | Tecnologias |
|---|---|---|
| 📦 `tech-challenge-auth-serverless` | Função Serverless de Autenticação | AWS Lambda, Node.js / C#, JWT, CI/CD |
| 📦 `tech-challenge-infra-k8s` | Infraestrutura do Cluster e Rede | Terraform, AWS EKS, VPC, API Gateway |
| 📦 `tech-challenge-infra-db` | Infraestrutura do Banco de Dados Gerenciado | Terraform, AWS RDS PostgreSQL 16 |
| 📦 `fase1-tech-challenge` | Aplicação Backend Principal + Manifestos K8s | .NET 8, C#, Clean Arch, K8s Manifests, CI/CD |
