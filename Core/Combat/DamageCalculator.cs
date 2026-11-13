namespace TextGameCLI.Core.Combat;

using TextGameCLI.Core.Characters;

public static class DamageCalculator
{
    public static int CalculatePhysical(Character attacker, Character target)
    {
        int raw = attacker.Power;
        int effectiveArmor = target.IsDefending ? target.Armor * 2 : target.Armor;
        return Math.Max(0, raw - effectiveArmor);
    }

    public static int CalculateMagic(Character attacker, Character target)
    {
        double multiplier = 1.0 - target.Resistance;
        int raw = (int)(attacker.Magic * 1.45);
        return Math.Max(1, (int)(raw * multiplier));
    }
}