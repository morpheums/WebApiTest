using AutoMapper;
using Ninject.Modules;
using WebApiTest.Data;
using WebApiTest.Data.DbContexts;
using WebApiTest.Data.Definitions;
using WebApiTest.Logic.Configuration;
using WebApiTest.Logic.Definitions;
using WebApiTest.Logic.Services;

namespace WebApiTest.DI
{
    public class WebApiTestNinjectModule : NinjectModule
    {
        public override void Load()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile<AutoMappperProfile>(); });
            Bind<IMapper>().ToConstructor(c => new Mapper(mapperConfiguration)).InSingletonScope();


            Bind<IGenericUoW>().ToConstructor(c => new EntityFrameworkUoW(new FinalDataContext())).InSingletonScope();

            Bind<IUserService>().To<UserService>();
            Bind<IAuthenticationService>().To<AuthenticationService>();
        }
    }
}
