using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonMaterializeDialogFeature : BaseFeature
{
    public override string Name => nameof(AddonMaterializeDialogFeature);

    [AddonPostSetup("MaterializeDialog")]
    protected static unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (!P.Config.MaterializeDialogEnabled)
            return;

        ClickMaterializeDialog.Using((nint)addon).Materialize();
    }
}
