# Simple Bank Manager App 🏦

A modern, cross-platform banking application built with **.NET MAUI (.NET 10)** for macOS and Android. This application features a secure authentication system, persistent local storage using **Entity Framework Core**, and a user-friendly interface with Persian Calendar support.

## ✨ Features

*   **Authentication System 🔐**:
    *   Secure Login & Registration (Username/Email/Password).
    *   Password Hashing using **BCrypt**.
    *   Multi-user support (Data isolation per user).
    *   Session management (Auto-logout on invalid session).
    *   Password Reset functionality.

*   **Account Management 💳**:
    *   Create unlimited wallets/accounts.
    *   Real-time balance tracking.
    *   Deposit and Withdraw functionality.

*   **Transaction History 📝**:
    *   Detailed history of all transactions.
    *   **Persian Calendar (Jalali)** support for dates.
    *   **Categorization**:
        *   Automatic category detection based on transaction type.
        *   Database-driven categories (Seeded defaults).
        *   *Ready for future user-specific categories.*

*   **Persistent Storage 💾**:
    *   Powered by **Entity Framework Core (Sqlite)**.
    *    robust database migrations.
    *   Secure local database (`bankmanager.db`).

*   **Modern UI 🎨**:
    *   Clean, responsive interface.
    *   Dark/Light mode compatible colors.
    *   Persian (RTL) friendly design elements.
    *   Native macOS and Android look and feel.

## 🚀 Getting Started

### Prerequisites

*   **.NET 10.0 SDK** (or later)
*   **Xcode** (for macOS development)
*   **Android SDK** (for Android development)

### Architecture

The solution consists of three main projects:
1.  **BankManagerApp**: The main MAUI application (UI & Logic).
2.  **BankManager.Data**: Class library containing Entities and DbContext (EF Core).
3.  **MigrationHelper**: A console application used for generating and applying EF Core migrations securely.

### Installation & Run

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/yourusername/Simple-Bank-Manager.git
    cd Simple-Bank-Manager
    ```

2.  **Run on macOS (MacCatalyst)**:
    We have provided a helper script to clean, build, and run the app:
    ```bash
    chmod +x run.sh
    ./run.sh
    ```
    *Or manually:*
    ```bash
    dotnet build -f net10.0-maccatalyst -c Debug
    open "BankManagerApp/bin/Debug/net10.0-maccatalyst/maccatalyst-arm64/BankManagerApp.app"
    ```

3.  **Run on Android**:
    ```bash
    dotnet build BankManagerApp/BankManagerApp.csproj -f net10.0-android -c Debug
    ```
    The signed APK will be available at:
    `BankManagerApp/bin/Debug/net10.0-android/com.companyname.bankmanagerapp-Signed.apk`

## 🛠️ Technologies Used

*   **Framework**: .NET MAUI (.NET 10)
*   **Language**: C#
*   **ORM**: Entity Framework Core (Microsoft.EntityFrameworkCore.Sqlite)
*   **Security**: BCrypt.Net-Next
*   **Architecture**: MVVM (Model-View-ViewModel) pattern
*   **Design**: XAML

## 🔒 Privacy & Security

*   User data is stored locally in an isolated Sandbox environment (`ApplicationData`).
*   Passwords are **never** stored in plain text; they are hashed using BCrypt.
*   Database connections use secure connection strings.

## 📝 License

This project is licensed under the MIT License.
