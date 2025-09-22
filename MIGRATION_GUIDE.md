# Migration Guide - QuizAppBlazor Database & Entity Changes

## 📋 Tổng quan thay đổi

Dự án đã được cải thiện với cấu trúc database và entity mới, cùng với nhiều tính năng bảo mật và performance được nâng cấp.

## 🗄️ Thay đổi Database

### Cấu trúc cũ:
- `QuestionModel`: Chứa tất cả thông tin câu hỏi trong 1 table
- `ScoreModel`: Lưu điểm số dạng string concatenated

### Cấu trúc mới:
- `Question`: Thông tin câu hỏi cơ bản
- `QuestionOption`: Các lựa chọn trả lời
- `QuestionMedia`: Media content (hình ảnh, video)
- `Quiz`: Quiz sessions với tracking chi tiết
- `QuizQuestionAnswer`: Chi tiết từng câu trả lời

## 🔄 Migration Process

### 1. Database Migration
```bash
cd QuizAppBlazor/API
dotnet ef migrations add InitialCreate --context ApplicationDbContext
dotnet ef database update --context ApplicationDbContext
```

### 2. Data Migration (Tùy chọn)
Sử dụng `DataMigrationService` để migrate data từ cấu trúc cũ:

```csharp
// Trong Program.cs hoặc startup
var migrationService = serviceProvider.GetRequiredService<DataMigrationService>();
if (await migrationService.IsMigrationNeededAsync())
{
    await migrationService.RunFullMigrationAsync();
}
```

## 🎨 Thay đổi Blazor Client

### DTOs mới:
- `QuizDTO`: Quản lý quiz sessions
- `QuestionV2DTO`: Cấu trúc câu hỏi mới
- `QuestionOptionDTO`: Lựa chọn trả lời
- `QuestionMediaDTO`: Media content

### Services mới:
- `IApiService` & `ApiService`: HTTP client với error handling tốt hơn
- Validation được tăng cường với DataAnnotations

### Components mới:
- `CreateQuestionV2.razor`: Form tạo câu hỏi với validation

## ⚙️ Configuration Changes

### Environment Variables
Sử dụng environment variables cho sensitive data:

```bash
# Ví dụ .env file
DATABASE_CONNECTION_STRING="Host=localhost:5432;Database=quizapp;Username=postgres;Password=yourpassword"
JWT_SECRET_KEY="YourSecretKey"
JWT_ISSUER="http://localhost:5000"
JWT_AUDIENCE="QuizAppUsers"
REDIS_CONNECTION_STRING="localhost:6379"
```

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${DATABASE_CONNECTION_STRING:Host=localhost:5432;Database=quizapp;Username=postgres;Password=defaultpassword}",
    "Redis": "${REDIS_CONNECTION_STRING:localhost:6379}"
  }
}
```

## 🔒 Security Improvements

### 1. Password Policy
- Minimum 8 characters
- Requires uppercase, lowercase, digit, special character
- Account lockout after 5 failed attempts

### 2. CORS Policy
- Restricted to specific origins
- No more wildcard permissions

### 3. JWT Configuration
- Environment-based secrets
- Proper token expiration

## 🚀 Performance Improvements

### 1. Caching
- Memory cache cho frequent data
- Distributed cache (Redis) cho scalability
- Smart cache invalidation

### 2. Service Layer
- Business logic tách biệt khỏi controllers
- Dependency injection
- Better error handling

### 3. Database Optimization
- Proper indexes
- Foreign key relationships
- Query optimization

## 📊 Logging & Monitoring

### 1. Structured Logging
- JSON-based logging
- Different log levels per component
- File rotation

### 2. Audit Logging
- User activity tracking
- Security event monitoring
- API request/response logging

### 3. Error Handling
- Global exception middleware
- Proper error responses
- Client-side error handling

## 🔧 Development Workflow

### 1. Chạy ứng dụng lần đầu:
```bash
# API
cd QuizAppBlazor/API
dotnet ef database update
dotnet run

# Client (terminal mới)
cd QuizAppBlazor/Client
dotnet run
```

### 2. Environment Setup:
- Tạo file `.env` với các environment variables
- Hoặc set trực tiếp trong system environment

### 3. Testing:
- API documentation: `https://localhost:8080/swagger`
- Client: `https://localhost:7000`

## 🚨 Breaking Changes

### API Endpoints:
- Responses giờ có cấu trúc consistent với `ResponseBaseHttp<T>`
- Error messages được chuẩn hóa
- Validation errors trả về dạng array

### Client:
- DTOs có validation attributes
- HTTP calls thông qua `IApiService`
- Improved error handling với Snackbar notifications

## 📝 Migration Checklist

- [ ] Database migration completed
- [ ] Data migration (if needed)
- [ ] Environment variables configured
- [ ] Client DTOs updated
- [ ] API endpoints tested
- [ ] Authentication flow verified
- [ ] Caching working properly
- [ ] Logging configured
- [ ] Error handling tested

## 🔮 Future Enhancements

### Planned Features:
1. Real-time quiz với SignalR
2. Question categories và difficulty levels
3. Advanced analytics và reporting
4. Mobile app với shared DTOs
5. Microservices architecture

### Performance Optimizations:
1. Database sharding
2. CDN cho media content
3. Advanced caching strategies
4. Load balancing

---

Để biết thêm chi tiết, xem source code hoặc liên hệ team phát triển.
