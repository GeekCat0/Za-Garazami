using System.Collections.Generic;
using UnityEngine;

public static class Matchups // nic nie dziedziczy wiec mamy zaliczony warunek z klasą bez dziedziczenia :D 
{

    private static readonly Dictionary<UnitType, Dictionary<UnitType, float>> matchups
        = new Dictionary<UnitType, Dictionary<UnitType, float>>()
    {
        {
            UnitType.Tank, new Dictionary<UnitType, float>
            {
                { UnitType.Tank, 1.0f },
                { UnitType.Rogue, 1.5f },
                { UnitType.Ranged, 0.5f },
                { UnitType.Base, 1.0f },
            }
        },
        {
            UnitType.Rogue, new Dictionary<UnitType, float>
            {
                { UnitType.Tank, 0.5f },
                { UnitType.Rogue, 1.0f },
                { UnitType.Ranged, 1.5f },
                { UnitType.Base, 1.0f },
            }
        },
        {
            UnitType.Ranged, new Dictionary<UnitType, float>
            {
                { UnitType.Tank, 1.5f },
                { UnitType.Rogue,  0.5f },
                { UnitType.Ranged,   1.0f },
                { UnitType.Base, 1.0f },
            }
        },
    };

    public static float GetMultiplier(UnitType attacker, UnitType defender)
    {

        if (matchups.TryGetValue(attacker, out var defenderTable))
            if (defenderTable.TryGetValue(defender, out float multiplier))
                return multiplier;

        return 1.0f;
    }
}
