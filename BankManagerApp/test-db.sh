#!/bin/bash

echo "🧪 تست دیتابیس..."
echo ""

DB_PATH=~/Documents/bankmanager.db

# پاک کردن فایل قدیمی
rm -f "$DB_PATH"

# ساخت دیتابیس با sqlite3
sqlite3 "$DB_PATH" << 'EOF'
CREATE TABLE IF NOT EXISTS User (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PhoneNumber TEXT,
    Password TEXT,
    MonthlyBudget REAL
);

CREATE TABLE IF NOT EXISTS bank_accounts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER,
    Name TEXT,
    Balance REAL,
    CreatedAt TEXT
);

CREATE TABLE IF NOT EXISTS transactions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    AccountId INTEGER,
    Type TEXT,
    Category TEXT,
    IncomeType TEXT,
    Amount REAL,
    Description TEXT,
    DateTime TEXT
);

-- اضافه کردن داده‌های تستی
INSERT INTO User (PhoneNumber, Password, MonthlyBudget) VALUES ('09123456789', 'test123', 5000000);
INSERT INTO bank_accounts (UserId, Name, Balance, CreatedAt) VALUES (1, 'کیف پول اصلی', 1000000, datetime('now'));
INSERT INTO transactions (AccountId, Type, Category, Amount, DateTime) VALUES (1, 'Deposit', 'حقوق', 5000000, datetime('now'));
INSERT INTO transactions (AccountId, Type, Category, Amount, DateTime) VALUES (1, 'Withdraw', 'خوراکی', 50000, datetime('now'));
EOF

echo "✅ دیتابیس ساخته شد: $DB_PATH"
echo ""
echo "📊 جدول‌ها:"
sqlite3 "$DB_PATH" ".tables"
echo ""
echo "👤 کاربران:"
sqlite3 "$DB_PATH" "SELECT * FROM User;"
echo ""
echo "💰 حساب‌ها:"
sqlite3 "$DB_PATH" "SELECT * FROM bank_accounts;"
echo ""
echo "📝 تراکنش‌ها:"
sqlite3 "$DB_PATH" "SELECT Id, Type, Category, Amount FROM transactions;"
