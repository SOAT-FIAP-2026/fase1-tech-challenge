# Sistema Integrado de Atendimento e Execução de Serviços - Oficina Mecânica

> **Tech Challenge - FIAP SOAT** | MVP (Fase 1)

Esta é uma API desenvolvida em **.NET 8**, seguindo os princípios da **Clean Architecture** e **Domain-Driven Design (DDD)**, com foco em gestão de ordens de serviço, clientes e peças para uma oficina mecânica de médio porte.

## 🎯 Desafio

A oficina mecânica enfrenta desafios operacionais significativos:

- ❌ Erros na priorização dos atendimentos
- ❌ Falhas no controle de peças e insumos
- ❌ Dificuldade em acompanhar o status dos serviços
- ❌ Perda de histórico de clientes e veículos
- ❌ Ineficiência no fluxo de orçamentos e autorizações

**Solução:** Um sistema integrado que permite aos clientes acompanhar em tempo real o andamento do serviço, autorizar reparos adicionais via API e garantir uma gestão interna eficiente e segura.


### 1️⃣ Criação da Ordem de Serviço (OS)

- ✅ Identificação do cliente por CPF/CNPJ
- ✅ Cadastro de veículo (placa, marca, modelo, ano)
- ✅ Inclusão dos serviços solicitados
- ✅ Inclusão de peças e insumos necessários
- ✅ Geração automática de orçamento
- ✅ Envio do orçamento ao cliente para aprovação

### 2️⃣ Acompanhamento da OS

**Status da Ordem de Serviço:**
- 📍 Recebida
- 🔍 Em diagnóstico
- ⏳ Aguardando aprovação
- 🔧 Em execução
- ✔️ Finalizada
- 🚗 Entregue

**Recursos:**
- ✅ Alteração automática de status conforme ações
- ✅ Consulta por parte do cliente via API
- ✅ Acompanhamento em tempo real

### 3️⃣ Gestão Administrativa

- ✅ CRUD de clientes
- ✅ CRUD de veículos
- ✅ CRUD de serviços
- ✅ CRUD de peças e insumos (com controle de estoque)
- ✅ Listagem e detalhamento de ordens de serviço
- ✅ Monitoramento do tempo médio de execução

### 🔐 Segurança e Qualidade

- ✅ Autenticação JWT para APIs administrativas
- ✅ Validação de dados sensíveis (CPF/CNPJ, placa)
- ✅ Testes unitários e de integração (cobertura mínima 80%)

## 🏗️ Arquitetura

O projeto está estruturado nas seguintes camadas, garantindo baixo acoplamento e alta coesão:

*   **Domain (`Fiap.TechChallenge.Domain`)**: Contém as entidades (ex: `Usuario`), interfaces, enums, exceções e regras de negócio essenciais. É o núcleo do sistema e não possui dependências de outras camadas.
*   **Application (`Fiap.TechChallenge.Application`)**: Contém os casos de uso da aplicação, DTOs e orquestra a lógica de negócio utilizando as entidades do domínio.
*   **Infrastructure (`Fiap.TechChallenge.Infrastructure`)**: Implementação de acesso a dados, repositórios e outras preocupações de infraestrutura.
*   **External (`Fiap.TechChallenge.External`)**: Camada designada para integrações com serviços e APIs de terceiros.
*   **Api (`Fiap.TechChallenge.Api`)**: O ponto de entrada (Entrypoint) da aplicação. Contém os Controllers, Middlewares, configurações do Swagger e a configuração da injeção de dependência (Startup/Program).


## 🛠️ Stack

*   **.NET 8** com C# 12
*   **Clean Architecture** + **Domain-Driven Design (DDD)**
*   **JWT** para autenticação
*   **Swagger/OpenAPI** para documentação
*   **Testes Unitários e de Integração**
*   **Docker** e **docker-compose** para containerização
*   **Banco de dados** Postgres
*   **ReportGenerator** para análise de cobertura de testes

## 🚀 Como Executar o Projeto

### Pré-requisitos
* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/products/docker-desktop) (opcional)
* [Docker Compose](https://docs.docker.com/compose/install/) (opcional)

### Setup inicial (na raiz do repositório)

```bash
dotnet restore
dotnet build fase1-tech-challenge.sln
```

### Rodar API (modo normal)

```bash
dotnet run --project src/Fiap.TechChallenge.Api/Fiap.TechChallenge.Api.csproj
```

### Rodar API com Hot Reload

```bash
dotnet watch --project src/Fiap.TechChallenge.Api/Fiap.TechChallenge.Api.csproj run
```

Alternativa, se estiver dentro de src/Fiap.TechChallenge.Api:

```bash
dotnet watch run
```

### Endpoints locais

* Swagger UI: https://localhost:5001/swagger (ou porta definida no launchSettings)
* HTTP local: http://localhost:5000

### Execução com Docker Compose

```bash
docker compose up -d --build
```

Se seu ambiente ainda usar o comando legado:

```bash
docker-compose up -d --build
```

## 🧪 Testes e Cobertura

### Rodar todos os testes da solução

```bash
dotnet test fase1-tech-challenge.sln
```

### Rodar apenas o projeto de testes

```bash
dotnet test tests/Fiap.TechChallenge.Tests/Fiap.TechChallenge.Domain.Tests.csproj
```

### Rodar teste por filtro

```bash
dotnet test tests/Fiap.TechChallenge.Tests/Fiap.TechChallenge.Domain.Tests.csproj --filter "NomeDoTeste"
```

### Coleta de cobertura

```bash
dotnet test tests/Fiap.TechChallenge.Tests/Fiap.TechChallenge.Domain.Tests.csproj --collect:"XPlat Code Coverage"
```

### Gerar relatório HTML de cobertura

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"tests/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Abra o arquivo coveragereport/index.html no navegador.


## 📁 Estrutura do Projeto

```
Fiap.TechChallenge/
├── src/
│   ├── Fiap.TechChallenge.Domain/          # Camada de Domínio (DDD)
│   ├── Fiap.TechChallenge.Application/     # Camada de Aplicação
│   ├── Fiap.TechChallenge.Infrastructure/  # Camada de Infraestrutura
│   ├── Fiap.TechChallenge.External/        # Camada de Integrações Externas
│   └── Fiap.TechChallenge.Api/             # Camada de API (Controllers, Swagger)
├── tests/
│   └── Fiap.TechChallenge.Domain.Tests/    # Testes Unitários e de Integração
├── Dockerfile                               # Configuração para build do container
├── docker-compose.yml                       # Orquestração de containers
└── README.md                                # Este arquivo
```


## 🚀 Entregáveis da Fase 1

- ✅ Código-fonte no repositório
- ✅ APIs conforme requisitos
- ✅ Dockerfile e docker-compose configurados
- ✅ README.md completo com instruções
- ✅ Testes automatizados com cobertura mínima 80%
- ✅ Análise de vulnerabilidades
- ⏳ Vídeo de demonstração (até 15 minutos)
- ⏳ Documentação DDD (Miro)

## 📖 Como Usar a API

### Exemplo 1: Cadastro e Login

```bash
# Cadastrar novo usuário
curl -X POST http://localhost:5000/api/v1/autenticacao/cadastrar \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "email": "joao@email.com",
    "senha": "SenhaSegura123!"
  }'

# Fazer login
curl -X POST http://localhost:5000/api/v1/autenticacao/login \
  -H "Content-Type: application/json" \
  -d '{
    "login": "joao@email.com",
    "senha": "SenhaSegura123!"
  }'
```

### Exemplo 2: Criar Ordem de Serviço

```bash
curl -X POST http://localhost:5000/api/v1/ordens-servico \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {JWT_TOKEN}" \
  -d '{
    "clienteCpfCnpj": "12345678901234",
    "veiculoPlaca": "ABC1234",
    "servicos": [
      {
        "servicoId": "uuid-do-servico",
        "quantidade": 1
      }
    ],
    "pecas": [
      {
        "pecaId": "uuid-da-peca",
        "quantidade": 2
      }
    ]
  }'
```
