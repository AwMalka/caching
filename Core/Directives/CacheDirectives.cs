using System;

namespace PubComp.Caching.Core.Directives
{
    public class CacheDirectives
    {
        public static readonly string HeadersKey = $"X-{nameof(CacheDirectives)}";

        public CacheMethod Method { get; set; }
        public DateTimeOffset? MinimumSynchronizationTimestamp { get; set; }
    }
}
