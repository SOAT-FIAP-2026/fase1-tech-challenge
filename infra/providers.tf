terraform {
  required_version = ">= 1.5.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }

  backend "s3" {
    bucket = "fiap-soat-techchallenge-backend"
    key    = "fiap/terraform.tfstate"
    region = "sa-east-1"
  }
}

provider "aws" {
  region = var.default_region
}
