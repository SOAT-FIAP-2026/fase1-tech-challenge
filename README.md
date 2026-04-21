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

## 📋 Funcionalidades Obrigatórias

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

## 🏗️ Arquitetura

O projeto está estruturado em **camadas (Layered Architecture)** com princípios de **Clean Architecture**, garantindo baixo acoplamento e alta coesão:

*   **Domain (`Fiap.TechChallenge.Domain`)**: Contém as entidades (ex: `Usuario`, `Cliente`, `OrdemServico`), interfaces, enums, exceções e regras de negócio essenciais. É o núcleo do sistema e não possui dependências de outras camadas. Segue princípios de **Domain-Driven Design (DDD)**.
*   **Application (`Fiap.TechChallenge.Application`)**: Contém os casos de uso da aplicação, DTOs, serviços de aplicação e orquestra a lógica de negócio utilizando as entidades do domínio.
*   **Infrastructure (`Fiap.TechChallenge.Infrastructure`)**: Implementação de acesso a dados, repositórios, ORM e outras preocupações de infraestrutura.
*   **External (`Fiap.TechChallenge.External`)**: Camada designada para integrações com serviços e APIs de terceiros.
*   **Api (`Fiap.TechChallenge.Api`)**: O ponto de entrada (Entrypoint) da aplicação. Contém os Controllers, Middlewares, configurações do Swagger e a configuração da injeção de dependência (Startup/Program).

## 🛠️ Stack Tecnológico

*   **.NET 8** com C# 12
*   **Clean Architecture** + **Domain-Driven Design (DDD)**
*   **JWT** para autenticação
*   **Swagger/OpenAPI** para documentação
*   **Testes Unitários e de Integração**
*   **Docker** e **docker-compose** para containerização
*   Banco de dados: *[A definir/Justificar]*
*   **ReportGenerator** para análise de cobertura de testes

## 🚀 Como Executar o Projeto

### Pré-requisitos
*   [.NET 8 SDK](https://dotnet.microsoft.com/download) ou superior
*   [Docker](https://www.docker.com/products/docker-desktop) (para execução containerizada)
*   [Docker Compose](https://docs.docker.com/compose/install/) (opcional, para ambiente completo)

### Passos para rodar localmente - Modo Debug

1. Clone o repositório:
   ```bash
   git clone https://github.com/SOAT-FIAP-2026/fase1-tech-challenge.git  
   ```

2. Navegue até a pasta da API:
   ```bash
   cd src/Fiap.TechChallenge.Api
   ```

3. Restaure os pacotes:
   ```bash
   dotnet restore
   ```

4. Execute o projeto:
   ```bash
   dotnet run
   ```

5. Acesse a aplicação:
   * Aplicação: `https://localhost:7XXX` ou `http://localhost:5XXX`
   * Swagger UI: `https://localhost:7XXX/swagger/index.html`

### Execução com Docker

1. Build da imagem:
   ```bash
   docker build -t fiap-techchallenge-api .
   ```

2. Execução do container:
   ```bash
   docker run -p 8080:80 fiap-techchallenge-api
   ```

3. Acesse em: `http://localhost:8080/swagger/index.html`

### Execução com Docker Compose

```bash
docker-compose up -d
```

Isso orquestrará a aplicação junto com o banco de dados e outras dependências.

## 🧪 Executando os Testes e Cobertura

O projeto conta com uma estrutura de testes separada na pasta `tests`, com cobertura mínima de **80% nos domínios críticos**.

### Rodar todos os testes

```bash
dotnet test
```

### Rodar testes de um projeto específico

```bash
dotnet test tests/Fiap.TechChallenge.Domain.Tests/
```

### Rodar com filtro por nome de teste

```bash
dotnet test --filter "MethodName~NomeDoTeste"
```

### Gerando o Relatório de Cobertura de Código

Você pode gerar um painel HTML interativo para visualizar a cobertura de código dos testes.

1. Rode os testes solicitando a coleta de dados de cobertura:
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```

2. Em seguida, gere o relatório HTML utilizando o ReportGenerator:
   ```bash
   dotnet tool install -g dotnet-reportgenerator-globaltool
   reportgenerator -reports:"tests/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
   ```

3. O relatório será criado na pasta `coveragereport`. Basta abrir o arquivo `index.html` em seu navegador padrão.

## 📊 Análise de Segurança

O projeto inclui análise de vulnerabilidades do código-fonte. Consulte o **Relatório de Vulnerabilidades** incluído na documentação de entrega.

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

## 🔑 Endpoints Principais

### Autenticação
- `POST /api/v1/autenticacao/cadastrar` - Cadastrar novo usuário
- `POST /api/v1/autenticacao/login` - Realizar login e obter JWT

### Ordens de Serviço
- `POST /api/v1/ordens-servico` - Criar nova ordem de serviço
- `GET /api/v1/ordens-servico/{id}` - Obter detalhes da OS
- `GET /api/v1/ordens-servico` - Listar todas as OS
- `PATCH /api/v1/ordens-servico/{id}/status` - Atualizar status da OS

### Clientes
- `POST /api/v1/clientes` - Cadastrar cliente
- `GET /api/v1/clientes/{cpfCnpj}` - Obter cliente
- `PUT /api/v1/clientes/{cpfCnpj}` - Atualizar cliente

### Veículos
- `POST /api/v1/veiculos` - Cadastrar veículo
- `GET /api/v1/veiculos/{placa}` - Obter veículo
- `GET /api/v1/veiculos/cliente/{cpfCnpj}` - Listar veículos do cliente

### Serviços
- `POST /api/v1/servicos` - Cadastrar serviço
- `GET /api/v1/servicos` - Listar serviços

### Peças e Insumos
- `POST /api/v1/pecas` - Cadastrar peça
- `GET /api/v1/pecas` - Listar peças
- `PATCH /api/v1/pecas/{id}/estoque` - Atualizar estoque

*Documentação completa disponível em `/swagger/index.html` quando a aplicação está em execução.*

## 📚 Documentação DDD

A documentação de **Domain-Driven Design (DDD)** inclui:

- **Event Storming** dos fluxos principais:
  - Criação e acompanhamento da Ordem de Serviço
  - Gestão de peças e insumos
- **Diagramas de Domínio**
- **Linguagem Ubíqua** aplicada no projeto
- **Bounded Contexts** identificados

Documentação completa disponível em [Link da Documentação DDD].

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

## 🛡️ Segurança

- ✅ Autenticação via **JWT**
- ✅ Hash de senhas com algoritmo seguro
- ✅ Validação de entrada em todos os endpoints
- ✅ Validação de CPF/CNPJ
- ✅ Validação de placa de veículo
- ✅ Testes de segurança nos fluxos críticos

## 👥 Contribuidores

*[Adicionar nomes dos integrantes do grupo]*

## 📝 Licença

Este projeto é propriedade da FIAP e foi desenvolvido como Tech Challenge da especialização SOAT.

## 📞 Suporte

Para dúvidas ou sugestões, acesse o Discord da FIAP SOAT.
