#!/usr/bin/env bash
# ==============================================================================
# install.sh — Instala o Metrics Server no cluster (local ou AWS)
# ==============================================================================
# O Metrics Server é pré-requisito para o HPA funcionar.
# No EKS >= 1.23, pode ser instalado como Add-on — mas este script funciona
# para qualquer cluster.
#
# Uso:
#   chmod +x k8s/metrics-server/install.sh
#   ./k8s/metrics-server/install.sh          # auto-detecta o ambiente
#   ./k8s/metrics-server/install.sh local    # força modo local (--kubelet-insecure-tls)
#   ./k8s/metrics-server/install.sh aws      # sem patch
# ==============================================================================
set -euo pipefail

METRICS_SERVER_VERSION="v0.7.2"
METRICS_SERVER_URL="https://github.com/kubernetes-sigs/metrics-server/releases/download/${METRICS_SERVER_VERSION}/components.yaml"

# Auto-detecta ambiente pelo contexto kubectl
CURRENT_CONTEXT=$(kubectl config current-context 2>/dev/null || echo "")
if [ "${1:-auto}" = "local" ] || [[ "$CURRENT_CONTEXT" == *"minikube"* ]] || [[ "$CURRENT_CONTEXT" == *"kind"* ]]; then
  ENV="local"
else
  ENV="aws"
fi

echo "📦 Instalando Metrics Server ${METRICS_SERVER_VERSION} (ambiente: $ENV)..."
kubectl apply -f "$METRICS_SERVER_URL"

if [ "$ENV" = "local" ]; then
  echo "🔧 Aplicando patch --kubelet-insecure-tls (necessário para Minikube/Kind)..."
  kubectl patch deployment metrics-server \
    -n kube-system \
    --type='json' \
    -p='[{
      "op": "add",
      "path": "/spec/template/spec/containers/0/args/-",
      "value": "--kubelet-insecure-tls"
    }]'
  echo "✅ Patch aplicado."
fi

echo "⏳ Aguardando Metrics Server ficar pronto..."
kubectl rollout status deployment/metrics-server -n kube-system --timeout=120s

echo ""
echo "✅ Metrics Server instalado com sucesso!"
echo ""
echo "📊 Verificar métricas dos nodes:"
echo "   kubectl top nodes"
echo ""
echo "📊 Verificar métricas dos pods:"
echo "   kubectl top pods -n techchallenge"
