using System;

using ClickLib.Clicks;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonSelectIconStringFeature : OnSetupSelectListFeature
{
    [AddonPostSetup("SelectIconString")]
    protected unsafe void AddonSetup(AtkUnitBase* addon)
    {
        var addonPtr = (AddonSelectIconString*)addon;
        var popupMenu = &addonPtr->PopupMenu.PopupMenu;

        SetupOnItemSelectedHook(popupMenu);
        CompareNodesToEntryTexts((nint)addon, popupMenu);
    }

    protected override void SelectItemExecute(IntPtr addon, int index)
    {
        Svc.Log.Debug($"AddonSelectIconString: Selecting {index}");
        ClickSelectIconString.Using(addon).SelectItem((ushort)index);
    }
}
