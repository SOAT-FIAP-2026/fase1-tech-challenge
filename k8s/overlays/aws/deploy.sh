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

# secrets.yaml nao e versionado. Se nao existir, busca os valores automaticamente da AWS:
if [ ! -f "$OVERLAY_DIR/secrets.yaml" ]; then
  echo "🔍 secrets.yaml não encontrado. Tentando obter dados automaticamente da AWS..."
  
  # 1. Busca endpoint do RDS PostgreSQL
  RDS_HOST=$(aws rds describe-db-instances \
    --db-instance-identifier "fiap-soat-terraform-prod-db" \
    --region sa-east-1 \
    --query "DBInstances[0].Endpoint.Address" \
    --output text 2>/dev/null || true)
  
  if [ -n "$RDS_HOST" ] && [ "$RDS_HOST" != "None" ]; then
    echo "   ✅ RDS PostgreSQL encontrado: $RDS_HOST"
    DB_CONN="Host=${RDS_HOST};Port=5432;Database=techchallengedb;Username=postgres;Password=TechChallenge2026!#$%"
  else
    echo "   ⚠️ RDS não encontrado via AWS CLI. Usando placeholder temporário..."
    DB_CONN="Host=placeholder.rds.amazonaws.com;Port=5432;Database=techchallengedb;Username=postgres;Password=placeholder"
  fi

  # 2. Gera os valores em Base64 (usando -w0 para evitar quebra de linha na coluna 76)
  export JWT_SECRET_BASE64=$(echo -n "chave-secreta-jwt-techchallenge-fiap-2026-segura" | base64 -w0)
  export DB_CONNECTION_BASE64=$(echo -n "$DB_CONN" | base64 -w0)

  if [ -f ~/.docker/config.json ]; then
    export DOCKER_CONFIG_JSON_BASE64=$(cat ~/.docker/config.json | base64 -w0)
  else
    export DOCKER_CONFIG_JSON_BASE64=$(echo -n '{"auths":{}}' | base64)
  fi

  # 3. Substitui as variáveis no template via envsubst
  envsubst < "$OVERLAY_DIR/secrets.yaml.template" > "$OVERLAY_DIR/secrets.yaml"
  echo "   ✅ $OVERLAY_DIR/secrets.yaml gerado com sucesso!"
fi
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

# --- 6. Obtém endpoint do LoadBalancer / ALB ------------------------------
echo ""
echo "⏳ Obtendo endpoint público do Application Load Balancer..."
ENDPOINT=$(aws elbv2 describe-load-balancers \
  --region sa-east-1 \
  --query "LoadBalancers[?contains(LoadBalancerName, 'alb')].DNSName" \
  --output text 2>/dev/null | awk '{print $1}' || true)

if [ -z "$ENDPOINT" ]; then
  LB_HOST=$(kubectl get svc api-service -n techchallenge \
    -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || true)
  ENDPOINT="${LB_HOST:-}"
fi

echo ""
echo "✅ Deploy concluído!"
echo ""
echo "📡 Acesso à API:"
echo "   http://${ENDPOINT:-<consulte-outputs-terraform>}/swagger"
echo ""
echo "📊 Monitorar HPA:"
echo "   kubectl get hpa -n techchallenge -w"
echo ""
echo "🔥 Gerar carga para testar HPA:"
echo "   kubectl run load-gen --image=busybox --restart=Never -n techchallenge -- \\"
echo "     /bin/sh -c 'while true; do wget -q -O- http://api-service/api/ping; done'"
echo ""
echo " 🔥 Remover container load-gen:"
echo "kubectl delete pod load-gen -n techchallenge"
