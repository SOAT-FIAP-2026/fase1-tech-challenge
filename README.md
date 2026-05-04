# Sistema Integrado de Atendimento e Execução de Serviços - Oficina Mecânica

Esta é uma API desenvolvida em **.NET 8**, seguindo os princípios da **Clean Architecture** e **Domain-Driven Design (DDD)**, com foco em gestão de ordens de serviço, clientes e peças para uma oficina mecânica de médio porte.

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
- [Docker](https://www.docker.com/products/docker-desktop) e [Docker Compose](https://docs.docker.com/compose/install/)

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

### Execução com Docker Compose

```bash
docker compose up -d 
```

### Endpoints locais

| Serviço | URL |
|---|---|
| Swagger UI | http://localhost:8080/swagger |
| HTTP | http://localhost:8080/api/ping |

## Usuário para testes de rotas autenticadas

```bash
Login: admin@techchallenge.com
Senha: Admin@123
```


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

