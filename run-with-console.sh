#!/bin/bash
# Run app and capture console output

echo "🚀 Starting BankManagerApp with console output..."
echo ""

# Kill any existing instance
pkill -f "BankManagerApp" 2>/dev/null

# Run the app in background and capture output
BankManagerApp/bin/Debug/net10.0-maccatalyst/maccatalyst-arm64/BankManagerApp.app/Contents/MacOS/BankManagerApp 2>&1 &

APP_PID=$!
echo "App started with PID: $APP_PID"
echo "Waiting for output..."
echo ""

# Wait a bit for app to start
sleep 5

# Check if still running
if ps -p $APP_PID > /dev/null; then
    echo "✅ App is running"
else
    echo "❌ App crashed or exited"
fi
