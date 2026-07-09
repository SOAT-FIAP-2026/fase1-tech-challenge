# Kubernetes — Manifests

Manifests para deploy da API no Kubernetes, organizados com o padrão **base + overlays** para suportar ambientes distintos (local e AWS).

## Estrutura

```
k8s/
├── base/                      # Recursos compartilhados entre ambientes
│   ├── namespace.yaml         # Namespace "techchallenge"
│   ├── deployment.yaml        # Deployment da API (imagem, probes, resources)
│   ├── hpa.yaml               # HPA — autoscaling (1→5 réplicas, CPU 50%)
│   ├── ingress.yaml           # Ingress (roteamento HTTP)
│   └── configmap.yaml         # ConfigMap base
│
├── overlays/
│   ├── local/                 # Overlay para Minikube / Kind
│   │   ├── postgres.yaml      # PostgreSQL in-cluster (dev only)
│   │   ├── configmap.yaml     # ConfigMap apontando para postgres-service
│   │   ├── secrets.yaml       # Secrets locais (base64)
│   │   ├── service.yaml       # Service NodePort
│   │   └── deploy.sh          # Script de deploy automatizado
│   │
│   └── aws/                   # Overlay para EKS
│       ├── configmap.yaml     # ConfigMap apontando para RDS
│       ├── secrets.yaml       # Secrets com credenciais AWS (base64)
│       ├── service.yaml       # Service LoadBalancer
│       └── deploy.sh          # Script de deploy automatizado
│
└── metrics-server/
    └── install.sh             # Instala o Metrics Server (necessário para HPA)
```

**Base** contém o Deployment, HPA e Namespace — comuns a qualquer ambiente.  
**Overlays** sobrescrevem ConfigMap, Secrets e Service conforme o ambiente.

---

## Deploy Local (Minikube / Kind)

### Pré-requisitos

- [Minikube](https://minikube.sigs.k8s.io/) ou [Kind](https://kind.sigs.k8s.io/)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)
- Metrics Server instalado (`./k8s/metrics-server/install.sh`)

### Deploy automatizado

```bash
# Minikube (padrão)
./k8s/overlays/local/deploy.sh

# Kind
./k8s/overlays/local/deploy.sh kind
```

O script aplica tudo na ordem correta: namespace → postgres → configmap/secrets → deployment/service/hpa.

### Deploy manual (passo a passo)

```bash
kubectl apply -f k8s/base/namespace.yaml
kubectl apply -f k8s/overlays/local/postgres.yaml
kubectl apply -f k8s/overlays/local/configmap.yaml
kubectl apply -f k8s/overlays/local/secrets.yaml
kubectl apply -f k8s/base/deployment.yaml
kubectl apply -f k8s/overlays/local/service.yaml
kubectl apply -f k8s/base/hpa.yaml
```

### Acessar a API

```bash
# Port-forward (funciona em Minikube e Kind)
kubectl port-forward svc/api-service 8080:80 -n techchallenge

# Swagger: http://localhost:8080/swagger

# Minikube: alternativa via NodePort
minikube service api-service -n techchallenge
```

---

## Deploy AWS (EKS)

### Pré-requisitos

- Infraestrutura provisionada via Terraform ([ver infra/](../infra/))
- `kubectl` configurado para o cluster EKS:
  ```bash
  aws eks update-kubeconfig --name eks-fiap-soat-terraform --region sa-east-1
  ```

### Deploy automatizado

```bash
./k8s/overlays/aws/deploy.sh
```

### Deploy manual

```bash
kubectl apply -f k8s/base/namespace.yaml
kubectl apply -f k8s/overlays/aws/configmap.yaml
kubectl apply -f k8s/overlays/aws/secrets.yaml
kubectl apply -f k8s/base/deployment.yaml
kubectl apply -f k8s/overlays/aws/service.yaml
kubectl apply -f k8s/base/hpa.yaml
kubectl apply -f k8s/base/ingress.yaml
```

> [!IMPORTANT]
> No overlay AWS **não** existe `postgres.yaml` — o banco é o RDS provisionado pelo Terraform.

---

## Comandos Úteis

```bash
# Ver todos os recursos do namespace
kubectl get all -n techchallenge

# Logs da API em tempo real
kubectl logs -l app.kubernetes.io/name=api -n techchallenge -f

# Detalhes de um pod com erro
kubectl describe pod <nome-do-pod> -n techchallenge

# Reiniciar pods (sem deletar)
kubectl rollout restart deployment/api -n techchallenge

# Status do HPA
kubectl get hpa -n techchallenge -w

# Testar HPA com carga artificial
kubectl run load-gen --image=busybox --restart=Never -n techchallenge -- \
  /bin/sh -c 'while true; do wget -q -O- http://api-service/api/ping; done'

# Remover pod de carga
kubectl delete pod load-gen -n techchallenge
```

---

## Secrets

Os Secrets ficam nos overlays (`overlays/<env>/secrets.yaml`) e os valores são codificados em **base64**:

```bash
# Codificar
echo -n "meu valor" | base64 -w 0

# Decodificar um secret do cluster
kubectl get secret api-secret -n techchallenge \
  -o jsonpath='{.data.ConnectionStrings__DefaultConnection}' | base64 -d
```

O Deployment consome Secrets e ConfigMaps via `envFrom`, injetando todas as chaves como variáveis de ambiente no container.

> [!WARNING]
> Secrets K8s são base64 (codificação, **não** criptografia). Em produção, use Sealed Secrets, External Secrets Operator ou AWS Secrets Manager.

---

## Atualização de Imagem

```bash
# Build e push da nova versão
docker build -t gabrielnetto94/techchallenge-api:v2 .
docker push gabrielnetto94/techchallenge-api:v2

# Atualizar o deployment
kubectl set image deployment/api api=gabrielnetto94/techchallenge-api:v2 -n techchallenge
```

---

## Remover Tudo

```bash
# Deletar todos os recursos do namespace
kubectl delete -f k8s/base/ -f k8s/overlays/local/   # local
kubectl delete -f k8s/base/ -f k8s/overlays/aws/     # aws
```
