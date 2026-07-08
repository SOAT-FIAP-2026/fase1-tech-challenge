output "vpc_id" {
  description = "ID da VPC"
  value       = aws_vpc.main.id
}

output "vpc_cidr_block" {
  description = "CIDR block da VPC"
  value       = aws_vpc.main.cidr_block
}

output "public_subnet_ids" {
  description = "IDs das subnets públicas"
  value       = aws_subnet.public[*].id
}

output "public_subnet_cidr_blocks" {
  description = "CIDR blocks das subnets públicas"
  value       = aws_subnet.public[*].cidr_block
}

output "main_security_group_id" {
  description = "ID do Security Group principal do cluster"
  value       = aws_security_group.main.id
}
