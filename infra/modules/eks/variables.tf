variable "project_name" {
  description = "Nome do projeto — usado como prefixo nos recursos EKS"
  type        = string
}

variable "cluster_role_arn" {
  description = "ARN da IAM Role do EKS Control Plane"
  type        = string
}

variable "node_group_role_arn" {
  description = "ARN da IAM Role do Node Group"
  type        = string
}

variable "subnet_ids" {
  description = "IDs das subnets onde o cluster e node group serão provisionados"
  type        = list(string)
}

variable "security_group_id" {
  description = "ID do Security Group principal associado ao cluster"
  type        = string
}

variable "instance_types" {
  description = "Tipos de instância EC2 para os nodes do cluster"
  type        = list(string)
  default     = ["t3.micro"]
}

variable "terraform_user_arn" {
  description = "ARN do IAM User que receberá acesso de admin ao cluster"
  type        = string
}

# Dependências explícitas dos policy attachments (passadas como any para depends_on)
variable "cluster_policy_attachment_dep" {
  description = "Referência ao cluster IAM policy attachment — garante ordem de criação/destruição"
  type        = any
}

variable "node_cni_policy_attachment_dep" {
  description = "Referência ao CNI policy attachment do node group"
  type        = any
}

variable "node_ecr_policy_attachment_dep" {
  description = "Referência ao ECR policy attachment do node group"
  type        = any
}
