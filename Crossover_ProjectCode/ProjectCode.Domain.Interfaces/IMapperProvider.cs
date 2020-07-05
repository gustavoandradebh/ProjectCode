namespace ProjectCode.Domain.Interfaces
{
    public interface IMapperProvider
    {
        TResult Map<TResult>(object source);
        TResult Map<TSource, TResult>(TSource source);
    }
}
