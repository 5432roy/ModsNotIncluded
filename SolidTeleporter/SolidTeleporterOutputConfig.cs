using TUNING;
using UnityEngine;

namespace SolidTeleporter
{
    public class SolidTeleporterOutputConfig : IBuildingConfig
    {
        public const string ID = "SolidTeleporter_output";
        public const string DISPLAYNAME = "Solid Teleporter Output";
        public const string DESCRIPTION = "The teleporter output for the conveyance system. Receives elements from a linked teleporter input.";
        public const string EFFECT = "Receives and outputs elements sent from the teleporter input.";

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 4,
                height: 3,
                anim: "warp_conduit_receiver_kanim",
                hitpoints: 100,
                construction_time: 30f,
                construction_mass: new float[] { 200f },
                construction_materials: new string[] { "RefinedMetal" },
                melting_point: 800f,
                build_location_rule: BuildLocationRule.Anywhere,
                decor: DECOR.BONUS.TIER1,
                noise: NOISE_POLLUTION.NONE
            );
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            // Add any necessary components.
            go.AddOrGet<LogicPorts>();
            // Attach custom behavior for the output building.
            go.AddOrGet<SolidTeleporterOutputBehaviour>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            // Additional post-configuration if needed.
        }
    }

    public class SolidTeleporterOutputBehaviour : KMonoBehaviour
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
            // Initialization logic for the output building.
            Debug.Log("SolidTeleporterOutput: Spawned.");
        }
    }
}
