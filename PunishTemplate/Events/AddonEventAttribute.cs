using System;
using Dalamud.Game.Addon.Lifecycle;

namespace YesAlready.Events;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AddonEventAttribute(AddonEvent @event, params string[] addonNames) : EventAttribute
{
    public AddonEvent Event { get; } = @event;
    public string[] AddonNames { get; } = addonNames.Length == 0 ? new[] { "ALL_ADDONS" } : addonNames;
}

#region Aliases
public class AddonPreSetupAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PreSetup, addonNames) { }

public class AddonPostSetupAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PostSetup, addonNames) { }

public class AddonFinalizeAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PreFinalize, addonNames) { }

public class AddonPreUpdateAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PreUpdate, addonNames) { }

public class AddonPostUpdateAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PostUpdate, addonNames) { }

public class AddonPreRequestedUpdateAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PreRequestedUpdate, addonNames) { }

public class AddonPostRequestedUpdateAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PostRequestedUpdate, addonNames) { }

public class AddonPreDrawAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PreDraw, addonNames) { }

public class AddonPostDrawAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PostDraw, addonNames) { }

public class AddonPreRefreshAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PreRefresh, addonNames) { }

public class AddonPostRefreshAttribute(params string[] addonNames) : AddonEventAttribute(AddonEvent.PostRefresh, addonNames) { }

#endregion
