using System.Collections.Generic;
using HarmonyLib;
using STRINGS;
using KMod;


namespace HeatExchangeNotIncluded_StorageBin
{
    public class InsulateStorageBinMod_Patch : UserMod2
    {
        public static class InsulateStorageBinPatch
        {
            [HarmonyPatch(typeof(GeneratedBuildings))]
            [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
            public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
            {
                // Add insulate storage bin to build menu
                public static void Prefix()
                {
                    Utils.AddBuildingStrings(InsulateStorageBinConfig.ID, InsulateStorageBinConfig.DISPLAYNAME, InsulateStorageBinConfig.DESCRIPTION, InsulateStorageBinConfig.EFFECT);
                    Utils.AddPlan("Base", "storage", InsulateStorageBinConfig.ID, "StorageLocker");
                }
            }


            [HarmonyPatch(typeof(ResourceRemainingDisplayScreen))]
            [HarmonyPatch(nameof(ResourceRemainingDisplayScreen.GetString))]
            public static class ResourceRemainingDisplayScreen_Patch
            {
                //this clumsy patch overrites the 'sandstone 500/1kg' on the hover text card when building hauling points.
                //checks the buildingdef in the hovercard, since it's public, rather than buildtool itself
                //also checks to make sure that it's exactly 1kg mass - draggable items (wires, pipes etc) otherwise cause errors since they use a drag tool not build tool
                public static string Postfix(string __result, Recipe ___currentRecipe)
                {
                    if (___currentRecipe.Ingredients[0].amount == 1f)
                    {
                        if (BuildTool.Instance.GetComponent<BuildToolHoverTextCard>().currentDef.name == "HaulingPoint")
                        {
                            __result = "No resources required";
                        }
                    }
                    return __result;
                }
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

        public static void AddPlan(HashedString category, string subcategory, string idBuilding, string addAfter = null)
        {
            Debug.Log("Adding " + idBuilding + " to category " + category);
            foreach (PlanScreen.PlanInfo menu in TUNING.BUILDINGS.PLANORDER)
            {
                if (menu.category == category)
                {
                    AddPlanToCategory(menu, subcategory, idBuilding, addAfter);
                    return;
                }
            }

            Debug.Log($"Unknown build menu category: ${category}");
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
                    int index = data.IndexOf(new KeyValuePair<string, string>(addAfter, subcategory));
                    if (index == -1)
                    {
                        Debug.Log($"Could not find building {subcategory}/{addAfter} to add {idBuilding} after. Adding at the end !");
                        data.Add(new KeyValuePair<string, string>(idBuilding, subcategory));
                        return;
                    }
                    data.Insert(index + 1, new KeyValuePair<string, string>(idBuilding, subcategory));
                }
            }
        }

    }
}