# SOLID Refactoring Documentation - GradeBook Project

This document provides a detailed breakdown of the SOLID principle violations identified in the original codebase and the specific refactoring applied to resolve them.

---

## 1. Single Responsibility Principle (SRP)

### Violation 1: `ItemController`
- **Location**: `Controllers/ItemController.cs` (Methods: `GetAll`, `GetById`)
- **Description**: The controller was responsible for three distinct concerns:
    1.  **Request Handling**: Managing HTTP verbs and routes.
    2.  **Business Logic**: Calculating statistics (`totalCount`, `averageValue`).
    3.  **Logging**: Writing directly to the console (`Console.WriteLine`).
- **Why it's a violation**: A controller should only be responsible for managing the interaction between the user and the service layer. Mixing business logic and logging makes the class harder to test and maintain.
- **Applied Fix**: 
    - Created `GradeService` to handle the statistics calculation logic.
    - Replaced `Console.WriteLine` with the standard `ILogger<GradeController>` abstraction.
    - The controller now only delegates tasks to the service layer.

### Violation 2: `ItemRepository`
- **Location**: `Repositories/ItemRepository.cs`
- **Description**: The repository contained unused state (`_nextId`) and lacked a clear responsibility for data initialization or persistence.
- **Why it's a violation**: A repository should have a single responsibility: providing an abstraction over data access.
- **Applied Fix**: 
    - Refactored into `GradeRepository`.
    - Implemented a clean constructor for seed data.
    - Provided consistent CRUD implementations.

---

## 2. Open/Closed Principle (OCP)

- **Location**: `Controllers/ItemController.cs` (Method: `GetAll`)
- **Description**: The statistics calculation was hardcoded directly inside the `GetAll` action.
- **Why it's a violation**: If a requirement would arise to add more statistics, the controller's code would have to be modified. The class was not "closed for modification".
- **Applied Fix**: 
    - Moved statistics logic to `GradeService.GetStatisticsAsync()`.
    - Statistics are now returned as a `GradeStatistics` DTO.
    - New statistics can now be added to the service without touching the controller.

---

## 3. Interface Segregation Principle (ISP)

- **Location**: `Interfaces/IItemReader.cs`
- **Description**: The original interface only supported reading. While not a direct violation of its own methods, it was inadequate for the growing needs of the application.
- **Why it's a violation**: If we added write methods to `IItemReader`, classes that only need to read would be forced to implement methods they don't use.
- **Applied Fix**: 
    - Split the interfaces into `IGradeReader` (Read-only) and `IGradeWriter` (Write-only).
    - Created `IGradeRepository` which inherits from both for a unified data access layer.
    - This allows clients (like a read-only dashboard) to depend only on `IGradeReader`.

---

## 4. Liskov Substitution Principle (LSP)

- **Status**: No direct violation was present in the original code due to its simplicity, but the design was prone to issues.
- **Why it matters**: Any subclass should be substitutable for its base class without breaking the application.
- **Applied Fix / Prevention**: 
    - By using interfaces (`IGradeRepository`) instead of deep class hierarchies, we ensure that any implementation (e.g., `SqlGradeRepository`, `MongoGradeRepository`) will satisfy the contract required by the `GradeService`.
    - Removed unnecessary `virtual` keywords from the repository where they didn't serve a clear extension purpose, reducing the risk of improper overrides.

---

## 5. Dependency Inversion Principle (DIP)

### Violation A: Missing DI Registrations
- **Location**: `Program.cs`
- **Description**: The `IItemReader` interface was used in the controller, but its implementation (`ItemRepository`) was never registered in the Dependency Injection container.
- **Why it's a violation**: High-level modules (Controller) were technically depending on an abstraction, but the system lacked the configuration to provide the low-level implementation, leading to runtime failures.
- **Applied Fix**: 
    - Added registrations in `Program.cs`:
        ```csharp
        builder.Services.AddSingleton<IGradeRepository, GradeRepository>();
        builder.Services.AddScoped<IGradeService, GradeService>();
        ```

### Violation B: Logging Dependency
- **Location**: `Controllers/ItemController.cs`
- **Description**: Dependency on the static `Console` class for logging.
- **Why it's a violation**: The controller was dependent on a specific low-level implementation of logging (Standard Output), making it impossible to redirect logs without changing the code.
- **Applied Fix**: 
    - Injected `ILogger<GradeController>` via the constructor.
    - The controller now depends on an abstraction provided by the framework.

---

## 5. Domain Logic & Clean Code Improvements

- **Renaming**: Changed `Item` to `Grade` across the entire solution.
    - *Reasoning*: Following the principle of **Ubiquitous Language**, the code now matches the project's domain (`GradeBook`).
- **Async Consistency**: Ensured all data access and service methods use `Task` and `await`.
    - *Reasoning*: Improves scalability and follows modern C# best practices for I/O-bound operations.

---

## 6. Infrastructure & Platform Upgrade

- **Framework Upgrade**: Upgraded the project from **.NET 8** to **.NET 10**.
    - *Reasoning*: To leverage the latest performance improvements, security patches, and language features provided by the .NET 10 platform.
    - *Verification*: Successfully performed a clean build targeting `net10.0`.

---

## 7. Business Logic Implementation

### Feature: Top N Passing Grades Filter
- **Location**: `GradeService.GetTopPassingGradesAsync(int count)`
- **Logic**:
    1.  Filters grades that are **Active** (`IsActive == true`).
    2.  Filters grades that are **Passing** (`Value >= 5`).
    3.  Returns only the first **N** results (where N is provided by the user).
- **API Endpoint**: `GET /api/grade/passing/{n}`
- **Reasoning**: This encapsulates specific domain rules within the Service Layer, keeping the Controller and Repository lean.

---

## 8. Repository Refactoring (External Data)

- **Data Source**: Replaced the in-memory `List<Grade>` with an external JSON source hosted on GitHub Gist.
- **Implementation**:
    - Used `HttpClient` to fetch data asynchronously.
    - Implemented `GistResponse` and `GistItem` DTOs to map the external schema (fields: `id`, `value`, `isActive`) to the internal `Grade` model.
- **Lifetime Change**: Changed `GradeRepository` registration to `Scoped` in `Program.cs` as it now depends on `HttpClient`.
- **Read-Only Notice**: Since the external source is a static gist, write operations (`Add`, `Update`, `Delete`) now throw a `NotSupportedException`.

---

## 9. How to Run and Test

### Prerequisites
- .NET 10 SDK installed.

### Running the Application
1.  Navigate to the project directory:
    ```powershell
    cd Siemens.Internship2026.GradeBook
    ```
2.  Start the application:
    ```powershell
    dotnet run
    ```

### Testing with HTTP
The application starts on `http://localhost:5030` by default.
- **Landing Page**: `http://localhost:5030/`
- **All Grades**: `http://localhost:5030/api/grade`
- **Filter Passing Grades**: `http://localhost:5030/api/grade/passing/5` (replace 5 with any number N).

### Testing with HTTPS
1.  Trust the .NET development certificate:
    ```powershell
    dotnet dev-certs https --trust
    ```
2.  Run with the HTTPS profile:
    ```powershell
    dotnet run --launch-profile https
    ```
3.  Access the secure endpoints:
    - **Secure API**: `https://localhost:7069/api/grade`
    - **Secure Filter**: `https://localhost:7069/api/grade/passing/5`
