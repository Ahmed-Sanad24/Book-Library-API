# Book Library API

This API manages a book library system that includes user accounts, book management, borrowing flows, and notifications. Below is a structured overview.

---

## 1. Prerequisites
- .NET 6 or later installed
- SQL Server or another database provider configured
- Visual Studio or VS Code (optional but recommended)

---

## 2. Project Setup
1. **Clone/Download** the repository.
2. **Restore Packages**:  
   - Using CLI: `dotnet restore`
   - Using Visual Studio: Build the solution.
3. **Database Configuration**:  
   - Check `appsettings.json` (in the root) for the connection string under `"ConnectionStrings"`.
   - Adjust to match your local or production DB.
4. **Migrations** (if not already applied):  
   - Using CLI: `dotnet ef database update`
5. **Run the Application**:  
   - Using CLI: `dotnet run`
   - Using Visual Studio: Press `F5`.

---

## 3. Project Structure & Logic

### 3.1 `Program.cs`
- Registers services (e.g., Identity, repositories, email service).
- Applies role seeding (if `SeedRoles` is invoked).
- Configures middleware and launches the Kestrel server.

### 3.2 Controllers

#### 3.2.1 `AccountController.cs`
- **Endpoints**:  
  - `POST /api/account/register` *(Authorization: `AllowAnonymous`)*  
    Creates a new user account with role assignment.  
  - `POST /api/account/login` *(Authorization: `AllowAnonymous`)*  
    Authenticates user credentials, returns JWT or token on success.
- **Logic**:  
  - Uses Identity services to handle user creation, password hashing, token generation via configured sign-in manager.

#### 3.2.2 `BookController.cs`
- **Endpoints**:  
  - `GET /api/books` *(Authorization: `Admin`, `User`)*  
    Retrieves a list of all books.  
  - `GET /api/books/{id}` *(Authorization: `Admin`, `User`)*
    Retrieves details of a specific book by ID.  
  - `POST /api/books` *(Authorization: `Admin`)*  
    Creates a new book (requires admin or privileged role).  
  - `PUT /api/books/{id}` *(Authorization: `Admin`)*  
    Updates an existing book’s info.  
  - `DELETE /api/books/{id}` *(Authorization: `Admin`)*  
    Removes a book from the system.
- **Logic**:  
  - Relies on `GenericRepository<Book>` for underlying CRUD.
  - Checks user roles for authorization on certain endpoints.

#### 3.2.3 `BorrowedBookController.cs`
- **Endpoints**:  
  - `POST /api/borrow/{bookId}` *(Authorization: `User`)*
    Creates a borrowed record for a user checking out a book.  
  - `POST /api/borrow/return/{bookId}` *(Authorization: `User`)*  
    Marks a borrowed book as returned.  
  - `GET /api/borrow` *(Authorization: `User`)*  
    Lists all borrowed books for a user.
- **Logic**:  
  - Uses `GenericRepository<BorrowedBook>` for simple data operations.
  - Ensures a user has not already borrowed the same book before completing the request.

#### 3.2.4 `NotificationController.cs`
- **Endpoints**:  
  - `POST /api/notification/notify-overdue` *(Authorization: `Admin`)*  
    Sends a notification message (often via email) to a user which has borrowed at least one book more than 14 days ago and did not return it yet.
- **Logic**:  
  - Leverages `IEmailService` (implemented in `EmailService`) to send emails.
  - Stores notifications if needed for logging or retrieval.

### 3.3 Repositories & Services

- **`GenericRepository<T>`**  
  - Provides common CRUD operations (Add, GetAll, Update, Delete).
- **`UnitOfWork`**  
  - Manages multiple repository transactions, ensuring atomic operations.
- **`IEmailService` / `EmailService`**  
  - Abstracts email sending; can be replaced by a different provider.
- **`SeedRoles.cs`**  
  - Initializes default roles (e.g., Admin, User) on first run if they do not yet exist.

---

## 4. Running and Testing
1. **Swagger/OpenAPI**  
   - By default, the API may expose Swagger docs at `http://localhost:<port>/swagger`, it often opens automatically.
   - Explore endpoints, run test requests, view details.
2. **Manual Testing**  
   - Postman or cURL can be used with valid tokens for authorized endpoints.

---

## 5. Deployment Tips
- Update connection strings and environment variables in production.
- Use a secure certificate and HTTPS in deployment scenarios.
- Configure roles and user accounts carefully to limit unauthorized access.

---

**Enjoy exploring and extending the Book Library API!**