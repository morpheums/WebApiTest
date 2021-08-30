
using Ninject;
using Ninject.Web.WebApi;
using Ninject.Web.WebApi.Filter;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Validation;
using WebApiTest.DI;
using WebApiTest.WebApi.Utils;

namespace WebApiTest.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var kernel = new StandardKernel();
            kernel.Bind<DefaultModelValidatorProviders>().ToConstant(new DefaultModelValidatorProviders(config.Services.GetServices(typeof(ModelValidatorProvider)).Cast<ModelValidatorProvider>()));
            kernel.Bind<DefaultFilterProviders>().ToSelf().WithConstructorArgument(GlobalConfiguration.Configuration.Services.GetFilterProviders());
            config.DependencyResolver = new NinjectDependencyResolver(kernel);

            kernel.Load(new WebApiTestNinjectModule());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new TokenValidationHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
