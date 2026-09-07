#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_ROOT="$SCRIPT_DIR/../.."

echo "Aplicando ServiceMonitor e alertas do Tech Challenge..."
kubectl apply -f "$SCRIPT_DIR/servicemonitor.yaml"
kubectl apply -f "$SCRIPT_DIR/prometheusrule.yaml"
kubectl apply -f "$SCRIPT_DIR/probe.yaml"

DASHBOARD_FILE="$REPO_ROOT/observability/grafana/dashboards/techchallenge-observability.json"
if [[ ! -f "$DASHBOARD_FILE" ]]; then
  echo "Dashboard nao encontrado: $DASHBOARD_FILE" >&2
  exit 1
fi

echo "Publicando dashboard no Grafana..."
kubectl create configmap techchallenge-grafana-dashboard --namespace techchallenge --from-file=techchallenge-observability.json="$DASHBOARD_FILE" --dry-run=client -o yaml | kubectl apply -f -
kubectl label configmap techchallenge-grafana-dashboard --namespace techchallenge grafana_dashboard=1 --overwrite

echo "Observabilidade aplicada."
echo "Grafana: kubectl port-forward svc/monitoring-grafana 3000:80 -n monitoring"
echo "Prometheus: kubectl port-forward svc/monitoring-kube-prometheus-prometheus 9090:9090 -n monitoring"
