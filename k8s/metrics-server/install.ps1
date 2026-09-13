$ErrorActionPreference = "Stop"

helm repo add metrics-server https://kubernetes-sigs.github.io/metrics-server/ --force-update
helm upgrade --install metrics-server metrics-server/metrics-server `
    --namespace kube-system `
    --set args[0]=--kubelet-insecure-tls `
    --wait

kubectl rollout status deployment/metrics-server -n kube-system --timeout=180s
Write-Host "Metrics Server instalado. Confirme com: kubectl top nodes"
