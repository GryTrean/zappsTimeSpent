using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace zappsTimeSpent.Models;

public class TimePlayer
{
    public readonly CCSPlayerController Controller;
    public readonly string PlayerName;
    public readonly ulong SteamID;

    public Dictionary<CsTeam, int> TimeSpent = new Dictionary<CsTeam, int>
    {
        { CsTeam.Spectator, 0 },
        { CsTeam.CounterTerrorist, 0 },
        { CsTeam.Terrorist, 0 }
    };

    public TimePlayer(CCSPlayerController controller)
    {
        Controller = controller;
        PlayerName = controller.PlayerName;
        SteamID = controller.SteamID;
    }
}