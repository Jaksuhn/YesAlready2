using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Reflection;
using YesAlready.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Attributes;

namespace YesAlready.BaseFeatures;

public abstract class BaseFeature
{
    public virtual bool Enabled { get; protected set; }
    public virtual string Key => GetType().Name;
    public virtual bool Ready { get; protected set; }

    public virtual void Enable()
    {
        Svc.Log.Debug($"Enabling {Key}");
        ExtensionMethods.RegisterEvents(this);
        Enabled = true;
    }

    public virtual void Disable()
    {
        Svc.Log.Debug($"Disabling {Key}");
        ExtensionMethods.UnregisterEvents(this);
        Enabled = false;
    }
}

public static class ExtensionMethods
{
    private static bool IsPointer<T>(this ParameterInfo p, int levels = 1)
    {
        var ptrType = p.ParameterType;
        for (var i = 0; i < levels; i++)
        {
            if (ptrType == null) return false;
            if (!ptrType.IsPointer) return false;
            if (!ptrType.HasElementType) return false;
            ptrType = ptrType.GetElementType();
        }
        return ptrType == typeof(T);
    }

    public class EventSubscriber
    {
        public BaseFeature Feature { get; init; }
        public MethodInfo Method { get; init; }
        public SubscriberKind Kind { get; private set; } = SubscriberKind.Unknown;
        private Type addonPointerType;
        public uint NthTick { get; init; } = 0;
        private uint tick = 0;

        public static EventSubscriber CreateFrameworkSubscriber(BaseFeature feature, MethodInfo method, uint nthTick)
        {
            var s = new EventSubscriber
            {
                Feature = feature,
                Method = method,
                Kind = SubscriberKind.Framework,
                NthTick = nthTick,
            };
            return s;
        }

        public enum SubscriberKind
        {
            Unknown,
            Invalid,
            Error,
            Framework,
            NoParameter,
            AtkUnitBase, // (AtkUnitBase*)
            AtkUnitBaseWithArrays, // (AtkUnitBase*, NumberArrayData**, StringArrayData**)
            AddonPointer,  // (AddonX*)
            AddonPointerWithArrays, // (AddonX*, NumberArrayData**. StringArrayData**) 
            AddonArgs,
            AddonSetupArgs,
            AddonUpdateArgs,
            AddonDrawArgs,
            AddonFinalizeArgs,
            AddonRequestedUpdateArgs,
            AddonRefreshArgs,
        }

        private bool IsAddonPointer(ParameterInfo p)
        {
            if (!p.ParameterType.IsPointer) return false;
            var elementType = p.ParameterType.GetElementType();
            if (elementType == null || elementType.IsPointer) return false;
            var addonAttribute = elementType.GetCustomAttribute<Addon>();
            if (addonAttribute == null) return false;
            return true;
        }

        private unsafe bool DetermineInvokeKind()
        {
            var p = Method.GetParameters();

            try
            {
                if (p.Length == 0)
                {
                    Kind = SubscriberKind.NoParameter;
                    return true;
                }
                else if (p.Length == 1)
                {

                    if (IsAddonPointer(p[0]))
                    {
                        Kind = SubscriberKind.AddonPointer;
                        addonPointerType = p[0].ParameterType;
                        return true;
                    }

                    if (p[0].IsPointer<AtkUnitBase>())
                    {
                        Kind = SubscriberKind.AtkUnitBase;
                        return true;
                    }

                    if (p[0].ParameterType == typeof(AddonArgs))
                    {
                        Kind = SubscriberKind.AddonArgs;
                        return true;
                    }

                    if (p[0].ParameterType == typeof(AddonSetupArgs))
                    {
                        Kind = SubscriberKind.AddonSetupArgs;
                        return true;
                    }

                    if (p[0].ParameterType == typeof(AddonUpdateArgs))
                    {
                        Kind = SubscriberKind.AddonUpdateArgs;
                        return true;
                    }

                    if (p[0].ParameterType == typeof(AddonDrawArgs))
                    {
                        Kind = SubscriberKind.AddonDrawArgs;
                        return true;
                    }

                    if (p[0].ParameterType == typeof(AddonFinalizeArgs))
                    {
                        Kind = SubscriberKind.AddonFinalizeArgs;
                        return true;
                    }

                    if (p[0].ParameterType == typeof(AddonRequestedUpdateArgs))
                    {
                        Kind = SubscriberKind.AddonRequestedUpdateArgs;
                        return true;
                    }

                    if (p[0].ParameterType == typeof(AddonRefreshArgs))
                    {
                        Kind = SubscriberKind.AddonRefreshArgs;
                        return true;
                    }

                }
                else if (p.Length == 3)
                {
                    if (p[1].IsPointer<NumberArrayData>(2) && p[2].IsPointer<StringArrayData>(2))
                    {
                        if (IsAddonPointer(p[0]))
                        {
                            Kind = SubscriberKind.AddonPointer;
                            addonPointerType = p[0].ParameterType;
                            return true;
                        }

                        if (p[0].IsPointer<AtkUnitBase>())
                        {
                            Kind = SubscriberKind.AtkUnitBase;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Error("", ex);
            }

            Svc.Log.Error($"[EventController] Failed to determine a valid delegate type for '{Feature.GetType().Name}.{Method.Name}'");

            foreach (var param in p)
            {
                Svc.Log.Error($"[EventController] \t - {param.ParameterType} {param.Name}");
            }

            Kind = SubscriberKind.Invalid;
            return false;
        }

        public unsafe void Invoke(AddonArgs args)
        {
            if (NthTick > 1)
            {
                if (++tick < NthTick) return;
                tick = 0;
            }

            if (Kind is SubscriberKind.Invalid or SubscriberKind.Error) return;
            if (!Feature.Enabled) return;

            if (Kind == SubscriberKind.Unknown)
            {
                if (!DetermineInvokeKind()) return;
            }


            try
            {
                var _ = Kind switch
                {
                    SubscriberKind.Invalid => null,
                    SubscriberKind.Unknown => null,
                    SubscriberKind.Framework => Method.Invoke(Feature, Array.Empty<object>()),
                    SubscriberKind.NoParameter => Method.Invoke(Feature, Array.Empty<object>()),
                    SubscriberKind.AtkUnitBase => Method.Invoke(Feature, new[] { Pointer.Box((void*)args.Addon, typeof(AtkUnitBase*)) }),
                    SubscriberKind.AtkUnitBaseWithArrays => Method.Invoke(Feature, new[] { Pointer.Box((void*)args.Addon, typeof(AtkUnitBase*)), Pointer.Box(AtkStage.GetSingleton()->GetNumberArrayData(), typeof(NumberArrayData**)), Pointer.Box(AtkStage.GetSingleton()->GetStringArrayData(), typeof(StringArrayData**)), }),
                    SubscriberKind.AddonPointer => Method.Invoke(Feature, new[] { Pointer.Box((void*)args.Addon, addonPointerType) }),
                    SubscriberKind.AddonPointerWithArrays => Method.Invoke(Feature, new[] { Pointer.Box((void*)args.Addon, addonPointerType), Pointer.Box(AtkStage.GetSingleton()->GetNumberArrayData(), typeof(NumberArrayData**)), Pointer.Box(AtkStage.GetSingleton()->GetStringArrayData(), typeof(StringArrayData**)), }),
                    SubscriberKind.AddonArgs => Method.Invoke(Feature, new object[] { args }),
                    SubscriberKind.AddonSetupArgs when args is AddonSetupArgs addonSetupArgs => Method.Invoke(Feature, new object[] { addonSetupArgs }),
                    SubscriberKind.AddonUpdateArgs when args is AddonUpdateArgs addonUpdateArgs => Method.Invoke(Feature, new object[] { addonUpdateArgs }),
                    SubscriberKind.AddonDrawArgs when args is AddonDrawArgs addonDrawArgs => Method.Invoke(Feature, new object[] { addonDrawArgs }),
                    SubscriberKind.AddonFinalizeArgs when args is AddonFinalizeArgs addonFinalizeArgs => Method.Invoke(Feature, new object[] { addonFinalizeArgs }),
                    SubscriberKind.AddonRequestedUpdateArgs when args is AddonRequestedUpdateArgs addonRequestedUpdateArgs => Method.Invoke(Feature, new object[] { addonRequestedUpdateArgs }),
                    SubscriberKind.AddonRefreshArgs when args is AddonRefreshArgs addonRefreshArgs => Method.Invoke(Feature, new object[] { addonRefreshArgs }),
                    _ => null,
                };
            }
            catch (Exception ex)
            {
                Svc.Log.Error($"Error invoking {Feature.Key} :: {Method.Name}. Event has been disabled.", ex);
                Kind = SubscriberKind.Error;
            }

        }
    }
    private static Dictionary<AddonEvent, Dictionary<string, List<EventSubscriber>>> AddonEventSubscribers { get; } = new();
    private static List<EventSubscriber> FrameworkUpdateSubscribers { get; } = new();

    public static bool TryGetCustomAttribute<T>(this MemberInfo element, out T attribute) where T : Attribute
    {
        attribute = element.GetCustomAttribute<T>();
        return attribute != null;
    }

    public static void RegisterEvents(BaseFeature feature)
    {
        if (feature == null) return;

        var methods = feature.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var method in methods)
        {

            if (method.TryGetCustomAttribute<FrameworkUpdateAttribute>(out var fwUpdateAttribute))
            {
                var subscriber = EventSubscriber.CreateFrameworkSubscriber(feature, method, fwUpdateAttribute.NthTick);

                if (FrameworkUpdateSubscribers.Count == 0)
                {
                    Svc.Framework.Update -= HandleFrameworkUpdate;
                    Svc.Framework.Update += HandleFrameworkUpdate;
                }

                FrameworkUpdateSubscribers.Add(subscriber);
            }

            foreach (var attr in method.GetCustomAttributes<AddonEventAttribute>())
            {
                foreach (var addon in attr.AddonNames)
                {
                    Svc.Log.Verbose($"[{nameof(RegisterEvents)}] {feature.Key} requesting event '{attr.Event}' on method '{method.Name}' for addon '{addon}'");

                    var subscriber = new EventSubscriber { Feature = feature, Method = method };

                    if (!AddonEventSubscribers.TryGetValue(attr.Event, out var addonSubscriberDict))
                    {
                        addonSubscriberDict = new Dictionary<string, List<EventSubscriber>>();
                        AddonEventSubscribers.Add(attr.Event, addonSubscriberDict);
                        AddonLifecycle.RegisterListener(attr.Event, HandleEvent);
                    }

                    if (!addonSubscriberDict.TryGetValue(addon, out var addonSubscriberList))
                    {
                        addonSubscriberList = new List<EventSubscriber>();
                        addonSubscriberDict.Add(addon, addonSubscriberList);
                    }

                    addonSubscriberList.Add(subscriber);
                }
            }
        }
    }

    private static void HandleFrameworkUpdate(IFramework framework)
    {
        foreach (var fwSubscriber in FrameworkUpdateSubscribers)
        {
            fwSubscriber.Invoke(null);
        }
    }

    private static void HandleEvent(AddonEvent type, AddonArgs args)
    {
        if (!AddonEventSubscribers.TryGetValue(type, out var addonSubscriberDict)) return;

        if (addonSubscriberDict.TryGetValue(args.AddonName, out var addonSubscriberList))
        {
            foreach (var subscriber in addonSubscriberList)
            {
                if (!subscriber.Feature.Enabled) continue;
                subscriber.Invoke(args);
            }
        }

        if (addonSubscriberDict.TryGetValue("ALL_ADDONS", out var allAddonSubscriberList))
        {
            foreach (var subscriber in allAddonSubscriberList)
            {
                if (!subscriber.Feature.Enabled) continue;
                subscriber.Invoke(args);
            }
        }
    }

    public static void UnregisterEvents(BaseFeature feature)
    {
        FrameworkUpdateSubscribers.RemoveAll(f => f.Feature == feature);
        if (FrameworkUpdateSubscribers.Count == 0)
        {
            Svc.Framework.Update -= HandleFrameworkUpdate;
        }

        foreach (var (_, addonSubscribers) in AddonEventSubscribers)
        {
            foreach (var (_, subscribers) in addonSubscribers)
            {
                subscribers.RemoveAll(subscriber => subscriber.Feature == feature);
            }
        }
    }
}
