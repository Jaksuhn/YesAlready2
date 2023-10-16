using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonSalvageDialogFeature : BaseFeature
{
    public override string Name => nameof(AddonSalvageDialogFeature);

    [AddonPostSetup("SalvageDialog")]
    protected unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (P.Config.DesynthBulkDialogEnabled)
        {
            ((AddonSalvageDialog*)addon)->BulkDesynthEnabled = true;
        }

        if (P.Config.DesynthDialogEnabled)
        {
            var clickAddon = ClickSalvageDialog.Using((nint)addon);
            clickAddon.CheckBox();
            clickAddon.Desynthesize();
        }
    }
}
