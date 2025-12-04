#!/bin/bash
# Kill existing app
pkill -f "BankManagerApp" 2>/dev/null

# Run app and capture output
echo "Starting app with console output..."
BankManagerApp/bin/Debug/net10.0-maccatalyst/maccatalyst-arm64/BankManagerApp.app/Contents/MacOS/BankManagerApp 2>&1 &

APP_PID=$!
echo "App PID: $APP_PID"
echo "Monitoring logs..."
echo ""

# Monitor logs for 60 seconds
sleep 60
