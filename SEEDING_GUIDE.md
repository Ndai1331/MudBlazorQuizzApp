# Data Seeding Guide - QuizApp

## 📋 Tổng quan

Hệ thống QuizApp có tính năng tự động seed dữ liệu ban đầu bao gồm:
- **Tài khoản Admin mặc định** để quản lý hệ thống
- **Câu hỏi mẫu** để test functionality
- **Tài khoản test** (chỉ trong môi trường development)

## 🔐 Tài khoản mặc định

### Admin Account
```
Email: admin@quizapp.com
Password: Abc@123!!!
Role: Admin
```

### Test Accounts (Development only)
```
Email: user1@test.com
Password: Test@123!
Role: User

Email: user2@test.com  
Password: Test@123!
Role: User

Email: teacher@test.com
Password: Teacher@123!
Role: Teacher
```

## 🚀 Cách chạy seeding

### Phương pháp 1: Tự động (Khuyến nghị)
Khi chạy ứng dụng lần đầu, seeding sẽ tự động thực hiện:

```bash
cd QuizAppBlazor/API
dotnet run
```

### Phương pháp 2: Sử dụng script
```bash
# Linux/macOS
./seed-data.sh

# Windows PowerShell
./seed-data.ps1
```

### Phương pháp 3: Manual seeding
```bash
cd QuizAppBlazor/API
dotnet ef database update
dotnet run
```

## 📊 Dữ liệu được seed

### 1. Admin User
- **Email**: admin@quizapp.com
- **Password**: Abc@123!!! (đáp ứng policy bảo mật mới)
- **Role**: Admin
- **Permissions**: Full access to all features

### 2. Sample Questions (15 câu)
- **Câu hỏi trắc nghiệm**: Về lập trình, công nghệ, Việt Nam
- **Câu hỏi tự luận**: Mô tả về ASP.NET Core
- **Time limits**: 15-120 giây tùy độ khó
- **Topics**: C#, Blazor, Database, Web Development, General Knowledge

### 3. Test Users (Development)
- **user1@test.com**: Regular user for testing
- **user2@test.com**: Another regular user for testing  
- **teacher@test.com**: Teacher role for testing

## 🔧 Cấu hình

### Environment Variables
Đảm bảo các environment variables được cấu hình đúng:

```bash
# Database connection
DATABASE_CONNECTION_STRING="Host=your_host;Database=your_db;Username=your_user;Password=your_pass"

# JWT settings
JWT_SECRET_KEY="Your_Secret_Key_Here"
JWT_ISSUER="http://localhost:5000"
JWT_AUDIENCE="QuizAppUsers"
```

### Database Requirements
- **PostgreSQL** database accessible
- **Connection string** configured correctly
- **Network access** to database server

## 🔍 Verification

### 1. Check Seeding Status
Logs sẽ hiển thị:
```
info: QuizAppBlazor.API.Services.SeedDataService[0]
      Starting data seeding...
info: QuizAppBlazor.API.Services.SeedDataService[0]
      Admin user created successfully: admin@quizapp.com
info: QuizAppBlazor.API.Services.SeedDataService[0]
      Seeded 15 sample questions
```

### 2. Test Login
1. Mở browser và truy cập ứng dụng
2. Navigate to `/login`
3. Sử dụng credentials admin:
   - Email: `admin@quizapp.com`
   - Password: `Abc@123!!!`
4. Verify admin features accessible

### 3. Test Questions
1. Login as admin
2. Navigate to `/questions-v2`
3. Verify 15 sample questions loaded
4. Test creating new questions
5. Test quiz functionality

## 🚨 Troubleshooting

### Common Issues

#### 1. Database Connection Error
```
Error: Couldn't set ${database_connection_string...
```
**Solution**: Check environment variables hoặc update connection string trong appsettings.json

#### 2. Password Policy Error
```
Error: Password does not meet requirements
```
**Solution**: Password `Abc@123!!!` đáp ứng tất cả requirements:
- 8+ characters ✓
- Uppercase letter ✓  
- Lowercase letter ✓
- Digit ✓
- Special character ✓

#### 3. Admin User Already Exists
```
Info: Admin user already exists: admin@quizapp.com
```
**Solution**: Đây là normal behavior, không cần action

#### 4. Questions Already Exist
```
Info: Questions already exist, skipping sample questions seeding
```
**Solution**: Đây là normal behavior, không cần action

### Manual Database Reset
Nếu cần reset toàn bộ database:

```bash
cd QuizAppBlazor/API
dotnet ef database drop --force
dotnet ef database update
dotnet run
```

**⚠️ Warning**: Lệnh này sẽ xóa toàn bộ dữ liệu!

## 📝 Customization

### Thay đổi Admin Credentials
Edit file `Services/SeedDataService.cs`:

```csharp
private async Task SeedAdminUserAsync()
{
    const string adminEmail = "your-admin@email.com";     // ← Change here
    const string adminPassword = "YourSecurePassword!";   // ← Change here
    const string adminNickname = "YourAdminName";         // ← Change here
    
    // ... rest of the method
}
```

### Thêm Sample Questions
Edit method `SeedSampleQuestionsAsync()` trong `SeedDataService.cs`:

```csharp
var sampleQuestions = new List<QuestionModel>
{
    // Add your custom questions here
    new QuestionModel
    {
        Question = "Your custom question?",
        CorrectAnswer = "Correct answer",
        Alternativ2 = "Wrong answer 1",
        Alternativ3 = "Wrong answer 2",
        Alternativ4 = "Wrong answer 3",
        IsTextInput = false,
        HasTimeLimit = true,
        TimeLimit = 60
    },
    // ... existing questions
};
```

## 🔄 Re-seeding

Để seed lại dữ liệu:

1. **Xóa users và questions hiện tại** (nếu cần)
2. **Chạy lại seeding**:
   ```bash
   dotnet run
   ```

Hoặc sử dụng script:
```bash
./seed-data.sh
```

## 📊 Production Considerations

### Security
- **Change default passwords** trước khi deploy production
- **Remove test users** trong production environment
- **Use strong JWT secrets** và environment variables

### Performance
- **Disable automatic seeding** trong production
- **Use migration scripts** thay vì runtime seeding
- **Monitor seeding performance** với large datasets

### Monitoring
- **Check seeding logs** để đảm bảo success
- **Verify admin access** sau seeding
- **Test core functionality** với seeded data

---

## 🎯 Quick Start

Để bắt đầu nhanh:

```bash
# 1. Clone và navigate to project
cd QuizAppBlazor

# 2. Update database
cd API
dotnet ef database update

# 3. Run application (auto-seeding will occur)
dotnet run

# 4. Open browser và login với admin credentials
# Email: admin@quizapp.com
# Password: Abc@123!!!
```

**🎉 Enjoy your QuizApp with pre-loaded data!**
