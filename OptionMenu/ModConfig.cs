using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace OptionMenu
{
    [JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
    [ConfigFile(SharedConfigLocation: true)]
    public class ModConfig
    {
        public static ModConfig Instance { get; set; } = new ModConfig();

        [Option("Enable Feature", "Toggle the feature on or off.")]
        [JsonProperty]
        public bool EnableFeature { get; set; } = true;

        [Option("Feature Intensity", "Set the intensity level between 1 and 200.")]
        [Limit(1, 200)]
        [JsonProperty]
        public int FeatureIntensity { get; set; } = 100;
    }
}
