# CLED Academy

CledAcademy is a web application built with ASP.NET Core 1.0, Entity Framework Core 1.0, and MySQL. It provides a platform for course management, video streaming and user authentication.
It especially developed to help bachelor students to prepare for their master exams.

I have built this project back to 2016 for commercial purposes, and I have decided to open-source it to help other developers to learn how to build a complete web application with ASP.NET Core.

## Table of Contents

- [Features](#features)
- [Technologies](#technologies)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Features

- User authentication and authorization
- Role-based access control
- Integration with Facebook for authentication
- Database management with Entity Framework Core and MySQL
- MVC architecture
- Payment gateway with hesab.az

## Technologies

- **Languages**: C#, JavaScript
- **Frameworks**: ASP.NET Core, Entity Framework Core
- **Database**: MySQL
- **Authentication**: ASP.NET Identity, Facebook Authentication

## Getting Started

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download)
- [MySQL](https://www.mysql.com/downloads/)

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/nurlanvalizada/CLED.git
    cd CLED
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

3. Update the database:
    ```sh
    dotnet ef database update
    ```

### Running the Application

1. Build and run the application:
    ```sh
    dotnet run --project src/CledAcademy.Web
    ```

2. Open your browser and navigate to `http://localhost:8050`.

## Configuration

The application settings are configured in the `appsettings.json` and `config.json` files. Update these files with your database connection string and other settings as needed.

## Usage

- Register a new user or log in with an existing account.
- Manage academic activities through the provided interface.
- Use the admin panel to manage users and roles.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.