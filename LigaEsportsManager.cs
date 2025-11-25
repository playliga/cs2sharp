using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;
using Microsoft.Extensions.Logging;

namespace LigaEsportsManager;

/// <summary>
/// A CounterStrikeSharp plugin for LIGA Esports Manager.
/// </summary>
public class LigaEsportsManagerPlugin : BasePlugin
{
    /// <summary>
    /// Cached hostname of the server.
    /// </summary>
    private string? hostname;

    /// <summary>
    /// Indicates whether players have been welcomed already.
    /// </summary>
    private bool welcomed = false;

    /// <summary>
    /// Timer used to repeatedly remind players to start the match.
    /// </summary>
    private CounterStrikeSharp.API.Modules.Timers.Timer? welcomeTimer;

    /// <inheritdoc/>
    public override string ModuleAuthor => "LIGA Esports Manager";

    /// <inheritdoc/>
    public override string ModuleDescription => "A CounterStrikeSharp plugin for LIGA Esports Manager.";

    /// <inheritdoc/>
    public override string ModuleName => "LIGA Esports Manager";

    /// <inheritdoc/>
    public override string ModuleVersion => "1.0.1";

    /// <summary>
    /// Delay (in seconds) before the server shuts down after a game ends.
    /// </summary>
    public FakeConVar<int> CvarGameOverDelay = new("liga_gameover_delay", "Delay (in seconds) before the server shuts down after a game ends.", 5);

    /// <inheritdoc/>
    public override void Load(bool hotReload)
    {
        // @todo
    }

    /// <summary>
    /// Ends warmup and starts the match.
    /// </summary>
    /// <param name="player">The player who issued the command.</param>
    /// <param name="command">Command context information.</param>
    [ConsoleCommand("ready", "Start the match")]
    public void OnCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            return;
        }

        welcomeTimer?.Kill();
        Server.ExecuteCommand("mp_warmup_end");
    }

    /// <summary>
    /// Shuts down the server after a delay when the match is over.
    /// </summary>
    /// <param name="event">The event data.</param>
    /// <param name="info">Additional event context.</param>
    /// <returns>A HookResult indicating event continuation.</returns>
    [GameEventHandler]
    public HookResult OnGameOver(EventCsWinPanelMatch @event, GameEventInfo info)
    {
        Server.PrintToChatAll($"<{hostname}> SHUTTING DOWN SERVER IN {CvarGameOverDelay.Value}s");
        AddTimer(CvarGameOverDelay.Value, () => Server.ExecuteCommand("quit"));
        return HookResult.Continue;
    }

    /// <summary>
    /// Starts a repeating reminder to type `.ready` after the user joins a team.
    /// </summary>
    /// <param name="event">The event data.</param>
    /// <param name="info">Additional event context.</param>
    /// <returns>A HookResult indicating event continuation.</returns>
    [GameEventHandler]
    public HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
    {
        if (@event.Isbot || welcomed)
        {
            return HookResult.Continue;
        }

        welcomed = true;
        welcomeTimer = AddTimer(
            CvarGameOverDelay.Value,
            () => Server.PrintToChatAll($"<{hostname}> TO START THE MATCH TYPE: .ready"),
            TimerFlags.REPEAT
        );
        return HookResult.Continue;
    }

    /// <summary>
    /// Fetches hostname if unset and executes a bot setup
    /// command when the user first joins the server.
    /// </summary>
    /// <param name="playerSlot">The slot index of the connecting player.</param>
    [ListenerHandler<Listeners.OnClientPutInServer>]
    public void OnClientPutInServer(int playerSlot)
    {
        var player = Utilities.GetPlayerFromSlot(playerSlot);

        if (player == null || player.IsBot || !player.IsValid)
        {
            return;
        }

        // get hostname here because on plugin load it may not
        // be set yet as it is defined by the server config
        if (string.IsNullOrEmpty(hostname))
        {
            hostname = ConVar.Find("hostname")?.StringValue ?? hostname;
        }

        Logger.LogInformation("Player {Name} has connected! Adding bots...", player.PlayerName);
        Server.ExecuteCommand("exec liga-bots");
    }
}
