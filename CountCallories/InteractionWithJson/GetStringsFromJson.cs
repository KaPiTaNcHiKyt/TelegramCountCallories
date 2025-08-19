using System.Text.Json;

namespace CountCallories.InteractionWithJson;

public static class GetStringsFromJson
{
    public static ConnectionStrings GetStrings()
    {
        string jsonString =
            File.ReadAllText(@"/Users/stanislav/RiderProjects/CountCallories/CountCallories/appsettings.json");
        var connectionStrings = JsonSerializer.Deserialize<ConnectionStringsFromJson>(jsonString);
        return connectionStrings.ConnectionStrings;
    }
}