# Expense Tracker

A simple expense tracker application built with .NET Core that allows users to manage their expenses, generate statements, and export reports as PDFs. It includes user authentication, transaction management, category management, and date-based statement generation.

## Features

- **User Authentication**: Register and log in securely.
- **Transactions**:
  - Add, edit, and delete expenses.
  - View a list of all transactions.
- **Categories**:
  - Create, edit, and delete categories for organizing expenses.
- **Statement Generation**:
  - Generate statements for a specific date range.
  - Download statements as PDF files.

## Technologies Used

- **Backend**: .NET Core
- **Database**: SQL Server 
- **PDF Generation**: Library used for exporting statements in PDF format

## Getting Started

Follow these steps to set up and run the project locally on your machine.


### Installation

1. **Clone the repository**:
    ```bash
    git clone https://github.com/vandit-muniya81/Expense-Tracker.git
    ```

2. **Navigate to the project directory**:
    ```bash
    cd expense-tracker
    ```

3. **Restore dependencies**:
    ```bash
    dotnet restore
    ```

4. **Set up the database**:
   - Open `appsettings.json` and update the connection string with your database configuration.
   - Run the Entity Framework migrations to set up the database:
     ```bash
     dotnet ef database update
     ```

### Running the Project

1. **Build the project**:
    ```bash
    dotnet build
    ```

2. **Run the project**:
    ```bash
    dotnet run
    ```

3. **Access the application**:
   Open a browser and navigate to `https://localhost:21741` (or the specified port).

### Usage

- **Authentication**: 
  - Register as a new user or log in with existing credentials.
- **Managing Transactions**:
  - Once logged in, you can add new transactions (expenses), edit existing ones, or delete transactions.
- **Managing Categories**:
  - Create custom categories for organizing your transactions, and edit or delete them as needed.
- **Generate Statements**:
  - Choose a date range to generate statements and view summaries of your transactions.
  - Export the generated statement as a PDF.

## Contributing

If you'd like to contribute to this project, please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
