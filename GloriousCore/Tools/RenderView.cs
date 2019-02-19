using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GloriousCore.Tools
{
    public interface IRenderView
    {
        Task<byte[]> RenderAsync<TModel>(
            string viewName,
            TModel model,
            CancellationToken cancellationToken = default);
    }

    public class RenderView : IRenderView
    {
        private readonly IActionResultExecutor<ViewResult> _executor;
        private readonly IModelMetadataProvider _metadataProvider;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public RenderView(
            IActionResultExecutor<ViewResult> executor,
            IModelMetadataProvider metadataProvider,
            ITempDataDictionaryFactory tempDataDictionaryFactory,
            IServiceScopeFactory scopeFactory)
        {
            _executor = executor;
            _metadataProvider = metadataProvider;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _scopeFactory = scopeFactory;
        }

        public async Task<byte[]> RenderAsync<TModel>(
            string viewName,
            TModel model,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(viewName))
                throw new ArgumentNullException(nameof(viewName));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            using (var scope = _scopeFactory.CreateScope())
            using (var resultStream = new MemoryStream())
            {
                var httpContextFeatures = new FeatureCollection();
                httpContextFeatures.Set<IHttpRequestFeature>(new HttpRequestFeature());
                httpContextFeatures.Set<IHttpResponseFeature>(new HttpResponseFeature
                {
                    Body = resultStream
                });

                var httpContext = new DefaultHttpContext(httpContextFeatures)
                {
                    RequestAborted = cancellationToken,
                    RequestServices = scope.ServiceProvider
                };

                var modelState = new ModelStateDictionary();
                var tempDataDictionary = _tempDataDictionaryFactory.GetTempData(httpContext);
                var viewData = new ViewDataDictionary<TModel>(_metadataProvider, modelState)
                {
                    Model = model
                };

                var viewResult = new ViewResult
                {
                    ViewName = viewName,
                    ViewData = viewData,
                    TempData = tempDataDictionary
                };

                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                await _executor.ExecuteAsync(actionContext, viewResult);
                resultStream.Seek(0L, SeekOrigin.Begin);
                return resultStream.ToArray();
            }
        }
    }
}
