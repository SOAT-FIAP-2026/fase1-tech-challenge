# ==============================================================================
# Módulo: eks
# Responsável por: EKS Cluster, Node Group e Access Entries
# ==============================================================================

# --- EKS Cluster --------------------------------------------------------------
resource "aws_eks_cluster" "main" {
  name    = "eks-${var.project_name}"
  version = "1.35"

  access_config {
    authentication_mode = "API"
  }

  role_arn = var.cluster_role_arn

  vpc_config {
    subnet_ids         = var.subnet_ids
    security_group_ids = [var.security_group_id]
  }

  # Garante que as policies IAM existam antes de criar/destruir o cluster
  depends_on = [var.cluster_policy_attachment_dep]

  tags = {
    Name    = "eks-${var.project_name}"
    Project = var.project_name
  }
}

# --- EKS Node Group -----------------------------------------------------------
resource "aws_eks_node_group" "main" {
  cluster_name    = aws_eks_cluster.main.name
  node_group_name = "node-group-${var.project_name}"
  node_role_arn   = var.node_group_role_arn
  subnet_ids      = var.subnet_ids
  disk_size       = 20
  instance_types  = var.instance_types

  scaling_config {
    desired_size = 2
    max_size     = 3
    min_size     = 1
  }

  update_config {
    max_unavailable = 1
  }

  # Garante que as policies IAM existam antes de criar/destruir o node group
  depends_on = [
    var.cluster_policy_attachment_dep,
    var.node_cni_policy_attachment_dep,
    var.node_ecr_policy_attachment_dep,
  ]

  tags = {
    Name    = "node-group-${var.project_name}"
    Project = var.project_name
  }
}

# --- Access Entry: permissão de acesso ao cluster via IAM User ----------------
resource "aws_eks_access_entry" "terraform_user" {
  cluster_name      = aws_eks_cluster.main.name
  principal_arn     = var.terraform_user_arn
  kubernetes_groups = ["group-1", "group-2"]
  type              = "STANDARD"
}

resource "aws_eks_access_policy_association" "cluster_admin" {
  cluster_name  = aws_eks_cluster.main.name
  policy_arn    = "arn:aws:eks::aws:cluster-access-policy/AmazonEKSClusterAdminPolicy"
  principal_arn = var.terraform_user_arn

  access_scope {
    type = "cluster"
  }
}
