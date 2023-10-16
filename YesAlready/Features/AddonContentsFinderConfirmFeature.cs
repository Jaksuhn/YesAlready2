using ClickLib.Clicks;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonContentsFinderConfirmFeature : BaseFeature
{
    public override string Name => nameof(AddonContentsFinderConfirmFeature);

    [AddonPostSetup("ContentsFinderConfirm")]
    protected static unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (!P.Config.ContentsFinderConfirmEnabled)
            return;

        ClickContentsFinderConfirm.Using((nint)addon).Commence();

        if (P.Config.ContentsFinderOneTimeConfirmEnabled)
        {
            P.Config.ContentsFinderConfirmEnabled = false;
            P.Config.ContentsFinderOneTimeConfirmEnabled = false;
            P.Config.Save();
        }
    }
}