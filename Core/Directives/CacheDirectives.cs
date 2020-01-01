using System;

namespace PubComp.Caching.Core.Directives
{
    public class CacheDirectives
    {
        public static string HeadersKey = $"X-{nameof(CacheDirectives)}";

        public CacheMethod Method { get; set; }
        public DateTimeOffset? MinimumValueTimestamp { get; set; }
    }
}
