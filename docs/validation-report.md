# Validação dos repositórios — 07/09/2026

## Conclusão e escopo

A solução ainda não está pronta para ser considerada integralmente validada. A aplicação
compila e a validação local de observabilidade foi executada com sucesso, mas há requisitos
de cloud, CI/CD e infraestrutura pendentes.
Esta revisão cobre os arquivos locais de fase1-tech-challenge, soat-infra e soat-db,
incluindo alterações ainda sem commit, e consultas somente de leitura ao GitHub.
Nenhum deploy, merge, alteração de proteção ou envio de notificações foi realizado.

O ambiente local com Grafana foi escolhido para desenvolvimento e demonstração. O
[enunciado](tech-challenge-spec/TC_FASE3.md) exige deploy automático em nuvem, banco
gerenciado e Kubernetes: executar localmente não dispensa esses requisitos. Uma entrega
exclusivamente local precisa de aceite explícito dos responsáveis pela avaliação.

## Evidências de validação

| Verificação | Resultado nesta revisão |
|---|---|
| Build dos projetos .NET via projeto de testes, SDK 8.0.424, Release | Passou |
| Testes sem os namespaces Api e Infrastructure | 239 passaram; 0 falharam; 0 ignorados |
| Testes de API, integração e migração com PostgreSQL | 268 passaram; 0 falharam; 0 ignorados, com PostgreSQL do Compose ativo |
| Compose com .env.example | docker compose config --quiet passou; não comprova que containers iniciam |
| Stack Compose de observabilidade | Containers iniciaram; API, Grafana, Prometheus, Alertmanager, Blackbox, Loki, Tempo, Collector e receiver responderam |
| Targets e healthchecks | Prometheus `techchallenge-api` e `techchallenge-health` `UP`; `/health/live` e `/health/ready` retornaram HTTP 200; `probe_success=1` |
| Dashboard e datasources Grafana | Dashboard `Tech Challenge - Observabilidade` carregado; Prometheus, Loki e Tempo provisionados |
| Logs e traces | Loki recebeu logs com metadados estruturados de `correlation_id`/`trace_id`; Tempo recebeu traces da API |
| Alertmanager | Regras carregadas; POST sintético entregue ao receiver local com HTTP 202 |
| kube-prometheus-stack | helm template passou com chart 90.0.0; datasource uid prometheus e seletores compatíveis com os manifests da aplicação |
| Scripts Grafana e aplicação | bash -n passou usando Git Bash; não foi feito deploy |
| Scripts Grafana, aplicação e Datadog | bash -n passou usando Git Bash; o script Datadog foi corrigido nesta rodada |
| Terraform fmt de soat-db | Passou |
| Terraform fmt de soat-infra | Falhou: environments/dev/main.tf |
| Terraform validate, dev/prod dos dois repos | Não concluído: init com backend desabilitado e lockfile readonly encontrou checksums incompatíveis no Windows; prod de soat-infra requer criação/atualização do lock. Isso não comprova erro nos módulos; requer preparar dependências antes de validar |
| CD .NET 8 usando .slnx | Falha reproduzida: MSB4068, elemento Solution não reconhecido |
| Kubernetes local | Nenhum contexto configurado; coleta de CPU/memória e probes sem validação em cluster |

Comandos de reprodução na raiz da aplicação:

    dotnet test tests/Fiap.TechChallenge.Tests/Fiap.TechChallenge.Tests.csproj --configuration Release --filter "FullyQualifiedName!~Fiap.TechChallenge.Tests.Api&FullyQualifiedName!~Fiap.TechChallenge.Tests.Infrastructure"
    docker compose --env-file .env.example config --quiet

Com Docker ativo, executar todos os testes usando o caminho do .csproj, sem o filtro. Nesta
rodada, o comando retornou 268 testes aprovados.

## Atualização — Kubernetes local (09/09/2026)

Foi criado um cluster Kind com Kubernetes 1.37 e instalado o kube-prometheus-stack
(chart 90.0.0), Metrics Server e Blackbox Exporter. A aplicação foi compilada a partir
do código atual e implantada juntamente com PostgreSQL local.

| Evidência | Resultado |
|---|---|
| API e banco | Pods `api` e `postgres` ficaram `Running`; `/health/live`, `/health/ready` e `/metrics` responderam HTTP 200 |
| Métricas e scraping | ServiceMonitor da API ficou `UP`; a métrica `techchallenge_http_request_duration_milliseconds_count` foi consultada no Prometheus |
| Recursos Kubernetes | `kubectl top` retornou CPU/memória dos pods e o HPA recebeu percentuais reais de CPU e memória |
| Uptime | Probe Blackbox de `/health/ready` entrou na configuração do Prometheus e retornou `probe_success=1` |
| Alertas | As cinco regras `TechChallenge*` foram carregadas pelo Prometheus |
| Dashboard | O ConfigMap foi importado pelo sidecar; a API do Grafana retornou o dashboard `Tech Challenge - Observabilidade` |

O receiver externo de incidentes continua uma escolha operacional da equipe. O
Alertmanager, as regras e sua visualização no Grafana já estão funcionais localmente.

## Situação confirmada no GitHub

| Repositório | main protegida | soat-architecture | Evidência de execução |
|---|---|---|---|
| [fase1-tech-challenge](https://github.com/SOAT-FIAP-2026/fase1-tech-challenge) | Sim; regra de PR com 1 aprovação | write | Última execução consultada: Code Quality: Scheduled, sucesso; não é evidência de deploy |
| [soat-infra](https://github.com/SOAT-FIAP-2026/soat-infra) | Não | write | Último Terraform Deploy consultado falhou em Configure AWS Credentials |
| [soat-db](https://github.com/SOAT-FIAP-2026/soat-db) | Não | write | Último Terraform Deploy consultado falhou em Configure AWS Credentials |
| [lambda-auth-function](https://github.com/SOAT-FIAP-2026/lambda-auth-function) | Ainda não protegida | write | `main` criada com README inicial nesta rodada; sem implementação ou pipeline |

Portanto, existem quatro repositórios destinados à solução; o quarto agora tem documentação
inicial publicada, mas ainda precisa ser implementado. A ausência de clone local não
significava ausência do repositório remoto.

Execuções consultadas: [qualidade da aplicação](https://github.com/SOAT-FIAP-2026/fase1-tech-challenge/actions/runs/33795267739),
[deploy Kubernetes](https://github.com/SOAT-FIAP-2026/soat-infra/actions/runs/33330203865) e
[deploy banco](https://github.com/SOAT-FIAP-2026/soat-db/actions/runs/32792194330).
As regras remotas podem mudar após esta revisão.

## Correções necessárias para a observabilidade local

| Prioridade | Achado e evidência | Critério de conclusão |
|---|---|---|
| Alta | Correlação em erros tratados foi corrigida para restaurar X-Correlation-ID e usar o trace_id no payload | Testes automatizados e resposta runtime com X-Correlation-ID passaram; falta demonstrar 400/500 no vídeo |
| Alta | Alertmanager agora envia webhooks ao receiver local; entrega externa ainda não está configurada | Regras foram carregadas e receiver local recebeu POST sintético HTTP 202; falta substituir por destino da equipe quando houver |
| Alta | Compose agora usa Blackbox Exporter para sondar /health/ready; Probe do Kubernetes depende do chart instalado | Probe local retornou `probe_success=1`; indisponibilidade/recuperação e cluster ainda não foram demonstrados |
| Alta | Compose agora inclui Loki, Tempo e OpenTelemetry Collector | Stack iniciou; Loki recebeu logs estruturados com metadados de correlação e Tempo recebeu traces; falta evidência visual no vídeo |
| Média | Métricas de duração de diagnóstico, execução e finalização agora são emitidas após Atualizar concluir em OrdemServicoService | Testar falha no banco sem amostra de sucesso |
| Média | Painel de volume usa janela móvel de 24h; etapas usam rate de 5m, apenas para etapas concluídas | Documentar semântica e validar com ordens sintéticas; ajustar caso se deseje volume por dia civil ou média no intervalo selecionado |
| Média | Séries de negócio só aparecem após eventos e increase precisa de amostras; painel vazio não significa zero | Nomes de métricas foram conferidos; é necessário gerar ordens sintéticas para validar volume e etapas no dashboard |
| Média | Latência customizada inclui scrapes e healthchecks; histograma não possui buckets explícitos para a carga observada | Validar distribuição/buckets e separar tráfego operacional para que o p95 reflita chamadas de negócio |
| Média | Resend registra resultado final após retries, usa destinatário fixo e Compose não injeta RESEND_API_KEY | Definir integração de demonstração sem enviar dados reais, configurar credencial quando necessário e testar sucesso/falha; distinguir tentativa de envio de operação final |
| Média | CPU/memória no dashboard dependem de Kubernetes, kubelet/cAdvisor e kube-state-metrics | Confirmar painéis com pods reais; no Compose esses painéis permanecerão sem dados |
| Média | install-grafana.sh não fixa versão do chart; armazenamento persistente do stack Kubernetes não está configurado | Fixar versão validada e definir persistência/limites adequados ao ambiente local |
| Média | Grafana usa admin/admin e portas do Compose estão publicadas em todas as interfaces | Restringir exposição ao localhost para demonstração e trocar senha quando compartilhado |

As falhas de OS atualmente contabilizadas são respostas HTTP 5xx nas rotas
/api/v1/ordens-servico. Rejeições de negócio 4xx e falhas assíncronas não entram nesse
contador. A finalização mede DataConclusao até ConfirmarEntrega; execução usa o primeiro
início e último fim dos itens, não a soma de horas de trabalho.

## Pendências por repositório

### Aplicação

- Corrigir o CD: ele instala SDK .NET 8 e tenta testar .slnx. Usar o projeto de testes
  compatível ou alinhar o SDK/formato da solução.
- Definir estratégia de deploy local e homologação/produção. O CD atual escuta feat/infra;
  o trigger da main está comentado. Não reativar deploy AWS sem ambiente preparado.
- Corrigir o script local: o provisionamento do PostgreSQL está comentado, apesar de o
  README anteriormente afirmar que o script o instala. O deploy manual documentado aplica esse recurso.
- Validar migração data_inicio_diagnostico em PostgreSQL e todos os testes de API/E2E.
- Implementar o fluxo por CPF integrado à função e ao Gateway. O login atual usa e-mail
  e senha em AutenticacaoService; JWT e autorização por papel já existem.
- Esclarecer responsabilidade da pasta infra/ legada, que ainda duplica Terraform dos
  repositórios separados, antes de escolher qual pipeline será fonte oficial do deploy.

### soat-infra

- Proteger main e exigir PR; definir homologação, hoje ausente dos triggers de infra.
- Corrigir formatação de environments/dev/main.tf para liberar o check do CI.
- Preparar locks de providers reproduzíveis para as plataformas usadas, incluindo
  Windows e Linux do CI, e repetir terraform validate; não foi executado apply.
- Resolver configuração de credenciais, apontada pela última execução de deploy.
- Preparar um cluster local real. A emulação Floci não fornece os nós Kubernetes
  necessários para executar a aplicação e coletar métricas de containers.
- Versionar/bootstrapar o Floci compartilhado se esse caminho continuar nos READMEs:
  não há docker-compose.yml na raiz deste workspace.
- Corrigir install-datadog.sh antes de apresentá-lo como alternativa executável.

### soat-db

- Proteger main e exigir PR.
- Corrigir working-directory dos dois workflows: executam Terraform na raiz sem
  arquivos .tf; módulos de ambiente estão em environments/dev e environments/prod.
- Definir estado e variáveis separados por ambiente: branches homolog/develop/main
  atualmente executam os mesmos comandos sem seleção de ambiente.
- Habilitar/configurar backend remoto para uso em CI de produção: bloco S3 está comentado.
- Injetar variáveis obrigatórias de banco/rede no pipeline e alinhar região com a VPC
  do Kubernetes (defaults atuais: banco us-east-1, infra sa-east-1).
- Alarmes e Enhanced Monitoring não estão provisionados pelo módulo atual. PostgreSQL
  local é uma instância em container; não é um banco gerenciado em nuvem.

### lambda-auth-function

- Implementar validação de CPF, consulta de existência/status do cliente e emissão JWT.
- Adicionar CI/CD, infraestrutura/deploy da função, testes, README, diagrama e contrato de API.
- Integrar API Gateway e aplicação; proteger a main quando existir.

## Documentação e entrega

| Requisito | Situação |
|---|---|
| README nos 4 repos | README inicial criado para a Lambda; implementação e links ativos ainda pendentes |
| Swagger/Postman | Swagger configurado na aplicação em /swagger; Postman é alternativa, não é obrigatório ter ambos; falta contrato da Lambda |
| Componentes e sequência | Diagramas existem, mas CPF/Lambda/cloud representam arquitetura alvo; diagrama local disponível em observability.md |
| RFCs e ADRs | Existem; ajustar coerência entre proposta cloud e execução local. ADR-002 descreve CPU 70%/memória 80%, enquanto manifest usa 50%/95% |
| ER e justificativa do banco | Documento existe; atualizar modelo para incluir data_inicio_diagnostico |
| Vídeo até 15 minutos | Link/evidência de demonstração não verificado; incluir auth, pipeline, deploy, APIs, dashboards, logs e traces |
| PDF único | Artefato final não localizado nesta revisão; reunir 4 links, vídeo, docs e confirmação de acesso do avaliador |
| Deploy ativo | Não comprovado; sucesso de build/qualidade não comprova deploy |

Ordem sugerida: resolver execução local e observabilidade, validar com dados sintéticos,
corrigir CI/proteções e implementar Lambda/Gateway; depois alinhar a entrega cloud com
o enunciado e gravar a demonstração. Estes itens são backlog da revisão, não correções
já aplicadas.

## Atualização — Lacunas de documentação e observabilidade (13/09/2026)

Itens desta revisão que foram fechados, com o artefato correspondente:

| Item anterior | Situação | Artefato |
|---|---|---|
| README da Lambda descrevia o repositório como vazio, apesar de a função já estar implementada | Corrigido | README reescrito em lambda-auth-function com tecnologias usadas, contrato, variáveis de ambiente, empacotamento e deploy |
| Faltava contrato/collection da Lambda | Corrigido | `docs/postman/lambda-auth.postman_collection.json` no repositório da Lambda |
| Swagger citado sem collection Postman da API | Corrigido | `docs/postman/techchallenge-api.postman_collection.json` (34 requisições, fluxo completo da OS) |
| Escolha de Prometheus/Grafana em vez de Datadog/New Relic não estava justificada | Corrigido | [ADR-003](adrs/ADR-003-observability-stack.md), com mapeamento requisito → métrica → painel → alerta |
| Dashboard sem uptime, logs e traces | Corrigido | 7 painéis novos: uptime, healthcheck, réplicas, reinícios, erros por integração, logs Loki com variável Correlation ID, traces Tempo |
| Sem alerta de consumo de recursos do Kubernetes | Corrigido | grupo `techchallenge.kubernetes`: CPU, memória, CrashLoopBackOff, OOMKilled, reinícios e HPA no teto |
| Alertmanager sem destino externo | Parcial | módulo Terraform de soat-infra aceita webhook de Slack ou HTTP; o valor ainda precisa ser fornecido pela equipe |
| Lambda sem log estruturado nem correlação | Corrigido | `StructuredLogger` + propagação de `X-Correlation-ID`, com testes em `tests/.../Observability/CorrelationTests.cs` |
| RDS sem alarmes nem Enhanced Monitoring | Corrigido | `soat-db/modules/rds/monitoring.tf`: 5 alarmes CloudWatch, Enhanced Monitoring e exportação de logs configuráveis |
| `terraform fmt` falhando em soat-infra | Corrigido | `modules/networking/main.tf` reformatado |

Permanecem abertos os itens de execução e entrega: CD da aplicação, Terraform e API
Gateway da Lambda, proteção de branches, deploy AWS ativo, vídeo de demonstração e PDF
final. A ausência do .NET SDK na estação usada nesta revisão impediu rodar `dotnet build`
e `dotnet test` localmente; a validação das mudanças em C# depende da execução do CI.

## Atualização — Segurança, CI/CD e testes (13/09/2026)

Ambiente desta rodada: .NET SDK 8.0.425 instalado localmente, o que permitiu executar
build e testes de verdade — o que não tinha sido possível na atualização anterior.

| Item anterior | Situação | Evidência |
|---|---|---|
| Branch `fix/correcao-issues-seguranca-sonar` aberta e conflitante | Integrada | merge com correção de build, do caminho local e das credenciais |
| `JWTConfig`/`TokenService` não compilavam (`throw new ArgumentNullException(jwtSecret)` dentro do próprio inicializador) | Corrigido | `JwtSecretResolver` único, com fallback de configuração e validação de tamanho mínimo |
| `k8s/overlays/aws/secrets.yaml` com endpoint, senha do RDS e chave JWT reais versionados | Corrigido no HEAD | arquivo destrackeado e ignorado; apenas o `.template` é versionado. **Os valores seguem no histórico e precisam ser rotacionados** |
| `.env.example` com a chave JWT real | Corrigido | placeholder |
| Overlay local removido pela branch de segurança | Restaurado | `secrets.yaml.template` renderizado por `deploy.sh`/`deploy.ps1` |
| Lambda sem Terraform, API Gateway e CD | Corrigido | `infra/` com Lambda, IAM, Security Group, log groups e rota `POST /auth/token`; workflow `cd.yml` |
| CI da Lambda não executava testes nem cobertura | Corrigido | `build.yml` roda a suíte com OpenCover e entrega ao Sonar |
| `UnitTest1` vazio e literais de CPF duplicados | Corrigido | teste removido, literais extraídos para constantes |
| Workflows do soat-db executando Terraform na raiz, sem `.tf` | Corrigido | `working-directory: environments/prod`, variáveis injetadas, gatilho só na `main` |
| Região do soat-db divergente da VPC | Corrigido | padrão passou para `sa-east-1` |
| ER sem `data_inicio_diagnostico` | Corrigido | coluna incorporada ao diagrama |
| Painéis de logs e traces sem datasource no caminho de script | Corrigido | `install-grafana.sh`/`.ps1` passam a instalar Loki, Tempo e OTel Collector |

Resultado dos testes nesta revisão:

| Repositório | Testes | Cobertura |
|---|---|---|
| fase1-tech-challenge | 289 passando, 0 falhas | coletada no CI pelo `coverlet.runsettings` |
| lambda-auth-function | 33 passando, 0 falhas | 86,9% de sequência (OpenCover) |

### Ação obrigatória de segurança

A chave JWT e a senha do RDS de produção estiveram versionadas em
`k8s/overlays/aws/secrets.yaml` e em `.env.example`, presentes no histórico público do
repositório desde 10/09/2026. Removê-las do HEAD não as remove do histórico. É preciso:

1. rotacionar a senha do usuário `postgres` no RDS e atualizar o secret `DB_CONNECTION_BASE64`;
2. gerar uma nova chave JWT e atualizar `JWT_SECRET_BASE64`;
3. avaliar reescrita do histórico (`git filter-repo`) ou considerar os valores comprometidos.

### Ainda aberto

Deploy AWS ativo e comprovado, proteção de branch com PR obrigatório nos quatro
repositórios, execução do CD da Lambda (depende de credenciais e da VPC), vídeo de
demonstração e PDF final de entrega. A divergência do ADR-002 (70%/80% na decisão,
50%/95% no manifesto para facilitar o teste local) segue registrada e não foi alterada.
