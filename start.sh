#!/bin/bash

echo "🚀 Starting Agility Scoring Monorepo..."
echo ""
echo "This will start:"
echo "  - API server on http://localhost:3000"
echo "  - Web app on http://localhost:3001"
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

# Go back to root
echo "✅ Both servers started!"
echo ""
echo "API: http://localhost:3000"
echo "Web: http://localhost:3001"
echo ""
echo "Press Ctrl+C to stop both servers"

# Wait for both processes
wait $API_PID $WEB_PID
