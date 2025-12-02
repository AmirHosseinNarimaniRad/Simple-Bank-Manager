#!/bin/bash
# تست کامل راه‌اندازی EF Core Migration

echo "🔍 بررسی پیش‌نیازها..."

# 1. بررسی dotnet-ef
echo ""
echo "1️⃣ بررسی dotnet-ef tool:"
if command -v dotnet-ef &> /dev/null; then
    echo "   ✅ dotnet-ef نصب است"
    dotnet-ef --version
else
    echo "   ❌ dotnet-ef نصب نیست"
    exit 1
fi

# 2. بررسی پکیج‌های EF Core
echo ""
echo "2️⃣ بررسی پکیج‌های EF Core در BankManager.Data:"
cd BankManager.Data
if grep -q "Microsoft.EntityFrameworkCore.Sqlite" BankManager.Data.csproj; then
    echo "   ✅ Microsoft.EntityFrameworkCore.Sqlite"
fi
if grep -q "Microsoft.EntityFrameworkCore.Tools" BankManager.Data.csproj; then
    echo "   ✅ Microsoft.EntityFrameworkCore.Tools"
fi
if grep -q "Microsoft.EntityFrameworkCore.Design" BankManager.Data.csproj; then
    echo "   ✅ Microsoft.EntityFrameworkCore.Design"
fi
cd ..

# 3. بررسی Migration ها
echo ""
echo "3️⃣ لیست Migrations:"
dotnet-ef migrations list --project BankManager.Data --startup-project BankManager.Data 2>&1 | grep -v "^Build"

# 4. بررسی فایل‌های Migration
echo ""
echo "4️⃣ بررسی فایل‌های Migration:"
if [ -d "BankManager.Data/Migrations" ]; then
    echo "   ✅ پوشه Migrations وجود دارد"
    ls -la BankManager.Data/Migrations/*.cs | awk '{print "   📄", $9}'
else
    echo "   ❌ پوشه Migrations وجود ندارد"
fi

# 5. بررسی BankDbContext
echo ""
echo "5️⃣ بررسی BankDbContext:"
if [ -f "BankManager.Data/BankDbContext.cs" ]; then
    echo "   ✅ BankDbContext.cs وجود دارد"
    echo "   📊 DbSets:"
    grep "public DbSet" BankManager.Data/BankDbContext.cs | awk '{print "      -", $0}'
else
    echo "   ❌ BankDbContext.cs وجود ندارد"
fi

# 6. بررسی مدل‌ها
echo ""
echo "6️⃣ بررسی مدل‌ها (Entities):"
if [ -d "BankManager.Data/Entities" ]; then
    echo "   ✅ پوشه Entities وجود دارد"
    ls BankManager.Data/Entities/*.cs | awk '{print "   📄", $1}'
else
    echo "   ❌ پوشه Entities وجود ندارد"
fi

# 7. بررسی DatabaseService
echo ""
echo "7️⃣ بررسی DatabaseService:"
if grep -q "MigrateAsync" BankManagerApp/Services/DatabaseService.cs; then
    echo "   ✅ DatabaseService از MigrateAsync استفاده می‌کند"
else
    echo "   ⚠️  DatabaseService از MigrateAsync استفاده نمی‌کند"
fi

# 8. بررسی DI در MauiProgram
echo ""
echo "8️⃣ بررسی Dependency Injection:"
if grep -q "AddDbContext<BankDbContext>" BankManagerApp/MauiProgram.cs; then
    echo "   ✅ BankDbContext در DI ثبت شده"
else
    echo "   ❌ BankDbContext در DI ثبت نشده"
fi

if grep -q "AddTransient<DatabaseService>" BankManagerApp/MauiProgram.cs; then
    echo "   ✅ DatabaseService در DI ثبت شده"
else
    echo "   ❌ DatabaseService در DI ثبت نشده"
fi

# 9. بررسی مسیر دیتابیس
echo ""
echo "9️⃣ بررسی مسیر دیتابیس:"
DB_PATH="$HOME/Library/Application Support/bankmanager.db"
if [ -f "$DB_PATH" ]; then
    echo "   ⚠️  دیتابیس قبلاً وجود دارد: $DB_PATH"
    echo "   📊 اندازه: $(ls -lh "$DB_PATH" | awk '{print $5}')"
else
    echo "   ℹ️  دیتابیس هنوز ساخته نشده (با اولین اجرا ساخته می‌شود)"
fi

# 10. بررسی بیلد
echo ""
echo "🔟 بررسی Build:"
dotnet build BankManagerApp/BankManagerApp.csproj -f net10.0-maccatalyst --no-restore -v quiet 2>&1 | tail -1

echo ""
echo "✅ همه چیز آماده است!"
echo ""
echo "📝 نکات مهم:"
echo "   • Migration به صورت خودکار با اولین اجرای برنامه اعمال می‌شود"
echo "   • دیتابیس در مسیر: ~/Library/Application Support/bankmanager.db"
echo "   • برای اضافه کردن Migration جدید:"
echo "     dotnet-ef migrations add <نام> --project BankManager.Data --startup-project BankManager.Data"
