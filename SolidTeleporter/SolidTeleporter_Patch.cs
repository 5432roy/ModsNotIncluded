using System.Collections.Generic;
using HarmonyLib;
using STRINGS;
using KMod;
using UnityEngine;

namespace SolidTeleporter
{
    public class SolidTeleporter_Patch
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                // Register strings for the Input building.
                Utils.AddBuildingStrings(
                    SolidTeleporterInputConfig.ID,
                    SolidTeleporterInputConfig.DISPLAYNAME,
                    SolidTeleporterInputConfig.DESCRIPTION,
                    SolidTeleporterInputConfig.EFFECT
                );
                // Register strings for the Output building.
                Utils.AddBuildingStrings(
                    SolidTeleporterOutputConfig.ID,
                    SolidTeleporterOutputConfig.DISPLAYNAME,
                    SolidTeleporterOutputConfig.DESCRIPTION,
                    SolidTeleporterOutputConfig.EFFECT
                );

                // Add the Input building to the build menu:
                //   Category: "Base", Subcategory: "Conveyance"
                //   Placed after "ConveyorInput"
                Utils.AddPlan("Conveyance", "Conveyance", SolidTeleporterInputConfig.ID, "ConveyorInput");

                // Add the Output building to the build menu:
                //   Category: "Base", Subcategory: "Conveyance"
                //   Placed after the input building.
                Utils.AddPlan("Conveyance", "Conveyance", SolidTeleporterOutputConfig.ID, SolidTeleporterInputConfig.ID);
            }
        }
    }

    public static class Utils
    {
        public static void AddBuildingStrings(string buildingId, string name, string description, string effect)
        {
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, buildingId));
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.DESC", description);
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.EFFECT", effect);
        }

        public static void AddPlan(string category, string subcategory, string idBuilding, string addAfter = null)
        {
            Debug.Log("Adding " + idBuilding + " to category " + subcategory);
            foreach (PlanScreen.PlanInfo menu in TUNING.BUILDINGS.PLANORDER)
            {
                if (menu.category == category)
                {
                    AddPlanToCategory(menu, subcategory, idBuilding, addAfter);
                    return;
                }
            }
            Debug.Log($"Unknown build menu category: {category}");
        }

        private static void AddPlanToCategory(PlanScreen.PlanInfo menu, string subcategory, string idBuilding, string addAfter = null)
        {
            List<KeyValuePair<string, string>> data = menu.buildingAndSubcategoryData;
            if (data != null)
            {
                if (addAfter == null)
                {
                    data.Add(new KeyValuePair<string, string>(idBuilding, subcategory));
                }
                else
                {
                    int index = data.FindIndex(pair => pair.Key == addAfter && pair.Value == subcategory);
                    if (index == -1)
                    {
                        Debug.Log($"Could not find building {addAfter} in subcategory {subcategory} to add {idBuilding} after. Adding at the end.");
                        data.Add(new KeyValuePair<string, string>(idBuilding, subcategory));
                    }
                    else
                    {
                        data.Insert(index + 1, new KeyValuePair<string, string>(idBuilding, subcategory));
                    }
                }
            }
        }
    }
}
