# Variáveis do Terraform para infraestrutura Azure

# Geral
variable "resource_group_name" {
  description = "Nome do Resource Group"
  type        = string
  default     = "rg-linkmanager"
}

variable "location" {
  description = "Localização dos recursos Azure"
  type        = string
  default     = "brazilsouth"
}

variable "environment" {
  description = "Ambiente (Development, Staging, Production)"
  type        = string
  default     = "Production"
}

variable "tags" {
  description = "Tags para os recursos"
  type        = map(string)
  default = {
    Project     = "LinkManager"
    Environment = "Production"
    ManagedBy   = "Terraform"
  }
}

# SQL Server
variable "sql_server_name" {
  description = "Nome do SQL Server (deve ser globalmente único)"
  type        = string
  default     = "sql-linkmanager"
}

variable "sql_admin_username" {
  description = "Username do administrador do SQL Server"
  type        = string
  default     = "sqladmin"
  sensitive   = true
}

variable "sql_admin_password" {
  description = "Senha do administrador do SQL Server"
  type        = string
  sensitive   = true
}

variable "sql_database_name" {
  description = "Nome do banco de dados"
  type        = string
  default     = "LinkManagerDb"
}

variable "sql_database_sku" {
  description = "SKU do banco de dados (Basic, S0, S1, P1, etc)"
  type        = string
  default     = "S0"
}

variable "sql_database_max_size_gb" {
  description = "Tamanho máximo do banco em GB"
  type        = number
  default     = 2
}

variable "azuread_admin_login" {
  description = "Login do administrador Azure AD"
  type        = string
  default     = ""
}

variable "azuread_admin_object_id" {
  description = "Object ID do administrador Azure AD"
  type        = string
  default     = ""
}

variable "allow_my_ip" {
  description = "Seu IP para acesso ao SQL Server (deixe vazio para não criar regra)"
  type        = string
  default     = ""
}

# App Service
variable "app_service_plan_name" {
  description = "Nome do App Service Plan"
  type        = string
  default     = "plan-linkmanager"
}

variable "app_service_plan_sku" {
  description = "SKU do App Service Plan (B1, B2, S1, P1V2, etc)"
  type        = string
  default     = "B1"
}

variable "app_service_name" {
  description = "Nome do App Service (deve ser globalmente único)"
  type        = string
  default     = "app-linkmanager"
}

variable "app_service_always_on" {
  description = "Manter App Service sempre ativo"
  type        = bool
  default     = true
}

variable "enable_staging_slot" {
  description = "Habilitar slot de staging"
  type        = bool
  default     = false
}

# Application Insights
variable "app_insights_name" {
  description = "Nome do Application Insights"
  type        = string
  default     = "appi-linkmanager"
}

variable "log_analytics_workspace_name" {
  description = "Nome do Log Analytics Workspace"
  type        = string
  default     = "log-linkmanager"
}
