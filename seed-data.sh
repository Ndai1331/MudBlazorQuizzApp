#!/bin/bash

# QuizApp Data Seeding Script
echo "🌱 QuizApp Data Seeding Script"
echo "=============================="

# Navigate to API directory
cd "$(dirname "$0")/QuizAppBlazor/API"

# Check if database exists and is accessible
echo "📊 Checking database connection..."
if ! dotnet ef database update --context ApplicationDbContext; then
    echo "❌ Database connection failed. Please check your connection string."
    exit 1
fi

echo "✅ Database connection successful"

# Run the application to trigger seeding
echo "🌱 Starting data seeding..."
echo "The application will start and automatically seed data if needed."
echo ""
echo "📋 Default Admin Credentials:"
echo "Email: admin@quizapp.com"
echo "Password: Abc@123!!!"
echo "Role: Admin"
echo ""
echo "🧪 Test User Credentials (Development only):"
echo "Email: user1@test.com | Password: Test@123! | Role: User"
echo "Email: user2@test.com | Password: Test@123! | Role: User"
echo "Email: teacher@test.com | Password: Teacher@123! | Role: Teacher"
echo ""
echo "Press Ctrl+C to stop the application after seeding is complete."
echo ""

# Start the application (seeding will happen automatically)
dotnet run

echo "✅ Seeding completed!"
