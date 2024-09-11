using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlobWithTables
{
    public class UserManager
    {
        private readonly TableClient _tableClient;
        private readonly BlobService _blobService;

        public UserManager(string connString, string tableName, string blobname)
        {
            _tableClient = new TableClient(connString, tableName);
            _tableClient.CreateIfNotExists();
            _blobService = new BlobService(connString, blobname);
        }

        public async Task AddUser(User user, string blobName, IEnumerable<byte> pictureData)
        {
            await _blobService.AddBlob(blobName, pictureData);
            user.PictureUrl = _blobService.GetBlobUrl(blobName);
            user.PictureName = blobName;    
            await _tableClient.AddEntityAsync(user);
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = new List<User>();

            await foreach (User user in _tableClient.QueryAsync<User>())
            {
                users.Add(user);
            }

            return users;
        }

        public async Task DeleteUser(string partitionKey, string rowKey)
        {
            var user = await _tableClient.GetEntityAsync<User>(partitionKey, rowKey);

            if (user != null)
            {
                await _blobService.DeleteBlob(user.Value.PictureName);

                await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
            }
        }
    }
}
