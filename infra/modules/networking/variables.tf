variable "project_name" {
  description = "Nome do projeto — usado como prefixo nos recursos"
  type        = string
}

variable "cidr_block" {
  description = "CIDR block da VPC"
  type        = string
  default     = "10.0.0.0/16"
}

variable "availability_zones" {
  description = "Lista de AZs onde as subnets públicas serão criadas"
  type        = list(string)
}
