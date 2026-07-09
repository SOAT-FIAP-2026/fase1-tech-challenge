# ==============================================================================
# Root Module — Orquestra os módulos de infraestrutura
# ==============================================================================

# --- Data Sources -------------------------------------------------------------
data "aws_iam_user" "terraform_user" {
  user_name = "terraform-user"
}

# --- S3 Bucket (backend de estado — criado antes do backend ser configurado) --
resource "aws_s3_bucket" "state_backend" {
  bucket = var.project_name
}

# --- Módulo: Networking -------------------------------------------------------
module "networking" {
  source = "./modules/networking"

  project_name       = var.project_name
  cidr_block         = var.cidr_block
  availability_zones = var.availability_zones
}

# --- Módulo: IAM --------------------------------------------------------------
module "iam" {
  source = "./modules/iam"

  project_name = var.project_name
}

# --- Módulo: EKS --------------------------------------------------------------
module "eks" {
  source = "./modules/eks"

  project_name        = var.project_name
  cluster_role_arn    = module.iam.cluster_role_arn
  node_group_role_arn = module.iam.node_group_role_arn
  subnet_ids          = module.networking.public_subnet_ids
  security_group_id   = module.networking.main_security_group_id
  instance_types      = var.instance_types
  terraform_user_arn  = data.aws_iam_user.terraform_user.arn

  # Propaga dependências de policy para garantir a ordem correta de create/destroy
  cluster_policy_attachment_dep  = module.iam.cluster_policy_attachment
  node_cni_policy_attachment_dep = module.iam.node_cni_policy_attachment
  node_ecr_policy_attachment_dep = module.iam.node_ecr_policy_attachment
}

# --- Módulo: RDS --------------------------------------------------------------
module "rds" {
  source = "./modules/rds"

  project_name          = var.project_name
  vpc_id                = module.networking.vpc_id
  vpc_cidr_block        = module.networking.vpc_cidr_block
  subnet_ids            = module.networking.public_subnet_ids
  eks_security_group_id = module.networking.main_security_group_id
  db_name               = var.rds_db_name
  db_username           = var.rds_username
  db_password           = var.rds_password
}
