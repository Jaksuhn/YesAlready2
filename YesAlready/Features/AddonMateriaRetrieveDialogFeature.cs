using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonMateriaRetrieveDialogFeature : BaseFeature
{
    public override string Name => nameof(AddonMateriaRetrieveDialogFeature);

    [AddonPostSetup("MateriaRetrieveDialog")]
    protected static unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (!P.Config.MateriaRetrieveDialogEnabled)
            return;

        ClickMateriaRetrieveDialog.Using((nint)addon).Begin();
    }
}
