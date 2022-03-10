using System;
using System.Reflection;

namespace TwoMQTT.Interfaces;

public interface IMQTTGenerator
{
    /// <summary>
    /// Generate the availability topic.
    /// </summary>
    string AvailabilityTopic();

    /// <summary>
    /// Convert a boolean to an appropriate on/off string.
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    string BooleanOnOff(bool val);

    /// <summary>
    /// Convert a boolean to an appropriate home/not_home string.
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    string BooleanHomeNotHome(bool val);

    /// <summary>
    /// Generate the command topic.
    /// </summary>
    string CommandTopic(string slug, string sensor = "");

    /// <summary>
    /// Generate the state topic.
    /// </summary>
    string StateTopic(string slug, string sensor = "");

    /// <summary>
    /// Generate the last updated topic and payload
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    (string, string) DataReceivedTopicPayload(string slug);

    /// <summary>
    /// Generate a messy string.
    /// </summary>
    string Stringify(string prefix, string slug, string sensor, char seperator);

    /// <summary>
    /// Build an appropriate discovery message.
    /// </summary>
    Models.MQTTDiscovery BuildDiscovery(string slug, string sensor, AssemblyName assembly, bool hasCommand);

    /// <summary>
    /// Build an appropriate last updated discovery message
    /// </summary>
    /// <param name="slug"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    (string, string, string, Models.MQTTDiscovery) DataReceivedDiscovery(string slug, AssemblyName assembly, TimeSpan expiration);
}
