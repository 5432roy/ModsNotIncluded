using PeterHan.PLib.Options;

namespace HeatExchangeNotIncluded_StorageBin
{
    [OptionMenu("Insulate Storage Bin Settings")]
    public sealed class Config
    {
        [Option("Storage Capacity (tons)", "Set the storage capacity (from 1t to 1000t).")]
        [Limit(1.0, 1000.0)]
        public float StorageCapacityTons { get; set; } = 200f;

        [Option("Use Insulite Material", "If enabled, Insulite will be used for building construction.")]
        public bool UseInsulite { get; set; } = true;

        [Option("Sealed Storage", "If enabled, the storage will be sealed; otherwise, it will be unsealed.")]
        public bool SealedStorage { get; set; } = true;
    }
}
