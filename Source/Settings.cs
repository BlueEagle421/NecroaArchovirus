using RimWorld;
using UnityEngine;
using Verse;

namespace USH_NA;

public class NA_Mod : Mod
{
    public static NA_Settings Settings { get; private set; }
    private static Vector2 scrollPosition = new(0f, 0f);
    private static float totalContentHeight = 1000f;
    private const float ScrollBarWidthMargin = 18f;

    public NA_Mod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<NA_Settings>();
    }
    public override void DoSettingsWindowContents(Rect inRect)
    {
        Rect outerRect = inRect.ContractedBy(10f);

        bool scrollBarVisible = totalContentHeight > outerRect.height;
        var scrollViewTotal = new Rect(0f, 0f, outerRect.width - (scrollBarVisible ? ScrollBarWidthMargin : 0), totalContentHeight);
        Widgets.BeginScrollView(outerRect, ref scrollPosition, scrollViewTotal);

        Listing_Standard listingStandard = new();
        listingStandard.Begin(new Rect(0f, 0f, scrollViewTotal.width, 9999f));


        //InfectionChance
        listingStandard.Label("USH_NA_InfectionChance".Translate().Colorize(Color.cyan));
        float formingSliderValue = listingStandard.Slider(Settings.InfectionChance.Value, 0f, 1f);
        listingStandard.Label("USH_NA_InfectionChanceDesc".Translate(formingSliderValue.ToStringPercent()));
        Settings.InfectionChance.Value = formingSliderValue;

        //SeverityPerAttack
        listingStandard.Label("\n");
        listingStandard.Label("USH_NA_SeverityPerAttack".Translate().Colorize(Color.cyan));
        float positiveSliderValue = listingStandard.Slider(Settings.SeverityPerAttack.Value, 0f, 1f);
        listingStandard.Label("USH_NA_SeverityPerAttackDesc".Translate(positiveSliderValue.ToString("0:00")));
        Settings.SeverityPerAttack.Value = positiveSliderValue;

        //BleedingOnly
        listingStandard.Label("\n");
        listingStandard.CheckboxLabeled("USH_NA_BleedingOnly".Translate().Colorize(Color.cyan), ref Settings.BleedingOnly.Value);
        listingStandard.Label("USH_NA_BleedingOnlyDesc".Translate());

        //Reset button
        listingStandard.Label("\n");
        bool shouldReset = listingStandard.ButtonText("USH_NA_ResetSettings".Translate());
        if (shouldReset) Settings.ResetAll();
        listingStandard.Label("\n");

        //End
        listingStandard.End();
        totalContentHeight = listingStandard.CurHeight + 10f;
        Widgets.EndScrollView();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory() => "Necroa Archovirus";
}

public class NA_Settings : ModSettings
{
    public Setting<float> InfectionChance = new(0.85f);
    public Setting<float> SeverityPerAttack = new(0.08f);
    public Setting<bool> BleedingOnly = new(true);

    public void ResetAll()
    {
        InfectionChance.ToDefault();
        SeverityPerAttack.ToDefault();
        BleedingOnly.ToDefault();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        InfectionChance.ExposeData(nameof(InfectionChance));
        SeverityPerAttack.ExposeData(nameof(SeverityPerAttack));
        BleedingOnly.ExposeData(nameof(BleedingOnly));
    }

    public class Setting<T>(T defaultValue)
    {
        public T Value = defaultValue;
        public T DefaultValue { get; private set; } = defaultValue;

        public void ToDefault() => Value = DefaultValue;
        public void ExposeData(string key) => Scribe_Values.Look(ref Value, $"USH_{key}", DefaultValue);
    }
}