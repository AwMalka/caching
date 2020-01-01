using System;

namespace PubComp.Caching.Core.Directives
{
    [Flags]
    public enum CacheMethodTaken
    {
        None = 0b_0000_0000,// 0
        Set = 0b_0000_0001, // 1
        Get = 0b_0000_0010, // 2
        GetOrSet = Get | Set
    }
}
