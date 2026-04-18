# Dental Clinic Management System

## Full Project Documentation by Sprints and Hardening Phases

Last updated: April 18, 2026
Target runtime: .NET 8, ASP.NET Core Web API, EF Core, SQL Server

---

## 1. Project Summary

This backend is a production-focused Dental Clinic Management System built with Clean Architecture and CQRS. The implementation evolved in two tracks:

1. Product Delivery Sprints (Sprint 0 to Sprint 5)
2. Production Hardening Phases (P0 to P2)

The final system supports:

- Multi-clinic tenancy
- JWT authentication with clinic-bound identity
- Role-based authorization policies
- Patients, Appointments, Treatments, Billing workflows
- Optimistic concurrency and idempotent payment handling
- Global exception handling and standardized API responses

---

## 2. Architecture Overview

### 2.1 Layers

- `DentalClinic.Domain`
  - Core entities, enums, and base entity model
- `DentalClinic.Application`
  - CQRS commands/queries, validators, mappings, abstractions
- `DentalClinic.Infrastructure`
  - EF Core persistence, JWT/password services, tenant context
- `DentalClinic.API`
  - Controllers, middleware, authentication/authorization pipeline

### 2.2 Key Patterns

- Clean Architecture boundaries
- MediatR for CQRS request handling
- FluentValidation in MediatR pipeline behavior
- EF Core DbContext abstraction via `IAppDbContext`
- Global query filters for tenant isolation

---

## 3. Sprint-by-Sprint Delivery

## Sprint 0: Foundation

### Implemented

- Clean Architecture project structure
- EF Core SQL Server persistence
- MediatR and CQRS setup
- FluentValidation pipeline behavior
- Serilog logging bootstrap
- Global exception middleware with standardized `ApiResponse<T>` envelope

### Primary files

- `DentalClinic.API/Program.cs`
- `DentalClinic.API/Middleware/GlobalExceptionHandlingMiddleware.cs`
- `DentalClinic.Application/DependencyInjection.cs`
- `DentalClinic.Application/Behaviors/ValidationBehavior.cs`
- `DentalClinic.Infrastructure/DependencyInjection.cs`
- `DentalClinic.Infrastructure/Persistence/AppDbContext.cs`

---

## Sprint 1: Authentication and Authorization Baseline

### Implemented

- Password hashing using ASP.NET Core `PasswordHasher<User>`
- JWT issuance and validation
- Login and registration endpoints
- Role model: Admin, Doctor, Receptionist

### Notes

- Auth contracts were later hardened in P0/P1 to become clinic-aware and safer for SaaS.

### Primary files

- `DentalClinic.Infrastructure/Authentication/PasswordHasherService.cs`
- `DentalClinic.Infrastructure/Authentication/JwtTokenService.cs`
- `DentalClinic.API/Controllers/AuthController.cs`
- `DentalClinic.Application/Features/Auth/**`

---

## Sprint 2: Patients Module

### Implemented

- Create, update, delete, get by id, paginated list
- Search support in list endpoint
- Soft-delete (`IsDeleted`, `DeletedAt`) and query filter exclusion

### Primary files

- `DentalClinic.API/Controllers/PatientsController.cs`
- `DentalClinic.Application/Features/Patients/**`
- `DentalClinic.Domain/Entities/Patient.cs`
- `DentalClinic.Infrastructure/Persistence/AppDbContext.cs`

---

## Sprint 3: Appointments Module

### Implemented

- Appointment scheduling and updates
- Cancellation endpoint
- Conflict detection (no overlapping appointments for same doctor)
- Doctor and patient relation checks

### Primary files

- `DentalClinic.API/Controllers/AppointmentsController.cs`
- `DentalClinic.Application/Features/Appointments/**`
- `DentalClinic.Domain/Entities/Appointment.cs`
- `DentalClinic.Infrastructure/Persistence/AppDbContext.cs`

---

## Sprint 4: Treatments Module

### Implemented

- Treatment CRUD operations
- Tooth number constraints (1-32)
- Patient linkage and optional invoice linkage

### Primary files

- `DentalClinic.API/Controllers/TreatmentsController.cs`
- `DentalClinic.Application/Features/Treatments/**`
- `DentalClinic.Domain/Entities/Treatment.cs`
- `DentalClinic.Infrastructure/Persistence/AppDbContext.cs`

---

## Sprint 5: Billing Module

### Implemented

- Invoice creation from selected treatments
- Payment recording
- Invoice totals, paid amount, balance, status handling
- Query invoice by id and list invoices by patient

### Primary files

- `DentalClinic.API/Controllers/BillingController.cs`
- `DentalClinic.Application/Features/Billing/**`
- `DentalClinic.Domain/Entities/Invoice.cs`
- `DentalClinic.Domain/Entities/Payment.cs`
- `DentalClinic.Infrastructure/Persistence/AppDbContext.cs`

---

## 4. Hardening Phase Documentation

## P0 (Security Hotfix)

### Goals

- Eliminate role escalation risk
- Enforce role-based authorization policies
- Remove secrets from source-controlled settings

### Implemented

- Public registration no longer accepts role from client
- Admin-only user creation endpoint added (`POST /api/auth/users`)
- Policy-based authorization added:
  - `AdminOnly`
  - `ClinicalStaff`
  - `DoctorOrAdmin`
  - `BillingStaff`
- Rate limiting added for public auth endpoints (`register`, `login`, `refresh`) using fixed-window policy
- Swagger restricted to development environment only
- Sensitive values removed from source `appsettings*.json`

### Primary files

- `DentalClinic.API/Controllers/AuthController.cs`
- `DentalClinic.API/Program.cs`
- `DentalClinic.API/Controllers/PatientsController.cs`
- `DentalClinic.API/Controllers/AppointmentsController.cs`
- `DentalClinic.API/Controllers/TreatmentsController.cs`
- `DentalClinic.API/Controllers/BillingController.cs`
- `DentalClinic.API/appsettings.json`
- `DentalClinic.API/appsettings.Development.json`

---

## P1 (SaaS Multi-Tenant Isolation)

### Goals

- Introduce explicit clinic tenancy
- Prevent cross-clinic data access
- Make authentication clinic-aware

### Implemented

- New entity: `Clinic` (`Name`, `Code`, `IsActive`)
- `ClinicId` added to tenant-owned entities
- `clinic_id` claim included in JWT
- Strict tenant context from JWT (`HttpTenantContext`)
- Global query filters enforce clinic scoping
- Public registration now creates a new clinic and owner admin user
- Login now requires clinic code + email + password
- User lookup is clinic-scoped, not global

### Primary files

- `DentalClinic.Domain/Entities/Clinic.cs`
- `DentalClinic.Domain/Entities/User.cs`
- `DentalClinic.Domain/Entities/Patient.cs`
- `DentalClinic.Domain/Entities/Appointment.cs`
- `DentalClinic.Domain/Entities/Treatment.cs`
- `DentalClinic.Domain/Entities/Invoice.cs`
- `DentalClinic.Domain/Entities/Payment.cs`
- `DentalClinic.Infrastructure/Security/HttpTenantContext.cs`
- `DentalClinic.Infrastructure/Authentication/JwtTokenService.cs`
- `DentalClinic.Infrastructure/Persistence/AppDbContext.cs`
- `DentalClinic.Application/Features/Auth/DTOs/LoginUserRequestDto.cs`
- `DentalClinic.Application/Features/Auth/Queries/LoginUser/LoginUserQuery.cs`
- `DentalClinic.Application/Features/Auth/Queries/LoginUser/LoginUserQueryHandler.cs`
- `DentalClinic.Application/Features/Auth/Commands/RegisterUser/RegisterUserCommandHandler.cs`

---

## P2 (Concurrency and Race Condition Hardening)

### Goals

- Prevent lost updates and payment race issues
- Strengthen appointment conflict behavior under parallel requests

### Implemented

- `RowVersion` added to `BaseEntity` and mapped as concurrency token
- Invoice DTO now returns row version for safe client-side update calls
- Add-payment contract requires:
  - `InvoiceRowVersion`
  - `RequestId` for idempotency
- Billing payment handler uses serializable transaction and request deduping
- Appointment create/update handlers execute inside serializable transaction and acquire doctor lock (`sp_getapplock`)
- `DbUpdateConcurrencyException` mapped to HTTP 409 in middleware
- Refresh-token session model added with rotation and server-side hash persistence

### Primary files

- `DentalClinic.Domain/Common/BaseEntity.cs`
- `DentalClinic.Application/Features/Billing/DTOs/InvoiceDto.cs`
- `DentalClinic.Application/Features/Billing/DTOs/AddPaymentRequestDto.cs`
- `DentalClinic.Application/Features/Billing/Commands/AddPayment/AddPaymentCommand.cs`
- `DentalClinic.Application/Features/Billing/Commands/AddPayment/AddPaymentCommandHandler.cs`
- `DentalClinic.Application/Features/Billing/Commands/AddPayment/AddPaymentCommandValidator.cs`
- `DentalClinic.Application/Features/Appointments/Commands/CreateAppointment/CreateAppointmentCommandHandler.cs`
- `DentalClinic.Application/Features/Appointments/Commands/UpdateAppointment/UpdateAppointmentCommandHandler.cs`
- `DentalClinic.Infrastructure/Persistence/AppDbContext.cs`
- `DentalClinic.API/Middleware/GlobalExceptionHandlingMiddleware.cs`

---

## 5. API Documentation (Current Contracts)

## 5.1 Auth

### POST `/api/auth/register` (public)

Registers a new clinic owner account.

Request body:

```json
{
  "fullName": "Clinic Owner",
  "email": "owner@clinic.com",
  "password": "StrongPass123!",
  "clinicName": "Bright Dental"
}
```

Response data includes `clinicCode` and JWT.
Response data also includes a `refreshToken`.

### POST `/api/auth/login` (public)

Authenticates within a specific clinic.

Request body:

```json
{
  "clinicCode": "A1B2C3D4",
  "email": "owner@clinic.com",
  "password": "StrongPass123!"
}
```

Response data includes `accessToken`, `refreshToken`, and `clinicCode`.

### POST `/api/auth/refresh` (public)

Exchanges a valid refresh token for a new access token and refresh token pair (rotation).

Request body:

```json
{
  "refreshToken": "<refresh-token-value>"
}
```

### POST `/api/auth/users` (AdminOnly)

Creates clinic staff user in caller's clinic.

Request body:

```json
{
  "fullName": "Dr. Jane",
  "email": "jane@clinic.com",
  "password": "StrongPass123!",
  "role": 2
}
```

Roles:

- 1 = Admin
- 2 = Doctor
- 3 = Receptionist

---

## 5.2 Patients

Base route: `/api/patients`
Policy: `ClinicalStaff`

- `POST /api/patients`
- `PUT /api/patients/{id}`
- `DELETE /api/patients/{id}` (soft delete)
- `GET /api/patients/{id}`
- `GET /api/patients?pageNumber=1&pageSize=10&search=...`

---

## 5.3 Appointments

Base route: `/api/appointments`
Policy: `ClinicalStaff`

- `POST /api/appointments`
- `PUT /api/appointments/{id}`
- `POST /api/appointments/{id}/cancel`
- `GET /api/appointments/{id}`
- `GET /api/appointments?from=&to=&doctorId=&patientId=`

---

## 5.4 Treatments

Base route: `/api/treatments`
Policy: `DoctorOrAdmin`

- `POST /api/treatments`
- `PUT /api/treatments/{id}`
- `DELETE /api/treatments/{id}`
- `GET /api/treatments/{patientId}`

---

## 5.5 Billing

Base route: `/api/invoices`
Policy: `BillingStaff`

- `POST /api/invoices`
- `POST /api/invoices/{id}/payments`
- `GET /api/invoices/{id}`
- `GET /api/invoices?patientId=`

Add payment request body:

```json
{
  "amount": 100,
  "paymentDate": "2026-04-18T14:00:00Z",
  "method": 1,
  "notes": "Cash at desk",
  "invoiceRowVersion": "AAAAAAAAB9E=",
  "requestId": "payment-2026-04-18-001"
}
```

---

## 6. Security and Authorization Matrix

- `AdminOnly`
  - Auth: create user endpoint
- `ClinicalStaff`
  - Patients and Appointments modules
- `DoctorOrAdmin`
  - Treatments module
- `BillingStaff`
  - Invoices and Payments module

JWT claims include:

- Subject/user id
- Name
- Email
- Role
- `clinic_id`

Auth endpoint throttling:

- Policy `AuthPolicy`
- Fixed window: 10 requests/minute per client IP
- Applied to `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/refresh`

---

## 7. Database and Migrations

Applied migrations in order:

1. `20260417165351_InitialAuthFoundation`
2. `20260417171653_AddPatientsModule`
3. `20260417172648_AddAppointmentsModule`
4. `20260417173613_AddTreatmentsModule`
5. `20260417174930_AddBillingModule`
6. `20260418120839_HardenSaasSecurityTenancyConcurrency`
7. `20260418123623_AddClinicTenantModel`
8. `20260418124704_AddRefreshTokenSessions`

Key migration files:

- `DentalClinic.Infrastructure/Persistence/Migrations/20260418120839_HardenSaasSecurityTenancyConcurrency.cs`
- `DentalClinic.Infrastructure/Persistence/Migrations/20260418123623_AddClinicTenantModel.cs`

---

## 8. Configuration and Environment

Required settings:

- `ConnectionStrings:DefaultConnection`
- `Jwt:SecretKey`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:ExpirationMinutes`

Important:

- Secrets are intentionally removed from source settings files.
- Use environment variables or user-secrets for local/dev.

---

## 9. Operational Commands

Build:

```powershell
dotnet build DentalClinic.sln
```

Apply database migrations:

```powershell
dotnet ef database update --project DentalClinic.Infrastructure --startup-project DentalClinic.API
```

Run API:

```powershell
dotnet run --project DentalClinic.API
```

---

## 10. Known Constraints and Next Recommendations

- Add integration tests for:
  - cross-tenant isolation behavior
  - clinic-aware login and registration
  - payment idempotency under retries
  - appointment conflict under high concurrency
- Add API versioning before external client rollout
- Add rate limiting to auth endpoints
- Add refresh-token flow if long-lived sessions are required

---

## 11. Release Readiness Checklist

- [x] Layered architecture in place
- [x] Module delivery complete (Patients, Appointments, Treatments, Billing)
- [x] JWT + role policies + clinic claim
- [x] Multi-tenant data isolation model with clinic foreign keys
- [x] Concurrency controls for billing and scheduling
- [x] Source secrets removed
- [x] Migrations generated and applied
- [ ] Automated integration test suite for tenant and concurrency scenarios
- [ ] CI policy gates for migration drift and security checks
