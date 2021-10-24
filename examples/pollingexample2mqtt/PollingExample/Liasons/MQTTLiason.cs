using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PollingExample.Models.Options;
using PollingExample.Models.Shared;
using TwoMQTT;
using TwoMQTT.Interfaces;
using TwoMQTT.Liasons;
using TwoMQTT.Models;
using TwoMQTT.Utils;

namespace PollingExample.Liasons;

/// <summary>
/// An class representing a managed way to interact with MQTT.
/// </summary>
public class MQTTLiason : MQTTLiasonBase<Resource, object, SlugMapping, SharedOpts>, IMQTTLiason<Resource, object>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="generator"></param>
    /// <param name="sharedOpts"></param>
    public MQTTLiason(ILogger<MQTTLiason> logger, IMQTTGenerator generator, IOptions<SharedOpts> sharedOpts) :
        base(logger, generator, sharedOpts)
    {
    }

    /// <inheritdoc />
    public IEnumerable<(string topic, string payload)> MapData(Resource input)
    {
        var results = new List<(string, string)>();
        var slug = this.Questions
            .Where(x => x.Key == input.Key)
            .Select(x => x.Slug)
            .FirstOrDefault() ?? string.Empty;

        this.Logger.LogDebug("Found slug {slug} for incoming data for {key}", slug, input.Key);
        if (string.IsNullOrEmpty(slug))
        {
            this.Logger.LogDebug("Unable to find slug for {key}", input.Key);
            return results;
        }

        this.Logger.LogDebug("Found slug {slug} for incoming data for {key}", slug, input.Key);
        results.AddRange(new[]
            {
                    (this.Generator.StateTopic(slug, nameof(Resource.Key)), input.Key),
                }
        );

        return results;
    }

    /// <inheritdoc />
    public IEnumerable<(string slug, string sensor, string type, MQTTDiscovery discovery)> Discoveries()
    {
        var discoveries = new List<(string, string, string, MQTTDiscovery)>();
        var assembly = Assembly.GetAssembly(typeof(Program))?.GetName() ?? new AssemblyName();
        var mapping = new[]
        {
                new { Sensor = nameof(Resource.Key), Type = Const.SENSOR },
            };

        foreach (var input in this.Questions)
        {
            foreach (var map in mapping)
            {
                this.Logger.LogDebug("Generating discovery for {key} - {sensor}", input.Key, map.Sensor);
                var discovery = this.Generator.BuildDiscovery(input.Slug, map.Sensor, assembly, false);
                discovery = discovery with
                {
                    // Extra attributes placed here
                };

                discoveries.Add((input.Slug, map.Sensor, map.Type, discovery));
            }
        }

        return discoveries;
    }
}
