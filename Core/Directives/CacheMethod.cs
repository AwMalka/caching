namespace PubComp.Caching.Core.Directives
{
    public enum CacheMethod
    {
        Ignore = 0,
        TryGet = 1,
        GetOrAdd = 2,
        AddOrUpdate = 3
    }
}
