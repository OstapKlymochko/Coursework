resource "aws_ecr_repository" "repo" {
  for_each = var.services
  name     = each.value.repository_name
}

resource "aws_cloudwatch_log_group" "ecs_logs" {
  for_each          = var.services
  name              = "/ecs/${each.value.ecs_service_name}"
  retention_in_days = 7
}

resource "aws_ecs_cluster" "cluster" {
  name = var.ecs_cluster_name
}

resource "aws_service_discovery_private_dns_namespace" "namespace" {
  name        = "local"
  vpc         = var.vpc_id
  description = "Private DNS namespace for ECS services"
}

resource "aws_service_discovery_service" "service" {
  for_each = var.services

  name         = each.value.cloudmap_service_name
  namespace_id = aws_service_discovery_private_dns_namespace.namespace.id

  dns_config {
    namespace_id   = aws_service_discovery_private_dns_namespace.namespace.id
    routing_policy = "MULTIVALUE"
    dns_records {
      ttl  = 10
      type = "A"
    }
  }

  health_check_custom_config {
    failure_threshold = 1
  }
}

resource "aws_ecs_task_definition" "task" {
  for_each = var.services

  family                   = each.value.task_family
  cpu                      = "256"
  memory                   = "512"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  execution_role_arn       = var.ecs_task_execution_role_arn

  container_definitions = jsonencode([{
    name      = each.key
    image     = "${aws_ecr_repository.repo[each.key].repository_url}:${each.value.image_tag}"
    essential = true
    portMappings = [{
      containerPort = each.value.container_port
      protocol      = "tcp"
    }]
    logConfiguration = {
      logDriver = "awslogs"
      options = {
        "awslogs-group"         = "/ecs/${each.value.ecs_service_name}"
        "awslogs-region"        = var.region
        "awslogs-stream-prefix" = each.key
      }
    }
  }])
}

resource "aws_ecs_service" "service" {
  for_each        = var.services
  name            = each.value.ecs_service_name
  cluster         = aws_ecs_cluster.cluster.id
  task_definition = aws_ecs_task_definition.task[each.key].arn
  desired_count   = 1
  launch_type     = "FARGATE"
  platform_version = "LATEST"

  network_configuration {
    subnets          = var.subnet_ids
    assign_public_ip = true
    security_groups  = [var.ecs_security_group_id]
  }

  service_registries {
    registry_arn = aws_service_discovery_service.service[each.key].arn
  }
}
