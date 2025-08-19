namespace CountCallories;

public class ConnectionStringsFromJson
{
    public ConnectionStrings ConnectionStrings { get; set; }
}

public class ConnectionStrings
{
    public string DefaultConnection { get; set; }
    public string BotKeyConnection { get; set; }
    public string KeySiteConnection { get; set; }
}

