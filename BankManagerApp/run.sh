#!/bin/bash
echo "🧹 Cleaning up..."
rm -rf bin obj
dotnet clean

echo "� Building..."
dotnet build -f net10.0-maccatalyst

if [ $? -eq 0 ]; then
    echo "🚀 Running..."
    open "bin/Debug/net10.0-maccatalyst/maccatalyst-arm64/BankManagerApp.app"
else
    echo "❌ Build failed."
    exit 1
fi
