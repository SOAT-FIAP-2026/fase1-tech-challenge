variable "project_name" {
  description = "Nome do projeto — usado como prefixo nos recursos RDS"
  type        = string
}

variable "vpc_id" {
  description = "ID da VPC onde o RDS será provisionado"
  type        = string
}

variable "vpc_cidr_block" {
  description = "CIDR block da VPC — usado na regra de ingress do SG do RDS"
  type        = string
}

variable "subnet_ids" {
  description = "IDs das subnets para o DB Subnet Group (mínimo 2 AZs)"
  type        = list(string)
}

variable "eks_security_group_id" {
  description = "ID do Security Group do EKS — permite acesso ao RDS na porta 5432"
  type        = string
}

variable "db_name" {
  description = "Nome do banco de dados inicial"
  type        = string
  default     = "techchallengedb"
}

variable "db_username" {
  description = "Usuário master do RDS PostgreSQL"
  type        = string
  default     = "postgres"
}

variable "db_password" {
  description = "Senha master do RDS PostgreSQL"
  type        = string
  sensitive   = true
}
