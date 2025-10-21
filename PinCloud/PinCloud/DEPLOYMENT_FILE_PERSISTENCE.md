# File Upload Deployment Guide

## 🚨 **File Persistence Solutions**

Your file upload system now supports persistent file storage that survives deployments and publishes.

## **Current Implementation**

### **Default Behavior:**
- **Development**: Files stored in `/tmp/apini_dev_uploads`
- **Production**: Files stored in `/var/apini/uploads` (configurable)

### **Configuration:**
Update `appsettings.json` or `appsettings.Production.json`:
```json
{
  "FileUpload": {
    "BasePath": "/path/to/persistent/storage",
    "MaxFileSize": 52428800,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".mp3", ".wav", ".flac", ".aac", ".ogg", ".pdf", ".txt", ".doc", ".docx"]
  }
}
```

## **Deployment Options**

### **Option 1: External Directory (Recommended)**
```bash
# Create persistent directory on server
sudo mkdir -p /var/apini/uploads
sudo chown your-app-user:your-app-group /var/apini/uploads
sudo chmod 755 /var/apini/uploads

# Update appsettings.Production.json
{
  "FileUpload": {
    "BasePath": "/var/apini/uploads"
  }
}
```

### **Option 2: Docker Volume**
```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY . .
VOLUME ["/app/data/uploads"]
EXPOSE 80
ENTRYPOINT ["dotnet", "APinI.dll"]
```

```yaml
# docker-compose.yml
version: '3.8'
services:
  apini:
    build: .
    ports:
      - "80:80"
    volumes:
      - ./uploads:/app/data/uploads
    environment:
      - FileUpload__BasePath=/app/data/uploads
```

### **Option 3: Cloud Storage (Azure Blob, AWS S3)**
For production scale, consider cloud storage:
```json
{
  "FileUpload": {
    "Provider": "AzureBlob",
    "ConnectionString": "your-connection-string",
    "ContainerName": "uploads"
  }
}
```

## **Publishing Process**

### **Safe Deployment Steps:**
1. **Backup existing uploads** (if any)
   ```bash
   sudo cp -r /var/apini/uploads /var/apini/uploads_backup_$(date +%Y%m%d)
   ```

2. **Publish the application**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

3. **Deploy to server**
   ```bash
   # Copy published files (app code only)
   sudo cp -r ./publish/* /var/www/apini/
   
   # Uploads directory remains untouched at /var/apini/uploads
   ```

4. **Restart the service**
   ```bash
   sudo systemctl restart apini
   ```

## **Environment Variables**

You can also use environment variables:
```bash
export FileUpload__BasePath="/var/apini/uploads"
export FileUpload__MaxFileSize="52428800"
```

## **Backup Strategy**

### **Automated Backup Script:**
```bash
#!/bin/bash
# backup-uploads.sh
UPLOAD_DIR="/var/apini/uploads"
BACKUP_DIR="/var/backups/apini"
DATE=$(date +%Y%m%d_%H%M%S)

mkdir -p $BACKUP_DIR
tar -czf "$BACKUP_DIR/uploads_$DATE.tar.gz" -C "$UPLOAD_DIR" .

# Keep only last 7 days of backups
find $BACKUP_DIR -name "uploads_*.tar.gz" -mtime +7 -delete
```

### **Cron Job:**
```bash
# Run daily at 2 AM
0 2 * * * /path/to/backup-uploads.sh
```

## **Testing File Persistence**

1. **Upload some test files**
2. **Publish and redeploy the application**
3. **Verify files are still accessible**

## **Security Considerations**

- **File permissions**: Ensure proper read/write permissions
- **Disk space monitoring**: Monitor upload directory size
- **Path validation**: Current implementation prevents directory traversal
- **File type validation**: Only allowed extensions are accepted

## **Monitoring**

Add logging to track file operations:
```csharp
_logger.LogInformation("File uploaded: {FileName} to {Path}", fileName, relativePath);
_logger.LogWarning("Upload directory space: {FreeSpace}GB", GetFreeSpace());
```

Your uploaded files will now persist through deployments! 🎉
