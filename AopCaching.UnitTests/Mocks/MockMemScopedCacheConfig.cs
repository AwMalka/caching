using PubComp.Caching.Core;
using PubComp.Caching.Core.Config;

namespace PubComp.Caching.AopCaching.UnitTests.Mocks
{
    public class MockMemScopedCacheConfig : CacheConfig
    {
        public override ICache CreateCache()
        {
            return new MockMemScopedCache(this.Name);
        }
    }
}
