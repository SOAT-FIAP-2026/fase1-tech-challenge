#!/usr/bin/env bash
# ==============================================================================
# deploy-aws.sh — Deploy no ambiente AWS (EKS)
# ==============================================================================
# Uso:
#   chmod +x k8s/overlays/aws/deploy.sh
#   ./k8s/overlays/aws/deploy.sh
#
# Pré-requisitos:
#   - aws eks update-kubeconfig --name eks-fiap-soat-terraform --region sa-east-1
#   - metrics-server instalado no EKS (já incluso no EKS >= 1.23 via add-on)
#   - terraform apply executado (RDS e EKS provisionados)
# ==============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BASE_DIR="$SCRIPT_DIR/../../base"
OVERLAY_DIR="$SCRIPT_DIR"

echo "🚀 Deploy AWS (EKS)"

# --- 1. Verifica contexto kubectl ------------------------------------------
CURRENT_CONTEXT=$(kubectl config current-context)
echo "📡 Contexto kubectl: $CURRENT_CONTEXT"

if [[ "$CURRENT_CONTEXT" != *"eks"* ]] && [[ "$CURRENT_CONTEXT" != *"aws"* ]]; then
  echo "⚠️  Aviso: contexto não parece ser EKS. Confirme com 'kubectl config get-contexts'"
  read -r -p "   Continuar mesmo assim? (s/N): " confirm
  [[ "$confirm" =~ ^[sS]$ ]] || exit 1
fi

# --- 2. Namespace ----------------------------------------------------------
echo "📁 Aplicando namespace..."
kubectl apply -f "$BASE_DIR/namespace.yaml"

# --- 3. ConfigMap e Secrets -----------------------------------------------
echo "🔧 Aplicando ConfigMap e Secrets..."
kubectl apply -f "$OVERLAY_DIR/configmap.yaml"
kubectl apply -f "$OVERLAY_DIR/secrets.yaml"

# --- 4. Deployment, Service e HPA ----------------------------------------
echo "🌐 Aplicando Deployment, Service e HPA..."
kubectl apply -f "$BASE_DIR/deployment.yaml"
kubectl apply -f "$OVERLAY_DIR/service.yaml"
kubectl apply -f "$BASE_DIR/hpa.yaml"
kubectl apply -f "$BASE_DIR/ingress.yaml"

# --- 5. Aguarda API ficar pronta ------------------------------------------
echo "⏳ Aguardando Deployment ficar pronto..."
kubectl rollout status deployment/api -n techchallenge --timeout=300s

# --- 6. Obtém endpoint do LoadBalancer ------------------------------------
echo ""
echo "⏳ Aguardando LoadBalancer receber IP/hostname (pode levar ~1 min)..."
for i in {1..12}; do
  LB_HOST=$(kubectl get svc api-service -n techchallenge \
    -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || true)
  LB_IP=$(kubectl get svc api-service -n techchallenge \
    -o jsonpath='{.status.loadBalancer.ingress[0].ip}' 2>/dev/null || true)

  if [ -n "$LB_HOST" ] || [ -n "$LB_IP" ]; then
    ENDPOINT="${LB_HOST:-$LB_IP}"
    break
  fi
  echo "   Tentativa $i/12 — aguardando..."
  sleep 10
done

echo ""
echo "✅ Deploy concluído!"
echo ""
echo "📡 Acesso à API:"
echo "   http://${ENDPOINT:-<aguardando-lb>}/swagger"
echo ""
echo "📊 Monitorar HPA:"
echo "   kubectl get hpa -n techchallenge -w"
echo ""
echo "🔥 Gerar carga para testar HPA:"
echo "   kubectl run load-gen --image=busybox --restart=Never -n techchallenge -- \\"
echo "     /bin/sh -c 'while true; do wget -q -O- http://api-service/api/ping; done'"
