using Microsoft.AspNetCore.StaticFiles;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace API.Services;

public class FileService
{
    private readonly IMinioClient _minioClient;
    private const string Endpoint = "localhost:9000";
    private const string AccessKey = "admin";
    private const string SecretKey = "password123";

    public FileService()
    {
        _minioClient = new MinioClient()
            .WithEndpoint(Endpoint)
            .WithCredentials(AccessKey, SecretKey)
            .Build();
    }

// FileService.cs içindeki UploadFileAsync metodu

    public async Task UploadFileAsync(string filePath, string bucketName = "resimler")
    {
        try
        {
            string objectName = Path.GetFileName(filePath);

            // --- BU KISMI EKLE/GÜNCELLE ---
            // Uzantıya göre Content-Type belirle (Örn: .jpg -> image/jpeg)
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream"; // Bulamazsa varsayılan kalsın
            }
            // ------------------------------

            var found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName))
                .ConfigureAwait(false);
            if (!found)
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName)).ConfigureAwait(false);

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(filePath)
                    .WithContentType(contentType))
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            throw new Exception($"Hata: {e.Message}");
        }
    }

    public async Task<MemoryStream> GetFileAsync(string objectName, string bucketName = "resimler")
    {
        try
        {
            var memoryStream = new MemoryStream();
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(s => s.CopyTo(memoryStream))).ConfigureAwait(false);
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (MinioException e)
        {
            throw new Exception($"Dosya okuma hatası: {e.Message}");
        }
    }

    public async Task<string> GetPresignedUrlAsync(string objectName, string bucketName = "resimler")
    {
        try
        {
            // Headers oluşturarak "Content-Disposition: inline" ekliyoruz
            var reqParams = new Dictionary<string, string>
            {
                { "response-content-disposition", "inline" }
            };

            var args = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(3600)
                .WithHeaders(reqParams); 

            return await _minioClient.PresignedGetObjectAsync(args).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            throw new Exception($"Hata: {e.Message}");
        }
    }
}