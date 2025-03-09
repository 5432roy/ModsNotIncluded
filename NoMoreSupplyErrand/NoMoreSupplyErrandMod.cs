using KMod;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SuitMechnisimFix
{
    class SuitMechanism_Patch : UserMod2
    {
        [HarmonyPatch(typeof(ChoreType))]
        [HarmonyPatch(MethodType.Constructor, typeof(string), typeof(ResourceSet), typeof(string[]), typeof(string), typeof(string), typeof(string), typeof(string), typeof(IEnumerable<Tag>), typeof(int), typeof(int))]
        public static class Patch_ChoreType_Constructor
        {
            [HarmonyPrefix]
            public static void Prefix(ref string[] chore_groups)
            {
                if (chore_groups != null && chore_groups.Length > 0)
                {
                    if (chore_groups.Length == 1 && chore_groups[0] == "Hauling")
                    {
                        chore_groups = new string[] { "Storage" };
                    }
                    else if (chore_groups.Contains("Hauling"))
                    {
                        chore_groups = chore_groups.Where(chore => chore != "Hauling").ToArray();
                    }
                }
            }
        }
    }
}
