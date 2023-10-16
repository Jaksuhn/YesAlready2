using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonJournalResultFeature : BaseFeature
{
    [AddonPostSetup("JournalResult")]
    protected unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (!P.Config.JournalResultCompleteEnabled)
            return;

        var addonPtr = (AddonJournalResult*)addon;
        var completeButton = addonPtr->CompleteButton;
        if (!addonPtr->CompleteButton->IsEnabled)
            return;

        ClickJournalResult.Using((nint)addon).Complete();
    }
}
