using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using MySqlConnector;
using Dapper;
using zappsTimeSpent.Models;
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API.Core.Translations;

namespace zappsTimeSpent;

public class PlayerDbData
{
    public int TimeT = 0;
    public int TimeCT = 0;
    public int TimeSpec = 0;
}

public sealed partial class Plugin : BasePlugin
{
    public MySqlConnection CreateConnection(PluginConfig config)
    {
        DatabaseSettings _settings = config.DatabaseSettings;

        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = _settings.Host,
            UserID = _settings.Username,
            Password = _settings.Password,
            Database = _settings.Database,
            Port = (uint)_settings.Port,
            SslMode = Enum.Parse<MySqlSslMode>(_settings.Sslmode, true),
        };

        return new MySqlConnection(builder.ToString());
    }

    public async Task CreateTablesAsync()
    {
        string _PREFIX = Config.DatabaseSettings.TablePrefix;
        string query = $@"
            CREATE TABLE IF NOT EXISTS `{_PREFIX}playertime` (
                `SteamID` BIGINT UNSIGNED,
                `PlayerName` VARCHAR(255) DEFAULT NULL,
                `Date` DATE DEFAULT NULL,
                `TimeCT` INT DEFAULT 0,
                `TimeT` INT DEFAULT 0,
                `TimeSpec` INT DEFAULT 0,
                UNIQUE KEY (`SteamID`, `Date`)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
        ";

        using MySqlConnection connection = CreateConnection(Config);
        await connection.OpenAsync();
        await connection.ExecuteAsync(query);
    }

    public async Task SavePlayerList(List<TimePlayer> players)
    {
        using MySqlConnection connection = CreateConnection(Config);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        string _PREFIX = Config.DatabaseSettings.TablePrefix;

        string query = $@"
            INSERT INTO `{_PREFIX}playertime`
                (SteamID, PlayerName, Date, TimeCT, TimeT, TimeSpec)
            VALUES
                (@SteamID, @PlayerName, CURRENT_DATE, @TimeCT, @TimeT, @TimeSpec)
            ON DUPLICATE KEY UPDATE
                PlayerName = @PlayerName,
                TimeCT = TimeCT + @TimeCT,
                TimeT = TimeT + @TimeT,
                TimeSpec = TimeSpec + @TimeSpec;
        ";

        try
        {
            foreach(TimePlayer player in players)
            {
                await connection.ExecuteAsync(query, new
                {
                    player.PlayerName,
                    TimeCT = player.TimeSpent[CsTeam.CounterTerrorist],
                    TimeT = player.TimeSpent[CsTeam.Terrorist],
                    TimeSpec = player.TimeSpent[CsTeam.Spectator],
                    player.SteamID
                }, transaction);

                player.TimeSpent = player.TimeSpent.ToDictionary(el => el.Key, _ => 0);
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Logger.LogError($"Error saving all players: {ex.Message}");
            throw;
        }
    }

    public async Task PrintPlayerTimeSpent(TimePlayer cPlayer, CCSPlayerController player, int days)
    {
        string _PREFIX = Config.DatabaseSettings.TablePrefix;
        string query = $@"
            SELECT
                SUM(TimeCT) as TimeCT,
                SUM(TimeT) as TimeT,
                SUM(TimeSpec) as TimeSpec
            FROM `{_PREFIX}playertime`
            WHERE SteamID = @SteamID";

        if (days != -1)
            query += $@" AND Date > CURRENT_DATE - INTERVAL {days} DAY;";

        using MySqlConnection connection = CreateConnection(Config);
        await connection.OpenAsync();

        PlayerDbData? dbData = await connection.QueryFirstOrDefaultAsync<PlayerDbData>(query, new { cPlayer.SteamID });
        if (dbData == null)
        {
            Server.NextFrame(() =>
            {
                player.PrintToChat($"{Localizer["timespent.show.failload"]}");
            });
            return;
        }

        Server.NextFrame(() =>
        {
            foreach(string line in Config.CheckTimePrint)
            {
                string msg = line
                    .Replace("{PREFIX}", Localizer["timespent.show.prefix"])
                    .Replace("{TOTAL_TIME}", FormatMinutes(dbData.TimeCT + dbData.TimeT + dbData.TimeSpec))
                    .Replace("{T_TIME}", FormatMinutes(dbData.TimeT))
                    .Replace("{CT_TIME}", FormatMinutes(dbData.TimeCT))
                    .Replace("{TEAM_TIME}", FormatMinutes(dbData.TimeT + dbData.TimeCT))
                    .Replace("{SPEC_TIME}", FormatMinutes(dbData.TimeSpec));

                if (days == -1)
                    msg = msg.Replace("{DESCRIPTION}", Localizer["timespent.show.description.alltime", cPlayer.PlayerName]);
                else
                    msg = msg.Replace("{DESCRIPTION}", Localizer["timespent.show.description", days, cPlayer.PlayerName]);

                player.PrintToChat(msg.ReplaceColorTags());
            }
        });
    }
}