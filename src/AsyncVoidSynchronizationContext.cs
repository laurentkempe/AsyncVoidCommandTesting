using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncVoidCommandTesting;

// https://www.meziantou.net/awaiting-an-async-void-method-in-dotnet.htm
public sealed class AsyncVoidSynchronizationContext : SynchronizationContext
{
    private static readonly SynchronizationContext SDefault = new();

    private readonly SynchronizationContext _innerSynchronizationContext;
    private readonly TaskCompletionSource _tcs = new();
    private int _startedOperationCount;

    private AsyncVoidSynchronizationContext(SynchronizationContext? innerContext)
    {
        _innerSynchronizationContext = innerContext ?? SDefault;
    }

    private Task Completed => _tcs.Task;

    public override void OperationStarted()
    {
        Interlocked.Increment(ref _startedOperationCount);
    }

    public override void OperationCompleted()
    {
        if (Interlocked.Decrement(ref _startedOperationCount) == 0)
        {
            _tcs.TrySetResult();
        }
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        Interlocked.Increment(ref _startedOperationCount);

        try
        {
            _innerSynchronizationContext.Post(s =>
            {
                try
                {
                    d(s);
                }
                catch (Exception ex)
                {
                    _tcs.TrySetException(ex);
                }
                finally
                {
                    OperationCompleted();
                }
            }, state);
        }
        catch (Exception ex)
        {
            _tcs.TrySetException(ex);
        }
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        try
        {
            _innerSynchronizationContext.Send(d, state);
        }
        catch (Exception ex)
        {
            _tcs.TrySetException(ex);
        }
    }
    
    public static async Task Run(Action action)
    {
        var currentContext = Current;
        var synchronizationContext = new AsyncVoidSynchronizationContext(currentContext);
        SetSynchronizationContext(synchronizationContext);
        try
        {
            action();

            // Wait for the async void method to call OperationCompleted or to report an exception
            await synchronizationContext.Completed;
        }
        finally
        {
            // Reset the original SynchronizationContext
            SetSynchronizationContext(currentContext);
        }
    }
}

