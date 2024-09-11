using BlobWithTables;

var connectionString = "";
var tableName = "users";
var blobName = "imagesforusers";

var userManager = new UserManager(connectionString, tableName, blobName);

using (var stream = File.OpenRead("C:\\Users\\den95\\OneDrive\\Рабочий стол\\скрины\\2.png"))
{
    var user = new User
    {
        FirstName = "Doe",
        LastName = "Doe"
    };

    byte[] pictureData = new byte[stream.Length];
    stream.Read(pictureData, 0, pictureData.Length);

    await userManager.AddUser(user, "doe_john.png", pictureData);
}

var users = await userManager.GetAllUsers();
await userManager.DeleteUser(nameof(User), "f2ab6c45-f9cc-41d7-a29d-c116c2caaba3");
foreach (var user in users)
{
    Console.WriteLine($"{user.FirstName} {user.LastName} {user.PictureUrl}");
}
