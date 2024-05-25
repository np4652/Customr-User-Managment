# User Management Boilerplate for .NET Core

This project is a boilerplate for user management in .NET Core, developed using Dapper. It serves as an excellent alternative to the default .NET Core Identity system, providing a streamlined and flexible solution for user authentication and authorization.

## Features

1. **Easy to Modify and Extend**: The boilerplate is designed with simplicity in mind, allowing developers to easily make modifications and add new features as needed.
2. **Dapper with Onion Architecture**: Utilizes Dapper for data access within an Onion Architecture, following the repository pattern to ensure clean separation of concerns and maintainable code.
3. **Database**: Built to work seamlessly with MS SQL Server, providing robust and efficient database interactions.
4. **.NET Core 8.0**: Leveraging the latest features and improvements of .NET Core 8.0 for optimal performance and modern development practices.

## Getting Started

### Prerequisites

- .NET Core 8.0 SDK
- MS SQL Server

### Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/yourusername/your-repo-name.git
   ```
2. Navigate to the project directory:
   ```sh
   cd your-repo-name
   ```
3. Restore dependencies:
   ```sh
   dotnet restore
   ```
4. Update the database connection string in `appsettings.json` to match your MS SQL Server configuration.

### Usage

1. Apply database migrations:
   ```sh
   dotnet ef database update
   ```
2. Run the application:
   ```sh
   dotnet run
   ```

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests for any features, bug fixes, or improvements.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more information.

## Acknowledgements

- Thanks to the developers and contributors of Dapper and .NET Core for their outstanding tools and frameworks.
- Special mention to the community for continuous support and feedback.

---

Feel free to customize this README to better fit your project and personal preferences. Happy coding!
