using Azure;
using Azure.Data.Tables;

public class User : ITableEntity
{
    public User()
    {
        PartitionKey = nameof(User);
        RowKey = Guid.NewGuid().ToString();
        Timestamp = DateTime.UtcNow;
        ETag = new ETag();
    }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PictureName { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = string.Empty;
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
