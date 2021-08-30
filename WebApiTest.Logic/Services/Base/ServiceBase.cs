using AutoMapper;
using WebApiTest.Data.Definitions;

namespace WebApiTest.Logic.Services.Base
{
    public class ServiceBase
    {
        public readonly IGenericUoW _genericUoW;
        public readonly IMapper _mapper;

        public ServiceBase(IGenericUoW genericUoW, IMapper mapper)
        {
            this._genericUoW = genericUoW;
            this._mapper = mapper;
        }
    }
}
