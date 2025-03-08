using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace HeatExchangeNotIncluded_StorageBin
{
    [JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
    [ConfigFile(SharedConfigLocation: true)]
    [RestartRequired]
    public class OptionConfig
    {
        public static OptionConfig Instance { get; set; } = new OptionConfig();

        [Option("Required Insulite to build the insulate storage bin")]
        [JsonProperty]
        public bool RequiredInsulie { get; set; } = true;

        [Option("Storages prvent items from emitting gas")]
        [JsonProperty]
        public bool Sealed { get; set; } = true;

        [Option("Insulate Storage Bin Capacity", "Set the intensity level between 1 and 200.")]
        [Limit(1, 200)]
        [JsonProperty]
        public int StorageCapacity { get; set; } = 100;
    }
}
