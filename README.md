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

## ✅ Funcionalidades

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

- **Domain (`Fiap.TechChallenge.Domain`)**: Contém as entidades, interfaces, enums, exceções e regras de negócio essenciais. É o núcleo do sistema e não possui dependências de outras camadas.
- **Application (`Fiap.TechChallenge.Application`)**: Contém os casos de uso da aplicação, DTOs e orquestra a lógica de negócio utilizando as entidades do domínio.
- **Infrastructure (`Fiap.TechChallenge.Infrastructure`)**: Implementação de acesso a dados, repositórios e outras preocupações de infraestrutura.
- **External (`Fiap.TechChallenge.External`)**: Camada designada para integrações com serviços e APIs de terceiros.
- **Api (`Fiap.TechChallenge.Api`)**: Ponto de entrada da aplicação. Contém os Controllers, Middlewares, configurações do Swagger e injeção de dependência.

## 🛠️ Stack

- **.NET 8** com C# 12
- **Clean Architecture** + **Domain-Driven Design (DDD)**
- **xUnit** + **Moq** + **FluentAssertions** para testes
- **coverlet** + **ReportGenerator** para cobertura de testes
- **JWT** para autenticação
- **Swagger/OpenAPI** para documentação
- **PostgreSQL** como banco de dados
- **Docker** e **docker-compose** para containerização

## 🚀 Como Executar o Projeto

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop) e [Docker Compose](https://docs.docker.com/compose/install/) (opcional)

### Setup inicial (na raiz do repositório)

```bash
dotnet restore fase1-tech-challenge.sln
dotnet build fase1-tech-challenge.sln
```

### Rodar a API

```bash
dotnet run --project src/Fiap.TechChallenge.Api/Fiap.TechChallenge.Api.csproj
```

### Rodar a API com Hot Reload

```bash
# Na raiz do repositório
dotnet watch --project src/Fiap.TechChallenge.Api/Fiap.TechChallenge.Api.csproj run

# Ou, se já estiver dentro de src/Fiap.TechChallenge.Api/
dotnet watch run
```

### Endpoints locais

| Serviço | URL |
|---|---|
| Swagger UI | https://localhost:5001/swagger |
| HTTP | http://localhost:5000 |

### Execução com Docker Compose

```bash
docker compose up -d --build
```

> Se seu ambiente ainda usar o comando legado: `docker-compose up -d --build`

## 🧪 Testes e Cobertura

O projeto usa **xUnit** como framework de testes e **coverlet** para coleta de cobertura. O `reportgenerator` está configurado como ferramenta local via `dotnet-tools.json`.

### Pré-requisito: restaurar ferramentas locais

```bash
dotnet tool restore
```

### Rodar todos os testes da solução

```bash
dotnet test fase1-tech-challenge.sln
```

### Rodar apenas o projeto de testes

```bash
dotnet test tests/Fiap.TechChallenge.Tests/Fiap.TechChallenge.Tests.csproj
```

### Rodar testes por filtro (nome ou trait)

```bash
dotnet test tests/Fiap.TechChallenge.Tests/Fiap.TechChallenge.Tests.csproj --filter "NomeDoTeste"
```

### Coletar cobertura e gerar relatório HTML

Execute os comandos abaixo na raiz do repositório:

```bash
# 1. Restaurar ferramentas locais (necessário apenas na primeira vez)
dotnet tool restore

# 2. Rodar os testes coletando a cobertura
dotnet test fase1-tech-challenge.sln --configuration Release --collect:"XPlat Code Coverage" --results-directory ./TestResults

# 3. Gerar o relatório HTML
dotnet reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;HtmlSummary"
```

Abra `TestResults/CoverageReport/index.html` no navegador para visualizar o relatório.

> 💡 Este mesmo fluxo é executado automaticamente pelo pipeline de CI a cada push na branch `main`.

### Como medir a qualidade dos testes

Use tres sinais em conjunto:

- **Resultado da suite**: `dotnet test fase1-tech-challenge.sln`
- **Cobertura de linhas/branches**: relatorio Cobertura + ReportGenerator
- **Cobertura E2E**: testes no namespace `Fiap.TechChallenge.Tests.Api.EndToEnd`, que sobem a API com `WebApplicationFactory`, passam por HTTP real, middleware, JWT, DI, EF Core e seed de dados

Para gerar um resumo rapido em terminal, execute:

```bash
dotnet test fase1-tech-challenge.sln --configuration Release --collect:"XPlat Code Coverage" --results-directory ./TestResults
dotnet reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;TextSummary"
```

Os numeros principais ficam no final da saida do ReportGenerator e no arquivo `TestResults/CoverageReport/Summary.txt`. Para evoluir a base, acompanhe principalmente:

- **Line coverage**: percentual de linhas exercitadas
- **Branch coverage**: percentual de decisoes exercitadas
- **Cenarios E2E criticos**: autenticar, autorizar, criar/consultar dados, fluxo de ordem de servico e erros esperados

## 📁 Estrutura do Projeto

```
fase1-tech-challenge/
├── src/
│   ├── Fiap.TechChallenge.Domain/          # Camada de Domínio (DDD)
│   ├── Fiap.TechChallenge.Application/     # Camada de Aplicação
│   ├── Fiap.TechChallenge.Infrastructure/  # Camada de Infraestrutura
│   ├── Fiap.TechChallenge.External/        # Camada de Integrações Externas
│   └── Fiap.TechChallenge.Api/             # Camada de API (Controllers, Swagger)
├── tests/
│   └── Fiap.TechChallenge.Tests/           # Testes Unitários e de Integração
├── .github/workflows/ci.yml                # Pipeline de CI (GitHub Actions)
├── Dockerfile                              # Configuração para build do container
├── docker-compose.yml                      # Orquestração de containers
├── dotnet-tools.json                       # Ferramentas locais do projeto
└── README.md                               # Este arquivo
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

## 📖 Exemplos de Uso da API

### Cadastro e Login

```bash
# Cadastrar novo usuário
curl -X POST http://localhost:5000/api/v1/autenticacao/cadastrar \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "email": "joao@email.com",
    "senha": "SenhaSegura123!"
  }'

# Fazer login e obter o JWT
curl -X POST http://localhost:5000/api/v1/autenticacao/login \
  -H "Content-Type: application/json" \
  -d '{
    "login": "joao@email.com",
    "senha": "SenhaSegura123!"
  }'
```

### Criar Ordem de Serviço

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
