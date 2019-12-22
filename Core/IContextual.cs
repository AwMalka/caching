using System;

namespace PubComp.Caching.Core
{
    public interface IContextual<TContext> where TContext : class, new()
    {
        TContext CurrentOrDefault { get; }

        IDisposable CreateNewScope(TContext context);
    }
}