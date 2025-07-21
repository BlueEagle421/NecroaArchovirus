using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace USH_NA;

public static class NecroaUtils
{
    public static float AddInfectionSeverity(Pawn pawn, HediffDef hediffDefToAdd, float baseSeverity, BodyPartRecord partRecord = null)
    {
        if (pawn == null)
            return 0f;

        if (!CanPathogenInfect(pawn))
            return 0f;

        if (hediffDefToAdd == null)
            return 0f;

        if (pawn.IsImmuneTo(hediffDefToAdd))
            return 0f;

        Hediff hediffFound = pawn.health?.hediffSet?.GetFirstHediffOfDef(hediffDefToAdd, false);
        float severityToSet = InfectionSeverity(pawn, hediffDefToAdd, baseSeverity);

        if (hediffFound != null)
        {
            hediffFound.Severity += severityToSet;
            return severityToSet;
        }

        Hediff hediffMade = HediffMaker.MakeHediff(hediffDefToAdd, pawn, partRecord);
        hediffMade.Severity = severityToSet;
        pawn.health.AddHediff(hediffMade);
        return severityToSet;
    }

    private static float InfectionSeverity(Pawn pawn, HediffDef hediffDef, float baseSeverity)
    {
        float statMultiplier = 1 - pawn.GetStatValue(StatDefOf.ToxicEnvironmentResistance, true);
        float result = Mathf.Max(hediffDef.minSeverity, Rand.Range(baseSeverity / 2f, baseSeverity));

        result *= statMultiplier;
        result /= pawn.def.Size.Area;

        return result;
    }

    public static bool CanPathogenInfect(Pawn pawn)
    {
        if (pawn == null)
            return false;

        if (!pawn.RaceProps.IsFlesh)
            return false;

        if (pawn.RaceProps.isImmuneToInfections)
            return false;

        if (pawn.IsMutant && (pawn.mutant.Def.isImmuneToInfections || pawn.mutant.Def.preventIllnesses))
            return false;

        if (pawn.GetStatValue(StatDefOf.ToxicEnvironmentResistance) >= 0.8f)
            return false;

        if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Breathing))
            return false;

        return true;
    }

    public static bool IsImmuneTo(this Pawn pawn, HediffDef hediffDef)
    {
        if (pawn.health.immunity.AnyGeneMakesFullyImmuneTo(hediffDef))
            return true;

        if (AnyHediffMakesFullyImmuneTo(pawn, hediffDef))
            return true;

        if (pawn.IsShambler && hediffDef == USH_DefOf.USH_Necroa)
            return true;

        return false;
    }

    public static bool AnyHediffMakesFullyImmuneTo(Pawn pawn, HediffDef hediffDef)
    {
        List<Hediff> allHediffs = pawn.health.hediffSet.hediffs;

        if (allHediffs == null || allHediffs.Count == 0)
            return false;

        if (allHediffs.Any(x => CanCheckHediff(x) && x.CurStage.makeImmuneTo.Any(y => y == hediffDef)))
            return true;

        return false;
    }

    private static bool CanCheckHediff(Hediff hediff) => hediff.CurStage != null && hediff.CurStage.makeImmuneTo != null;
}