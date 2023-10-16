using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonRetainerTaskAskFeature : BaseFeature
{
    public override string Name => nameof(AddonRetainerTaskAskFeature);

    [AddonPostSetup("RetainerTaskAsk")]
    protected static unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (!P.Config.RetainerTaskAskEnabled)
            return;

        ClickRetainerTaskAsk.Using((nint)addon).Assign();
    }
}
