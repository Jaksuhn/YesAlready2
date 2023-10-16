using System;

using ClickLib.Clicks;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonSelectStringFeature : OnSetupSelectListFeature
{
    [AddonPostSetup("SelectString")]
    protected unsafe void AddonSetup(AtkUnitBase* addon)
    {
        var addonPtr = (AddonSelectString*)addon;
        var popupMenu = &addonPtr->PopupMenu.PopupMenu;

        SetupOnItemSelectedHook(popupMenu);
        CompareNodesToEntryTexts((nint)addon, popupMenu);
    }

    protected override void SelectItemExecute(IntPtr addon, int index)
    {
        Svc.Log.Debug($"AddonSelectString: Selecting {index}");
        ClickSelectString.Using(addon).SelectItem((ushort)index);
    }
}
