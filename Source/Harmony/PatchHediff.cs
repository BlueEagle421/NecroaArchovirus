using HarmonyLib;
using RimWorld;
using Verse;

namespace USH_NA;

[HarmonyPatch(typeof(Hediff), nameof(Hediff.Notify_PawnDamagedThing))]
public static class PatchHediffComps
{
    [HarmonyPostfix]
    public static void AddNecroaInfection(Hediff __instance, Thing thing, DamageWorker.DamageResult result)
    {
        if (__instance is not Hediff_Shambler hediff || hediff.pawn == null || !hediff.pawn.IsShambler)
            return;

        if (thing is not Pawn victim)
            return;

        if (result.hediffs == null)
            return;

        bool causedBleeding = result.hediffs.Any(h => h is Hediff_Injury inj && inj.Bleeding);

        if (!causedBleeding && NA_Mod.Settings.BleedingOnly.Value)
            return;

        if (!Rand.Chance(NA_Mod.Settings.InfectionChance.Value))
            return;

        NecroaUtils.AddInfectionSeverity(victim, USH_DefOf.USH_Necroa, NA_Mod.Settings.SeverityPerAttack.Value, result.LastHitPart);
        SetNecroaFaction(victim, hediff.pawn.Faction);
    }


    private static void SetNecroaFaction(Pawn pawn, Faction faction)
    {
        if (faction == null)
            return;

        Hediff necroaHediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(USH_DefOf.USH_Necroa);

        if (necroaHediff == null)
            return;

        necroaHediff.TryGetComp<HediffCompNecroa>().PostMortemFaction = faction;
    }
}
