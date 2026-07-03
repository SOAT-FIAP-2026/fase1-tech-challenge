kind create cluster --name meu-cluster-local

//apply changes

kubectl apply -f .

// port mapping

kubectl port-forward svc/api-service 8081:8080 -n techchallenge