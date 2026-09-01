# ADR-001: Padrão de Comunicação entre Componentes (Síncrono REST/HTTP)

* **Status**: Aceito
* **Data**: 2026-08-31
* **Decisores**: Equipe de Arquitetura (SOAT - Tech Challenge)
* **Domínio**: Arquitetura de Software & Integração

---

## 1. Contexto e Declaração do Problema

A solução da Oficina Mecânica compreende múltiplos componentes distribuídos:
1. Cliente (Web / Mobile / Postman).
2. AWS API Gateway (Edge Routing).
3. AWS Lambda Function (Autenticação Serverless).
4. Aplicação Backend (.NET 8 rodando em Kubernetes EKS).
5. AWS RDS PostgreSQL (Banco de dados relacional).

É necessário definir o **padrão de comunicação primário** entre esses componentes para o recebimento de requisições de clientes, consulta de status, criação e atualização de Ordens de Serviço.

---

## 2. Opções Consideradas

### Opção 1: Comunicação Síncrona REST/HTTP via API Gateway e VPC Link (Escolhida)
* Todas as interações com o sistema utilizam chamadas HTTP/REST padronizadas com payloads em formato JSON.
* O tráfego externo passa pelo API Gateway e é roteado internamente para o Cluster EKS via VPC Link / Internal Load Balancer.

### Opção 2: Comunicação Totalmente Assíncrona Baseada em Eventos (Event-Driven via SQS/SNS/EventBridge)
* Cada requisição de abertura de Ordem de Serviço gera uma mensagem em uma fila ou barramento de eventos.
* Os microsserviços consomem as mensagens de forma assíncrona sem dar resposta imediata de confirmação ao cliente HTTP.

### Opção 3: Comunicação gRPC Interna
* Uso de gRPC (HTTP/2 + Protocol Buffers) para a comunicação entre o API Gateway/Lambda e os Pods da API .NET no EKS.

---

## 3. Matriz de Decisão

| Critério | Síncrono REST/HTTP (Opção 1) | Assíncrono Mensageria (Opção 2) | gRPC Interno (Opção 3) |
|---|---|---|---|
| **Feedback ao Cliente** | **Imediato**: O cliente recebe o ID da OS e o status criado na resposta da requisição HTTP. | **Diferido**: O cliente recebe apenas `202 Accepted` e precisa fazer polling ou receber WebSocket. | Imediato. |
| **Complexidade de Implementação** | **Baixa**: Padrão REST bem estabelecido com OpenAPI/Swagger nativo no .NET 8. | **Alta**: Exige provisionamento e gestão de filas (SQS/RabbitMQ), tratamento de Dead Letter Queues (DLQ) e consistência eventual. | **Média**: Exige definição de arquivos `.proto` e suporte do ingress a HTTP/2. |
| **Rastreabilidade e Observabilidade** | **Excelente**: Propagação simples do `Correlation-ID` via headers HTTP (`X-Correlation-ID`) para ferramentas de APM (Datadog/New Relic). | Requer headers de contexto nas mensagens da fila. | Exige interceptores gRPC para APM. |

---

## 4. Decisão

Decidiu-se adotar o **Padrão de Comunicação Síncrono REST/HTTP sobre JSON** para a totalidade dos endpoints da aplicação backend e para a integração com o AWS API Gateway.

### Detalhes da Implementação:
1. **Contratos RESTful**: Respeito estrito aos verbos HTTP (`GET`, `POST`, `PUT`, `PATCH`, `DELETE`) e aos códigos de status HTTP (`200 OK`, `201 Created`, `400 Bad Request`, `401 Unauthorized`, `404 Not Found`).
2. **Descoberta e Roteamento**: O API Gateway utiliza **VPC Link** para conectar a borda pública ao Network Load Balancer (NLB) interno da AWS, que distribui a carga diretamente entre os Pods do Kubernetes EKS.
3. **Evolução Futura (Híbrida)**: O sistema é projetado para permitir comunicação assíncrona secundária (ex: emissão de eventos em fila SQS quando a OS atingir o status `Finalizado` para notificação ao cliente), sem alterar o contrato síncrono dos endpoints de API.

---

## 5. Consequências

### Positivas:
* **Simplicidade de Consumo**: Fácil integração com o frontend e ferramentas de teste (Swagger, Postman, cURL).
* **Consistência Imediata**: O cliente obtém a confirmação direta de que a Ordem de Serviço foi criada ou atualizada.
* **Documentação Viva**: Através do Swagger UI integrado à API .NET 8.

### Negativas / Riscos Mitigados:
* **Acoplamento Temporal**: Se a API backend estiver indisponível, as requisições HTTP falharão imediatamente.
  * *Mitigação*: Uso de Kubernetes Deployment com múltiplas réplicas, Healthchecks (`/healthz`) e auto-scaling via HPA.
