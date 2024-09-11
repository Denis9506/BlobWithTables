using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BlobWithTables
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        private readonly string _blobContainerName = "imagesforusers";

        public BlobService(string connectionString, string blobName)
        {
            _blobContainerName = blobName;

            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task AddBlob(string blobName, IEnumerable<byte> data)
        {
            string uniqueBlobName = await GetUniqueBlobName(blobName);
            BlobClient blobClient = _containerClient.GetBlobClient(uniqueBlobName);

            using (var stream = new MemoryStream(data.ToArray(), writable: false))
            {
                await blobClient.UploadAsync(stream);
            }
        }

        public async Task<IEnumerable<byte>> GetBlob(string blobName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                BlobDownloadInfo download = await blobClient.DownloadAsync();
                using (MemoryStream ms = new MemoryStream())
                {
                    await download.Content.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
            else
            {
                throw new FileNotFoundException($"Blob '{blobName}' not found.");
            }
        }

        public async Task DeleteBlob(string blobName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }

        public string GetBlobUrl(string blobName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);
            return blobClient.Uri.ToString();
        }

        private async Task<string> GetUniqueBlobName(string blobName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);
            int copyNumber = 1;
            string baseName = Path.GetFileNameWithoutExtension(blobName);
            string extension = Path.GetExtension(blobName);
            string uniqueBlobName = blobName;

            while (await blobClient.ExistsAsync())
            {
                uniqueBlobName = $"{baseName}({copyNumber}){extension}";
                blobClient = _containerClient.GetBlobClient(uniqueBlobName);
                copyNumber++;
            }

            return uniqueBlobName;
        }
    }
}
