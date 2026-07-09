# ==============================================================================
# Outputs do Root Module — agrega outputs de todos os módulos
# ==============================================================================

# ── Networking ────────────────────────────────────────────────────────────────
output "vpc_id" {
  description = "ID da VPC criada"
  value       = module.networking.vpc_id
}

output "vpc_cidr_block" {
  description = "CIDR block da VPC"
  value       = module.networking.vpc_cidr_block
}

output "subnet_ids" {
  description = "IDs das subnets públicas"
  value       = module.networking.public_subnet_ids
}

output "subnet_cidr_blocks" {
  description = "CIDR blocks das subnets públicas"
  value       = module.networking.public_subnet_cidr_blocks
}

# ── EKS ───────────────────────────────────────────────────────────────────────
output "eks_cluster_name" {
  description = "Nome do cluster EKS — usado pelo kubectl e CI/CD"
  value       = module.eks.cluster_name
}

output "eks_cluster_endpoint" {
  description = "Endpoint do API server do EKS"
  value       = module.eks.cluster_endpoint
}

output "eks_cluster_ca_certificate" {
  description = "Certificado CA do cluster (base64)"
  value       = module.eks.cluster_ca_certificate
  sensitive   = true
}

# ── RDS ───────────────────────────────────────────────────────────────────────
output "rds_endpoint" {
  description = "Endpoint do RDS PostgreSQL (host:port)"
  value       = module.rds.endpoint
}

output "rds_host" {
  description = "Hostname do RDS PostgreSQL (sem porta)"
  value       = module.rds.host
}

output "rds_port" {
  description = "Porta do RDS PostgreSQL"
  value       = module.rds.port
}

output "rds_db_name" {
  description = "Nome do banco de dados criado no RDS"
  value       = module.rds.db_name
}

output "rds_connection_string" {
  description = "Connection string completa para uso no .NET (Entity Framework / Npgsql)"
  value       = "Host=${module.rds.host};Port=${module.rds.port};Database=${module.rds.db_name};Username=${var.rds_username};Password=${var.rds_password}"
  sensitive   = true
}