# CleanRoute

CleanRoute is a small waste-collection management application for local communities. Residents can create collection requests, collectors can view their assigned schedules and confirm completed pickups, and administrators can assign routes, review reports, and see user accounts.

It is deliberately written with beginner-readable ASP.NET Core Razor Pages, C#, MySQL, plain HTML, CSS, and JavaScript. There is no Bootstrap and no AI feature in the application.

## Features

- Resident registration and sign-in
- Secure password hashing with PBKDF2 and cookie authentication
- Role permissions for Residents, Collectors, and Administrators
- Waste collection requests with type, location, notes, and preferred date
- Administrator scheduling and collector completion workflow
- Collector-specific schedule view
- Operations report and user list for administrators
- Responsive custom interface with no CSS framework

## Project structure

```
CleanRoute/
├── Data/             Database context and table mapping
├── Database/         MySQL schema and realistic sample data
├── Models/           Clear data classes and enums
├── Pages/            Razor Pages grouped by feature
├── Services/         Authentication and request business logic
├── wwwroot/          Custom CSS and small JavaScript enhancement
└── docs/screenshots/ Interface previews
```

## Before you run it

CleanRoute is a separate database and a separate repository. It does not use, alter, or connect to TradeTrack.

You need .NET 8 SDK and MySQL Server 8 (or compatible MySQL server) installed locally. This project contains no real database credentials.

## Set up the MySQL database

1. Open MySQL Workbench and connect to **your own local server**.
2. Open and run `Database/01_create_schema.sql`.
3. Open and run `Database/02_seed_data.sql`.

The schema script creates only `CleanRouteDb`. It uses `CREATE ... IF NOT EXISTS`, so it will not delete any existing database, including TradeTrack.

## Set the connection securely

From the CleanRoute project folder, run the following once. Replace the placeholder parts with your local MySQL user name and password **on your own computer**:

```powershell
dotnet user-secrets set "ConnectionStrings:CleanRouteDb" "Server=localhost;Port=3306;Database=CleanRouteDb;User ID=YOUR_MYSQL_USER;Password=YOUR_MYSQL_PASSWORD;"
```

User Secrets stores this value outside the project folder and outside Git. Do not put a real password in `appsettings.json`, commit it, or send it in chat.

## Run the application

```powershell
dotnet restore
dotnet run
```

Open the HTTPS address shown in the terminal. You can create a new resident account or use the locally seeded examples below.

| Role          | Email                    | Local sample password |
| ------------- | ------------------------ | --------------------- |
| Administrator | `admin@cleanroute.local` | `CleanRoute123!`      |
| Collector     | `chinedu@example.com`    | `CleanRoute123!`      |
| Resident      | `ada@example.com`        | `CleanRoute123!`      |

These are deliberately local demo accounts. Change or remove them before any real deployment.

## Git and GitHub

From the `CleanRoute` folder, create a new repository for this project only:

```powershell
git init
git add .
git commit -m "Create CleanRoute waste collection app"
git branch -M main
git remote add origin https://github.com/YOUR-GITHUB-USERNAME/cleanroute.git
git push -u origin main
```

First create the empty `cleanroute` repository in your GitHub account, then replace the sample GitHub address above. This process does not overwrite TradeTrack because it runs inside the separate CleanRoute folder.

## Screens

The included [dashboard preview](docs/screenshots/dashboard-preview.svg) and [request details preview](docs/screenshots/request-preview.svg) illustrate the custom interface. They are design previews generated from the included pages; after running the app locally, you can replace them with browser captures for a portfolio submission.

## Build check

The application source was checked for expected project files, Razor Page links, service registrations, database scripts, and configuration safety. A build was also attempted with the installed .NET SDK, but NuGet package restore was blocked because this workspace could not access `api.nuget.org`. On a normal local connection, run:

```powershell
dotnet restore
dotnet build
```

The only external dependency is `Pomelo.EntityFrameworkCore.MySql`, which provides the MySQL connection support.
