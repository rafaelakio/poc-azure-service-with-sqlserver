# Terraform AWS - Link Manager Infrastructure

Infrastructure as Code for deploying Link Manager on AWS using ECS Fargate and RDS SQL Server.

## 📋 Resources Provisioned

This Terraform configuration creates:

- ✅ **VPC** with public and private subnets across 2 AZs
- ✅ **Internet Gateway** and route tables
- ✅ **Security Groups** for ALB, ECS, and RDS
- ✅ **RDS SQL Server Express** (Multi-AZ capable)
- ✅ **ECR Repository** for Docker images
- ✅ **ECS Fargate Cluster** and Service
- ✅ **Application Load Balancer** (ALB)
- ✅ **CloudWatch Logs** for application logging
- ✅ **AWS Secrets Manager** for database credentials
- ✅ **IAM Roles and Policies** for ECS tasks

## 🏗️ Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                         Internet                             │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│              Application Load Balancer (ALB)                 │
│                    (Public Subnets)                          │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│                    ECS Fargate Service                       │
│              (Public Subnets - 2 AZs)                        │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Task 1: Link Manager Container                      │  │
│  │  - .NET 8 Blazor App                                 │  │
│  │  - 0.25 vCPU, 0.5 GB RAM                            │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ↓
┌─────────────────────────────────────────────────────────────┐
│              RDS SQL Server Express                          │
│              (Private Subnets - Multi-AZ)                    │
│  - db.t3.small (2 vCPU, 2 GB RAM)                          │
│  - 20 GB Storage                                            │
│  - Automated Backups                                        │
└─────────────────────────────────────────────────────────────┘
```

## 💰 Cost Estimate

### Monthly Costs (us-east-1)

| Resource | Configuration | Monthly Cost |
|----------|--------------|--------------|
| RDS SQL Server | db.t3.small, 20GB | ~$50 |
| ECS Fargate | 1 task, 0.25 vCPU, 0.5GB | ~$15 |
| Application Load Balancer | Standard | ~$20 |
| Data Transfer | ~5GB | ~$5 |
| CloudWatch Logs | 1GB | ~$1 |
| ECR Storage | <1GB | ~$0.10 |
| **Total** | | **~$90-100/month** |

### Cost Optimization Options

#### Development/Test Environment
```hcl
rds_instance_class = "db.t3.micro"      # ~$25/month
ecs_task_cpu = "256"                     # 0.25 vCPU
ecs_task_memory = "512"                  # 0.5 GB
backup_retention_period = 1
skip_final_snapshot = true
```
**Estimated cost**: ~$60-70/month

#### Production Environment
```hcl
rds_instance_class = "db.t3.medium"      # ~$100/month
ecs_task_cpu = "512"                     # 0.5 vCPU
ecs_task_memory = "1024"                 # 1 GB
ecs_desired_count = 2                    # High availability
backup_retention_period = 30
enable_deletion_protection = true
```
**Estimated cost**: ~$180-200/month

## 🚀 Quick Start

### Prerequisites

1. **AWS CLI** installed and configured
   ```bash
   aws --version
   aws configure
   ```

2. **Terraform** installed (v1.0+)
   ```bash
   terraform --version
   ```

3. **Docker** installed (for building images)
   ```bash
   docker --version
   ```

### Step 1: Configure Variables

```bash
cd terraform-aws
cp terraform.tfvars.example terraform.tfvars
nano terraform.tfvars
```

**Important**: Change these values:
- `db_password`: Use a strong password
- `aws_region`: Choose your preferred region
- `project_name`: Make it unique if needed

### Step 2: Initialize Terraform

```bash
terraform init
```

### Step 3: Plan and Apply

```bash
# Review what will be created
terraform plan

# Create infrastructure
terraform apply
```

This will take approximately 10-15 minutes.

### Step 4: Build and Push Docker Image

```bash
# Go back to project root
cd ..

# Build Docker image
docker build -t linkmanager:latest .

# Get ECR repository URL from Terraform output
ECR_URL=$(cd terraform-aws && terraform output -raw ecr_repository_url)

# Authenticate Docker to ECR
aws ecr get-login-password --region us-east-1 | \
  docker login --username AWS --password-stdin $ECR_URL

# Tag and push image
docker tag linkmanager:latest $ECR_URL:latest
docker push $ECR_URL:latest
```

### Step 5: Deploy to ECS

```bash
# Get cluster and service names
CLUSTER=$(cd terraform-aws && terraform output -raw ecs_cluster_name)
SERVICE=$(cd terraform-aws && terraform output -raw ecs_service_name)

# Force new deployment
aws ecs update-service \
  --cluster $CLUSTER \
  --service $SERVICE \
  --force-new-deployment \
  --region us-east-1
```

### Step 6: Access Application

```bash
# Get ALB URL
cd terraform-aws
terraform output alb_url
```

Visit the URL in your browser. It may take 2-3 minutes for the service to become healthy.

## 📊 Monitoring

### View Logs

```bash
# Real-time logs
aws logs tail /ecs/linkmanager --follow --region us-east-1

# Last 100 lines
aws logs tail /ecs/linkmanager --since 1h --region us-east-1
```

### Check ECS Service Status

```bash
aws ecs describe-services \
  --cluster linkmanager-cluster \
  --services linkmanager-service \
  --region us-east-1
```

### Check RDS Status

```bash
aws rds describe-db-instances \
  --db-instance-identifier linkmanager-sqlserver \
  --region us-east-1
```

## 🔄 CI/CD with GitHub Actions

Create `.github/workflows/deploy-aws.yml`:

```yaml
name: Deploy to AWS

on:
  push:
    branches: [ main ]

env:
  AWS_REGION: us-east-1
  ECR_REPOSITORY: linkmanager-app
  ECS_CLUSTER: linkmanager-cluster
  ECS_SERVICE: linkmanager-service

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Configure AWS credentials
      uses: aws-actions/configure-aws-credentials@v4
      with:
        aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
        aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
        aws-region: ${{ env.AWS_REGION }}
    
    - name: Login to Amazon ECR
      id: login-ecr
      uses: aws-actions/amazon-ecr-login@v2
    
    - name: Build and push Docker image
      env:
        ECR_REGISTRY: ${{ steps.login-ecr.outputs.registry }}
        IMAGE_TAG: ${{ github.sha }}
      run: |
        docker build -t $ECR_REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG .
        docker push $ECR_REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG
        docker tag $ECR_REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG $ECR_REGISTRY/$ECR_REPOSITORY:latest
        docker push $ECR_REGISTRY/$ECR_REPOSITORY:latest
    
    - name: Deploy to ECS
      run: |
        aws ecs update-service \
          --cluster $ECS_CLUSTER \
          --service $ECS_SERVICE \
          --force-new-deployment \
          --region $AWS_REGION
```

## 🔒 Security Best Practices

### 1. Secrets Management

Database credentials are stored in AWS Secrets Manager:

```bash
# View secret (without value)
aws secretsmanager describe-secret \
  --secret-id linkmanager-db-connection \
  --region us-east-1

# Get secret value (requires permissions)
aws secretsmanager get-secret-value \
  --secret-id linkmanager-db-connection \
  --region us-east-1
```

### 2. Network Security

- RDS is in **private subnets** (no internet access)
- ECS tasks communicate with RDS via **security groups**
- ALB is in **public subnets** with restricted ingress (80/443 only)

### 3. IAM Least Privilege

- ECS task execution role: Only ECR and Secrets Manager access
- ECS task role: Minimal permissions for application

### 4. Encryption

- RDS storage is **encrypted at rest**
- Secrets Manager values are **encrypted**
- ECR images use **AES256 encryption**

## 🔧 Troubleshooting

### Service Won't Start

```bash
# Check service events
aws ecs describe-services \
  --cluster linkmanager-cluster \
  --services linkmanager-service \
  --region us-east-1 \
  --query 'services[0].events[0:5]'

# Check task status
aws ecs list-tasks \
  --cluster linkmanager-cluster \
  --service-name linkmanager-service \
  --region us-east-1
```

### Database Connection Issues

```bash
# Test RDS connectivity from ECS task
aws ecs execute-command \
  --cluster linkmanager-cluster \
  --task <task-id> \
  --container linkmanager-container \
  --interactive \
  --command "/bin/bash"

# Inside container:
curl -v telnet://linkmanager-sqlserver.xxxxx.us-east-1.rds.amazonaws.com:1433
```

### High Costs

```bash
# Check current month costs
aws ce get-cost-and-usage \
  --time-period Start=2024-11-01,End=2024-11-30 \
  --granularity MONTHLY \
  --metrics BlendedCost \
  --group-by Type=SERVICE
```

## 🧹 Cleanup

To destroy all resources:

```bash
cd terraform-aws
terraform destroy
```

**Warning**: This will delete:
- All ECS tasks and services
- RDS database (unless skip_final_snapshot=false)
- ALB and target groups
- VPC and networking
- ECR repository and images

## 📚 Additional Resources

- [AWS ECS Documentation](https://docs.aws.amazon.com/ecs/)
- [AWS RDS SQL Server](https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/CHAP_SQLServer.html)
- [Terraform AWS Provider](https://registry.terraform.io/providers/hashicorp/aws/latest/docs)
- [Main Deployment Guide](../DEPLOYMENT.md)

## 🆘 Support

For AWS-specific issues:
- [AWS Support](https://aws.amazon.com/support/)
- [AWS Forums](https://forums.aws.amazon.com/)
- [Stack Overflow - AWS Tag](https://stackoverflow.com/questions/tagged/amazon-web-services)

---

**Note**: Always review costs and security settings before deploying to production!
