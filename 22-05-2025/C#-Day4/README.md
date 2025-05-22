# Product & Order Management System - SOLID Principles

This project demonstrates a simple console-based Product and Order Management System in C#, designed with the SOLID principles in mind.

---

## SOLID Principles Implementation

### 1. **Single Responsibility Principle (SRP)**
- **Each class has one responsibility.**
    - `Product` and `Order` classes only represent data.
    - `ProductRepository` and `OrderRepository` handle data storage/retrieval.
    - `ProductService` and `OrderService` contain business logic.
    - `ManageProducts` and `ManageOrders` handle user interaction (console UI).

### 2. **Open/Closed Principle (OCP)**
- **Classes are open for extension, closed for modification.**
    - You can add new features (e.g., new search/filter logic) by extending services or repositories without changing their existing code.
    - The use of interfaces (`IRepository<T, K>`) allows easy extension for new entities.

### 3. **Liskov Substitution Principle (LSP)**
- **Derived classes can substitute their base types.**
    - `ProductRepository` and `OrderRepository` both implement the generic `IRepository<T, K>` interface, so they can be used interchangeably where the interface is expected.

### 4. **Interface Segregation Principle (ISP)**
- **Clients are not forced to depend on methods they do not use.**
    - The generic `IRepository<T, K>` interface contains only CRUD methods relevant to repositories.
    - If needed, more specific interfaces can be created for specialized behaviors.
    - Reducing Product quantity are done at the service level, not the repository level.

### 5. **Dependency Inversion Principle (DIP)**
- **High-level modules do not depend on low-level modules; both depend on abstractions.**
    - Services (`ProductService`, `OrderService`) depend on the `IRepository<T, K>` abstraction, not on concrete repository classes.
    - Dependencies are injected via constructors, making the system flexible and testable.

---

## Future Implementation of the Project

- To implement proper error handling, consider using exceptions or a logging framework.
- To add a new entity (e.g., Customer), create a model, repository, service, and frontend class, all following the SOLID principles.
---