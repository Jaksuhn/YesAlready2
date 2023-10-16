using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonRetainerTaskAskFeature : BaseFeature
{
    [AddonPostSetup("RetainerTaskAsk")]
    protected static unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (!P.Config.RetainerTaskAskEnabled)
            return;

        ClickRetainerTaskAsk.Using((nint)addon).Assign();
    }
}
