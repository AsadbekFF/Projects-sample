namespace ProjectWithAuthenticationSample.Infrastructure
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);
    }
}
