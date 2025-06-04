region               = "eu-north-1"
ecs_cluster_name     = "dyplom-cluster"
vpc_id               = "vpc-05d4b8421beedd59b"
subnet_ids           = ["subnet-03285335b2d7a1930", "subnet-03285335b2d7a1930"]
ecs_task_execution_role_arn = "arn:aws:iam::123456789012:role/ecsTaskExecutionRole"  # заміни на свій ARN

services = {
  api_gateway = {
    task_family           = "api-gateway-task"
    container_port        = 5000
    repository_name       = "apigateway-service"
    image_tag             = "latest"
    ecs_service_name      = "api-gateway-service"
    cloudmap_service_name = "api-gateway"
  }
  auth_service = {
    task_family           = "auth-task"
    container_port        = 5001
    repository_name       = "auth-service"
    image_tag             = "latest"
    ecs_service_name      = "auth-service"
    cloudmap_service_name = "auth-service"
  }
  user_service = {
    task_family           = "user-task"
    container_port        = 5002
    repository_name       = "user-service"
    image_tag             = "latest"
    ecs_service_name      = "user-service"
    cloudmap_service_name = "user-service"
  }
  music_service = {
    task_family           = "music-task"
    container_port        = 5003
    repository_name       = "music-service"
    image_tag             = "latest"
    ecs_service_name      = "music-service"
    cloudmap_service_name = "music-service"
  }
  files_service = {
    task_family           = "files-task"
    container_port        = 5004
    repository_name       = "files-service"
    image_tag             = "latest"
    ecs_service_name      = "files-service"
    cloudmap_service_name = "files-service"
  }
  recommendation_service = {
    task_family           = "recommendation-task"
    container_port        = 5005
    repository_name       = "recommendation-service"
    image_tag             = "latest"
    ecs_service_name      = "recommendation-service"
    cloudmap_service_name = "recommendation-service"
  }
}
