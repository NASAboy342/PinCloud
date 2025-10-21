# File Upload and Retrieval API

This project now includes comprehensive file upload and retrieval functionality that allows you to:

## Features

### 1. Upload Files
- **Endpoint**: `POST /api/file/upload`
- **Supported file types**: Images (jpg, jpeg, png, gif, bmp, webp), Audio (mp3, wav, flac, aac, ogg), Documents (pdf, txt, doc, docx)
- **File size limit**: 50MB
- **Optional subfolder organization**: You can organize files into subfolders

### 2. Retrieve Files
- **Direct access**: `GET /files/{filePath}` or `GET /{fileType}/{fileName}`
- **Examples**: 
  - `http://localhost:5000/files/image.png`
  - `http://localhost:5000/png/image.png`
  - `http://localhost:5000/files/audio/song.mp3`
  - `http://localhost:5000/mp3/song.mp3`

### 3. List Files
- **Endpoint**: `GET /api/file/list`
- **With subfolder**: `GET /api/file/list?subfolder=images`

### 4. Delete Files
- **Endpoint**: `DELETE /api/file/{filePath}`

## Usage Examples

### Upload a file with cURL:
```bash
# Upload to root directory
curl -X POST -F "file=@/path/to/your/image.png" http://localhost:5000/api/file/upload

# Upload to subfolder
curl -X POST -F "file=@/path/to/your/image.png" -F "subfolder=images" http://localhost:5000/api/file/upload
```

### Access uploaded files:
- Direct file access: `http://localhost:5000/files/filename.png`
- With subfolder: `http://localhost:5000/files/images/filename.png`
- Type-based routing: `http://localhost:5000/png/filename.png`

## Test Interface

You can test the upload functionality using the built-in test page:
- **URL**: `http://localhost:5000/upload-test.html`
- This page provides a user-friendly interface to upload, list, view, and delete files

## API Response Examples

### Upload Response:
```json
{
  "message": "File uploaded successfully",
  "fileName": "abc123-guid.png",
  "originalFileName": "my-image.png",
  "size": 12345,
  "path": "images/abc123-guid.png",
  "accessUrl": "http://localhost:5000/files/images/abc123-guid.png"
}
```

### List Files Response:
```json
[
  {
    "fileName": "abc123-guid.png",
    "size": 12345,
    "lastModified": "2025-10-03T10:30:00",
    "path": "images/abc123-guid.png",
    "accessUrl": "http://localhost:5000/files/images/abc123-guid.png"
  }
]
```

## Security Features

- File type validation (only allowed extensions)
- File size limits (50MB max)
- Directory traversal protection
- Unique filename generation to prevent conflicts
- Subfolder name sanitization

## File Storage

- Files are stored in the `uploads/` directory within the project root
- Directory structure is automatically created as needed
- Files are organized by optional subfolders (e.g., `uploads/images/`, `uploads/audio/`)

## Getting Started

1. Run the application: `dotnet run`
2. Open browser to: `http://localhost:5000/upload-test.html`
3. Upload some files and test the functionality
4. Access files directly via URLs like: `http://localhost:5000/png/your-image.png`
