using System;

using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonGrandCompanySupplyRewardFeature : BaseFeature
{
    [AddonPostSetup("GrandCompanySupplyReward")]
    protected static unsafe void AddonSetup(AtkUnitBase* addon)
    {
        if (!P.Config.GrandCompanySupplyReward)
            return;

        ClickGrandCompanySupplyReward.Using((IntPtr)addon).Deliver();
    }
}
