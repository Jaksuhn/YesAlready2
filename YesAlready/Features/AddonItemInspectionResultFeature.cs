using ClickLib.Clicks;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonItemInspectionResultFeature : BaseFeature
{
    private int itemInspectionCount = 0;

    [AddonPostSetup("ItemInspectionResult")]
    protected unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (!P.Config.ItemInspectionResultEnabled)
            return;

        var addonPtr = (AddonItemInspectionResult*)addon;
        if (addonPtr->AtkUnitBase.UldManager.NodeListCount < 64)
            return;

        var nameNode = (AtkTextNode*)addonPtr->AtkUnitBase.UldManager.NodeList[64];
        var descNode = (AtkTextNode*)addonPtr->AtkUnitBase.UldManager.NodeList[55];
        if (!nameNode->AtkResNode.IsVisible || !descNode->AtkResNode.IsVisible)
            return;

        var nameText = Utils.SEString.GetSeString(nameNode->NodeText.StringPtr);
        var descText = Utils.SEString.GetSeStringText(descNode->NodeText.StringPtr);
        // This is hackish, but works well enough (for now).
        // Languages that dont contain the magic character will need special handling.
        if (descText.Contains('※') || descText.Contains("liées à Garde-la-Reine"))
        {
            nameText.Payloads.Insert(0, new TextPayload("Received: "));
            Svc.Log.Info(nameText.ToString());
        }

        this.itemInspectionCount++;
        var rateLimiter = P.Config.ItemInspectionResultRateLimiter;
        if (rateLimiter != 0 && itemInspectionCount % rateLimiter == 0)
        {
            itemInspectionCount = 0;
            Svc.Log.Info("Rate limited, pausing item inspection loop.");
            return;
        }

        ClickItemInspectionResult.Using((nint)addon).Next();
    }
}
