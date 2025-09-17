using Azure.Storage.Blobs;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using Azure.Storage.Blobs.Models;

public class GoogleDriveService
{
    private readonly DriveService _service;
    private const string FolderId = "1lUBr6pU903r6wcscwm2XY0Ofb47HWpfw"; // The ID of your Google Drive folder

    public GoogleDriveService(string azureConnectionString, string blobContainerName, string blobName)
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(azureConnectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        if (blobClient == null || !blobClient.Exists())
            throw new InvalidOperationException($"Blob {blobName} does not exist in Container {blobContainerName}.");

        using var ms = new MemoryStream();
        BlobDownloadInfo blobDownloadInfo = blobClient.Download();
        blobDownloadInfo.Content.CopyTo(ms);
        ms.Position = 0;

        GoogleCredential credential = GoogleCredential.FromStream(ms).CreateScoped(DriveService.Scope.Drive);

        _service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "TalkativeParentAPI",
        });
    }


    public string UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("The provided file is null or empty.");
        }

        var fileName = Path.GetFileName(file.FileName);
        var mimeType = GetMimeType(fileName);

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = fileName,
            MimeType = mimeType,
            Parents = new List<string> { FolderId }
        };

        FilesResource.CreateMediaUpload request;
        using (var stream = file.OpenReadStream())
        {
            request = _service.Files.Create(fileMetadata, stream, mimeType);
            request.Fields = "id";
            request.Upload();
        }

        var uploadedFile = request.ResponseBody.Id;
        return uploadedFile.ToString();
    }

    private string GetMimeType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        switch (ext)
        {
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            case ".gif":
                return "image/gif";
            case ".bmp":
                return "image/bmp";
            // ... Add other cases as needed
            case ".pdf":
                return "application/pdf";
            default:
                throw new ArgumentException($"Unsupported file extension: {ext}");
        }
    }
    public Stream GetFile(string fileId)
    {
        if (string.IsNullOrEmpty(fileId))
        {
            throw new ArgumentException("File ID is null or empty.");
        }

        var request = _service.Files.Get(fileId);
        var stream = new MemoryStream();
        request.Download(stream);
        stream.Position = 0; // Reset the stream position to the beginning

        return stream;
    }

}