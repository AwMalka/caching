using System;
using System.Threading.Tasks;
using PubComp.Caching.AopCaching;

#pragma warning disable 1591

namespace TestHost.WebApi.Service
{
    public class ExampleV2Service
    {
        public const string CacheName = "ScopedNumbers";

        private readonly string dbPath;

        public ExampleV2Service() : this(Startup.DbPath)
        {
        }

        private ExampleV2Service(string dbPath)
        {
            this.dbPath = dbPath;
        }

        public string Get(int id)
        {
            var num = DateTime.Now.Ticks % id;
            return GetInner(num);
        }

        public Task<string> GetAsync(int id)
        {
            var num = DateTime.Now.Ticks % id;
            return GetAsyncInner(num);
        }

        [Cache(CacheName)]
        private string GetInner(long id)
        {
            return FileHelper.ReadLine($"{dbPath}\\{id}.txt");
        }
        
        [Cache(CacheName)]
        private Task<string> GetAsyncInner(long id)
        {
            return FileHelper.ReadLineAsync($"{dbPath}\\{id}.txt");
        }
    }
}