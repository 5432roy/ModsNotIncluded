using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace HeatExchangeNotIncluded_StorageBin
{
    public class InsulateStorageBinConfig : IBuildingConfig
    {
        public const string ID = "InsulateStorageBin";
        public const string DISPLAYNAME = "Insulate Storage Bin";
        public const string DESCRIPTION = "A storage bin that insulate the stored items";
        public const string EFFECT = "A storage bin that stored items will not exchange heat with the environment";
        public const string ANIM_NAME = "storagelocker_kanim";
        public const string DeconstructButtonText = "Deconstruct";
        public const string DeconstructButtonTooltip = "Deconstruct this storage bin and drop all items here";

        private static float MAXCAPACITY = InsulateStorageBinMod_Patch.Option.StorageCapacity * 1000f;
        private static readonly int HEIGHT = 2;
        private static readonly int WIDTH = 1;
        private static readonly List<Storage.StoredItemModifier> StoredItemModifiersNotSealed = new List<Storage.StoredItemModifier> {
            Storage.StoredItemModifier.Insulate,
            Storage.StoredItemModifier.Hide
        };
        private static readonly List<Storage.StoredItemModifier> StoredItemModifiersSealed = new List<Storage.StoredItemModifier> {
            Storage.StoredItemModifier.Insulate,
            Storage.StoredItemModifier.Hide,
            Storage.StoredItemModifier.Seal
        };

        public override BuildingDef CreateBuildingDef()
        {
            string[] construction_materials = new string[2] { "RefinedMetal", "SUPERINSULATOR" };
            float[] construction_mass = new float[2] { BUILDINGS.CONSTRUCTION_MASS_KG.TIER4[0], BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0] };
            BuildingDef obj = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: WIDTH,
                height: HEIGHT,
                anim: ANIM_NAME,
                hitpoints: 30,
                construction_time: 60f,
                // retrun type needs to be the same
                construction_mass: InsulateStorageBinMod_Patch.Option.RequiredInsulie ? construction_mass : new float[1] { BUILDINGS.CONSTRUCTION_MASS_KG.TIER4[0] },
                construction_materials: InsulateStorageBinMod_Patch.Option.RequiredInsulie ? construction_materials : new string[1] { "RefinedMetal" },
                melting_point: 800f,
                build_location_rule: BuildLocationRule.OnFloor,
                decor: DECOR.PENALTY.TIER1,
                noise: NOISE_POLLUTION.NONE);

            obj.Floodable = false;
            obj.Entombable = false;
            obj.Overheatable = false;
            obj.UseStructureTemperature = false;
            obj.AudioCategory = "Metal";
            obj.BaseTimeUntilRepair = -1f;

            return obj;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            // Add and configure the Storage component, similar to the standard storage building.
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(InsulateStorageBinMod_Patch.Option.Sealed ? StoredItemModifiersSealed : StoredItemModifiersNotSealed);
            storage.capacityKg = MAXCAPACITY;
            storage.showInUI = true;
            storage.allowItemRemoval = true;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.STORAGE_LOCKERS_STANDARD;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.showCapacityStatusItem = true;
            storage.showCapacityAsMainStatus = true;

            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
            go.AddOrGet<StorageLocker>();
            go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
            go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}