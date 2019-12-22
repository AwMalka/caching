using System;

namespace PubComp.Caching.Core.Directives
{
    public class CacheDirectivesOutcome
    {
        public static readonly string HeadersKey = $"X-{nameof(CacheDirectivesOutcome)}";

        public CacheMethodTaken MethodTaken { get; set; }
        public DateTimeOffset? SynchronizationTimestamp { get; set; }
    }
}
