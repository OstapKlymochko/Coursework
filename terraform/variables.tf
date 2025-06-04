variable "region" {
  type        = string
  description = "AWS region"
}

variable "ecs_cluster_name" {
  type        = string
  description = "Name of ECS cluster"
}

variable "vpc_id" {
  type        = string
  description = "VPC ID"
}

variable "subnet_ids" {
  type        = list(string)
  description = "List of subnet IDs"
}

variable "ecs_task_execution_role_arn" {
  type        = string
  description = "ARN of ECS task execution IAM role"
}

variable "services" {
  type = map(object({
    task_family          : string
    container_port       : number
    repository_name      : string
    image_tag            : string
    ecs_service_name     : string
    cloudmap_service_name: string
  }))
  description = "Map of ECS services definitions"
}
