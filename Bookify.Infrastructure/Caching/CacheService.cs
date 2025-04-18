﻿using Bookify.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Buffers;
using System.Text.Json;

namespace Bookify.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await _cache.GetAsync(key, cancellationToken);

        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);

        await _cache.SetAsync(key, bytes, CacheOptions.Create(expiration) ,cancellationToken);
    }

    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)!; // here we could also use the newtonsoft json then we could use the string based api instead of working with bytes
    }

    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();

        using var writer = new Utf8JsonWriter(buffer);

        JsonSerializer.Serialize(writer,value);

        return buffer.WrittenSpan.ToArray();
    }
}


// we will be using redis under the hood as the actual persistance store for the cache values.
// we will interface with redis is using the built-in abstraction in asp.net core called IDistributed cache.

// This abstraction IDistributedCache _cache; allow us to get,set or remove the cache value from the underlying persistance store. It's default implementation is
// memory cache. But there is also an implementation that we can use to talk to the redis.