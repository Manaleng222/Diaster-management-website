# 🌍 Disaster Allowance Foundation - ASP.NET MVC Prototype

The **Disaster Allowance Foundation** is a comprehensive web application designed to facilitate disaster relief efforts through user management, incident reporting, donation processing, and volunteer coordination.
This **ASP.NET MVC prototype** demonstrates the core functionality of a disaster response management system.

---

## 🚀 Features

### 1. User Authentication System

* **Secure Registration & Login**: ASP.NET Identity-based authentication
* **Role Management**: Donor, Volunteer, Admin roles
* **Profile Management**: User profile creation & management
* **Email Confirmation**: Basic verification workflow

### 2. Disaster Incident Reporting

* **Incident Submission**: Categorized disaster type reporting
* **Interactive Map**: Visual representation of active incidents (Leaflet.js)
* **Real-time Tracking**: Status updates & severity indicators
* **Validation**: Client & server-side validation

### 3. Donation Management System

* **Donation Types**: Financial, supplies, or combined
* **Payment Integration**: Multiple payment methods supported
* **Impact Tracking**: Statistics & progress visualization
* **Anonymous Donations**: Option available

### 4. Volunteer Coordination

* **Volunteer Registration**: Skills-based system
* **Opportunity Management**: Categorized volunteer opportunities
* **Urgency Indicators**: Priority-based task assignment
* **Availability Tracking**: Flexible scheduling

---

## 🛠 Technology Stack

**Backend**

* ASP.NET MVC 5 / ASP.NET Core MVC
* ASP.NET Identity (authentication)
* Entity Framework with SQL Server
* Data Annotations & Fluent Validation

**Frontend**

* Bootstrap 5.3.0
* Font Awesome 6.4.0
* Leaflet.js (interactive maps)
* jQuery (dynamic interactions)

**Development Tools**

* Git + Azure DevOps
* Azure Pipelines (CI/CD)
* Azure Boards (Project Management)

---

## 📁 Project Structure

```
APPR_P_2/
├── Controllers/
│   ├── AccountController.cs
│   ├── DonationController.cs
│   ├── VolunteerController.cs
│   └── HomeController.cs
├── Data/
│   ├──ApplicationDbContext.cs
│   ├──SeedData.cs
├── Models/
│   ├──AdminDashboardViewModel.cs
│   ├──ApplicationUser.cs
│   ├──Donation.cs
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── DonationViewModel.cs
│   ├── VolunteerViewModel.cs
│   ├──VolunteerProfile.cs
│   └── IncidentReportViewModel.cs
│   ├──IncidentReport.cs
├── Views/
│   ├── Account/
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   ├── Admin/
│   │   └── Index.cshtml
│   │   └──Indcident.cshtml
│   │   └──Users.cshtml
│   │   └──Volunteer.cshtml
│   ├── Donation/
│   │   └── Donate.cshtml
│   ├── Volunteer/
│   │   └── Volunteer.cshtml
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── ReportIncident.cshtml
│   └── Shared/
│       └── _Layout.cshtml
├── Content/
│   └── Site.css
└── Scripts/
    └── Custom/
```

---

## 🗄 Database Schema

**Core Tables**

* `Users`: User accounts and authentication
* `Donations`: Financial & supply donation records
* `IncidentReports`: Disaster incident tracking
* `Volunteers`: Volunteer profiles & skills
* `InventoryItems`: Supply catalog management
* `ReliefProjects`: Disaster relief project coordination

**Relationships**

* One-to-Many: Users → Donations
* One-to-Many: Users → IncidentReports
* Many-to-Many: Volunteers ↔ Skills
* Many-to-Many: ReliefProjects ↔ InventoryItems

---

## 🚀 Getting Started

### Prerequisites

* .NET Framework 4.8 **or** .NET 6+
* SQL Server 2016+
* Visual Studio 2022+
* Git

### Installation

```bash
# Clone repository
git clone https://dev.azure.com/your-organization/DisasterAllowanceFoundation/_git/DisasterAllowanceFoundation
cd APPR_P_2

## Clone github resitory
https://github.com/st123896/Diaster-management-website.git

# Create Gitflow branches
git checkout -b develop
git push -u origin develop

git checkout -b feature/user-auth
git push -u origin feature/user-auth

git checkout -b feature/incident-reporting
git push -u origin feature/incident-reporting

git checkout -b feature/donation-system
git push -u origin feature/donation-system

git checkout -b feature/volunteer-management
git push -u origin feature/volunteer-management
```

**Database Setup**

```sql
-- Run initial migration or database script
Update-Database
```

**Configuration**

* Update connection strings in `Web.config`
* Configure email SMTP settings
* Add payment gateway credentials

**Build & Run**

```bash
dotnet build
dotnet run
```

**Access Application**
👉 Navigate to [https://localhost:7000](https://localhost:7000)
👉 Default admin credentials available in seed data

---

## 🧪 Testing

### Unit Tests

```bash
dotnet test
```

### Integration Tests

* Database connectivity
* Payment gateway integration
* Email services

### User Acceptance Tests

* **Donor Flow**: Registration → Donation → Confirmation
* **Volunteer Flow**: Registration → Skill selection → Opportunity application
* **Admin Flow**: User management → Incident monitoring → Reporting

---

## 🔧 Configuration

### App Settings (`Web.config`)

```xml
<appSettings>
  <add key="PaymentGateway:ApiKey" value="your-api-key"/>
  <add key="Email:SmtpServer" value="smtp.gmail.com"/>
  <add key="Email:Port" value="587"/>
  <add key="MapService:ApiKey" value="your-map-api-key"/>
</appSettings>
```

### Environment Variables

* `ASPNETCORE_ENVIRONMENT`: Development / Production
* `DATABASE_CONNECTION`: Connection string
* `PAYMENT_GATEWAY_URL`: Payment service endpoint

---

## 📊 API Endpoints

### Authentication

* `POST /Account/Login` → Authenticate user
* `POST /Account/Register` → Register new user
* `POST /Account/ForgotPassword` → Password recovery

### Donations

* `GET /Donation/Donate` → Donation form
* `POST /Donation/ProcessDonation` → Process donation
* `GET /Donation/ThankYou` → Confirmation page

### Volunteers

* `GET /Volunteer/Index` → View opportunities
* `POST /Volunteer/RegisterVolunteer` → Register volunteer
* `GET /Volunteer/ThankYou` → Confirmation page

### Incidents

* `GET /Home/ReportIncident` → Report form
* `POST /Home/ReportIncident` → Submit report
* `GET /Home/Incidents` → View active incidents

---

## 🛡 Security Features

* Password hashing (PBKDF2 + salt)
* CSRF protection (anti-forgery tokens)
* SQL injection prevention (parameterized queries)
* XSS protection (input sanitization)
* Role-based authorization
* HTTPS enforcement

---

## 📈 Performance Optimization

**Database**

* Indexed frequently queried columns
* Query optimization + caching
* Connection pooling

**Application**

* Client-side validation
* AJAX for dynamic content
* Bundled & minified assets
* CDN for static resources

---
# Demo Credentials

| Role      | Email                   | Password        |
| --------- | ----------------------- | --------------- |
| Admin     | `admin@disaster.org`    | `Admin123!`     |
| Donor     | `donor@example.com`     | `Donor123!`     |
| Volunteer | `volunteer@example.com` | `Volunteer123!` |

> These demo credentials are for testing only. Do not use them in production.



---
## 🤝 Contributing

### Workflow

1. Fork the repository
2. Create feature branch

   ```bash
   git checkout -b feature/AmazingFeature
   ```
3. Commit changes

   ```bash
   git commit -m 'Add AmazingFeature'
   ```
4. Push to branch

   ```bash
   git push origin feature/AmazingFeature
   ```
5. Open Pull Request

## 🔄 DevOps & CI/CD

### Repository Structure
- **Main Branch**: Production-ready code
- **Develop Branch**: Integration branch for features
- **Feature Branches**: Individual feature development
- **Release Branches**: Release preparation
- **Hotfix Branches**: Emergency production fixes

### Build Pipeline
The Azure DevOps pipeline automatically:
- Builds the solution on every push to `develop` and `main`
- Runs unit tests
- Publishes artifacts
- Deploys to test environment on successful builds

### Quality Gates
- ✅ Code review required for all pull requests
- ✅ Build must pass before merge
- ✅ Work items must be linked to commits
- ✅ Unit tests must pass

### Deployment Environments
- **Test**: Automatic deployment from `develop` branch
- **Staging**: Manual deployment from `release/*` branches
- **Production**: Manual deployment from `main` branch

### Monitoring
- Build status badges in README
- Email notifications on failure
- Deployment history tracking
- 
### Code Standards

* Follow ASP.NET MVC best practices
* Use meaningful commit messages
* Include unit tests for new features
* Update documentation

---

### Author 
- Tshepiso Mokhine

---


⚠️ **Note**: This is a prototype implementation. For production deployment, add further **security hardening**, **error handling**, and **performance optimizations**.

