namespace TwoGether.Application.Abstractions;

public interface IReadStore
{
    IQueryable<T> Query<T>() where T : class;
}