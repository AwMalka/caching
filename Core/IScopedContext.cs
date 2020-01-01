using System;

namespace PubComp.Caching.Core
{
    public interface IScopedContext<TContext> where TContext : class, new()
    {
        TContext CurrentOrDefault { get; }

        IDisposable CreateNewScope(TContext context);
    }
}