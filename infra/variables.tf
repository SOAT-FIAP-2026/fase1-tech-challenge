# ==============================================================================
# Variáveis do Root Module
# ==============================================================================

variable "default_region" {
  description = "Região AWS padrão"
  type        = string
  default     = "sa-east-1"
}

variable "project_name" {
  description = "Nome do projeto — usado como prefixo em todos os recursos AWS"
  type        = string
  default     = "fiap-soat-terraform"
}

variable "cidr_block" {
  description = "CIDR block da VPC"
  type        = string
  default     = "10.0.0.0/16"
}

variable "availability_zones" {
  description = "Lista de AZs disponíveis na região (subnets usam as 2 primeiras)"
  type        = list(string)
  default     = ["sa-east-1a", "sa-east-1b", "sa-east-1c"]
}

variable "instance_types" {
  description = "Tipos de instância EC2 para os nodes do EKS"
  type        = list(string)
  default     = ["t3.micro"]
}

# ── RDS ───────────────────────────────────────────────────────────────────────
variable "rds_db_name" {
  description = "Nome do banco de dados inicial no RDS"
  type        = string
  default     = "techchallengedb"
}

variable "rds_username" {
  description = "Usuário master do RDS PostgreSQL"
  type        = string
  default     = "postgres"
}

variable "rds_password" {
  description = "Senha master do RDS PostgreSQL — defina via TF_VAR_rds_password ou terraform.tfvars"
  type        = string
  sensitive   = true
}
