using Newtonsoft.Json;
using PubComp.Caching.Core.Directives;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace PubComp.Caching.WebApiExtended
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CacheableActionFilterAttribute : FilterAttribute, IActionFilter
    {
        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            if (!(actionContext.ControllerContext?.Controller is ICacheableApiController cacheableController))
                throw new Exception($"controller does not implement {nameof(ICacheableApiController)}");

            if (cacheableController.ScopedContextCacheDirectives == null)
                throw new ArgumentNullException(nameof(cacheableController.ScopedContextCacheDirectives));

            cacheableController.CacheDirectivesOutcome = new CacheDirectivesOutcome { MethodTaken = CacheMethodTaken.None };

            var definedCacheDirectives = GetCacheDirectives(actionContext);
            using (cacheableController.ScopedContextCacheDirectives.CreateNewScope(definedCacheDirectives))
            {
                var response = await continuation().ConfigureAwait(false);

                if (cacheableController.CacheDirectivesOutcome != null)
                {
                    var cacheDirectivesOutcomeJson = JsonConvert.SerializeObject(cacheableController.CacheDirectivesOutcome);
                    response.Headers.Add(CacheDirectivesOutcome.HeadersKey, cacheDirectivesOutcomeJson);
                }
                return response;
            }
        }

        private CacheDirectives GetCacheDirectives(HttpActionContext actionContext)
        {
            var cacheDirectivesJson = actionContext.Request.Headers
                .SingleOrDefault(x => x.Key == CacheDirectives.HeadersKey)
                .Value?.First();

            if (string.IsNullOrEmpty(cacheDirectivesJson))
                return new CacheDirectives { Method = CacheMethod.None };

            return JsonConvert.DeserializeObject<CacheDirectives>(cacheDirectivesJson);
        }
    }
}
