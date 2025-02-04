using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace zappsTimeSpent;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("Database Settings")]
    public DatabaseSettings DatabaseSettings { get; set; } = new DatabaseSettings();

    [JsonPropertyName("Command Settings")]
    public CommandSettings CommandSettings { get; set; } = new CommandSettings();

    [JsonPropertyName("Server IP address")]
    public string ServerIP { get; set; } = "127.0.0.1";

    [JsonPropertyName("Server Name")]
    public string ServerName { get; set; } = "My server";

    [JsonPropertyName("Time interval")]
    public int TimeInterval { get; set; } = 60;

    [JsonPropertyName("Force save Data Interval")]
    public float SaveInterval { get; set; } = 600.0f;

    [JsonPropertyName("CheckTime Command Response")]
    public List<string> CheckTimePrint { get; set; } = new List<string>
    {
        "{PREFIX} {DESCRIPTION}",
        "{gold}» Total time: {TOTAL_TIME}",
        "{gold}» Terrorist time: {T_TIME}",
        "{gold}» Counter-Terrorist time: {CT_TIME}",
        "{gold}» IN-TEAM TIME (CT/T): {TEAM_TIME}",
        "{gold}» Spectator time: {SPEC_TIME}",
    };
}

public class DatabaseSettings
{
    [JsonPropertyName("Hostname")]
    public string Host { get; set; } = "localhost";

    [JsonPropertyName("Username")]
    public string Username { get; set; } = "root";

    [JsonPropertyName("Password")]
    public string Password { get; set; } = "password";

    [JsonPropertyName("Port")]
    public int Port { get; set; } = 3306;

    [JsonPropertyName("Database Name")]
    public string Database { get; set; } = "database";

    [JsonPropertyName("sslmode")]
    public string Sslmode { get; set; } = "none";

    [JsonPropertyName("table-prefix")]
    public string TablePrefix { get; set; } = "";
}

public class CommandSettings
{
    [JsonPropertyName("Check Time Commands")]
    public List<string> Commands { get; set; } = new List<string>
    {
        "css_checktime",
        "css_czasgry"
    };

    [JsonPropertyName("Check Time Command Description")]
    public string CommandDescription { get; set; } = "Check player time spent";
}
