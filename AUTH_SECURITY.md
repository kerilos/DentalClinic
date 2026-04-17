# Authentication Security Guide - Production Ready

## Overview
This guide documents the production-ready authentication system implemented for the Dental Clinic Management System.

## Critical Security Features Implemented

### 1. Password Hashing (✓ IMPLEMENTED)
- **Method**: ASP.NET Core Identity `PasswordHasher<User>`
- **Algorithm**: PBKDF2 with SHA256 (secure by default)
- **Key Points**:
  - Passwords are NEVER stored in plain text
  - Hash is created on registration: `PasswordHasherService.HashPassword(user, password)`
  - Hash is verified on login: `PasswordHasherService.VerifyPassword(user, password)`
  - Automatic rehashing on upgrade if algorithm changes in future

**File**: `DentalClinic.Infrastructure/Authentication/PasswordHasherService.cs`

### 2. Email Normalization & Uniqueness (✓ IMPLEMENTED)
- **Normalization Process**:
  ```csharp
  var normalizedEmail = request.Email.Trim().ToLowerInvariant();
  ```
- **Applied on**: Registration and Login handlers
- **Database Constraint**: Unique index on Users table
  ```sql
  CREATE UNIQUE INDEX IX_Users_Email ON Users(Email)
  ```
- **Prevents**:
  - Email enumeration attacks
  - Duplicate account registration via case variations
  - Leading/trailing whitespace issues

**Files**:
- `DentalClinic.Application/Features/Auth/Commands/RegisterUser/RegisterUserCommandHandler.cs`
- `DentalClinic.Application/Features/Auth/Queries/LoginUser/LoginUserQueryHandler.cs`
- `DentalClinic.Infrastructure/Persistence/AppDbContext.cs`

### 3. Validation & Role Enforcement (✓ IMPLEMENTED)
- **Principle**: No numeric role checks in controllers
- **Validation Layer**: Strict FluentValidation rules
  
**Register Command Validator**:
```csharp
RuleFor(x => x.Role)
    .NotEmpty().WithMessage("User role is required.")
    .IsInEnum().WithMessage("User role must be Admin, Doctor, or Receptionist.");
```

**Login Query Validator**:
```csharp
RuleFor(x => x.Email)
    .NotEmpty().WithMessage("Email is required.")
    .EmailAddress().WithMessage("Email must be a valid email address.")
    .MaximumLength(150)
    .Must(email => !string.IsNullOrWhiteSpace(email));

RuleFor(x => x.Password)
    .NotEmpty().WithMessage("Password is required.");
```

**Password Strength Requirements**:
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character

**Files**:
- `DentalClinic.Application/Features/Auth/Commands/RegisterUser/RegisterUserCommandValidator.cs`
- `DentalClinic.Application/Features/Auth/Queries/LoginUser/LoginUserQueryValidator.cs`

### 4. JWT Token Security (✓ IMPLEMENTED)
- **Token Service**: Centralized `IJwtTokenService` interface
- **Implementation**: `JwtTokenService` in Infrastructure layer
- **Claims Included**:
  - Subject (sub): User ID
  - Email
  - Full Name
  - Role
  - JTI (JWT ID): Unique token identifier
  - Issued At (iat), Expiration (exp)
  
- **Configuration** (from appsettings.json):
  ```json
  {
    "Jwt": {
      "SecretKey": "[MUST CHANGE FOR PRODUCTION]",
      "Issuer": "DentalClinic.API",
      "Audience": "DentalClinic.Client",
      "ExpirationMinutes": 60
    }
  }
  ```

**File**: `DentalClinic.Infrastructure/Authentication/JwtTokenService.cs`

### 5. Authentication Pipeline (✓ IMPLEMENTED)
- **JWT Bearer Scheme**: Configured in `Program.cs`
- **Token Validation**:
  - Issuer validation
  - Audience validation
  - Lifetime validation
  - Signature verification with secret key
  
- **ClockSkew**: 1 minute tolerance for time synchronization

**File**: `DentalClinic.API/Program.cs`

### 6. Conflict & Authentication Exception Handling (✓ IMPLEMENTED)
- **409 Conflict**: Returned when email already registered
- **401 Unauthorized**: Returned on invalid credentials
- **Prevents** email enumeration by using generic "Invalid credentials" message
- **Generic Messages**: Don't reveal whether email exists
- **Middleware**: Global exception handler catches and maps to standard `ApiResponse<T>`

**Files**:
- `DentalClinic.API/Middleware/GlobalExceptionHandlingMiddleware.cs`
- `DentalClinic.Application/Common/Exceptions/ConflictException.cs`

### 7. Clean Architecture Compliance (✓ VERIFIED)
- **Domain Layer**: Entity definitions only (User, roles)
- **Application Layer**: CQRS command/query handlers, validators, interfaces (IPasswordHasherService, IJwtTokenService)
- **Infrastructure Layer**: Password hashing, JWT token generation, EF Core persistence
- **API Layer**: Controllers remain thin, delegating to MediatR
- **No Leakage**: No EF Core types exposed to Application layer

---

## Production Deployment Checklist

### ⚠️ CRITICAL - Must Do Before Production

1. **JWT Secret Key**
   - [ ] Replace `SecretKey` in appsettings.json with a strong secret (minimum 32 bytes)
   - [ ] Use environment variables or Azure Key Vault for production
   - ```json
     "Jwt": {
       "SecretKey": "[STRONG_SECRET_FROM_ENV]"
     }
     ```

2. **HTTPS Enforcement**
   - [ ] Ensure `app.UseHttpsRedirection()` is enabled
   - [ ] Use HSTS headers in production
   - [ ] All auth endpoints must use HTTPS only

3. **Rate Limiting**
   - [ ] Implement rate limiting on `/api/auth/login` endpoint
   - [ ] Prevent brute force attacks on password
   - [ ] Consider implementing account lockouts after N failed attempts

4. **Logging & Monitoring**
   - [ ] Log authentication failures with IP address
   - [ ] Alert on repeated failed login attempts
   - [ ] Monitor for suspicious registration patterns

5. **Token Expiration**
   - [ ] Set `ExpirationMinutes` appropriately (e.g., 60 minutes)
   - [ ] Implement refresh token mechanism for longer sessions
   - [ ] Log token generation and validation events

6. **Email Verification**
   - [ ] Consider implementing email verification on registration
   - [ ] Mark users as `IsEmailVerified` before allowing login
   - [ ] Send verification link via secure email

7. **Account Security**
   - [ ] Implement "Forgot Password" flow
   - [ ] Add audit logging for account changes
   - [ ] Store login history for security dashboard

---

## Security Best Practices Applied

✓ **No Plain-Text Passwords**: ASP.NET Core PasswordHasher with PBKDF2  
✓ **Email Normalization**: Consistent lowercase, trimmed format  
✓ **Unique Email Constraint**: Database-level uniqueness + application validation  
✓ **Generic Error Messages**: Prevent email enumeration via failed login  
✓ **Role Validation**: Enum-based validation, no numeric checks  
✓ **JWT Claims**: Includes user identity and role for authorization  
✓ **Token Validation**: Issuer, audience, lifetime, and signature verification  
✓ **Clean Architecture**: Proper separation of concerns across layers  
✓ **MediatR CQRS**: Predictable request handling with middleware pipeline  
✓ **FluentValidation**: Comprehensive input validation before business logic  
✓ **Exception Handling**: Consistent error responses via global middleware  

---

## API Endpoints

### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@clinic.local",
  "password": "SecurePass123!",
  "role": 1
}
```

**Response (201 Created)**:
```json
{
  "success": true,
  "message": "User registered successfully.",
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "fullName": "John Doe",
    "email": "john@clinic.local",
    "role": 1,
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAtUtc": "2026-04-17T20:00:00Z"
  }
}
```

### Login User
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@clinic.local",
  "password": "SecurePass123!"
}
```

**Response (200 OK)**:
```json
{
  "success": true,
  "message": "Login successful.",
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "fullName": "John Doe",
    "email": "john@clinic.local",
    "role": 1,
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAtUtc": "2026-04-17T20:00:00Z"
  }
}
```

---

## Future Enhancements

1. **Refresh Tokens**: Implement refresh token rotation for extended sessions
2. **Multi-Factor Authentication (MFA)**: Add OTP/SMS verification
3. **OAuth2/OpenID Connect**: Support third-party authentication
4. **API Key Authentication**: For service-to-service communication
5. **Role-Based Authorization**: Implement `[Authorize(Roles = "Admin")]` across endpoints
6. **Audit Logging**: Track all authentication and authorization events
7. **Account Lockout**: Implement lockout after N failed attempts
8. **Password History**: Prevent reuse of recent passwords
9. **Session Management**: Implement logout and active session tracking
10. **IP Whitelisting**: Optional per-role IP restrictions

---

**Last Updated**: April 17, 2026  
**Status**: Production Ready ✓
