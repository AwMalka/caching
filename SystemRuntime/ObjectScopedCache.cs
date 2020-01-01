using PubComp.Caching.Core;
using PubComp.Caching.Core.CacheUtils;
using System;
using System.Threading.Tasks;
using PubComp.Caching.Core.Directives;

namespace PubComp.Caching.SystemRuntime
{
    public abstract class ObjectScopedCache : ICache, IScopedCache
    {
        private readonly String name;
        private System.Runtime.Caching.ObjectCache innerCache;
        private readonly MultiLock locks;
        private readonly InMemoryScopedPolicy policy;
        private readonly IScopedContext<CacheDirectives> cacheDirectivesScopedContext;

        private CacheDirectives ScopeCacheDirectives => cacheDirectivesScopedContext.CurrentOrDefault;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly string notiferName;

        // ReSharper disable once NotAccessedField.Local - reference isn't necessary, but it makes debugging easier
        private readonly CacheSynchronizer synchronizer;

        protected ObjectScopedCache(
            String name, System.Runtime.Caching.ObjectCache innerCache, InMemoryPolicy policy,
            IScopedContext<CacheDirectives> cacheDirectivesScopedContext)
        {
            this.name = name;
            this.policy = policy;
            this.innerCache = innerCache;
            this.cacheDirectivesScopedContext = cacheDirectivesScopedContext;

            this.locks = !this.policy.DoNotLock
                ? new MultiLock(
                    this.policy.NumberOfLocks ?? 50,
                    this.policy.LockTimeoutMilliseconds != null && this.policy.LockTimeoutMilliseconds > 0
                        ? this.policy.LockTimeoutMilliseconds
                        : null,
                    this.policy.DoThrowExceptionOnTimeout ?? true)
                : null;

            this.notiferName = this.policy?.SyncProvider;
            this.synchronizer = CacheSynchronizer.CreateCacheSynchronizer(this, this.notiferName);
        }

        public string Name { get { return this.name; } }

        protected System.Runtime.Caching.ObjectCache InnerCache { get { return this.innerCache; } }

        protected InMemoryExpirationPolicy ExpirationPolicy
        {
            get
            {
                if (synchronizer?.IsActive ?? true)
                {
                    return policy;
                }
                else
                {
                    return policy.OnSyncProviderFailure ?? policy;
                }
            }
        }

        bool ICache.TryGet<TValue>(string key, out TValue value)
        {
            return TryGetInner(key, out value, out _);
        }

        public bool TryGet<TValue>(string key, out TValue value, out CacheDirectivesOutcome outcome)
        {
            return TryGetInner(key, out value, out outcome);
        }

        public Task<TryGetResult<TValue>> TryGetAsync<TValue>(string key)
        {
            return Task.FromResult(new TryGetResult<TValue>
            {
                WasFound = TryGetInner<TValue>(key, out var value, out _),
                Value = value
            });
        }

        void ICache.Set<TValue>(string key, TValue value)
        {
            Add(key, value);
        }

        public void Set<TValue>(string key, TValue value, DateTimeOffset valueTimestamp)
        {
            Add(key, value, valueTimestamp);
        }

        Task ICache.SetAsync<TValue>(string key, TValue value)
        {
            Add(key, value);
            return Task.FromResult<object>(null);
        }

        public Task SetAsync<TValue>(string key, TValue value, DateTimeOffset valueTimestamp)
        {
            Add(key, value, valueTimestamp);
            return Task.FromResult<object>(null);
        }

        protected virtual bool TryGetInner<TValue>(String key, out TValue value, out CacheDirectivesOutcome outcome)
        {
            if (!ScopeCacheDirectives.Method.HasFlag(CacheMethod.Get) ||
                !ScopeCacheDirectives.MinimumValueTimestamp.HasValue)
            {
                value = default(TValue);
                outcome = null;
                return false;
            }

            if (innerCache.Get(key, null) is ScopedCacheItem item && 
                item.ValueTimestamp > ScopeCacheDirectives.MinimumValueTimestamp.Value)
            {
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                // ReSharper disable once MergeConditionalExpression
                value = item.Value is TValue ? (TValue) item.Value : default(TValue);
                outcome = new CacheDirectivesOutcome
                {
                    MethodTaken = CacheMethodTaken.Get,
                    SynchronizationTimestamp = item.ValueTimestamp
                };
                return true;
            }

            outcome = new CacheDirectivesOutcome
            {
                MethodTaken = CacheMethodTaken.None
            };
            value = default(TValue);
            return false;
        }

        protected virtual void Add<TValue>(String key, TValue value)
        {
            innerCache.Set(key, new ScopedCacheItem(value), GetRuntimePolicy(), null);
        }

        protected virtual void Add<TValue>(String key, TValue value, DateTimeOffset valueTimestamp)
        {
            innerCache.Set(key, new ScopedCacheItem(value, valueTimestamp), GetRuntimePolicy(), null);
        }

        // ReSharper disable once ParameterHidesMember
        protected System.Runtime.Caching.CacheItemPolicy GetRuntimePolicy()
            => ToRuntimePolicy(ExpirationPolicy);

        // ReSharper disable once ParameterHidesMember
        protected System.Runtime.Caching.CacheItemPolicy ToRuntimePolicy(InMemoryPolicy policy) 
            => ToRuntimePolicy((InMemoryExpirationPolicy)policy);
        
        // ReSharper disable once ParameterHidesMember
        protected System.Runtime.Caching.CacheItemPolicy ToRuntimePolicy(InMemoryExpirationPolicy expirationPolicy)
        {
            TimeSpan slidingExpiration;
            DateTimeOffset absoluteExpiration;

            if (expirationPolicy.SlidingExpiration != null && expirationPolicy.SlidingExpiration.Value < TimeSpan.MaxValue)
            {
                absoluteExpiration = System.Runtime.Caching.ObjectCache.InfiniteAbsoluteExpiration;
                slidingExpiration = expirationPolicy.SlidingExpiration.Value;
            }
            else if (expirationPolicy.ExpirationFromAdd != null && expirationPolicy.ExpirationFromAdd.Value < TimeSpan.MaxValue)
            {
                absoluteExpiration = DateTimeOffset.Now.Add(expirationPolicy.ExpirationFromAdd.Value);
                slidingExpiration = System.Runtime.Caching.ObjectCache.NoSlidingExpiration;
            }
            else if (expirationPolicy.AbsoluteExpiration != null && expirationPolicy.AbsoluteExpiration.Value < DateTimeOffset.MaxValue)
            {
                absoluteExpiration = expirationPolicy.AbsoluteExpiration.Value;
                slidingExpiration = System.Runtime.Caching.ObjectCache.NoSlidingExpiration;
            }
            else
            {
                absoluteExpiration = System.Runtime.Caching.ObjectCache.InfiniteAbsoluteExpiration;
                slidingExpiration = System.Runtime.Caching.ObjectCache.NoSlidingExpiration;
            }

            return new System.Runtime.Caching.CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            };
        }

        public TValue Get<TValue>(String key, Func<TValue> getter)
        {
        }

        public TValue Get<TValue>(String key, Func<ScopedCacheItem> getter, out CacheDirectivesOutcome outcome)
        {
            if (TryGetInner(key, out TValue value, out outcome))
                return value;

            TValue OnCacheMiss()
            {
                if (TryGetInner(key, out value, out outcome)) return value;

                value = getter();
                Set(key, value);

                outcome = new CacheDirectivesOutcome {MethodTaken = CacheMethodTaken.Set, };

                return value;
            }

            if (policy.DoNotLock)
                return OnCacheMiss();

            return this.locks.LockAndLoad(key, OnCacheMiss);
        }

        public async Task<TValue> GetAsync<TValue>(string key, Func<Task<TValue>> getter)
        {
            if (TryGetInner(key, out TValue value))
                return value;

            async Task<TValue> OnCacheMiss()
            {
                if (TryGetInner(key, out value)) return value;

                value = await getter().ConfigureAwait(false);
                Set(key, value);
                return value;
            }

            if (policy.DoNotLock)
                return await OnCacheMiss().ConfigureAwait(false);

            return await this.locks.LockAndLoadAsync(key, OnCacheMiss).ConfigureAwait(false);
        }

        public void Clear(String key)
        {
            innerCache.Remove(key, null);
        }

        public Task ClearAsync(string key)
        {
            innerCache.Remove(key, null);
            return Task.FromResult<object>(null);
        }

        public void ClearAll()
        {
            innerCache = new System.Runtime.Caching.MemoryCache(this.name);
        }

        public Task ClearAllAsync()
        {
            innerCache = new System.Runtime.Caching.MemoryCache(this.name);
            return Task.FromResult<object>(null);
        }
    }
}
