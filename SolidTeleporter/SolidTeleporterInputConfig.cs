using TUNING;
using UnityEngine;
using System.Collections.Generic;

namespace SolidTeleporter
{
    public class SolidTeleporterInputConfig : IBuildingConfig
    {
        public const string ID = "SolidTeleporter_Input";
        public const string DISPLAYNAME = "Solid Teleporter Input";
        public const string DESCRIPTION = "A configurable teleporter input for the conveyance system. Allows selection of a teleporter output.";
        public const string EFFECT = "Allows the user to choose which teleporter output to send items to.";

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 4,
                height: 3,
                anim: "warp_conduit_sender_kanim",
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
            go.AddOrGet<LogicPorts>();
            go.AddOrGet<SolidTeleporterInputBehaviour>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }

    public class SolidTeleporterInputBehaviour : KMonoBehaviour
    {
        // Stores the identifier of the selected output building.
        public string selectedOutputID = "";

        protected override void OnSpawn()
        {
            base.OnSpawn();
            // When the input building spawns, refresh the list of available teleporter outputs.
            RefreshAvailableOutputs();
        }

        public void RefreshAvailableOutputs()
        {
            List<string> availableOutputs = new List<string>();
            // TODO: Implement logic to detect available teleporter output buildings.
            Debug.Log("SolidTeleporterInput: Refreshed available outputs count: " + availableOutputs.Count);
        }

        // Placeholder for opening a configuration UI.
        public void OpenConfigurationUI()
        {
            Debug.Log("SolidTeleporterInput: Opening configuration UI.");
        }
    }
}
