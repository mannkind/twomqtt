using System.ComponentModel;
using Newtonsoft.Json;

namespace TwoMQTT.Core.Models
{
    public class MQTTDiscovery
    {
        [JsonProperty("aux_command_topic")]
        [DefaultValue("")]
        public string AuxCommandTopic { get; set; } = string.Empty;

        [JsonProperty("aux_state_template")]
        [DefaultValue("")]
        public string AuxStateTemplate { get; set; } = string.Empty;

        [JsonProperty("aux_state_topic")]
        [DefaultValue("")]
        public string AuxStateTopic { get; set; } = string.Empty;

        [JsonProperty("availability_topic")]
        [DefaultValue("")]
        public string AvailabilityTopic { get; set; } = string.Empty;

        [JsonProperty("away_mode_command_topic")]
        [DefaultValue("")]
        public string AwayModeCommandTopic { get; set; } = string.Empty;

        [JsonProperty("away_mode_state_template")]
        [DefaultValue("")]
        public string AwayModeStateTemplate { get; set; } = string.Empty;

        [JsonProperty("away_mode_state_topic")]
        [DefaultValue("")]
        public string AwayModeStateTopic { get; set; } = string.Empty;

        [JsonProperty("blue_template")]
        [DefaultValue("")]
        public string BlueTemplate { get; set; } = string.Empty;

        [JsonProperty("brightness_command_topic")]
        [DefaultValue("")]
        public string BrightnessCommandTopic { get; set; } = string.Empty;

        [JsonProperty("brightness_scale")]
        [DefaultValue("")]
        public string BrightnessScale { get; set; } = string.Empty;

        [JsonProperty("brightness_state_topic")]
        [DefaultValue("")]
        public string BrightnessStateTopic { get; set; } = string.Empty;

        [JsonProperty("brightness_template")]
        [DefaultValue("")]
        public string BrightnessTemplate { get; set; } = string.Empty;

        [JsonProperty("brightness_value_template")]
        [DefaultValue("")]
        public string BrightnessValueTemplate { get; set; } = string.Empty;

        [JsonProperty("battery_level_topic")]
        [DefaultValue("")]
        public string BatteryLevelTopic { get; set; } = string.Empty;

        [JsonProperty("battery_level_template")]
        [DefaultValue("")]
        public string BatteryLevelTemplate { get; set; } = string.Empty;

        [JsonProperty("charging_topic")]
        [DefaultValue("")]
        public string ChargingTopic { get; set; } = string.Empty;

        [JsonProperty("charging_template")]
        [DefaultValue("")]
        public string ChargingTemplate { get; set; } = string.Empty;

        [JsonProperty("color_temp_command_topic")]
        [DefaultValue("")]
        public string ColorTempCommandTopic { get; set; } = string.Empty;

        [JsonProperty("color_temp_state_topic")]
        [DefaultValue("")]
        public string ColorTempStateTopic { get; set; } = string.Empty;

        [JsonProperty("color_temp_value_template")]
        [DefaultValue("")]
        public string ColorTempValueTemplate { get; set; } = string.Empty;

        [JsonProperty("cleaning_topic")]
        [DefaultValue("")]
        public string CleaningTopic { get; set; } = string.Empty;

        [JsonProperty("cleaning_template")]
        [DefaultValue("")]
        public string CleaningTemplate { get; set; } = string.Empty;

        [JsonProperty("command_off_template")]
        [DefaultValue("")]
        public string CommandOffTemplate { get; set; } = string.Empty;

        [JsonProperty("command_on_template")]
        [DefaultValue("")]
        public string CommandOnTemplate { get; set; } = string.Empty;

        [JsonProperty("command_topic")]
        [DefaultValue("")]
        public string CommandTopic { get; set; } = string.Empty;

        [JsonProperty("current_temperature_topic")]
        [DefaultValue("")]
        public string CurrentTemperatureTopic { get; set; } = string.Empty;

        [JsonProperty("current_temperature_template")]
        [DefaultValue("")]
        public string CurrentTemperatureTemplate { get; set; } = string.Empty;

        [JsonProperty("device")]
        public MQTTDiscoveryDevice Device { get; set; } = new MQTTDiscoveryDevice();

        [JsonProperty("device_class")]
        [DefaultValue("")]
        public string DeviceClass { get; set; } = string.Empty;

        [JsonProperty("docked_topic")]
        [DefaultValue("")]
        public string DockedTopic { get; set; } = string.Empty;

        [JsonProperty("docked_template")]
        [DefaultValue("")]
        public string DockedTemplate { get; set; } = string.Empty;

        [JsonProperty("error_topic")]
        [DefaultValue("")]
        public string ErrorTopic { get; set; } = string.Empty;

        [JsonProperty("error_template")]
        [DefaultValue("")]
        public string ErrorTemplate { get; set; } = string.Empty;

        [JsonProperty("fan_speed_topic")]
        [DefaultValue("")]
        public string FanSpeedTopic { get; set; } = string.Empty;

        [JsonProperty("fan_speed_template")]
        [DefaultValue("")]
        public string FanSpeedTemplate { get; set; } = string.Empty;

        [JsonProperty("fan_speed_list")]
        [DefaultValue("")]
        public string FanSpeedList { get; set; } = string.Empty;

        [JsonProperty("effect_command_topic")]
        [DefaultValue("")]
        public string EffectCommandTopic { get; set; } = string.Empty;

        [JsonProperty("effect_list")]
        [DefaultValue("")]
        public string EffectList { get; set; } = string.Empty;

        [JsonProperty("effect_state_topic")]
        [DefaultValue("")]
        public string EffectStateTopic { get; set; } = string.Empty;

        [JsonProperty("effect_template")]
        [DefaultValue("")]
        public string EffectTemplate { get; set; } = string.Empty;

        [JsonProperty("effect_value_template")]
        [DefaultValue("")]
        public string EffectValueTemplate { get; set; } = string.Empty;

        [JsonProperty("expire_after")]
        [DefaultValue("")]
        public string ExpireAfter { get; set; } = string.Empty;

        [JsonProperty("fan_mode_command_topic")]
        [DefaultValue("")]
        public string FanModeCommandTopic { get; set; } = string.Empty;

        [JsonProperty("fan_mode_state_template")]
        [DefaultValue("")]
        public string FanModeStateTemplate { get; set; } = string.Empty;

        [JsonProperty("fan_mode_state_topic")]
        [DefaultValue("")]
        public string FanModeStateTopic { get; set; } = string.Empty;

        [JsonProperty("force_update")]
        [DefaultValue("")]
        public string ForceUpdate { get; set; } = string.Empty;

        [JsonProperty("green_template")]
        [DefaultValue("")]
        public string GreenTemplate { get; set; } = string.Empty;

        [JsonProperty("hold_command_topic")]
        [DefaultValue("")]
        public string HoldCommandTopic { get; set; } = string.Empty;

        [JsonProperty("hold_state_template")]
        [DefaultValue("")]
        public string HoldStateTemplate { get; set; } = string.Empty;

        [JsonProperty("hold_state_topic")]
        [DefaultValue("")]
        public string HoldStateTopic { get; set; } = string.Empty;

        [JsonProperty("icon")]
        [DefaultValue("")]
        public string Icon { get; set; } = string.Empty;

        [JsonProperty("initial")]
        [DefaultValue("")]
        public string Initial { get; set; } = string.Empty;

        [JsonProperty("json_attributes")]
        [DefaultValue("")]
        public string JsonAttributes { get; set; } = string.Empty;

        [JsonProperty("json_attributes_topic")]
        [DefaultValue("")]
        public string JsonAttributesTopic { get; set; } = string.Empty;

        [JsonProperty("max_temp")]
        [DefaultValue("")]
        public string MaxTemp { get; set; } = string.Empty;

        [JsonProperty("min_temp")]
        [DefaultValue("")]
        public string MinTemp { get; set; } = string.Empty;

        [JsonProperty("mode_command_topic")]
        [DefaultValue("")]
        public string ModeCommandTopic { get; set; } = string.Empty;

        [JsonProperty("mode_state_template")]
        [DefaultValue("")]
        public string ModeStateTemplate { get; set; } = string.Empty;

        [JsonProperty("mode_state_topic")]
        [DefaultValue("")]
        public string ModeStateTopic { get; set; } = string.Empty;

        [JsonProperty("name")]
        [DefaultValue("")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("on_command_type")]
        [DefaultValue("")]
        public string OnCommandType { get; set; } = string.Empty;

        [JsonProperty("optimistic")]
        [DefaultValue("")]
        public string Optimistic { get; set; } = string.Empty;

        [JsonProperty("oscillation_command_topic")]
        [DefaultValue("")]
        public string OscillationCommandTopic { get; set; } = string.Empty;

        [JsonProperty("oscillation_state_topic")]
        [DefaultValue("")]
        public string OscillationStateTopic { get; set; } = string.Empty;

        [JsonProperty("oscillation_value_template")]
        [DefaultValue("")]
        public string OscillationValueTemplate { get; set; } = string.Empty;

        [JsonProperty("payload_arm_away")]
        [DefaultValue("")]
        public string PayloadArmAway { get; set; } = string.Empty;

        [JsonProperty("payload_arm_home")]
        [DefaultValue("")]
        public string PayloadArmHome { get; set; } = string.Empty;

        [JsonProperty("payload_available")]
        [DefaultValue("")]
        public string PayloadAvailable { get; set; } = string.Empty;

        [JsonProperty("payload_close")]
        [DefaultValue("")]
        public string PayloadClose { get; set; } = string.Empty;

        [JsonProperty("payload_disarm")]
        [DefaultValue("")]
        public string PayloadDisarm { get; set; } = string.Empty;

        [JsonProperty("payload_high_speed")]
        [DefaultValue("")]
        public string PayloadHighSpeed { get; set; } = string.Empty;

        [JsonProperty("payload_lock")]
        [DefaultValue("")]
        public string PayloadLock { get; set; } = string.Empty;

        [JsonProperty("payload_low_speed")]
        [DefaultValue("")]
        public string PayloadLowSpeed { get; set; } = string.Empty;

        [JsonProperty("payload_medium_speed")]
        [DefaultValue("")]
        public string PayloadMediumSpeed { get; set; } = string.Empty;

        [JsonProperty("payload_not_available")]
        [DefaultValue("")]
        public string PayloadNotAvailable { get; set; } = string.Empty;

        [JsonProperty("payload_off")]
        [DefaultValue("")]
        public string PayloadOff { get; set; } = string.Empty;

        [JsonProperty("payload_on")]
        [DefaultValue("")]
        public string PayloadOn { get; set; } = string.Empty;

        [JsonProperty("payload_open")]
        [DefaultValue("")]
        public string PayloadOpen { get; set; } = string.Empty;

        [JsonProperty("payload_oscillation_off")]
        [DefaultValue("")]
        public string PayloadOscillationOff { get; set; } = string.Empty;

        [JsonProperty("payload_oscillation_on")]
        [DefaultValue("")]
        public string PayloadOscillationOn { get; set; } = string.Empty;

        [JsonProperty("payload_stop")]
        [DefaultValue("")]
        public string PayloadStop { get; set; } = string.Empty;

        [JsonProperty("payload_unlock")]
        [DefaultValue("")]
        public string PayloadUnlock { get; set; } = string.Empty;

        [JsonProperty("power_command_topic")]
        [DefaultValue("")]
        public string PowerCommandTopic { get; set; } = string.Empty;

        [JsonProperty("red_template")]
        [DefaultValue("")]
        public string RedTemplate { get; set; } = string.Empty;

        [JsonProperty("retain")]
        [DefaultValue("")]
        public string Retain { get; set; } = string.Empty;

        [JsonProperty("rgb_command_template")]
        [DefaultValue("")]
        public string RgbCommandTemplate { get; set; } = string.Empty;

        [JsonProperty("rgb_command_topic")]
        [DefaultValue("")]
        public string RgbCommandTopic { get; set; } = string.Empty;

        [JsonProperty("rgb_state_topic")]
        [DefaultValue("")]
        public string RgbStateTopic { get; set; } = string.Empty;

        [JsonProperty("rgb_value_template")]
        [DefaultValue("")]
        public string RgbValueTemplate { get; set; } = string.Empty;

        [JsonProperty("send_command_topic")]
        [DefaultValue("")]
        public string SendCommandTopic { get; set; } = string.Empty;

        [JsonProperty("send_if_off")]
        [DefaultValue("")]
        public string SendIfOff { get; set; } = string.Empty;

        [JsonProperty("set_position_template")]
        [DefaultValue("")]
        public string SetPositionTemplate { get; set; } = string.Empty;

        [JsonProperty("set_position_topic")]
        [DefaultValue("")]
        public string SetPositionTopic { get; set; } = string.Empty;

        [JsonProperty("position_topic")]
        [DefaultValue("")]
        public string PositionTopic { get; set; } = string.Empty;

        [JsonProperty("speed_command_topic")]
        [DefaultValue("")]
        public string SpeedCommandTopic { get; set; } = string.Empty;

        [JsonProperty("speed_state_topic")]
        [DefaultValue("")]
        public string SpeedStateTopic { get; set; } = string.Empty;

        [JsonProperty("speed_value_template")]
        [DefaultValue("")]
        public string SpeedValueTemplate { get; set; } = string.Empty;

        [JsonProperty("speeds")]
        [DefaultValue("")]
        public string Speeds { get; set; } = string.Empty;

        [JsonProperty("state_closed")]
        [DefaultValue("")]
        public string StateClosed { get; set; } = string.Empty;

        [JsonProperty("state_off")]
        [DefaultValue("")]
        public string StateOff { get; set; } = string.Empty;

        [JsonProperty("state_on")]
        [DefaultValue("")]
        public string StateOn { get; set; } = string.Empty;

        [JsonProperty("state_open")]
        [DefaultValue("")]
        public string StateOpen { get; set; } = string.Empty;

        [JsonProperty("state_topic")]
        [DefaultValue("")]
        public string StateTopic { get; set; } = string.Empty;

        [JsonProperty("state_template")]
        [DefaultValue("")]
        public string StateTemplate { get; set; } = string.Empty;

        [JsonProperty("state_value_template")]
        [DefaultValue("")]
        public string StateValueTemplate { get; set; } = string.Empty;

        [JsonProperty("supported_features")]
        [DefaultValue("")]
        public string SupportedFeatures { get; set; } = string.Empty;

        [JsonProperty("swing_mode_command_topic")]
        [DefaultValue("")]
        public string SwingModeCommandTopic { get; set; } = string.Empty;

        [JsonProperty("swing_mode_state_template")]
        [DefaultValue("")]
        public string SwingModeStateTemplate { get; set; } = string.Empty;

        [JsonProperty("swing_mode_state_topic")]
        [DefaultValue("")]
        public string SwingModeStateTopic { get; set; } = string.Empty;

        [JsonProperty("temperature_command_topic")]
        [DefaultValue("")]
        public string TemperatureCommandTopic { get; set; } = string.Empty;

        [JsonProperty("temperature_state_template")]
        [DefaultValue("")]
        public string TemperatureStateTemplate { get; set; } = string.Empty;

        [JsonProperty("temperature_state_topic")]
        [DefaultValue("")]
        public string TemperatureStateTopic { get; set; } = string.Empty;

        [JsonProperty("tilt_closed_value")]
        [DefaultValue("")]
        public string TiltClosedValue { get; set; } = string.Empty;

        [JsonProperty("tilt_command_topic")]
        [DefaultValue("")]
        public string TiltCommandTopic { get; set; } = string.Empty;

        [JsonProperty("tilt_invert_state")]
        [DefaultValue("")]
        public string TiltInvertState { get; set; } = string.Empty;

        [JsonProperty("tilt_max")]
        [DefaultValue("")]
        public string TiltMax { get; set; } = string.Empty;

        [JsonProperty("tilt_min")]
        [DefaultValue("")]
        public string TiltMin { get; set; } = string.Empty;

        [JsonProperty("tilt_opened_value")]
        [DefaultValue("")]
        public string TiltOpenedValue { get; set; } = string.Empty;

        [JsonProperty("tilt_status_optimistic")]
        [DefaultValue("")]
        public string TiltStatusOptimistic { get; set; } = string.Empty;

        [JsonProperty("tilt_status_topic")]
        [DefaultValue("")]
        public string TiltStatusTopic { get; set; } = string.Empty;

        [JsonProperty("topic")]
        [DefaultValue("")]
        public string Topic { get; set; } = string.Empty;

        [JsonProperty("unique_id")]
        [DefaultValue("")]
        public string UniqueId { get; set; } = string.Empty;

        [JsonProperty("unit_of_measurement")]
        [DefaultValue("")]
        public string UnitOfMeasurement { get; set; } = string.Empty;

        [JsonProperty("value_template")]
        [DefaultValue("")]
        public string ValueTemplate { get; set; } = string.Empty;

        [JsonProperty("white_value_command_topic")]
        [DefaultValue("")]
        public string WhiteValueCommandTopic { get; set; } = string.Empty;

        [JsonProperty("white_value_scale")]
        [DefaultValue("")]
        public string WhiteValueScale { get; set; } = string.Empty;

        [JsonProperty("white_value_state_topic")]
        [DefaultValue("")]
        public string WhiteValueStateTopic { get; set; } = string.Empty;

        [JsonProperty("white_value_template")]
        [DefaultValue("")]
        public string WhiteValueTemplate { get; set; } = string.Empty;

        [JsonProperty("xy_command_topic")]
        [DefaultValue("")]
        public string XyCommandTopic { get; set; } = string.Empty;

        [JsonProperty("xy_state_topic")]
        [DefaultValue("")]
        public string XyStateTopic { get; set; } = string.Empty;

        [JsonProperty("xy_value_template")]
        [DefaultValue("")]
        public string XyValueTemplate { get; set; } = string.Empty;
    }
}