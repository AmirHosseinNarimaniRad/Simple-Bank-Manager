# Simple Bank Manager App 🏦

A modern, cross-platform banking application built with **.NET MAUI** for macOS. This application features a secure authentication system, persistent local storage using SQLite, and a user-friendly interface for managing multiple bank accounts.

## ✨ Features

*   **Authentication System**:
    *   Secure Login & Registration using phone number.
    *   Multi-user support (each user has their own private data).
    *   Session management (stays logged in).
*   **Account Management**:
    *   Create unlimited bank accounts.
    *   Real-time balance tracking.
    *   Deposit and Withdraw functionality.
*   **Transaction History**:
    *   Detailed history of all transactions (Deposit/Withdraw).
    *   Date and time tracking for every action.
*   **Persistent Storage**:
    *   Powered by **SQLite**.
    *   Data persists even after closing the application.
    *   Secure local database (`bankmanager.db3` - excluded from git).
*   **Modern UI**:
    *   Clean, responsive interface.
    *   English language support (LTR).
    *   Native macOS look and feel.

## 🚀 Getting Started

### Prerequisites

*   **.NET 8.0 SDK** (or later)
*   **Xcode** (for macOS development)

### Installation & Run

1.  Clone the repository:
    ```bash
    git clone https://github.com/yourusername/Simple-Bank-Manager.git
    cd Simple-Bank-Manager
    ```

2.  Navigate to the project directory:
    ```bash
    cd BankManagerApp
    ```

3.  Build and Run the application:
    ```bash
    dotnet build -t:Run -f net10.0-maccatalyst
    ```

## 🛠️ Technologies Used

*   **Framework**: .NET MAUI (.NET 8/10)
*   **Language**: C#
*   **Database**: SQLite (sqlite-net-pcl)
*   **Architecture**: MVVM (Model-View-ViewModel) pattern
*   **Platform**: macOS (MacCatalyst)

## 🔒 Privacy & Security

*   User data is stored locally on your device in a SQLite database.
*   The database file (`bankmanager.db3`) is git-ignored to prevent accidental upload of personal data.
*   No data is sent to any external servers.

## 📝 License

This project is licensed under the MIT License.
