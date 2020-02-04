﻿using System;
using System.Collections.Immutable;
using System.Threading;

namespace PubComp.Caching.Core
{
    internal class ScopedContext<TContext> where TContext : class, new()
    {
        private static readonly AsyncLocal<ImmutableStack<DisposableScopedContext>> ContextsStack = new AsyncLocal<ImmutableStack<DisposableScopedContext>>();

        public static TContext CurrentContext
        {
            get
            {
                if (ContextsStack.Value?.IsEmpty ?? true)
                {
                    return new TContext();
                }

                return ContextsStack.Value.Peek().Context;
            }
        }

        public static DateTimeOffset CurrentTimestamp
        {
            get
            {
                if (ContextsStack.Value?.IsEmpty ?? true)
                {
                    return DateTimeOffset.UtcNow;
                }

                return ContextsStack.Value.Peek().ScopeTimestamp;
            }
        }

        public static IDisposable CreateNewScope(TContext context)
        {
            var disposableScopedContext = new DisposableScopedContext(context);

            var currentStackValue = ContextsStack.Value ?? ImmutableStack<DisposableScopedContext>.Empty;
            ContextsStack.Value = currentStackValue.Push(disposableScopedContext);

            return disposableScopedContext;
        }

        private class DisposableScopedContext : IDisposable
        {
            public TContext Context { get; }

            public DateTimeOffset ScopeTimestamp { get; }

            public DisposableScopedContext(TContext context)
            {
                Context = context;
                ScopeTimestamp = DateTimeOffset.UtcNow;
            }

            public void Dispose()
            {
                if (ContextsStack.Value?.IsEmpty ?? true)
                {
                    throw new ObjectDisposedException(nameof(TContext),
                        "This context was already disposed, and you should already know that.");

                }

                var currentContext = ContextsStack.Value.Peek();
                if (currentContext != this)
                {
                    throw new Exception("Disposing out of order will cause context to \"bleed\" " +
                                        "from the current scope. Consider this a fatal exception.");
                }

                ContextsStack.Value = ContextsStack.Value.Pop();
            }
        }
    }
}