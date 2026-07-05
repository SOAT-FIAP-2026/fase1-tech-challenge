# ☸️ Kubernetes — Guia de Provisionamento Local

Este guia descreve como subir o cluster Kubernetes local utilizando **Kind** (Kubernetes in Docker) e aplicar todos os manifestos do projeto.

## Pré-requisitos

- [Docker](https://docs.docker.com/get-docker/) instalado e rodando
- [Kind](https://kind.sigs.k8s.io/docs/user/quick-start/#installation) instalado
- [kubectl](https://kubernetes.io/docs/tasks/tools/) instalado

## 1. Criar o cluster

```bash
kind create cluster --name tech-challenge-cluster
```

## 2. Preparar o node para o Ingress Controller

```bash
kubectl label node tech-challenge-cluster-control-plane ingress-ready="true" --overwrite
```

## 3. Aplicar todos os manifestos

```bash
kubectl apply -f k8s/
```

> **Nota:** Este comando aplica todos os arquivos `.yaml` da pasta de uma vez. O Kubernetes resolve automaticamente a ordem correta (Namespace → Secrets → ConfigMaps → Services → Deployments → HPA → Ingress).

## 4. Verificar se tudo subiu corretamente

```bash
kubectl get pods,svc -n techchallenge
```

Resultado esperado:

```
NAME                          READY   STATUS    AGE
pod/api-xxxxx                 1/1     Running   ...
pod/api-xxxxx                 1/1     Running   ...
pod/postgres-xxxxx            1/1     Running   ...

NAME                       TYPE        PORT(S)          AGE
service/api-service        NodePort    8080:30081/TCP   ...
service/postgres-service   ClusterIP   5432/TCP         ...
```

## 5. Acessar a API

### Opção A — Port-Forward (recomendado para dev)

```bash
kubectl port-forward svc/api-service 8080:8080 -n techchallenge
```

Acesse: [http://localhost:8080/api/ping](http://localhost:8080/api/ping)

### Opção B — Via Ingress (simula produção)

```bash
kubectl port-forward -n ingress-nginx svc/ingress-nginx-controller 8080:80
```

```bash
curl -H "Host: api.techchallenge.local" http://localhost:8080/api/ping
```

## Comandos úteis

| Comando | Descrição |
|---|---|
| `kubectl get pods -n techchallenge` | Lista os pods |
| `kubectl logs -f deploy/api -n techchallenge` | Ver logs da API em tempo real |
| `kubectl describe pod <nome> -n techchallenge` | Detalhes de um pod específico |
| `kubectl delete -f k8s/` | Remove todos os recursos |
| `kind delete cluster --name tech-challenge-cluster` | Destrói o cluster inteiro |

## Carregar imagem local (se necessário)

Se a imagem Docker não estiver no Docker Hub (ou estiver com `imagePullPolicy: IfNotPresent`), carregue-a manualmente no cluster:

```bash
kind load docker-image gabrielnetto94/techchallenge-api:latest --name tech-challenge-cluster
```

## Estrutura dos manifestos

```
k8s/
├── namespace.yaml            # Namespace isolado (techchallenge)
├── configmap.yaml            # Variáveis de configuração (API + Postgres)
├── secret.yaml               # Credenciais sensíveis (JWT, connection string)
├── dockerhub-secret.yaml     # Credenciais do Docker Hub (não versionado)
├── postgres-deployment.yaml  # Deployment + Service + PVC do banco
├── api-deployment.yaml       # Deployment da API (.NET)
├── api-service.yaml          # Service da API (NodePort)
├── api-ingress.yaml          # Regras de roteamento do Ingress
├── ingress-controller.yaml   # NGINX Ingress Controller
├── hpa.yaml                  # Horizontal Pod Autoscaler (CPU/memória)
└── readme.md                 # Este arquivo
```
