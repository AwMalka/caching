﻿using System;
using System.Collections.Immutable;
using System.Threading;

namespace PubComp.Caching.Core
{
    public class Contextual<TContext> : IContextual<TContext> where TContext : class, new()
    {
        private static readonly AsyncLocal<ImmutableStack<DisposableScopedContext>> ContextsStack = new AsyncLocal<ImmutableStack<DisposableScopedContext>>();

        public TContext CurrentOrDefault
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

        public IDisposable CreateNewScope(TContext context)
        {
            var disposableScopedContext = new DisposableScopedContext(context);

            var currentStackValue = ContextsStack.Value ?? ImmutableStack<DisposableScopedContext>.Empty;
            ContextsStack.Value = currentStackValue.Push(disposableScopedContext);

            return disposableScopedContext;
        }

        private class DisposableScopedContext : IDisposable
        {
            public TContext Context { get; }

            public DisposableScopedContext(TContext context)
            {
                Context = context;
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
