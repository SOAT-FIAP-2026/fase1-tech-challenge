output "endpoint" {
  description = "Endpoint do RDS PostgreSQL (host:port)"
  value       = aws_db_instance.postgres.endpoint
}

output "host" {
  description = "Hostname do RDS PostgreSQL (sem porta)"
  value       = aws_db_instance.postgres.address
}

output "port" {
  description = "Porta do RDS PostgreSQL"
  value       = aws_db_instance.postgres.port
}

output "db_name" {
  description = "Nome do banco de dados criado"
  value       = aws_db_instance.postgres.db_name
}
