using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PubComp.Caching.AopCaching.UnitTests.Mocks;
using PubComp.Caching.Core;

namespace PubComp.Caching.AopCaching.UnitTests
{
    [TestClass]
    public class AopOptionalCacheDirectivesTests
    {
        private static MockMemScopedCache scopedCache1;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // The following cache is set in the Assembly Initialize

            const string scopedCache1Name = "PubComp.Caching.AopCaching.UnitTests.MocksScoped.Service";
            scopedCache1 = CacheManager.GetCache(scopedCache1Name) as MockMemScopedCache;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            scopedCache1.ClearAll();
        }

        [TestMethod]
        public void TestOptionalCacheDirectivesAttributeDoesNotExists()
        {
            Assert.IsTrue(CacheDirectives.IsScopeEmpty);

            Assert.AreEqual(CacheMethod.None, GetCurrentCacheDirectivesWithoutDefaults().Method);
            Assert.AreEqual(CacheMethod.None, GetCurrentCacheDirectivesWithNoneAsDefault().Method);
            Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithGetAsDefault().Method);
            Assert.AreEqual(CacheMethod.GetOrSet, GetCurrentCacheDirectivesWithGetOrSetAsDefault().Method);
        }

        [OptionalCacheDirectives(CacheMethod.None)]
        [TestMethod]
        public void TestOptionalCacheDirectivesAttributeDoesExists()
        {
            Assert.IsFalse(CacheDirectives.IsScopeEmpty);

            Assert.AreEqual(CacheMethod.None, GetCurrentCacheDirectivesWithoutDefaults().Method);
            Assert.AreEqual(CacheMethod.None, GetCurrentCacheDirectivesWithNoneAsDefault().Method);
            Assert.AreEqual(CacheMethod.None, GetCurrentCacheDirectivesWithGetAsDefault().Method);
            Assert.AreEqual(CacheMethod.None, GetCurrentCacheDirectivesWithGetOrSetAsDefault().Method);
        }

        [OptionalCacheDirectives(CacheMethod.Get)]
        [TestMethod]
        public void TestOptionalCacheDirectivesAttributeDoesExistsGet()
        {
            Assert.IsFalse(CacheDirectives.IsScopeEmpty);

            Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithoutDefaults().Method);
            Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithNoneAsDefault().Method);
            Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithGetAsDefault().Method);
            Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithGetOrSetAsDefault().Method);
        }

        [OptionalCacheDirectives(CacheMethod.GetOrSet)]
        [TestMethod]
        public void TestOptionalCacheDirectivesAttributeAndSetScope()
        {
            Assert.IsFalse(CacheDirectives.IsScopeEmpty);

            Assert.AreEqual(CacheMethod.GetOrSet, GetCurrentCacheDirectivesWithoutDefaults().Method);
            Assert.AreEqual(CacheMethod.GetOrSet, GetCurrentCacheDirectivesWithNoneAsDefault().Method);
            Assert.AreEqual(CacheMethod.GetOrSet, GetCurrentCacheDirectivesWithGetAsDefault().Method);
            Assert.AreEqual(CacheMethod.GetOrSet, GetCurrentCacheDirectivesWithGetOrSetAsDefault().Method);

            using (CacheDirectives.SetScope(CacheMethod.Get, default(DateTimeOffset)))
            {
                Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithoutDefaults().Method);
                Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithNoneAsDefault().Method);
                Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithGetAsDefault().Method);
                Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithGetOrSetAsDefault().Method);
            }

            Assert.AreEqual(CacheMethod.GetOrSet, GetCurrentCacheDirectivesWithoutDefaults().Method);
            Assert.AreEqual(CacheMethod.GetOrSet, GetCurrentCacheDirectivesWithNoneAsDefault().Method);
            Assert.AreEqual(CacheMethod.GetOrSet, GetCurrentCacheDirectivesWithGetAsDefault().Method);
            Assert.AreEqual(CacheMethod.GetOrSet, GetCurrentCacheDirectivesWithGetOrSetAsDefault().Method);
        }

        [OptionalCacheDirectives(CacheMethod.Get)]
        [OptionalCacheDirectives(CacheMethod.Set)]
        [TestMethod]
        public void TestOptionalCacheDirectivesAttributeOrder()
        {
            Assert.IsFalse(CacheDirectives.IsScopeEmpty);

            Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithoutDefaults().Method);
            Assert.AreEqual(CacheMethod.Get, GetCurrentCacheDirectivesWithGetOrSetAsDefault().Method);
        }

        [TestMethod]
        public void TestCacheAttributeWithOptionalCacheDirectivesAttributeOrder()
        {
            scopedCache1.ClearAll(doResetCounters: true);
            TestCacheAttributeWithoutOptionalCacheDirectives();
            TestCacheAttributeWithoutOptionalCacheDirectives();
            Assert.AreEqual(2, scopedCache1.Skipped);
            Assert.AreEqual(0, scopedCache1.Misses);
            Assert.AreEqual(0, scopedCache1.Hits);

            scopedCache1.ClearAll(doResetCounters: true);
            TestCacheAttributeWithOptionalCacheDirectivesWrapper();
            TestCacheAttributeWithOptionalCacheDirectivesWrapper();
            Assert.AreEqual(1, scopedCache1.Misses);
            Assert.AreEqual(1, scopedCache1.Hits);

            scopedCache1.ClearAll(doResetCounters: true);
            TestCacheAttributeWithOptionalCacheDirectivesBefore();
            TestCacheAttributeWithOptionalCacheDirectivesBefore();
            Assert.AreEqual(1, scopedCache1.Misses);
            Assert.AreEqual(1, scopedCache1.Hits);

            scopedCache1.ClearAll(doResetCounters: true);
            TestCacheAttributeWithOptionalCacheDirectivesAfter();
            TestCacheAttributeWithOptionalCacheDirectivesAfter();
            Assert.AreEqual(2, scopedCache1.Skipped);
            Assert.AreEqual(0, scopedCache1.Misses);
            Assert.AreEqual(0, scopedCache1.Hits);
        }

        [Cache("PubComp.Caching.AopCaching.UnitTests.MocksScoped.Service")]
        public string TestCacheAttributeWithoutOptionalCacheDirectives()
        {
            return Guid.NewGuid().ToString();
        }

        [OptionalCacheDirectives(CacheMethod.GetOrSet | CacheMethod.IgnoreMinimumValueTimestamp, 5_000)]
        public string TestCacheAttributeWithOptionalCacheDirectivesWrapper()
        {
            return TestCacheAttributeWithoutOptionalCacheDirectives();
        }

        [OptionalCacheDirectives(CacheMethod.GetOrSet | CacheMethod.IgnoreMinimumValueTimestamp, 5_000)]
        [Cache("PubComp.Caching.AopCaching.UnitTests.MocksScoped.Service")]
        public string TestCacheAttributeWithOptionalCacheDirectivesBefore()
        {
            return Guid.NewGuid().ToString();
        }

        [Cache("PubComp.Caching.AopCaching.UnitTests.MocksScoped.Service")]
        [OptionalCacheDirectives(CacheMethod.GetOrSet | CacheMethod.IgnoreMinimumValueTimestamp, 5_000)]
        public string TestCacheAttributeWithOptionalCacheDirectivesAfter()
        {
            return Guid.NewGuid().ToString();
        }

        private CacheDirectives GetCurrentCacheDirectivesWithoutDefaults() => CacheDirectives.CurrentScope;

        [OptionalCacheDirectives(CacheMethod.None)]
        private CacheDirectives GetCurrentCacheDirectivesWithNoneAsDefault() => CacheDirectives.CurrentScope;

        [OptionalCacheDirectives(CacheMethod.Get)]
        private CacheDirectives GetCurrentCacheDirectivesWithGetAsDefault() => CacheDirectives.CurrentScope;

        [OptionalCacheDirectives(CacheMethod.GetOrSet)]
        private CacheDirectives GetCurrentCacheDirectivesWithGetOrSetAsDefault() => CacheDirectives.CurrentScope;
    }
}
