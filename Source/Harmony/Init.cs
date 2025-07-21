using System.Reflection;
using HarmonyLib;
using Verse;

namespace USH_NA;

[StaticConstructorOnStartup]
public static class Start
{
    static Start()
    {
        Harmony harmony = new("NecroaArchovirus");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}
