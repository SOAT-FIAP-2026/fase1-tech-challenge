$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $PSCommandPath
$repoRoot = Resolve-Path (Join-Path $scriptDir "../..")
$dashboardFile = Join-Path $repoRoot "observability/grafana/dashboards/techchallenge-observability.json"

kubectl apply -f (Join-Path $scriptDir "servicemonitor.yaml")
kubectl apply -f (Join-Path $scriptDir "prometheusrule.yaml")
kubectl apply -f (Join-Path $scriptDir "probe.yaml")

kubectl create configmap techchallenge-grafana-dashboard `
    --namespace techchallenge `
    --from-file="techchallenge-observability.json=$dashboardFile" `
    --dry-run=client -o yaml | kubectl apply -f -
kubectl label configmap techchallenge-grafana-dashboard `
    --namespace techchallenge grafana_dashboard=1 --overwrite

Write-Host "Observabilidade aplicada. Grafana: kubectl port-forward svc/monitoring-grafana 3000:80 -n monitoring"
