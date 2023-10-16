using System;
using System.Linq;

using ClickLib.Clicks;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using YesAlready.BaseFeatures;
using YesAlready.Events;

namespace YesAlready.Features;

internal class AddonTalkFeature : BaseFeature
{
    public override string Name => nameof(AddonTalkFeature);

    private ClickTalk? clickTalk = null;
    private IntPtr lastTalkAddon = IntPtr.Zero;

    [AddonPostUpdate("Talk")]
    protected unsafe void AddonSetup(AtkUnitBase* addon)
    {
        var addonPtr = (AddonTalk*)addon;
        if (!addonPtr->AtkUnitBase.IsVisible)
            return;

        var target = Svc.Targets.Target;
        var targetName = P.LastSeenTalkTarget = target != null
            ? Utils.SEString.GetSeStringText(target.Name)
            : string.Empty;

        var nodes = P.Config.GetAllNodes().OfType<TalkEntryNode>();
        foreach (var node in nodes)
        {
            if (!node.Enabled || string.IsNullOrEmpty(node.TargetText))
                continue;

            var matched = EntryMatchesTargetName(node, targetName);
            if (!matched)
                continue;

            if (clickTalk == null || lastTalkAddon != (IntPtr)addon)
                clickTalk = ClickTalk.Using(lastTalkAddon = (IntPtr)addon);

            Svc.Log.Debug("AddonTalk: Advancing");
            clickTalk.Click();
            return;
        }
    }

    private static bool EntryMatchesTargetName(TalkEntryNode node, string targetName)
    {
        return (node.TargetIsRegex && (node.TargetRegex?.IsMatch(targetName) ?? false)) ||
              (!node.TargetIsRegex && targetName.Contains(node.TargetText));
    }
}
