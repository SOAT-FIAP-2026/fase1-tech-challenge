# ADR-002: Estratégia de Auto-scaling da Aplicação via Kubernetes HPA

* **Status**: Aceito
* **Data**: 2026-08-31
* **Decisores**: Equipe de Arquitetura (SOAT - Tech Challenge)
* **Domínio**: Infraestrutura, Kubernetes & Resiliência

---

## 1. Contexto e Declaração do Problema

O sistema da Oficina Mecânica vivencia padrões de carga variáveis ao longo do dia:
* **Horários de Pico**: Abertura de chamados, chegada de veículos e atualização de diagnósticos no início da manhã e final da tarde.
* **Horários de Baixa**: Período noturno e finais de semana.

Manter uma quantidade fixa e elevada de réplicas de Pods resulta em desperdício de recursos e aumento dos custos da infraestrutura na AWS. Por outro lado, manter poucas réplicas pode levar ao esgotamento de CPU/Memória, aumento do tempo de resposta (latência) e queda de requisições.

É necessário estabelecer uma estratégia automatizada de escalamento horizontal de instâncias da aplicação .NET 8 no cluster Amazon EKS.

---

## 2. Opções Consideradas

### Opção 1: Horizontal Pod Autoscaler (HPA) baseado em CPU e Memória (Escolhida)
* O recurso nativo `HorizontalPodAutoscaler` do Kubernetes monitora o consumo de recursos computacionais através do **Metrics Server**.
* Adiciona ou remove réplicas de Pods dinamicamente com base em limites percentuais pré-configurados.

### Opção 2: Vertical Pod Autoscaler (VPA)
* Ajusta dinamicamente as solicitações de CPU e Memória (`requests` e `limits`) dos Pods existentes, sem alterar a quantidade de réplicas.

### Opção 3: Escalamento Manual / Estático
* Quantidade fixa de Pods (ex: 2 réplicas) configurada estaticamente no `Deployment.yaml`.

---

## 3. Matriz de Decisão

| Critério | HPA (Opção 1) | VPA (Opção 2) | Escalamento Estático (Opção 3) |
|---|---|---|---|
| **Disponibilidade durante o Escala** | **Sem Downtime**: Adiciona novos Pods em paralelo sem reiniciar os Pods ativos. | Requer reinício do Pod para aplicar novos limites de recursos. | Sem escalamento automático. |
| **Resiliência a Picos** | **Alta**: Distribui o tráfego entre mais Pods em nós diferentes da VPC. | Limitada pela capacidade da máquina física do nó EKS. | Nula durante picos que excedam a capacidade. |
| **Complexidade Operacional** | **Baixa**: Suportado nativamente pelo Kubernetes EKS + Metrics Server. | Média (pode gerar oscilação se mal configurado). | Mínima. |

---

## 4. Decisão

Decidiu-se pela adoção do **Horizontal Pod Autoscaler (HPA)** como a estratégia oficial de escalamento horizontal da aplicação Backend no Kubernetes.

### Especificação da Configuração (Manifesto `k8s/base/hpa.yaml`):
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: techchallenge-api-hpa
  namespace: techchallenge
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: techchallenge-api
  minReplicas: 1
  maxReplicas: 5
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

### Regras de Funcionamento:
1. **Réplicas Mínimas**: 1 Pod rodando em condições normais de tráfego.
2. **Réplicas Máximas**: Até 5 Pods para suportar picos extremos de tráfego.
3. **Gatilhos de Escala**:
   * **Scale-Out (Expansão)**: Disparado quando a utilização média de CPU ultrapassar **70%** ou o consumo médio de Memória ultrapassar **80%**.
   * **Scale-In (Contração)**: Redução gradativa de réplicas após janela de estabilização (*cooldown*) de 5 minutos para evitar o efeito *flapping* (oscilação constante).

---

## 5. Requisitos de Infraestrutura

Para que o HPA opere com sucesso:
1. **Metrics Server**: Deve estar instalado e operacional no cluster Amazon EKS (provisionado via Terraform no repositório `tech-challenge-infra-k8s`).
2. **Resource Requests/Limits**: O `Deployment` da aplicação DEVE conter as especificações exatas de `resources.requests` e `resources.limits` para que o Kubernetes calcule a porcentagem de utilização.

---

## 6. Consequências

### Positivas:
* **Alta Disponibilidade e Resiliência**: A aplicação suporta picos repentinos de carga sem intervenção humana.
* **Eficiência Financeira**: Em horários de baixo tráfego, o cluster encolhe e libera recursos.
* **Observabilidade**: O status e métricas do HPA são coletados e exibidos diretamente no dashboard do Datadog / New Relic.

### Negativas / Riscos Mitigados:
* **Tempo de Startup do Pod**: Se o startup do container .NET 8 demorar, as novas réplicas demoram a responder.
  * *Mitigação*: Imagem Docker otimizada utilizando multi-stage build e runtime leve do .NET 8 Alpine.
