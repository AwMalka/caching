using PostSharp.Aspects;
using PubComp.Caching.Core;
using System;
using System.Threading.Tasks;

namespace PubComp.Caching.AopCaching
{
    [Serializable]
    public class OptionalCacheDirectivesAttribute : MethodInterceptionAspect
    {
        private CacheMethod defaultMethod;
        private double defaultMinimumAgeInMilliseconds;

        public OptionalCacheDirectivesAttribute(CacheMethod defaultMethod)
        {
            this.defaultMethod = defaultMethod;
            this.defaultMinimumAgeInMilliseconds = 0;
        }

        public OptionalCacheDirectivesAttribute(CacheMethod defaultMethod, double defaultMinimumAgeInMilliseconds)
        {
            this.defaultMethod = defaultMethod;
            this.defaultMinimumAgeInMilliseconds = defaultMinimumAgeInMilliseconds;
        }

        public sealed override void OnInvoke(MethodInterceptionArgs args)
        {
            if (CacheDirectives.IsScopeEmpty)
            {
                using (CacheDirectives.SetScope(defaultMethod,
                    DateTimeOffset.UtcNow.AddMilliseconds(-defaultMinimumAgeInMilliseconds)))
                {
                    base.OnInvoke(args);
                    Console.WriteLine("finished");
                }
            }
            else
            {
                base.OnInvoke(args);
            }
        }

        /// <inheritdoc />
        public sealed override async Task OnInvokeAsync(MethodInterceptionArgs args)
        {
            if (CacheDirectives.IsScopeEmpty)
            {
                using (CacheDirectives.SetScope(defaultMethod,
                    DateTimeOffset.UtcNow.AddMilliseconds(-defaultMinimumAgeInMilliseconds)))
                    await base.OnInvokeAsync(args).ConfigureAwait(false);
            }
            else
            {
                await base.OnInvokeAsync(args).ConfigureAwait(false);
            }
        }
    }
}