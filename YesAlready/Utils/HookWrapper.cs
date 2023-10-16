using System;
using Dalamud.Hooking;

namespace YesAlready.Utils;

public interface IHookWrapper : IDisposable
{
    public void Enable();
    public void Disable();

    public bool IsEnabled { get; }
    public bool IsDisposed { get; }

}

public class HookWrapper<T>(Hook<T> hook) : IHookWrapper where T : Delegate
{

    private readonly Hook<T> wrappedHook = hook;

    private bool disposed;

    public void Enable()
    {
        if (disposed) return;
        wrappedHook?.Enable();
    }

    public void Disable()
    {
        if (disposed) return;
        wrappedHook?.Disable();
    }

    public void Dispose()
    {
        Disable();
        disposed = true;
        wrappedHook?.Dispose();
    }

    public nint Address => wrappedHook.Address;
    public T Original => wrappedHook.Original;
    public bool IsEnabled => wrappedHook.IsEnabled;
    public bool IsDisposed => wrappedHook.IsDisposed;
}
