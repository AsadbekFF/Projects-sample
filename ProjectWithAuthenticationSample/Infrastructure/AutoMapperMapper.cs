namespace ProjectWithAuthenticationSample.Infrastructure
{
    public class AutoMapperMapper : IMapper
    {
        private readonly AutoMapper.IMapper _mapper;

        public AutoMapperMapper(AutoMapper.IMapper mapper)
        {
            _mapper = mapper;
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }
    }
}
