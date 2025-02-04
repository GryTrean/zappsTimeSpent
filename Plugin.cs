using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using zappsTimeSpent.Models;
using CounterStrikeSharp.API.Modules.Commands;

namespace zappsTimeSpent;

public sealed partial class Plugin : BasePlugin, IPluginConfig<PluginConfig>
{
    public required PluginConfig Config { get; set; } = new PluginConfig();
    public void OnConfigParsed(PluginConfig config)
    {
        this.Config = config;
    }

    public List<TimePlayer> Players = new List<TimePlayer>();

    public TimePlayer? GetPlayer(CCSPlayerController? player)
    {
        if (player == null)
            return null;

        return Players.FirstOrDefault(el => el.Controller.SteamID == player.SteamID);
    }

    public override void Load(bool hotReload)
    {
        Task.Run(CreateTablesAsync).Wait();

        AddTimer((float) Config.TimeInterval, () =>
        {
            Players.Where(el => el.Controller.TeamNum != 0).ToList().ForEach(p =>
            {
                p.TimeSpent[(CsTeam)p.Controller.TeamNum] += (int) Config.TimeInterval / 60;
            });
        }, TimerFlags.REPEAT);

        AddTimer(Config.SaveInterval, () =>
        {
            Task.Run(async () => await SavePlayerList(Players));
        }, TimerFlags.REPEAT);

        foreach(string cmd in Config.CommandSettings.Commands)
        {
            AddCommand(cmd, Config.CommandSettings.CommandDescription, (player, info) =>
            {
                if (player == null || !player.IsValid)
                    return;

                CCSPlayerController? target = info.ArgCount > 1 ? info.GetArgTargetResult(1).FirstOrDefault() : null;
                TimePlayer? cPlayer;
                bool checkingSelf = false;

                if (target == null || !target.IsValid)
                {
                    cPlayer = GetPlayer(player);
                    checkingSelf = true;
                }
                else
                    cPlayer = GetPlayer(target);

                if (cPlayer == null)
                {
                    Server.NextFrame(() =>
                    {
                        player.PrintToChat($"{Localizer["timespent.show.failloadplayer"]}");
                    });
                    return;
                }

                int requestedDays = -1;
                if(info.ArgCount > 1)
                    if(Int32.TryParse(info.ArgByIndex(checkingSelf ? 1 : 2), out int days))
                        requestedDays = days;

                Task.Run(async () =>
                {
                    await PrintPlayerTimeSpent(cPlayer, player, requestedDays);
                });
            });
        }
    }

    public string FormatMinutes(int minutes)
    {
        TimeSpan time = TimeSpan.FromMinutes(minutes);
        return string.Format("{0:00}:{1:00}", (int)time.TotalHours, time.Minutes);
    }
}