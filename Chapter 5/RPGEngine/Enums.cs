using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    public enum StatGenerationType
    {
        Points,
        Roll
    }

    public enum EntityType
    {
        Character,
        NPC,
        Creature,
        Monster
    }

    public enum EntityAlignment
    {
        Good,
        Neutral,
        Evil
    }

    public enum EntitySex
    {
        Male,
        Female
    }


    //Enum for types of dice
    public enum DieType
    {
        d4 = 4,
        d6 = 6,
        d8 = 8,
        d10 = 10,
        d12 = 12,
        d20 = 20,
        d100 = 100
    }
    public enum BonusType
    {
        Disease,
        Magic,
        Poison,
        Potion,
        Other
    }
    public enum ModifierType
    {
        Fire,
        Water,
        Magic,
        Disease,
        Poison
    }

    public enum EntityAttitude
    {
        Aggressive,
        Fearful,
        Neutral
    }
}
