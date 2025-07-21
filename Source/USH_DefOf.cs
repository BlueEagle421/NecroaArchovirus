using RimWorld;
using Verse;

namespace USH_NA;

[DefOf]
public static class USH_DefOf
{
    static USH_DefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(USH_DefOf));
    public static HediffDef USH_Necroa;
}