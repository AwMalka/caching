using Microsoft.AspNetCore.Mvc.Filters;
using PubComp.Caching.Core;
using PubComp.Caching.Core.Directives;
using System;

namespace PubComp.Caching.WebApiExtended.Net.Core
{
    public class CacheableFilterAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => throw new NotImplementedException();

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
            //var contextualDirectives = serviceProvider.GetService(typeof(IScopedContext<CacheDirectives>));
        }
    }
}
