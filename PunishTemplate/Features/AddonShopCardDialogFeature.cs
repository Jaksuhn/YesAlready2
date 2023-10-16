using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonShopCardDialogFeature : BaseFeature
{
    public override string Name => nameof(AddonShopCardDialogFeature);

    [AddonPostSetup("ShopCardDialog")]
    protected unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (!P.Config.ShopCardDialog)
            return;

        var addonPtr = (AddonShopCardDialog*)addon;
        if (addonPtr->CardQuantityInput != null)
            addonPtr->CardQuantityInput->SetValue(addonPtr->CardQuantityInput->Data.Max);

        ClickShopCardDialog.Using((nint)addon).Sell();
    }
}
