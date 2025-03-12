using HarmonyLib;
using KMod;

namespace SolidTeleporter
{
    public class SolidTeleporter : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            // Initialize PLib (if you plan to use additional PLib features).
            // Optionally, register mod options here.
        }
    }
}
