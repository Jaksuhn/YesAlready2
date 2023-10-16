using ClickLib.Clicks;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonRetainerTaskResultFeature : BaseFeature
{
    [AddonPostSetup("RetainerTaskResult")]
    protected unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (!P.Config.RetainerTaskResultEnabled)
            return;

        var addonPtr = (AddonRetainerTaskResult*)addon;
        var buttonText = addonPtr->ReassignButton->ButtonTextNode->NodeText.ToString();
        if (buttonText == Svc.Data.GetExcelSheet<Addon>(Svc.ClientState.ClientLanguage).GetRow(2365).Text)
            return;

        ClickRetainerTaskResult.Using((nint)addon).Reassign();
    }
}
