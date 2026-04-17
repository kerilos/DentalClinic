# DentalClinic API Deployment Guide (SmarterASP.NET)

## Overview
This document explains how to deploy DentalClinic API to SmarterASP.NET with SQL Server, apply EF Core migrations, and verify the deployment.

Local credentials and exact operational values are stored in DEPLOYMENT_SECRETS.local.md (git-ignored).

Current deployment target:
- Website: dentalclinic2-001-site1
- Base URL: http://dentalclinic2-001-site1.ktempurl.com
- Swagger: http://dentalclinic2-001-site1.ktempurl.com/swagger/index.html
- Health: http://dentalclinic2-001-site1.ktempurl.com/health

## Prerequisites
- .NET 8 SDK installed
- EF Core CLI installed
  - dotnet tool install --global dotnet-ef
- SmarterASP website created
- SmarterASP SQL database created
- MSDeploy enabled in SmarterASP panel

## 1) Configure Connection String
Update DefaultConnection in:
- DentalClinic.API/appsettings.json
- DentalClinic.API/appsettings.Development.json

Template:
Data Source=YOUR_SQL_HOST;Initial Catalog=YOUR_DB_NAME;User Id=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;Encrypt=True;TrustServerCertificate=True;

Notes:
- Keep secrets out of source control for real production usage.
- Prefer environment variables on hosting:
  - ConnectionStrings__DefaultConnection
  - Jwt__SecretKey

## 2) Apply Database Migrations
From repository root:

dotnet ef database update --project DentalClinic.Infrastructure --startup-project DentalClinic.API

Expected result:
- Build succeeded
- Command ends with Done.

## 3) Publish to SmarterASP (MSDeploy)
From DentalClinic.API directory:

dotnet msbuild DentalClinic.API.csproj /t:"Build;Publish" /p:Configuration=Release /p:DeployOnBuild=true /p:WebPublishMethod=MSDeploy /p:MSDeployServiceURL="https://YOUR_MSDEPLOY_HOST:8172/MsDeploy.axd?site=YOUR_SITE_NAME" /p:DeployIisAppPath="YOUR_SITE_NAME" /p:UserName="YOUR_DEPLOY_USER" /p:Password="YOUR_DEPLOY_PASSWORD" /p:AuthType="Basic" /p:AllowUntrustedCertificate=True

Example values used in this project:
- MSDeploy URL host: win8086.site4now.net
- Site name: dentalclinic2-001-site1
- Deploy user: dentalclinic2-001

## 4) Verify Deployment
Check these endpoints:
- Root: http://dentalclinic2-001-site1.ktempurl.com/
- Health: http://dentalclinic2-001-site1.ktempurl.com/health
- Swagger UI: http://dentalclinic2-001-site1.ktempurl.com/swagger/index.html
- OpenAPI JSON: http://dentalclinic2-001-site1.ktempurl.com/swagger/v1/swagger.json

Expected statuses:
- / returns 200 with service status payload
- /health returns 200
- /swagger and /swagger/index.html return 200
- Protected API endpoints may return 401 without JWT (expected)

## 5) Production Checklist
- Replace Jwt:SecretKey with a strong secret (minimum 32 bytes)
- Ensure ConnectionStrings__DefaultConnection is set on hosting
- Confirm SQL firewall/network access (if applicable)
- Enable HTTPS endpoint and test secure URL
- Restart app pool after config changes (SmarterASP panel)

## Troubleshooting

### Root URL returns 404
- Cause: no route mapped to /
- Fix: ensure Program.cs maps a root endpoint with app.MapGet("/", ...)

### Swagger not visible
- Cause: Swagger enabled only in Development
- Fix: ensure Program.cs includes:
  - app.UseSwagger();
  - app.UseSwaggerUI();

### EF migration fails to connect
- Verify SQL host, database, user, password
- Ensure Encrypt=True;TrustServerCertificate=True in connection string
- Re-run migration command from repository root

### Publish succeeds but app not updated
- Restart app pool in SmarterASP panel
- Re-publish with /t:"Build;Publish"
- Confirm site name in DeployIisAppPath exactly matches hosting site

## Useful Commands
From repository root:
- Build solution:
  - dotnet build DentalClinic.sln
- Apply migrations:
  - dotnet ef database update --project DentalClinic.Infrastructure --startup-project DentalClinic.API

From DentalClinic.API:
- Publish via MSDeploy:
  - dotnet msbuild DentalClinic.API.csproj /t:"Build;Publish" /p:Configuration=Release /p:DeployOnBuild=true /p:WebPublishMethod=MSDeploy /p:MSDeployServiceURL="https://YOUR_MSDEPLOY_HOST:8172/MsDeploy.axd?site=YOUR_SITE_NAME" /p:DeployIisAppPath="YOUR_SITE_NAME" /p:UserName="YOUR_DEPLOY_USER" /p:Password="YOUR_DEPLOY_PASSWORD" /p:AuthType="Basic" /p:AllowUntrustedCertificate=True

## Change Log
- Added deployment workflow for SmarterASP SQL + MSDeploy
- Added endpoint verification section
- Added troubleshooting for 404 root and missing Swagger
