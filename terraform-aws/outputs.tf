# Outputs for AWS Terraform Configuration - Link Manager

output "alb_dns_name" {
  description = "DNS name of the Application Load Balancer"
  value       = aws_lb.main.dns_name
}

output "alb_url" {
  description = "URL of the application"
  value       = "http://${aws_lb.main.dns_name}"
}

output "rds_endpoint" {
  description = "RDS SQL Server endpoint"
  value       = aws_db_instance.sqlserver.endpoint
}

output "rds_database_name" {
  description = "RDS database name"
  value       = aws_db_instance.sqlserver.db_name
}

output "ecr_repository_url" {
  description = "ECR repository URL"
  value       = aws_ecr_repository.app.repository_url
}

output "ecs_cluster_name" {
  description = "ECS cluster name"
  value       = aws_ecs_cluster.main.name
}

output "ecs_service_name" {
  description = "ECS service name"
  value       = aws_ecs_service.app.name
}

output "vpc_id" {
  description = "VPC ID"
  value       = aws_vpc.main.id
}

output "cloudwatch_log_group" {
  description = "CloudWatch log group name"
  value       = aws_cloudwatch_log_group.app.name
}

output "secrets_manager_secret_arn" {
  description = "Secrets Manager secret ARN for database connection"
  value       = aws_secretsmanager_secret.db_connection.arn
  sensitive   = true
}

output "deployment_commands" {
  description = "Commands to deploy the application"
  value = <<-EOT
    # 1. Build Docker image
    docker build -t ${var.project_name}:latest .
    
    # 2. Authenticate Docker to ECR
    aws ecr get-login-password --region ${var.aws_region} | docker login --username AWS --password-stdin ${aws_ecr_repository.app.repository_url}
    
    # 3. Tag image
    docker tag ${var.project_name}:latest ${aws_ecr_repository.app.repository_url}:latest
    
    # 4. Push image to ECR
    docker push ${aws_ecr_repository.app.repository_url}:latest
    
    # 5. Update ECS service
    aws ecs update-service --cluster ${aws_ecs_cluster.main.name} --service ${aws_ecs_service.app.name} --force-new-deployment --region ${var.aws_region}
    
    # 6. View logs
    aws logs tail /ecs/${var.project_name} --follow --region ${var.aws_region}
  EOT
}

output "connection_string" {
  description = "Database connection string (without password)"
  value       = "Server=${aws_db_instance.sqlserver.endpoint};Database=${var.database_name};User Id=${var.db_username};Password=<YOUR_PASSWORD>;Encrypt=True;TrustServerCertificate=True;"
  sensitive   = false
}
