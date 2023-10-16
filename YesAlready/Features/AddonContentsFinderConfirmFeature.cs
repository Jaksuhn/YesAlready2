using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonContentsFinderConfirmFeature : BaseFeature
{
    [AddonPostSetup("ContentsFinderConfirm")]
    protected static unsafe void AddonSetup(AtkUnitBase* addon)
    {
        Utils.SEString.PrintPluginMessage("contens finder");
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
