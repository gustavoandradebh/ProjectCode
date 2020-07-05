using AutoMapper;
using ProjectCode.Domain.Interfaces;

namespace ProjectCode.Infraestructure.Mapper.Mapper
{
    public class MapperProvider : IMapperProvider
    {
        private readonly IMapper _mapper;
        public MapperProvider(IMapper mapper)
        {
            _mapper = mapper;
        }
        public TResult Map<TResult>(object source)
        {
            return _mapper.Map<TResult>(source);
        }

        public TResult Map<TSource, TResult>(TSource source)
        {
            return _mapper.Map<TSource, TResult>(source);
        }
    }
}
