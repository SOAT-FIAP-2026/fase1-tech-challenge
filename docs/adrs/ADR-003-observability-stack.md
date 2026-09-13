# ADR-003: Stack de Observabilidade — Prometheus, Grafana, Loki e Tempo

* **Status**: Aceito
* **Data**: 2026-09-13
* **Decisores**: Equipe de Arquitetura (SOAT - Tech Challenge)
* **Domínio**: Observabilidade, Operação & Custo

---

## 1. Contexto e Declaração do Problema

O enunciado da Fase 3 pede monitoramento e observabilidade com "ferramentas como Datadog
ou New Relic", cobrindo:

* volume diário de ordens de serviço;
* tempo médio de execução por status (Diagnóstico, Execução, Finalização);
* erros e falhas nas integrações;
* latência das APIs;
* consumo de recursos do Kubernetes (CPU, memória);
* healthchecks e uptime;
* alertas para falhas no processamento de ordens de serviço;
* logs estruturados (JSON) com correlação entre requisições.

Datadog e New Relic são exemplos citados, não uma exigência de fornecedor. A decisão
precisa cobrir todos os itens acima e, ao mesmo tempo, ser executável por qualquer
avaliador em uma máquina local, sem conta paga e sem credencial compartilhada — restrição
concreta desta entrega, já que a demonstração roda em Kind/Minikube e o cluster EKS é
provisionado sob a conta de estudo da equipe.

---

## 2. Opções Consideradas

### Opção 1: Prometheus + Grafana + Alertmanager + Loki + Tempo, via OpenTelemetry (Escolhida)
* Métricas: instrumentação OpenTelemetry na aplicação .NET, exportador Prometheus e
  coleta por `ServiceMonitor` (Prometheus Operator).
* Logs: `AddJsonConsole` na API e `StructuredLogger` na Lambda, enviados por OTLP ao
  OpenTelemetry Collector e armazenados no Loki.
* Traces: instrumentação ASP.NET Core/HttpClient exportada por OTLP ao Tempo.
* Alertas: `PrometheusRule` avaliadas pelo Prometheus e roteadas pelo Alertmanager.
* Uptime: Blackbox Exporter sondando `/health/ready` por meio do CRD `Probe`.

### Opção 2: Datadog (SaaS)
* Agent instalado por Helm no cluster, com coleta de logs, métricas de infraestrutura e
  recepção de OTLP.
* Dashboards e monitores definidos na plataforma.

### Opção 3: New Relic (SaaS)
* Agente .NET ou OTLP nativo para a plataforma, com dashboards NRQL.

---

## 3. Matriz de Decisão

| Critério | Prometheus/Grafana (Opção 1) | Datadog (Opção 2) | New Relic (Opção 3) |
|---|---|---|---|
| **Custo** | Zero. Executa no próprio cluster. | Cobrança por host/GB de log após o trial de 14 dias. | Free tier de 100 GB/mês com um único usuário completo. |
| **Executável pelo avaliador** | Sim: `./observability/install-grafana.sh` ou `terraform apply` no módulo de observabilidade. | Não sem uma `DD_API_KEY` válida da equipe. | Não sem uma license key. |
| **Segredo em repositório público** | Nenhum. Senha do Grafana definida por variável do Terraform. | Exigiria distribuir API key para a avaliação. | Mesma exposição. |
| **Cobertura dos requisitos** | Total — métricas de negócio, latência, recursos, uptime, alertas, logs e traces. | Total. | Total. |
| **Métricas de negócio customizadas** | Nativas: `Meter` do .NET exposto no `/metrics` e consultado em PromQL. | Suportadas via OTLP/DogStatsD. | Suportadas via OTLP. |
| **Portabilidade** | Padrão OpenTelemetry; trocar o backend não altera o código da aplicação. | Mesma instrumentação OTLP, mas dashboards/monitores ficam presos ao formato Datadog. | Idem. |
| **Dependência externa na demonstração** | Nenhuma: roda offline. | Requer internet e disponibilidade do SaaS. | Idem. |
| **Esforço operacional** | Maior: a equipe opera Prometheus, Loki e Tempo. | Menor: serviço gerenciado. | Menor. |

---

## 4. Decisão

Adotamos a **Opção 1**. A instrumentação da aplicação usa exclusivamente OpenTelemetry,
de modo que o backend é um detalhe de infraestrutura e não do código.

Mapeamento requisito → implementação:

| Requisito do enunciado | Métrica / recurso | Onde aparece |
|---|---|---|
| Volume diário de ordens | `techchallenge_orders_created_total` | Painel "Ordens criadas - últimas 24h" |
| Tempo médio por status | `techchallenge_orders_status_duration_milliseconds` (label `status`) | Painel "Tempo médio por etapa" |
| Erros nas integrações | `techchallenge_integrations_errors_total` (labels `integration`, `operation`) | Painéis de erro + alerta `TechChallengeIntegrationError` |
| Latência das APIs | `techchallenge_http_request_duration_milliseconds` | Painel "Latência p95" + alerta `TechChallengeApiHighLatency` |
| CPU e memória do Kubernetes | `container_cpu_usage_seconds_total`, `container_memory_working_set_bytes`, `kube_pod_container_resource_limits` | Painéis de CPU/memória + alertas `TechChallengePodCpuHigh` / `TechChallengePodMemoryHigh` |
| Healthchecks e uptime | `probe_success`, `probe_duration_seconds` (Blackbox no `/health/ready`) | Painéis "Uptime da API" e "Healthcheck" + alerta `TechChallengeHealthcheckDown` |
| Alertas de falha no processamento | `techchallenge_orders_processing_failures_total` | Alerta `TechChallengeOrderProcessingFailure` (severity `critical`) |
| Logs estruturados com correlação | `AddJsonConsole` + `CorrelationIdMiddleware` (API) e `StructuredLogger` (Lambda), campo `correlation_id` | Painel de logs no Grafana, com a variável Correlation ID |

Componentes e onde são provisionados:

| Componente | Local (Docker Compose) | Kubernetes |
|---|---|---|
| Prometheus, Alertmanager, Grafana | `docker-compose.yml` + `observability/` | `soat-infra/modules/observability` (kube-prometheus-stack) |
| Blackbox Exporter | serviço `blackbox-exporter` | chart `prometheus-blackbox-exporter` + `k8s/observability/probe.yaml` |
| Loki e Tempo | serviços `loki` e `tempo` | charts `loki` e `tempo` |
| OpenTelemetry Collector | serviço `otel-collector` | chart `opentelemetry-collector` |
| Regras de alerta | `observability/prometheus/alerts.yml` | `k8s/observability/prometheusrule.yaml` |
| Dashboard | provisionamento de arquivo | ConfigMap com label `grafana_dashboard=1` |

---

## 5. Posição sobre Datadog e New Relic

O Datadog **permanece versionado** em `soat-infra/observability/` (values do Agent,
dashboard e monitores equivalentes) como caminho alternativo para um ambiente AWS/EKS
real com conta corporativa. Ele não é instalado nesta entrega e não é necessário para
atender a nenhum dos requisitos acima.

New Relic foi descartado sem implementação de referência: cobriria os mesmos requisitos,
mas traria a mesma dependência de credencial externa que motivou a recusa do Datadog,
sem vantagem adicional sobre ele.

Como toda a instrumentação é OpenTelemetry, migrar para qualquer um dos dois exige
apenas apontar `OTEL_EXPORTER_OTLP_ENDPOINT` para o agente correspondente — nenhuma
linha de código da aplicação muda.

---

## 6. Consequências

### Positivas
* Avaliação reproduzível: a stack sobe com um comando, sem conta paga nem segredo.
* Custo zero e nenhum dado de cliente saindo do cluster — relevante porque a Lambda
  processa CPF (dado pessoal sob a LGPD). CPF não é registrado em log em nenhum ponto.
* Métricas de negócio em PromQL, versionadas junto ao código que as emite.
* Independência de fornecedor pela padronização em OpenTelemetry.

### Negativas / Riscos Mitigados
* A equipe passa a operar Prometheus, Loki e Tempo. Mitigado por charts oficiais com
  values versionados e retenção curta (7 dias) dimensionada para nós pequenos.
* Retenção e alta disponibilidade são inferiores às de um SaaS. Aceitável para o escopo
  acadêmico; o caminho Datadog fica documentado caso a operação real exija.
* Prometheus, Loki e Tempo consomem recursos do próprio cluster. Mitigado com `requests`
  e `limits` explícitos nos values e PVCs `gp3` de 5 Gi.

---

## 7. Referências

* [docs/observability.md](../observability.md) — guia operacional da stack
* [soat-infra/observability/README.md](https://github.com/SOAT-FIAP-2026/soat-infra/blob/main/observability/README.md) — instalação no cluster
* [RFC-001](../rfcs/RFC-001-cloud-provider-choice.md) — escolha de provedor de nuvem
* [ADR-002](ADR-002-kubernetes-hpa-autoscaling.md) — HPA, cujos limites de CPU/memória são os mesmos observados aqui
