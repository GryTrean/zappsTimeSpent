using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace zappsTimeSpent;

public sealed partial class Plugin : BasePlugin
{
    public override string ModuleName => "zappsTimeSpent";
    public override string ModuleVersion => "1.0";
    public override string ModuleAuthor => "ZAWAPPS";
    public override string ModuleDescription => "Track player time spent in game";
}