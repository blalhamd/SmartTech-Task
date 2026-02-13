# EduNexus - SmartTech Task

EduNexus is a robust Employee Management System built with a focus on security, scalability, and a structured **Maker-Checker** workflow. It provides a comprehensive solution for managing employee lifecycles, from creation to updates and deletions, ensuring all critical actions are reviewed and approved before taking effect.

## Overview

The system implements a dual-control mechanism where one user (the **Maker**) initiates a request and another user (the **Checker**) reviews and either approves or rejects it. This ensures data integrity and prevents unauthorized or accidental changes to the employee database. The project is built using **ASP.NET Core 8.0** following **Clean Architecture** principles.

---

## Key Features

- **Maker-Checker Workflow**: Mandatory review process for creating, updating, or deleting employee records.
- **Role-Based Access Control (RBAC)**: Distinct permissions for Makers, Checkers, and Employees.
- **Soft Deletion**: Records are marked as deleted rather than physically removed, preserving audit trails.
- **Audit Logging**: Automatic tracking of creation and modification timestamps and user IDs.
- **JWT Authentication**: Secure stateless authentication using JSON Web Tokens.
- **Data Validation**: Strong server-side validation using FluentValidation.
- **Global Error Handling**: Centralized middleware for consistent API error responses.

---

## Database Schema

The database is organized into two primary schemas: `dbo` for business entities and `Security` for identity-related tables.

### Core Entities

| Table | Schema | Description |
| :--- | :--- | :--- |
| **Employees** | `dbo` | Stores active employee profiles linked to system users. |
| **EmployeeRequests** | `dbo` | Tracks all pending and historical change requests (Create/Update/Delete). |
| **Users** | `Security` | Extended Identity users with full names and audit fields. |
| **Roles** | `Security` | Defines system roles: `Maker`, `Checker`, and `Employee`. |

---

## Entity Relationships

The following relationships define the core data structure:

- **ApplicationUser (1) ↔ (1) Employee**: Each employee record is linked to a unique system user.
- **Employee (1) ↔ (N) EmployeeRequests**: An employee can have multiple historical requests, but typically only one pending request at a time.
- **ApplicationUser (N) ↔ (N) ApplicationRole**: Standard Identity relationship for role management.
- **BaseEntity**: All business entities inherit from a base class providing `Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, and `IsDeleted`.

---

## Technologies Used

- **Backend**: ASP.NET Core 8.0 (Web API)
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Security**: ASP.NET Core Identity, JWT Bearer Authentication
- **Validation**: FluentValidation
- **Mapping**: Manual DTO/ViewModel mapping for precision
- **Dependency Injection**: Native .NET DI Container

---

## Technical Highlights

- **Clean Architecture**: Separation of concerns across `API`, `Business`, `Core`, `Domain`, `Infrastructure`, and `Shared` layers.
- **Generic Repository Pattern**: Abstraction over EF Core for consistent data access.
- **Unit of Work**: Ensures atomic transactions across multiple repository operations.
- **Global Query Filters**: Automatically filters out soft-deleted records from all queries.
- **Result Pattern**: Uses a custom `Result` and `ValueResult` type for predictable service-to-controller communication.

---

## Key Use Cases

1.  **Employee Onboarding**: A Maker submits a `Create` request with employee details. The system remains unchanged until a Checker approves it, at which point the Employee and User records are created.
2.  **Profile Update**: A Maker initiates an `Update` request. The system stores the `OldData` and `NewData` as snapshots. Upon approval, the employee record is updated.
3.  **Termination**: A Maker submits a `Delete` request. After Checker approval, the record is soft-deleted.
4.  **Rejection Workflow**: If a Checker rejects a request, they must provide a reason, and the Maker is notified (via status change).

---

## API Documentation


### Authentication
| Method | Endpoint | Description | Auth Required |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/v1/auth/login` | Authenticate user and receive JWT. | No |
| `POST` | `/api/v1/auth/forget-password` | Generate a password reset token. | No |
| `POST` | `/api/v1/auth/reset-password` | Reset password using token. | No |

### Employee Requests (Maker Actions)
| Method | Endpoint | Description | Permission |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/v1/employee-requests` | Create a new employee request. | `EmployeeRequest.Create` |
| `POST` | `/api/v1/employee-requests/update/{id}` | Request an update for an employee. | `EmployeeRequest.Update` |
| `DELETE` | `/api/v1/employee-requests/{id}` | Request deletion of an employee. | `EmployeeRequest.Delete` |

### Employee Requests (Checker Actions)
| Method | Endpoint | Description | Permission |
| :--- | :--- | :--- | :--- |
| `PATCH` | `/api/v1/employee-requests/{id}/approve-create` | Approve a creation request. | `EmployeeRequest.Approve` |
| `PATCH` | `/api/v1/employee-requests/{id}/approve-update` | Approve an update request. | `EmployeeRequest.Approve` |
| `PATCH` | `/api/v1/employee-requests/{id}/approve-delete` | Approve a deletion request. | `EmployeeRequest.Approve` |
| `PATCH` | `/api/v1/employee-requests/{id}/reject` | Reject any pending request. | `EmployeeRequest.Reject` |

### Employees
| Method | Endpoint | Description | Permission |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/employees` | List all active employees (Paginated). | `Employee.View` |
| `GET` | `/api/v1/employees/{id}` | Get detailed employee profile. | `Employee.ViewDetails` |

---

> **Note**: All protected endpoints require a valid `Authorization: Bearer <token>` header.


live
