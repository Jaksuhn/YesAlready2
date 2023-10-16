using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using ECommons;
using ECommons.DalamudServices;
using PunishPlugin.UI;

namespace PunishPlugin;

public class Plugin : IDalamudPlugin
{
    public string Name => "Punish Plugin";
    private const string CommandName = "/pmycommand";
    internal Configuration Configuration { get; init; }
    internal WindowSystem Ws;
    internal MainWindow MainWindow;
    internal Configuration Config;

    internal static Plugin P;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        P = this;
        ECommonsMain.Init(pluginInterface, P);
        //SponsorManager.SetSponsorInfo();
        Ws = new();
        MainWindow = new();
        Ws.AddWindow(MainWindow);

        Config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Config.Initialize(Svc.PluginInterface);

        Svc.Commands.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Help!",
            ShowInHelp = true
        });

        Svc.PluginInterface.UiBuilder.Draw += Ws.Draw;
        Svc.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;


    }

    public void Dispose()
    {
        Svc.Commands.RemoveHandler(CommandName);
        Svc.PluginInterface.UiBuilder.Draw -= Ws.Draw;
        Svc.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
        Ws.RemoveAllWindows();
        MainWindow = null;
        Ws = null;
        ECommonsMain.Dispose();
        P = null;
    }

    private void OnCommand(string command, string args)
    {
        MainWindow.IsOpen = !MainWindow.IsOpen;
    }

    public void DrawConfigUI()
    {
        MainWindow.IsOpen = !MainWindow.IsOpen;
    }
}

