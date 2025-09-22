# QuizApp Data Seeding Script for Windows
Write-Host "🌱 QuizApp Data Seeding Script" -ForegroundColor Green
Write-Host "==============================" -ForegroundColor Green

# Navigate to API directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$scriptPath/QuizAppBlazor/API"

# Check if database exists and is accessible
Write-Host "📊 Checking database connection..." -ForegroundColor Yellow
try {
    dotnet ef database update --context ApplicationDbContext
    Write-Host "✅ Database connection successful" -ForegroundColor Green
}
catch {
    Write-Host "❌ Database connection failed. Please check your connection string." -ForegroundColor Red
    exit 1
}

# Display credentials information
Write-Host ""
Write-Host "📋 Default Admin Credentials:" -ForegroundColor Cyan
Write-Host "Email: admin@quizapp.com" -ForegroundColor White
Write-Host "Password: Abc@123!!!" -ForegroundColor White
Write-Host "Role: Admin" -ForegroundColor White
Write-Host ""
Write-Host "🧪 Test User Credentials (Development only):" -ForegroundColor Cyan
Write-Host "Email: user1@test.com | Password: Test@123! | Role: User" -ForegroundColor White
Write-Host "Email: user2@test.com | Password: Test@123! | Role: User" -ForegroundColor White
Write-Host "Email: teacher@test.com | Password: Teacher@123! | Role: Teacher" -ForegroundColor White
Write-Host ""
Write-Host "🌱 Starting data seeding..." -ForegroundColor Yellow
Write-Host "The application will start and automatically seed data if needed." -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop the application after seeding is complete." -ForegroundColor Yellow
Write-Host ""

# Start the application (seeding will happen automatically)
try {
    dotnet run
    Write-Host "✅ Seeding completed!" -ForegroundColor Green
}
catch {
    Write-Host "❌ Error during application startup: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
