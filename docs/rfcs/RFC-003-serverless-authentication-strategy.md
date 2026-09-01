# RFC-003: Estratégia de Autenticação Serverless (AWS Lambda + JWT + API Gateway)

* **Status**: Aprovado
* **Data**: 2026-08-31
* **Autor**: Equipe de Arquitetura (SOAT - Tech Challenge)
* **Domínio**: Segurança & Autenticação

---

## 1. Resumo Executivo

Este RFC propõe e detalha a arquitetura de **autenticação serverless desacoplada** para o sistema da Oficina Mecânica. A solução utiliza uma **AWS Lambda Function** independente (provisionada no repositório `tech-challenge-auth-serverless`) integrada ao **AWS API Gateway** para validar o CPF do cliente, verificar a existência do cadastro no banco RDS e emitir um **Token JWT** seguro para consumo das APIs protegidas.

---

## 2. Contexto e Desafio

Na Fase 3 do Tech Challenge, um dos requisitos obrigatórios é a proteção das rotas sensíveis do sistema exigindo que os clientes se autentiquem informando exclusivamente seu CPF.

### Requisitos Principais:
1. **Identificação por CPF**: O cliente não necessita de senha complexa para consultar o andamento da sua Ordem de Serviço, bastando o fornecimento do CPF cadastrado.
2. **Arquitetura Serverless**: A função de validação e emissão de tokens deve ser isolada da aplicação principal .NET executada no Kubernetes.
3. **Validação na Borda (Edge Layer)**: O tráfego não autenticado ou inválido deve ser bloqueado na camada do API Gateway antes de atingir o cluster Kubernetes.

---

## 3. Opções Avaliadas

| Critério | Lambda Authorizer + API Gateway (Escolha) | Autenticação Monolítica na API .NET | Provider Externo (Cognito / Auth0) |
|---|---|---|---|
| **Desacoplamento** | **Total**: Código e infraestrutura isolados no repositório `tech-challenge-auth-serverless`. | **Nulo**: Processado dentro dos Pods da API principal. | **Médio**: Dependência direta de SaaS externo com custo por usuário ativo. |
| **Escalabilidade** | **Elástica**: Escala instantaneamente a zero sem consumo de recursos do cluster EKS. | Limitada pela capacidade de réplicas de Pods do EKS. | Gerenciada pelo fornecedor. |
| **Segurança na Borda** | **Alta**: API Gateway filtra requisições inválidas antes da entrada na VPC. | **Baixa**: Todas as requisições atingem os Pods da aplicação. | **Alta**. |
| **Customização de Regras de CPF** | **Total**: Algoritmo customizado de validação de CPF + consulta ao RDS. | Total. | Baixa (requer fluxos customizados de Cognito Triggers). |

---

## 4. Detalhamento da Solução Proposta

```
┌─────────────┐        1. POST /autenticacao/login { cpf }         ┌──────────────────┐
│   Cliente   │───────────────────────────────────────────────────>│   API Gateway    │
└─────────────┘                                                    └────────┬─────────┘
       ▲                                                                    │
       │                                                            2. Invoca Lambda
       │ 5. Response 200 OK { token: "eyJhb..." }                           │
       │                                                                    ▼
┌──────┴──────┐          4. Retorna Token JWT Validado             ┌──────────────────┐
│  API Gateway│<───────────────────────────────────────────────────│ Lambda Authorizer│
└─────────────┘                                                    └────────┬─────────┘
                                                                            │
                                                                    3. SELECT por CPF
                                                                            │
                                                                            ▼
                                                                   ┌──────────────────┐
                                                                   │ Amazon RDS (DB)  │
                                                                   └──────────────────┘
```

### Passo a Passo do Fluxo:
1. **Recepção**: O cliente envia uma requisição `POST /api/v1/autenticacao/login` com o documento `{ cpf: "12345678900" }`.
2. **Validação de Formato**: A Lambda executa a sanitização do CPF (remoção de caracteres não numéricos) e a verificação matemática dos dígitos verificadores (Módulo 11).
3. **Consulta de Cadastro**: A Lambda realiza uma consulta rápida no banco de dados RDS (`SELECT * FROM clientes WHERE cpf = ...`).
4. **Geração do Token JWT**:
   - Caso o cliente exista e esteja ativo, a Lambda constrói um token JWT assinado digitalmente com a chave secreta da aplicação.
   - O payload do token contém claims padronizadas (`sub`, `cpf`, `role`, `exp` de 2 horas).
5. **Autorização no API Gateway**: Nas rotas protegidas (ex: `POST /api/v1/ordens-servico`), o API Gateway valida a assinatura e expiração do header `Authorization: Bearer <token>` e encaminha o tráfego via VPC Link para a API no EKS.

---

## 5. Estrutura do Repositório Dedicado

Para cumprir a separação de responsabilidades em 4 repositórios, esta solução reside no repositório `tech-challenge-auth-serverless`, contendo:
* Código fonte da função Lambda (Node.js / C# / Python).
* Manifestos de deploy automatizado via GitHub Actions.
* Testes unitários para a regra de validação de CPF e geração de JWT.

---

## 6. Considerações de Segurança

* **Assinatura do JWT**: Utiliza algoritmo `HS256` com chave secreta compartilhada com a API principal via AWS Secrets Manager / Kubernetes Secrets.
* **Prevenção contra Brute Force**: O API Gateway implementa *Throttling* e *Rate Limiting* (100 req/sec por IP).

---

## 7. Decisão Final

* **Estratégia Escolhida**: AWS Lambda Authorizer + Token JWT + HTTP API Gateway.
* **Status**: Aprovado e definido como padrão arquitetural.
