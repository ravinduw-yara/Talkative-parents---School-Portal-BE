using Azure.Storage.Blobs;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;

public class MUploadPdfSyllabusGoogleDriveService
{
    private readonly DriveService _service;
    //private const string FolderId = "1RMG_60SiDOyIrfzra1HaK8hQCUr_Nmt0"; // The ID of your Google Drive folder
     private const string FolderId = "1dJfxgNpqU-CK8T1h5Z1o6Cl7s_U3bSfU"; // The ID of your Google Drive folder

    public MUploadPdfSyllabusGoogleDriveService(string azureConnectionString, string blobContainerName, string blobName)
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

    public string UploadFile(IFormFile file, int academicYearId, int gradeId, int subjectId, int semesterId, int examId)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("The provided file is null or empty.");
        }

        // Create or get existing nested folder structure
        string folderId = GetOrCreateNestedFolderStructure(academicYearId, gradeId, subjectId, semesterId, examId);

        if (FindFileInFolder(file.FileName, folderId) != null)
        {
            throw new ArgumentException("A Question Paper With The Same Name Already Exists in the Specified Location.");
        }

        var fileName = Path.GetFileName(file.FileName);
        var mimeType = GetMimeType(fileName);

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = fileName,
            MimeType = mimeType,
            Parents = new List<string> { folderId }
        };

        FilesResource.CreateMediaUpload request;
        using (var stream = file.OpenReadStream())
        {
            request = _service.Files.Create(fileMetadata, stream, mimeType);
            request.Fields = "id";
            request.Upload();
        }

        var uploadedFile = request.ResponseBody;
        return uploadedFile?.Id;
    }

    private string FindFileInFolder(string fileName, string folderId)
    {
        // Search for a file with the given name in the specified folder
        var searchQuery = $"name='{fileName}' and '{folderId}' in parents and trashed=false";
        var searchRequest = _service.Files.List();
        searchRequest.Q = searchQuery;
        searchRequest.Fields = "files(id, name)";
        var searchResponse = searchRequest.Execute();
        if (searchResponse.Files != null && searchResponse.Files.Count > 0)
        {
            return searchResponse.Files[0].Id;
        }
        return null;
    }

    private string GetOrCreateNestedFolderStructure(int academicYearId, int gradeId, int subjectId, int semesterId, int examId)
    {
        // Start with the root folder ID and sequentially create or get each subfolder
        string currentFolderId = FolderId;
        currentFolderId = GetOrCreateSubFolder(currentFolderId, academicYearId.ToString());
        currentFolderId = GetOrCreateSubFolder(currentFolderId, gradeId.ToString());
        currentFolderId = GetOrCreateSubFolder(currentFolderId, subjectId.ToString());
        currentFolderId = GetOrCreateSubFolder(currentFolderId, semesterId.ToString());
        currentFolderId = GetOrCreateSubFolder(currentFolderId, examId.ToString());
        return currentFolderId;
    }

    private string GetOrCreateSubFolder(string parentFolderId, string folderName)
    {
        var searchQuery = $"mimeType='application/vnd.google-apps.folder' and name='{folderName}' and '{parentFolderId}' in parents and trashed=false";
        var searchRequest = _service.Files.List();
        searchRequest.Q = searchQuery;
        searchRequest.Spaces = "drive";
        searchRequest.Fields = "files(id, name)";
        var searchResponse = searchRequest.Execute();
        var files = searchResponse.Files;
        if (files != null && files.Count > 0)
        {
            return files[0].Id;
        }
        else
        {
            // Create folder
            var folderMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { parentFolderId }
            };

            var createRequest = _service.Files.Create(folderMetadata);
            createRequest.Fields = "id";
            var folder = createRequest.Execute();
            return folder.Id;
        }
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



    //public async Task<string> GetPdfFileAsBase64(int academicYearId, int gradeId, int subjectId, int semesterId, int examId, string pdfFileName)
    //{
    //    try
    //    {
    //        string folderId = GetOrCreateNestedFolderStructure(academicYearId, gradeId, subjectId, semesterId, examId);
    //        // Log folderId for debugging
    //        Console.WriteLine($"Folder ID: {folderId}");

    //        var fileId = FindFileInFolder(pdfFileName, folderId);
    //        // Log fileId for debugging
    //        Console.WriteLine($"File ID: {fileId}");

    //        if (string.IsNullOrEmpty(fileId))
    //        {
    //            throw new FileNotFoundException($"PDF file '{pdfFileName}' not found in the specified folder.");
    //        }

    //        return await GetFileAsBase64(fileId);
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log the exception
    //        Console.WriteLine($"Error in GetPdfFileAsBase64: {ex.Message}");
    //        return null;
    //    }
    //}

    //public async Task<string> GetPdfFileAsBase64(int academicYearId, int gradeId, int subjectId, int semesterId, int examId, string pdfFileName)
    //{
    //    try
    //    {
    //        string folderId = GetOrCreateNestedFolderStructure(academicYearId, gradeId, subjectId, semesterId, examId);


    //        Console.WriteLine($"Folder ID for searching: {folderId}");

    //        var fileId = FindFileInFolder(pdfFileName, folderId);


    //        Console.WriteLine($"File ID found: {fileId}");

    //        if (string.IsNullOrEmpty(fileId))
    //        {
    //            Console.WriteLine($"File '{pdfFileName}' not found in folder ID: {folderId}");
    //            throw new FileNotFoundException($"PDF file '{pdfFileName}' not found in the specified folder.");
    //        }

    //        return await GetFileAsBase64(fileId);
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error in GetPdfFileAsBase64: {ex.Message}");
    //        throw; 
    //    }
    //}


    public async Task<string> GetFileAsBase64(string fileId)
    {
        if (string.IsNullOrEmpty(fileId))
        {
            throw new ArgumentException("File ID is null or empty.");
        }

        var request = _service.Files.Get(fileId);
        var stream = new MemoryStream();
        await request.DownloadAsync(stream);
        stream.Position = 0;

        return Convert.ToBase64String(stream.ToArray());
    }



}


