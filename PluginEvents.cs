using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using zappsTimeSpent.Models;

namespace zappsTimeSpent;

public sealed partial class Plugin : BasePlugin
{
    [GameEventHandler]
    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;
        if (player == null || !player.IsValid)
            return HookResult.Continue;

        TimePlayer cPlayer = new(player);
        Players.Add(cPlayer);
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        TimePlayer? cPlayer = GetPlayer(@event.Userid);
        if (cPlayer == null)
            return HookResult.Continue;

        Task.Run(async () =>
        {
            await SavePlayerList([cPlayer]);
            Players.Remove(cPlayer);
        });

        return HookResult.Continue;
    }
}