using Dalamud.Plugin;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YesAlready.Events;

namespace YesAlready.BaseFeatures;

public abstract class BaseFeature
{
    public FeatureProvider Provider { get; private set; } = null!;

    public virtual bool Enabled { get; protected set; }
    public abstract string Name { get; }
    public virtual string Key => GetType().Name;
    public virtual bool Ready { get; protected set; }

    public void InterfaceSetup(FeatureProvider fp) => Provider = fp;

    public virtual void Enable()
    {
        Svc.Log.Debug($"Enabling {Name}");
        EventController.RegisterEvents(this);
        Enabled = true;
    }

    public virtual void Disable()
    {
        Svc.Log.Debug($"Disabling {Name}");
        EventController.UnregisterEvents(this);
        Enabled = false;
    }
}
