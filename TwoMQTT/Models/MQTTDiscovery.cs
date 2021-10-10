using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace TwoMQTT.Models
{
    /// <summary>
    /// An class representing fields in a MQTT Discovery payload.
    /// </summary>
    public record MQTTDiscovery
    {
        [JsonProperty("aux_command_topic")]
        [DefaultValue("")]
        public string AuxCommandTopic { get; init; } = string.Empty;

        [JsonProperty("aux_state_template")]
        [DefaultValue("")]
        public string AuxStateTemplate { get; init; } = string.Empty;

        [JsonProperty("aux_state_topic")]
        [DefaultValue("")]
        public string AuxStateTopic { get; init; } = string.Empty;

        [JsonProperty("availability_topic")]
        [DefaultValue("")]
        public string AvailabilityTopic { get; init; } = string.Empty;

        [JsonProperty("away_mode_command_topic")]
        [DefaultValue("")]
        public string AwayModeCommandTopic { get; init; } = string.Empty;

        [JsonProperty("away_mode_state_template")]
        [DefaultValue("")]
        public string AwayModeStateTemplate { get; init; } = string.Empty;

        [JsonProperty("away_mode_state_topic")]
        [DefaultValue("")]
        public string AwayModeStateTopic { get; init; } = string.Empty;

        [JsonProperty("blue_template")]
        [DefaultValue("")]
        public string BlueTemplate { get; init; } = string.Empty;

        [JsonProperty("brightness_command_topic")]
        [DefaultValue("")]
        public string BrightnessCommandTopic { get; init; } = string.Empty;

        [JsonProperty("brightness_scale")]
        [DefaultValue("")]
        public string BrightnessScale { get; init; } = string.Empty;

        [JsonProperty("brightness_state_topic")]
        [DefaultValue("")]
        public string BrightnessStateTopic { get; init; } = string.Empty;

        [JsonProperty("brightness_template")]
        [DefaultValue("")]
        public string BrightnessTemplate { get; init; } = string.Empty;

        [JsonProperty("brightness_value_template")]
        [DefaultValue("")]
        public string BrightnessValueTemplate { get; init; } = string.Empty;

        [JsonProperty("battery_level_topic")]
        [DefaultValue("")]
        public string BatteryLevelTopic { get; init; } = string.Empty;

        [JsonProperty("battery_level_template")]
        [DefaultValue("")]
        public string BatteryLevelTemplate { get; init; } = string.Empty;

        [JsonProperty("charging_topic")]
        [DefaultValue("")]
        public string ChargingTopic { get; init; } = string.Empty;

        [JsonProperty("charging_template")]
        [DefaultValue("")]
        public string ChargingTemplate { get; init; } = string.Empty;

        [JsonProperty("color_temp_command_topic")]
        [DefaultValue("")]
        public string ColorTempCommandTopic { get; init; } = string.Empty;

        [JsonProperty("color_temp_state_topic")]
        [DefaultValue("")]
        public string ColorTempStateTopic { get; init; } = string.Empty;

        [JsonProperty("color_temp_value_template")]
        [DefaultValue("")]
        public string ColorTempValueTemplate { get; init; } = string.Empty;

        [JsonProperty("cleaning_topic")]
        [DefaultValue("")]
        public string CleaningTopic { get; init; } = string.Empty;

        [JsonProperty("cleaning_template")]
        [DefaultValue("")]
        public string CleaningTemplate { get; init; } = string.Empty;

        [JsonProperty("command_off_template")]
        [DefaultValue("")]
        public string CommandOffTemplate { get; init; } = string.Empty;

        [JsonProperty("command_on_template")]
        [DefaultValue("")]
        public string CommandOnTemplate { get; init; } = string.Empty;

        [JsonProperty("command_topic")]
        [DefaultValue("")]
        public string CommandTopic { get; init; } = string.Empty;

        [JsonProperty("current_temperature_topic")]
        [DefaultValue("")]
        public string CurrentTemperatureTopic { get; init; } = string.Empty;

        [JsonProperty("current_temperature_template")]
        [DefaultValue("")]
        public string CurrentTemperatureTemplate { get; init; } = string.Empty;

        [JsonProperty("device")]
        public DiscoveryDevice Device { get; init; } = new DiscoveryDevice();

        [JsonProperty("device_class")]
        [DefaultValue("")]
        public string DeviceClass { get; init; } = string.Empty;

        [JsonProperty("docked_topic")]
        [DefaultValue("")]
        public string DockedTopic { get; init; } = string.Empty;

        [JsonProperty("docked_template")]
        [DefaultValue("")]
        public string DockedTemplate { get; init; } = string.Empty;

        [JsonProperty("error_topic")]
        [DefaultValue("")]
        public string ErrorTopic { get; init; } = string.Empty;

        [JsonProperty("error_template")]
        [DefaultValue("")]
        public string ErrorTemplate { get; init; } = string.Empty;

        [JsonProperty("fan_speed_topic")]
        [DefaultValue("")]
        public string FanSpeedTopic { get; init; } = string.Empty;

        [JsonProperty("fan_speed_template")]
        [DefaultValue("")]
        public string FanSpeedTemplate { get; init; } = string.Empty;

        [JsonProperty("fan_speed_list")]
        [DefaultValue("")]
        public string FanSpeedList { get; init; } = string.Empty;

        [JsonProperty("effect_command_topic")]
        [DefaultValue("")]
        public string EffectCommandTopic { get; init; } = string.Empty;

        [JsonProperty("effect_list")]
        [DefaultValue("")]
        public string EffectList { get; init; } = string.Empty;

        [JsonProperty("effect_state_topic")]
        [DefaultValue("")]
        public string EffectStateTopic { get; init; } = string.Empty;

        [JsonProperty("effect_template")]
        [DefaultValue("")]
        public string EffectTemplate { get; init; } = string.Empty;

        [JsonProperty("effect_value_template")]
        [DefaultValue("")]
        public string EffectValueTemplate { get; init; } = string.Empty;

        [JsonProperty("expire_after")]
        [DefaultValue("")]
        public string ExpireAfter { get; init; } = string.Empty;

        [JsonProperty("fan_mode_command_topic")]
        [DefaultValue("")]
        public string FanModeCommandTopic { get; init; } = string.Empty;

        [JsonProperty("fan_mode_state_template")]
        [DefaultValue("")]
        public string FanModeStateTemplate { get; init; } = string.Empty;

        [JsonProperty("fan_mode_state_topic")]
        [DefaultValue("")]
        public string FanModeStateTopic { get; init; } = string.Empty;

        [JsonProperty("force_update")]
        [DefaultValue("")]
        public string ForceUpdate { get; init; } = string.Empty;

        [JsonProperty("green_template")]
        [DefaultValue("")]
        public string GreenTemplate { get; init; } = string.Empty;

        [JsonProperty("hold_command_topic")]
        [DefaultValue("")]
        public string HoldCommandTopic { get; init; } = string.Empty;

        [JsonProperty("hold_state_template")]
        [DefaultValue("")]
        public string HoldStateTemplate { get; init; } = string.Empty;

        [JsonProperty("hold_state_topic")]
        [DefaultValue("")]
        public string HoldStateTopic { get; init; } = string.Empty;

        [JsonProperty("icon")]
        [DefaultValue("")]
        public string Icon { get; init; } = string.Empty;

        [JsonProperty("initial")]
        [DefaultValue("")]
        public string Initial { get; init; } = string.Empty;

        [JsonProperty("json_attributes")]
        [DefaultValue("")]
        public string JsonAttributes { get; init; } = string.Empty;

        [JsonProperty("json_attributes_topic")]
        [DefaultValue("")]
        public string JsonAttributesTopic { get; init; } = string.Empty;

        [JsonProperty("max_temp")]
        [DefaultValue("")]
        public string MaxTemp { get; init; } = string.Empty;

        [JsonProperty("min_temp")]
        [DefaultValue("")]
        public string MinTemp { get; init; } = string.Empty;

        [JsonProperty("mode_command_topic")]
        [DefaultValue("")]
        public string ModeCommandTopic { get; init; } = string.Empty;

        [JsonProperty("mode_state_template")]
        [DefaultValue("")]
        public string ModeStateTemplate { get; init; } = string.Empty;

        [JsonProperty("mode_state_topic")]
        [DefaultValue("")]
        public string ModeStateTopic { get; init; } = string.Empty;

        [JsonProperty("name")]
        [DefaultValue("")]
        public string Name { get; init; } = string.Empty;

        [JsonProperty("on_command_type")]
        [DefaultValue("")]
        public string OnCommandType { get; init; } = string.Empty;

        [JsonProperty("optimistic")]
        [DefaultValue("")]
        public string Optimistic { get; init; } = string.Empty;

        [JsonProperty("oscillation_command_topic")]
        [DefaultValue("")]
        public string OscillationCommandTopic { get; init; } = string.Empty;

        [JsonProperty("oscillation_state_topic")]
        [DefaultValue("")]
        public string OscillationStateTopic { get; init; } = string.Empty;

        [JsonProperty("oscillation_value_template")]
        [DefaultValue("")]
        public string OscillationValueTemplate { get; init; } = string.Empty;

        [JsonProperty("payload_arm_away")]
        [DefaultValue("")]
        public string PayloadArmAway { get; init; } = string.Empty;

        [JsonProperty("payload_arm_home")]
        [DefaultValue("")]
        public string PayloadArmHome { get; init; } = string.Empty;

        [JsonProperty("payload_available")]
        [DefaultValue("")]
        public string PayloadAvailable { get; init; } = string.Empty;

        [JsonProperty("payload_close")]
        [DefaultValue("")]
        public string PayloadClose { get; init; } = string.Empty;

        [JsonProperty("payload_disarm")]
        [DefaultValue("")]
        public string PayloadDisarm { get; init; } = string.Empty;
        
        [JsonProperty("payload_home")]
        [DefaultValue("")]
        public string PayloadHome { get; init; } = string.Empty;

        [JsonProperty("payload_high_speed")]
        [DefaultValue("")]
        public string PayloadHighSpeed { get; init; } = string.Empty;

        [JsonProperty("payload_lock")]
        [DefaultValue("")]
        public string PayloadLock { get; init; } = string.Empty;

        [JsonProperty("payload_low_speed")]
        [DefaultValue("")]
        public string PayloadLowSpeed { get; init; } = string.Empty;

        [JsonProperty("payload_medium_speed")]
        [DefaultValue("")]
        public string PayloadMediumSpeed { get; init; } = string.Empty;

        [JsonProperty("payload_not_available")]
        [DefaultValue("")]
        public string PayloadNotAvailable { get; init; } = string.Empty;

        [JsonProperty("payload_not_home")]
        [DefaultValue("")]
        public string PayloadNotHome { get; init; } = string.Empty;

        [JsonProperty("payload_off")]
        [DefaultValue("")]
        public string PayloadOff { get; init; } = string.Empty;

        [JsonProperty("payload_on")]
        [DefaultValue("")]
        public string PayloadOn { get; init; } = string.Empty;

        [JsonProperty("payload_open")]
        [DefaultValue("")]
        public string PayloadOpen { get; init; } = string.Empty;

        [JsonProperty("payload_oscillation_off")]
        [DefaultValue("")]
        public string PayloadOscillationOff { get; init; } = string.Empty;

        [JsonProperty("payload_oscillation_on")]
        [DefaultValue("")]
        public string PayloadOscillationOn { get; init; } = string.Empty;

        [JsonProperty("payload_stop")]
        [DefaultValue("")]
        public string PayloadStop { get; init; } = string.Empty;

        [JsonProperty("payload_unlock")]
        [DefaultValue("")]
        public string PayloadUnlock { get; init; } = string.Empty;

        [JsonProperty("power_command_topic")]
        [DefaultValue("")]
        public string PowerCommandTopic { get; init; } = string.Empty;

        [JsonProperty("red_template")]
        [DefaultValue("")]
        public string RedTemplate { get; init; } = string.Empty;

        [JsonProperty("retain")]
        [DefaultValue("")]
        public string Retain { get; init; } = string.Empty;

        [JsonProperty("rgb_command_template")]
        [DefaultValue("")]
        public string RgbCommandTemplate { get; init; } = string.Empty;

        [JsonProperty("rgb_command_topic")]
        [DefaultValue("")]
        public string RgbCommandTopic { get; init; } = string.Empty;

        [JsonProperty("rgb_state_topic")]
        [DefaultValue("")]
        public string RgbStateTopic { get; init; } = string.Empty;

        [JsonProperty("rgb_value_template")]
        [DefaultValue("")]
        public string RgbValueTemplate { get; init; } = string.Empty;

        [JsonProperty("send_command_topic")]
        [DefaultValue("")]
        public string SendCommandTopic { get; init; } = string.Empty;

        [JsonProperty("send_if_off")]
        [DefaultValue("")]
        public string SendIfOff { get; init; } = string.Empty;

        [JsonProperty("set_position_template")]
        [DefaultValue("")]
        public string SetPositionTemplate { get; init; } = string.Empty;

        [JsonProperty("set_position_topic")]
        [DefaultValue("")]
        public string SetPositionTopic { get; init; } = string.Empty;

        [JsonProperty("position_topic")]
        [DefaultValue("")]
        public string PositionTopic { get; init; } = string.Empty;

        [JsonProperty("source_type")]
        [DefaultValue("")]
        public string SourceType { get; init; } = string.Empty;

        [JsonProperty("speed_command_topic")]
        [DefaultValue("")]
        public string SpeedCommandTopic { get; init; } = string.Empty;

        [JsonProperty("speed_state_topic")]
        [DefaultValue("")]
        public string SpeedStateTopic { get; init; } = string.Empty;

        [JsonProperty("speed_value_template")]
        [DefaultValue("")]
        public string SpeedValueTemplate { get; init; } = string.Empty;

        [JsonProperty("speeds")]
        [DefaultValue("")]
        public string Speeds { get; init; } = string.Empty;

        [JsonProperty("state_closed")]
        [DefaultValue("")]
        public string StateClosed { get; init; } = string.Empty;

        [JsonProperty("state_off")]
        [DefaultValue("")]
        public string StateOff { get; init; } = string.Empty;

        [JsonProperty("state_on")]
        [DefaultValue("")]
        public string StateOn { get; init; } = string.Empty;

        [JsonProperty("state_open")]
        [DefaultValue("")]
        public string StateOpen { get; init; } = string.Empty;

        [JsonProperty("state_topic")]
        [DefaultValue("")]
        public string StateTopic { get; init; } = string.Empty;

        [JsonProperty("state_template")]
        [DefaultValue("")]
        public string StateTemplate { get; init; } = string.Empty;

        [JsonProperty("state_value_template")]
        [DefaultValue("")]
        public string StateValueTemplate { get; init; } = string.Empty;

        [JsonProperty("supported_features")]
        [DefaultValue("")]
        public string SupportedFeatures { get; init; } = string.Empty;

        [JsonProperty("swing_mode_command_topic")]
        [DefaultValue("")]
        public string SwingModeCommandTopic { get; init; } = string.Empty;

        [JsonProperty("swing_mode_state_template")]
        [DefaultValue("")]
        public string SwingModeStateTemplate { get; init; } = string.Empty;

        [JsonProperty("swing_mode_state_topic")]
        [DefaultValue("")]
        public string SwingModeStateTopic { get; init; } = string.Empty;

        [JsonProperty("temperature_command_topic")]
        [DefaultValue("")]
        public string TemperatureCommandTopic { get; init; } = string.Empty;

        [JsonProperty("temperature_state_template")]
        [DefaultValue("")]
        public string TemperatureStateTemplate { get; init; } = string.Empty;

        [JsonProperty("temperature_state_topic")]
        [DefaultValue("")]
        public string TemperatureStateTopic { get; init; } = string.Empty;

        [JsonProperty("tilt_closed_value")]
        [DefaultValue("")]
        public string TiltClosedValue { get; init; } = string.Empty;

        [JsonProperty("tilt_command_topic")]
        [DefaultValue("")]
        public string TiltCommandTopic { get; init; } = string.Empty;

        [JsonProperty("tilt_invert_state")]
        [DefaultValue("")]
        public string TiltInvertState { get; init; } = string.Empty;

        [JsonProperty("tilt_max")]
        [DefaultValue("")]
        public string TiltMax { get; init; } = string.Empty;

        [JsonProperty("tilt_min")]
        [DefaultValue("")]
        public string TiltMin { get; init; } = string.Empty;

        [JsonProperty("tilt_opened_value")]
        [DefaultValue("")]
        public string TiltOpenedValue { get; init; } = string.Empty;

        [JsonProperty("tilt_status_optimistic")]
        [DefaultValue("")]
        public string TiltStatusOptimistic { get; init; } = string.Empty;

        [JsonProperty("tilt_status_topic")]
        [DefaultValue("")]
        public string TiltStatusTopic { get; init; } = string.Empty;

        [JsonProperty("topic")]
        [DefaultValue("")]
        public string Topic { get; init; } = string.Empty;

        [JsonProperty("unique_id")]
        [DefaultValue("")]
        public string UniqueId { get; init; } = string.Empty;

        [JsonProperty("unit_of_measurement")]
        [DefaultValue("")]
        public string UnitOfMeasurement { get; init; } = string.Empty;

        [JsonProperty("value_template")]
        [DefaultValue("")]
        public string ValueTemplate { get; init; } = string.Empty;

        [JsonProperty("white_value_command_topic")]
        [DefaultValue("")]
        public string WhiteValueCommandTopic { get; init; } = string.Empty;

        [JsonProperty("white_value_scale")]
        [DefaultValue("")]
        public string WhiteValueScale { get; init; } = string.Empty;

        [JsonProperty("white_value_state_topic")]
        [DefaultValue("")]
        public string WhiteValueStateTopic { get; init; } = string.Empty;

        [JsonProperty("white_value_template")]
        [DefaultValue("")]
        public string WhiteValueTemplate { get; init; } = string.Empty;

        [JsonProperty("xy_command_topic")]
        [DefaultValue("")]
        public string XyCommandTopic { get; init; } = string.Empty;

        [JsonProperty("xy_state_topic")]
        [DefaultValue("")]
        public string XyStateTopic { get; init; } = string.Empty;

        [JsonProperty("xy_value_template")]
        [DefaultValue("")]
        public string XyValueTemplate { get; init; } = string.Empty;

        /// <summary>
        /// An class representing fields in a MQTT Discovery Device payload.
        /// </summary>
        public class DiscoveryDevice
        {
            [JsonProperty("identifiers")]
            [DefaultValue("")]
            public List<string> Identifiers { get; init; } = new List<string>();

            [JsonProperty("connections")]
            [DefaultValue("")]
            public List<string> Connections { get; init; } = new List<string>();

            [JsonProperty("manufacturer")]
            [DefaultValue("")]
            public string Manufacturer { get; init; } = string.Empty;

            [JsonProperty("model")]
            [DefaultValue("")]
            public string Model { get; init; } = string.Empty;

            [JsonProperty("name")]
            [DefaultValue("")]
            public string Name { get; init; } = string.Empty;

            [JsonProperty("sw_version")]
            [DefaultValue("")]
            public string SWVersion { get; init; } = string.Empty;
        }
    }
}
