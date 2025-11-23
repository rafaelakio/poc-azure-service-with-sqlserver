# Variables for AWS Terraform Configuration - Link Manager

# General
variable "project_name" {
  description = "Project name used for resource naming"
  type        = string
  default     = "linkmanager"
}

variable "environment" {
  description = "Environment (Development, Staging, Production)"
  type        = string
  default     = "Production"
}

variable "aws_region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}

variable "tags" {
  description = "Tags to apply to all resources"
  type        = map(string)
  default = {
    Project     = "LinkManager"
    Environment = "Production"
    ManagedBy   = "Terraform"
  }
}

# VPC and Networking
variable "vpc_cidr" {
  description = "CIDR block for VPC"
  type        = string
  default     = "10.0.0.0/16"
}

variable "public_subnet_1_cidr" {
  description = "CIDR block for public subnet 1"
  type        = string
  default     = "10.0.1.0/24"
}

variable "public_subnet_2_cidr" {
  description = "CIDR block for public subnet 2"
  type        = string
  default     = "10.0.2.0/24"
}

variable "private_subnet_1_cidr" {
  description = "CIDR block for private subnet 1"
  type        = string
  default     = "10.0.10.0/24"
}

variable "private_subnet_2_cidr" {
  description = "CIDR block for private subnet 2"
  type        = string
  default     = "10.0.11.0/24"
}

# RDS SQL Server
variable "rds_instance_class" {
  description = "RDS instance class"
  type        = string
  default     = "db.t3.small"
}

variable "rds_allocated_storage" {
  description = "Allocated storage in GB"
  type        = number
  default     = 20
}

variable "sqlserver_engine_version" {
  description = "SQL Server engine version"
  type        = string
  default     = "15.00.4335.1.v1" # SQL Server 2019 Express
}

variable "database_name" {
  description = "Database name"
  type        = string
  default     = "LinkManagerDb"
}

variable "db_username" {
  description = "Database master username"
  type        = string
  default     = "sqladmin"
  sensitive   = true
}

variable "db_password" {
  description = "Database master password"
  type        = string
  sensitive   = true
}

variable "backup_retention_period" {
  description = "Backup retention period in days"
  type        = number
  default     = 7
}

variable "skip_final_snapshot" {
  description = "Skip final snapshot when destroying"
  type        = bool
  default     = false
}

# ECS
variable "ecs_task_cpu" {
  description = "ECS task CPU units"
  type        = string
  default     = "256" # 0.25 vCPU
}

variable "ecs_task_memory" {
  description = "ECS task memory in MB"
  type        = string
  default     = "512" # 0.5 GB
}

variable "ecs_desired_count" {
  description = "Desired number of ECS tasks"
  type        = number
  default     = 1
}

# CloudWatch
variable "log_retention_days" {
  description = "CloudWatch log retention in days"
  type        = number
  default     = 7
}

# Load Balancer
variable "enable_deletion_protection" {
  description = "Enable deletion protection for ALB"
  type        = bool
  default     = false
}
