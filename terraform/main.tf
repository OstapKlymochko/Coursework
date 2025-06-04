terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
  }
  required_version = ">= 1.0"
}

provider "aws" {
  region = var.region
}

resource "aws_security_group" "ecs_sg" {
  name        = "ecs-sg1"
  description = "Allow outbound internet traffic"
  vpc_id      = var.vpc_id

  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}


module "services" {
  source = "./services"

  ecs_cluster_name            = var.ecs_cluster_name
  vpc_id                      = var.vpc_id
  subnet_ids                  = var.subnet_ids
  region                      = var.region
  ecs_task_execution_role_arn = aws_iam_role.ecs_task_execution_role.arn
  services                    = var.services
  ecs_security_group_id = aws_security_group.ecs_sg.id
}
