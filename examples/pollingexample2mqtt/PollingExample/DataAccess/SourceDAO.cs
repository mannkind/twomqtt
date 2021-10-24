using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PollingExample.Models.Shared;
using PollingExample.Models.Source;
using TwoMQTT.Interfaces;

namespace PollingExample.DataAccess;

public interface ISourceDAO : IPollingSourceDAO<SlugMapping, Response, object, object>
{
}

/// <summary>
/// An class representing a managed way to interact with a source.
/// </summary>
public class SourceDAO : ISourceDAO
{
    /// <summary>
    /// Initializes a new instance of the SourceDAO class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClientFactory"></param>
    /// <returns></returns>
    public SourceDAO(ILogger<SourceDAO> logger, IHttpClientFactory httpClientFactory)
    {
        this.Logger = logger;
        this.Client = httpClientFactory.CreateClient();
    }

    /// <inheritdoc />
    public async Task<Response?> FetchOneAsync(SlugMapping data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await this.FetchAsync(data.Key, cancellationToken);
        }
        catch (Exception e)
        {
            var msg = e switch
            {
                HttpRequestException => "Unable to fetch from the source",
                JsonException => "Unable to deserialize response from the source",
                _ => "Unable to send to the source"
            };
            this.Logger.LogError(msg + "; {exception}", e);
            return null;
        }
    }

    /// <summary>
    /// The logger used internally.
    /// </summary>
    private readonly ILogger<SourceDAO> Logger;

    /// <summary>
    /// The client used to access the source.
    /// </summary>
    private readonly HttpClient Client;

    /// <summary>
    /// Fetch one response from the source
    /// </summary>
    /// <param name="timeTravelId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Response?> FetchAsync(string key,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult((Response?)null);
    }
}
