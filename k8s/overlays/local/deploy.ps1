param(
    [ValidateSet("kind", "minikube")]
    [string]$ClusterType = "kind"
)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $PSCommandPath
$baseDir = Resolve-Path (Join-Path $scriptDir "../../base")

if ($ClusterType -eq "kind") {
    $kindCommand = Get-Command kind -ErrorAction SilentlyContinue
    if ($null -ne $kindCommand) {
        & $kindCommand.Source load docker-image gabrielnetto94/techchallenge-api:latest --name techchallenge
    }
    else {
        $kindPath = "${env:LOCALAPPDATA}\Microsoft\WinGet\Packages\Kubernetes.kind_Microsoft.Winget.Source_8wekyb3d8bbwe\kind.exe"
        if (-not (Test-Path -LiteralPath $kindPath)) {
            throw "Kind não encontrado. Instale-o e reinicie o terminal, ou adicione-o ao PATH."
        }

        & $kindPath load docker-image gabrielnetto94/techchallenge-api:latest --name techchallenge
    }
}

kubectl apply -f (Join-Path $baseDir "namespace.yaml")
kubectl apply -f (Join-Path $scriptDir "postgres.yaml")
kubectl rollout status deployment/postgres -n techchallenge --timeout=120s
# O secrets.yaml nao e versionado: e gerado do template. Os valores padrao
# servem apenas para o cluster local descartavel.
if (-not $env:JWT_SECRET) { $env:JWT_SECRET = "chave-local-de-desenvolvimento-32-chars" }
if (-not $env:POSTGRES_PASSWORD) { $env:POSTGRES_PASSWORD = "postgres" }

$secretTemplate = Join-Path $scriptDir "secrets.yaml.template"
$secretFile = Join-Path $scriptDir "secrets.yaml"
(Get-Content $secretTemplate -Raw).
    Replace('${JWT_SECRET}', $env:JWT_SECRET).
    Replace('${POSTGRES_PASSWORD}', $env:POSTGRES_PASSWORD) |
    Set-Content -Path $secretFile -Encoding UTF8

kubectl apply -f (Join-Path $scriptDir "configmap.yaml")
kubectl apply -f $secretFile
kubectl apply -f (Join-Path $baseDir "deployment.yaml")
kubectl apply -f (Join-Path $scriptDir "service.yaml")
kubectl apply -f (Join-Path $baseDir "hpa.yaml")
kubectl rollout status deployment/api -n techchallenge --timeout=180s

Write-Host "Deploy local concluído. Acesse a API com: kubectl port-forward svc/api-service 8080:80 -n techchallenge"
