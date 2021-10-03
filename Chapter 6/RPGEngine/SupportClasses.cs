﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    public class BaseBonus
    {
        public short Amount;
    }

    public class Bonus
    {
        public BonusType Type;
        public short Amount;
        public int TimeStarted;
        public int Duration;
        public float ElapsedTime;
    }

    public class MinMaxBonus
    {
        public int Min;
        public int Max;
        public int Amount;

        public bool IsValueInRange(int value)
        {
            return (value >= Min && value <= Max);
        }
    }

    public class Modifier
    {
        public ModifierType Type;
        public int Amount;
    }

    public class Damager
    {
        public DamageType Type;
        //Format = "n,n"
        public string DamageAmount;
        //The type of entity the damage affects, empty for all types
        public string Affects;
    }

    public class EntityLevel
    {
        public int MinExperience;
        public int MaxExperience;

        public EntityLevel(int min, int max)
        {
            MinExperience = min;
            MaxExperience = max;
        }
    }

}
