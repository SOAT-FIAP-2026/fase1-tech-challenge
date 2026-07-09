# ==============================================================================
# Módulo: rds
# Responsável por: Security Group do RDS, Subnet Group e RDS PostgreSQL
# ==============================================================================

# --- Security Group: acesso apenas via nodes do EKS ---------------------------
resource "aws_security_group" "rds" {
  name        = "${var.project_name}-rds-sg"
  description = "Permite acesso PostgreSQL apenas a partir dos nodes EKS"
  vpc_id      = var.vpc_id

  ingress {
    description     = "PostgreSQL from EKS nodes (SG declarado no Terraform)"
    from_port       = 5432
    to_port         = 5432
    protocol        = "tcp"
    security_groups = [var.eks_security_group_id]
  }

  ingress {
    description = "PostgreSQL from EKS managed node SG (criado automaticamente pelo EKS)"
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = [var.vpc_cidr_block]
  }

  egress {
    description = "All outbound"
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name    = "${var.project_name}-rds-sg"
    Project = var.project_name
  }
}

# --- Subnet Group: RDS exige pelo menos 2 AZs (mesmo em single-AZ) -----------
resource "aws_db_subnet_group" "main" {
  name        = "${var.project_name}-rds-subnet-group"
  description = "Subnet group para o RDS PostgreSQL do TechChallenge"
  subnet_ids  = var.subnet_ids

  tags = {
    Name    = "${var.project_name}-rds-subnet-group"
    Project = var.project_name
  }
}

# --- RDS Instance (Free Tier elegível) ----------------------------------------
resource "aws_db_instance" "postgres" {
  identifier = "${var.project_name}db"

  # Engine
  engine         = "postgres"
  engine_version = "16.14"

  # Free Tier: db.t3.micro + 20 GB gp2 + single-AZ
  instance_class      = "db.t3.micro"
  allocated_storage   = 20
  storage_type        = "gp2"
  multi_az            = false # single-AZ → free tier
  publicly_accessible = false # acesso apenas via VPC (pelos nodes EKS)

  # Banco de dados inicial
  db_name  = var.db_name
  username = var.db_username
  password = var.db_password

  # Rede
  db_subnet_group_name   = aws_db_subnet_group.main.name
  vpc_security_group_ids = [aws_security_group.rds.id]

  # Backup mínimo (free tier)
  backup_retention_period = 1    # 1 dia de backup automático
  skip_final_snapshot     = true # ⚠️ apenas para ambientes de teste
  deletion_protection     = false # ⚠️ apenas para ambientes de teste

  # Performance Insights desativado (não é free tier)
  performance_insights_enabled = false

  # Evitar downtime em atualizações de maintenance
  auto_minor_version_upgrade = true

  tags = {
    Name        = "${var.project_name}-db"
    Project     = var.project_name
    Environment = "test"
  }

  depends_on = [aws_db_subnet_group.main]
}
