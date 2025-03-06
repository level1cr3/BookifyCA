namespace Bookify.Application.Abstractions.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}

// These 3 methods are all that we need to support most of the caching feature that we require.
// If you want to you can implement more helper methods ex : a get or create method to easily implement cache aside pattern