output "cluster_role_arn" {
  description = "ARN da IAM Role do EKS Control Plane"
  value       = aws_iam_role.cluster.arn
}

output "cluster_role_name" {
  description = "Nome da IAM Role do EKS Control Plane"
  value       = aws_iam_role.cluster.name
}

output "node_group_role_arn" {
  description = "ARN da IAM Role do Node Group"
  value       = aws_iam_role.node_group.arn
}

output "cluster_policy_attachment" {
  description = "Referência ao policy attachment do cluster (para depends_on)"
  value       = aws_iam_role_policy_attachment.cluster_AmazonEKSClusterPolicy
}

output "node_cni_policy_attachment" {
  description = "Referência ao CNI policy attachment (para depends_on)"
  value       = aws_iam_role_policy_attachment.node_group_AmazonEKS_CNI_Policy
}

output "node_ecr_policy_attachment" {
  description = "Referência ao ECR policy attachment (para depends_on)"
  value       = aws_iam_role_policy_attachment.node_group_AmazonEC2ContainerRegistryReadOnly
}
