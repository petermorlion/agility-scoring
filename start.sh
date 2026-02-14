#!/bin/bash

echo "🚀 Starting Agility Scoring Monorepo..."
echo ""
echo "This will start:"
echo "  - API server on http://localhost:3000"
echo "  - Web app on http://localhost:3001"
echo "  - Mobile app on http://localhost:3002"
echo ""

# Start API in background
echo "🔄 Starting API server..."
cd apps/api
npm run dev &
API_PID=$!

# Give API a moment to start
sleep 5

# Start Web app in background
echo "🌐 Starting Web app..."
cd ../web
npm run dev &
WEB_PID=$!

# Start Mobile app in background
echo "📱 Starting Mobile app..."
cd ../mobile
npm run start &
MOBILE_PID=$!

# Go back to root
echo "✅ All servers started!"
echo ""
echo "API: http://localhost:3000"
echo "Web: http://localhost:3001"
echo "Mobile: http://localhost:3002"
echo ""
echo "Press Ctrl+C to stop all servers"

# Wait for all processes
wait $API_PID $WEB_PID $MOBILE_PID
