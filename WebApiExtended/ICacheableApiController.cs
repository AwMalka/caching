using PubComp.Caching.Core;
using PubComp.Caching.Core.Directives;

namespace PubComp.Caching.WebApiExtended
{
    public interface ICacheableApiController
    {
        IContextual<CacheDirectives> ContextualCacheDirectives { get; }

        CacheDirectivesOutcome CacheDirectivesOutcome { get; set; }
    }
}