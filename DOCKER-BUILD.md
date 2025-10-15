# Docker Build & Push Scripts

Scripts để tự động build và push Docker images lên Docker Hub.

## 📋 Prerequisites

1. **Docker đang chạy**
   ```bash
   docker --version
   ```

2. **Đăng nhập Docker Hub**
   ```bash
   docker login
   ```

3. **Dockerfiles tồn tại**
   - `API/Dockerfile`
   - `Client/Dockerfile`

## 🚀 Quick Start

### Option 1: Quick Build (Đơn giản nhất)
```bash
chmod +x quick-build.sh
./quick-build.sh
```

### Option 2: Full Script với Options
```bash
chmod +x docker-build-push.sh
./docker-build-push.sh
```

### Option 3: PowerShell (Windows)
```powershell
.\docker-build-push.ps1
```

## 📝 Usage Examples

### Build với tag mặc định (latest)
```bash
./quick-build.sh
```

### Build với version cụ thể
```bash
./quick-build.sh v1.0.0
```

### Build với date tag
```bash
./quick-build.sh $(date +%Y%m%d)
```

### Full script với help
```bash
./docker-build-push.sh --help
```

## 🐳 Docker Images

Sau khi build thành công, images sẽ có format:

- **API**: `longnguyen1331/qc-quizz-api:VERSION`
- **Client**: `longnguyen1331/qc-quizz-client:VERSION`

## 🔧 Pull Images

```bash
docker pull longnguyen1331/qc-quizz-api:latest
docker pull longnguyen1331/qc-quizz-client:latest
```

## 🛠️ Manual Commands

Nếu muốn chạy thủ công:

```bash
# Build API
docker build --platform linux/amd64 -t longnguyen1331/qc-quizz-api:latest -f API/Dockerfile .

# Build Client
docker build --platform linux/amd64 -t longnguyen1331/qc-quizz-client:latest -f Client/Dockerfile .

# Push API
docker push longnguyen1331/qc-quizz-api:latest

# Push Client
docker push longnguyen1331/qc-quizz-client:latest
```

## ⚠️ Troubleshooting

### Docker không chạy
```bash
# macOS
open -a Docker

# Windows
# Start Docker Desktop

# Linux
sudo systemctl start docker
```

### Chưa đăng nhập Docker Hub
```bash
docker login
# Nhập username: longnguyen1331
# Nhập password: [your password]
```

### Permission denied
```bash
chmod +x *.sh
```

### Build failed
- Kiểm tra Dockerfile có tồn tại không
- Kiểm tra kết nối internet
- Kiểm tra disk space

## 📊 Script Features

### quick-build.sh
- ✅ Simple và nhanh
- ✅ Minimal output
- ✅ Error handling cơ bản

### docker-build-push.sh
- ✅ Full featured
- ✅ Colored output
- ✅ Pre-flight checks
- ✅ Cleanup options
- ✅ Help documentation

### docker-build-push.ps1
- ✅ PowerShell version
- ✅ Windows compatible
- ✅ Same features as bash version

## 🎯 Recommended Usage

- **Development**: `quick-build.sh`
- **Production**: `docker-build-push.sh v1.0.0`
- **Windows**: `docker-build-push.ps1`
