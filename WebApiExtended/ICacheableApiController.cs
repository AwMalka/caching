using PubComp.Caching.Core;
using PubComp.Caching.Core.Directives;

namespace PubComp.Caching.WebApiExtended
{
    public interface ICacheableApiController
    {
        IScopedContext<CacheDirectives> ScopedContextCacheDirectives { get; }

        CacheDirectivesOutcome CacheDirectivesOutcome { get; set; }
    }
}