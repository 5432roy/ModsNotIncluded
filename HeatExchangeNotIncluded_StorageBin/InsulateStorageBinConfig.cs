using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace HeatExchangeNotIncluded_StorageBin
{
    public class InsulateStorageConfig : IBuildingConfig
    {
        public const string ID = "InsulateStorageBin";

        public const string ANIM_NAME = "storagelocker_kanim";

        private static float CAPACITY = 100000f;

        private static int HEIGHT = 2;

        private static int WIDTH = 1;

        private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier> {
            Storage.StoredItemModifier.Insulate
        };

        public override BuildingDef CreateBuildingDef()
        {
            // Create a building definition similar to the standard storage building.
            string[] construction_materials = new string[2] { "RefinedMetal", "Insulite" };
            float[] construction_mass = new float[2] { BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0], BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0] };
            BuildingDef obj = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: WIDTH,
                height: HEIGHT,
                anim: "storagelocker_kanim",
                hitpoints: 30,
                construction_time: 60f,
                construction_mass: construction_mass,
                construction_materials: construction_materials,
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
            storage.SetDefaultStoredItemModifiers(StoredItemModifiers);
            storage.capacityKg = CAPACITY;
            storage.showInUI = true;
            storage.allowItemRemoval = true;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.STORAGE_LOCKERS_STANDARD;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.showCapacityStatusItem = true;
            storage.showCapacityAsMainStatus = true;

            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
            go.AddOrGet<StorageLocker>();
            go.AddOrGet<TileTemperature>();
            go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
            go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}