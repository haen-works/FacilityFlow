FacilityFlow API


FacilityFlow is a role-based facility fault management REST API built with ASP.NET Core.
The project simulates a real workplace maintenance workflow where employees can report facility issues, administrators can assign those issues to technicians, and technicians can manage the status of their assigned tasks.
Features
- JWT-based authentication
- Role-based authorization
- Secure password hashing
- Employee, Technician and Admin roles
- Fault report creation and management
- Technician assignment and unassignment
- Fault status management
- Search and filtering
- Pagination
- Priority management
- Data validation
- Response DTOs
- SQLite database
- Entity Framework Core migrations
- OpenAPI documentation with Scalar
- JWT secret management using .NET User Secrets


Technologies
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- JWT Bearer Authentication
- ASP.NET Core PasswordHasher
- LINQ
- OpenAPI
- Scalar API Reference


User Roles


Employee

Employees can:
- Log in to the system
- Create new fault reports
- View only the fault reports they created
- Search and filter their own reports
The reporting user is automatically detected from the JWT token.
Clients cannot manually select another user as the reporter.

Technician

Technicians can:
- Log in to the system
- View only fault reports assigned to them
- Update the status of their assigned fault reports
A technician cannot update a fault report assigned to another technician.

Admin

Administrators can:

- View all fault reports
- View users
- Assign fault reports to technicians
- Remove technician assignments
- Update fault report status
- Manage the overall fault workflow


Fault Workflow
A typical FacilityFlow workflow:
Employee
↓
Creates a fault report
↓
Open
↓
Admin assigns a technician
↓
Technician starts working
↓
InProgress
↓
Work is completed
↓
Resolved


Fault Report Statuses

The API supports the following statuses:
- Open
- InProgress
- Resolved
Priority Levels

Fault reports support the following priority levels:
- Low
- Medium
- High
- Critical

Authentication

Users authenticate using their email address and password.
Endpoint:
POST /api/auth/login
Example request:
{
  "email": "user@example.com",
  "password": "password"
}
If authentication succeeds, the API returns a JWT token.
Authenticated requests use the following HTTP header:
Authorization: Bearer <JWT_TOKEN>

JWT tokens contain information about the authenticated user, including:
- User ID
- Name
- Email
- Role
Main API Endpoints
Login
POST /api/auth/login
Authenticates a user and returns a JWT token.
Get Fault Reports
GET /api/faultreports
The result depends on the authenticated user's role.
Employee:
Only fault reports created by that employee are returned.
Technician:
Only fault reports assigned to that technician are returned.
Admin:
All fault reports are returned.
Available query parameters:
- status
- priority
- reportedById
- assignedTechnicianId
- search
- page
- pageSize
Example:
GET /api/faultreports?status=Open&priority=High&page=1&pageSize=10
Get Fault Report By ID
GET /api/faultreports/{id}
Role-based access rules are also applied when viewing an individual fault report.
Employees can only view reports they created.
Technicians can only view reports assigned to them.
Admins can view all reports.
Create Fault Report
POST /api/faultreports
Allowed roles:
- Employee
- Admin
Example request:
{
  "title": "Meeting room air conditioner is not cooling",
  "description": "The air conditioner is running but does not produce cold air.",
  "location": "Block A, Floor 2",
  "priority": "High"
}
The reporting user's ID is automatically extracted from the JWT token.
The client does not manually send ReportedById.
Update Fault Status
PATCH /api/faultreports/{id}/status
Allowed roles:
- Technician
- Admin
Example request:
{
  "status": "InProgress"
}
Technicians can only update fault reports assigned to themselves.
Assign Technician
PATCH /api/faultreports/{id}/assignment
Allowed role:
- Admin
Example request:
{
  "technicianId": 3
}
Before assignment, the API checks:
- Whether the user exists
- Whether the selected user has the Technician role
Remove Technician Assignment
PATCH /api/faultreports/{id}/unassign
Allowed role:
- Admin
This removes the assigned technician from a fault report.
Get Users
GET /api/users
Allowed role:
- Admin
Returns users using a response DTO without exposing password hashes.
Create User
POST /api/users
User creation is currently available only in the Development environment.
Passwords are hashed before being stored in the database.
Pagination
Fault report listing supports pagination.
Example:
GET /api/faultreports?page=2&pageSize=10
Example response structure:
{
  "currentPage": 2,
  "pageSize": 10,
  "totalCount": 24,
  "totalPages": 3,
  "data": []
}
The pageSize value must be between 1 and 100.
Search
Fault reports can be searched using the search parameter.
Example:
GET /api/faultreports?search=air conditioner
The API searches both:
- Title
- Description
Filtering
Fault reports can be filtered using:
- Status
- Priority
- Reporter
- Assigned technician
Multiple filters can be combined.
Example:
GET /api/faultreports?status=Open&priority=Critical
Sorting
Fault reports are returned from newest to oldest using their creation date.
Validation
The API includes validation for values such as:
- Email addresses
- Password length
- Fault title length
- Description length
- Location length
- Priority values
- Status values
- Technician IDs
- Pagination values
Invalid requests return appropriate HTTP responses such as:
- 400 Bad Request
- 401 Unauthorized
- 403 Forbidden
- 404 Not Found
- 409 Conflict
Security
Passwords are never stored as plain text.
Password flow:
Password
↓
ASP.NET Core PasswordHasher
↓
Password Hash
↓
Database
Authentication flow:
Email + Password
↓
Credentials are verified
↓
JWT token is generated
↓
Client sends JWT with future requests
↓
Backend verifies user identity and role
Protected endpoints use:
[Authorize]
Role-specific endpoints use authorization such as:
[Authorize(Roles = "Admin")]
The JWT signing key is not stored in the Git repository.
During development, the secret is stored using .NET User Secrets.
Database
FacilityFlow uses SQLite together with Entity Framework Core.


Main entities:


AppUser

Contains:

- Id
- FullName
- Email
- Role
- PasswordHash

FaultReport

Contains:

- Id
- Title
- Description
- Location
- Status
- Priority
- AssignedTechnicianId
- ReportedById
- CreatedAt
Entity Framework Core migrations are used to manage database schema changes.
Demo Workflow
The demo database contains realistic users and facility fault reports.


Example roles include:

- Employees who create fault reports
- Technicians who receive assigned work
- An administrator who manages assignments
A complete example workflow:
1. Employee logs in.
2. Employee creates a fault report.
3. Reporter ID is automatically obtained from the JWT.
4. Admin views the fault report.
5. Admin assigns the report to a technician.
6. Technician logs in.
7. Technician sees only assigned reports.
8. Technician changes the status to InProgress.
9. Technician completes the task.
10. Fault report becomes Resolved.
Setup
Clone the repository:
git clone <repository-url>
Enter the project directory:
cd FacilityFlow
Restore dependencies:
dotnet restore
Configure the JWT secret:
dotnet user-secrets set "Jwt:Key" "your-development-secret-key"
Apply Entity Framework Core migrations:
dotnet ef database update
Run the API:
dotnet run
The application will display its localhost address in the terminal.
API Documentation
While the API is running, Scalar API Reference can be opened at:
https://localhost:<port>/scalar/v1
Scalar can be used to:
- View API endpoints
- Send requests
- Test request bodies
- Test query parameters
- Send JWT Authorization headers
- Inspect API responses
Project Structure

FacilityFlow.Api

Controllers

- AuthController.cs
- FaultReportsController.cs
- UsersController.cs

Data

- AppDbContext.cs

Dtos

- AssignTechnicianDto.cs
- CreateFaultReportDto.cs
- CreateUserDto.cs
- FaultReportResponseDto.cs
- LoginDto.cs
- UpdateFaultReportStatusDto.cs
- UserResponseDto.cs

Models

- AppUser.cs
- FaultReport.cs

Other

- Migrations
- Program.cs
- appsettings.json
- README.md
What I Learned
This project was developed as a practical backend learning project.
During development, I practiced:
- REST API design
- ASP.NET Core Controllers
- Dependency Injection
- Entity Framework Core
- LINQ
- SQLite
- Database migrations
- DTO design
- Data validation
- HTTP status codes
- JWT authentication
- Claims
- Role-based authorization
- Password hashing
- Pagination
- Search
- Filtering
- API security
- OpenAPI documentation
- Scalar API testing
- User Secrets
Future Improvements
Possible future improvements include:
- Refresh tokens
- Password reset
- Email verification
- Fault comments
- Image and file attachments
- Notification system
- Maintenance history
- Unit tests
- Integration tests
- Docker support
- Frontend dashboard
License
This project was created for educational and portfolio purposes.