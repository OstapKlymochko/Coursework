variable "ecs_cluster_name" {
  type = string
}

variable "vpc_id" {
  type = string
}

variable "subnet_ids" {
  type = list(string)
}

variable "region" {
  type = string
}

variable "ecs_task_execution_role_arn" {
  type = string
}

variable "services" {
  type = map(object({
    repository_name    : string
    image_tag          : string
    container_port     : number
    task_family        : string
    ecs_service_name   : string
    cloudmap_service_name : string
  }))
}


variable "ecs_security_group_id" {
  type = string
  description = "ID security group для ECS сервісів"
}