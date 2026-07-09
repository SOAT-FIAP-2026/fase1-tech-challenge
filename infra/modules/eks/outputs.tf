output "cluster_name" {
  description = "Nome do cluster EKS — usado pelo kubectl e CI/CD"
  value       = aws_eks_cluster.main.name
}

output "cluster_endpoint" {
  description = "Endpoint do API server do EKS"
  value       = aws_eks_cluster.main.endpoint
}

output "cluster_ca_certificate" {
  description = "Certificado CA do cluster (base64) — necessário para autenticação kubectl"
  value       = aws_eks_cluster.main.certificate_authority[0].data
  sensitive   = true
}
