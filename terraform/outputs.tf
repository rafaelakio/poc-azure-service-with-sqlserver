# Outputs do Terraform

output "resource_group_name" {
  description = "Nome do Resource Group criado"
  value       = azurerm_resource_group.main.name
}

output "sql_server_fqdn" {
  description = "FQDN do SQL Server"
  value       = azurerm_mssql_server.main.fully_qualified_domain_name
}

output "sql_server_name" {
  description = "Nome do SQL Server"
  value       = azurerm_mssql_server.main.name
}

output "sql_database_name" {
  description = "Nome do banco de dados"
  value       = azurerm_mssql_database.main.name
}

output "sql_connection_string" {
  description = "Connection string do SQL Server (sem senha)"
  value       = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.main.name};User ID=${var.sql_admin_username};Password=<YOUR_PASSWORD>;Encrypt=True;TrustServerCertificate=False;"
  sensitive   = false
}

output "app_service_name" {
  description = "Nome do App Service"
  value       = azurerm_linux_web_app.main.name
}

output "app_service_url" {
  description = "URL do App Service"
  value       = "https://${azurerm_linux_web_app.main.default_hostname}"
}

output "app_service_default_hostname" {
  description = "Hostname padrão do App Service"
  value       = azurerm_linux_web_app.main.default_hostname
}

output "app_insights_instrumentation_key" {
  description = "Instrumentation Key do Application Insights"
  value       = azurerm_application_insights.main.instrumentation_key
  sensitive   = true
}

output "app_insights_connection_string" {
  description = "Connection String do Application Insights"
  value       = azurerm_application_insights.main.connection_string
  sensitive   = true
}

output "deployment_commands" {
  description = "Comandos para deploy da aplicação"
  value = <<-EOT
    # 1. Build da aplicação
    dotnet publish -c Release -o ./publish
    
    # 2. Criar arquivo ZIP
    cd publish
    zip -r ../deploy.zip .
    cd ..
    
    # 3. Deploy para Azure
    az webapp deployment source config-zip \
      --resource-group ${azurerm_resource_group.main.name} \
      --name ${azurerm_linux_web_app.main.name} \
      --src deploy.zip
    
    # 4. Aplicar migrations
    dotnet ef database update --connection "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.main.name};User ID=${var.sql_admin_username};Password=${var.sql_admin_password};Encrypt=True;"
  EOT
}

output "sql_server_admin_username" {
  description = "Username do administrador do SQL Server"
  value       = var.sql_admin_username
  sensitive   = true
}

output "staging_slot_url" {
  description = "URL do slot de staging (se habilitado)"
  value       = var.enable_staging_slot ? "https://${azurerm_linux_web_app.main.name}-staging.azurewebsites.net" : "Staging slot não habilitado"
}
