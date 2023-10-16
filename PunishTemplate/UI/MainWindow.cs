using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace PunishPlugin.UI;

internal class MainWindow : Window
{
    public MainWindow() : base($"{P.Name} {P.GetType().Assembly.GetName().Version}###PunishPlugin")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void Dispose()
    {
        
    }

    public override void Draw()
    {
        PunishLib.ImGuiMethods.AboutTab.Draw(Plugin.P);
    }
}
