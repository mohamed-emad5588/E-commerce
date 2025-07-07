# E-commerce
A modern and secure ASP.NET Core Web API for E-Commerce with JWT Authentication, Role-based Authorization, Email Confirmation, Product Management, and more.
# 🛒 E-Commerce API - ASP.NET Core

A powerful and extendable backend for an e-commerce platform built with **ASP.NET Core Web API** and **Entity Framework Core**.  
It includes modern features like **JWT authentication**, **email confirmation**, **role-based access**, **product & order management**, and more.

---

##   Features

- ✅ User Registration & Login with **JWT Authentication**
- ✅ **Email Confirmation** via 6-digit code (valid for 10 minutes)
- ✅ Role-based Authorization (`Admin`, `User`)
- ✅ CRUD for Products, Categories, Orders, Users
- ✅ Image upload for products
- ✅ Filtered Search + Pagination for products
- ✅ Product Reviews (with authenticated users)
- ✅ Cart system
- ✅ Dashboard Statistics for admin
- ✅ Secure Passwords with `BCrypt`
- ✅ Email service using `MailKit` and Gmail SMTP

---

## 🛠 Technologies Used

- **ASP.NET Core 7 Web API**
- **Entity Framework Core**
- **SQL Server**
- **AutoMapper**
- **JWT (JSON Web Tokens)**
- **MailKit** for email sending
- **BCrypt.Net-Next** for password hashing
- **Swagger** for API documentation

---

##  Project Structure

```bash
EcommerceAPI/
├── Controllers/
├── DTOs/
├── Models/
├── Repositories/
│   ├── Interfaces/
│   └── Implementations/
├── Services/
├── Mappings/
├── Wrappers/
├── Program.cs
├── appsettings.json (not committed)
└── README.md
