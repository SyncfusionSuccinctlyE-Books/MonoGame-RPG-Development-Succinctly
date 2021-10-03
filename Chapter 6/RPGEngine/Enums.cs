using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
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

    public enum BonusType
    {
        Disease,
        Magic,
        Poison,
        Potion,
        Other
    }

    public enum DamageType
    {
        Crushing,
        Piercing,
        Slashing,
        Fire,
        Water,
        Magical,
        Disease,
        Poison
    }

    public enum ModifierType
    {
        Fire,
        Water,
        Magic,
        Disease,
        Poison
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

    [Flags]
    public enum AlignmentFlags
    {
        None = 0,
        Good = 1,
        Neutral = 2,
        Evil = 4
    }

    public enum SkillType
    {
        Defensive,
        NonCombat,
        Offensive
    }

    public enum Difficulty
    {
        Impossible = -50,
        VeryHard = -25,
        Hard = -10,
        Normal = 0,
        Easy = 10,
        VeryEasy = 25
    }

    public enum SpellType
    {
        Offensive,
        Defensive,
        NonCombat
        //BuffSelf,
        //BuffAny,
        //Miscellaneous
    }

    public enum EffectType
    {
        Number,
        DieAmount
    }

    public enum SpellCastRate
    {
        None = -1,             //No restrictions
        Round,           //Once per round of combat
        Minute,
        Hour,
        Day,
        Week,
        Month
    }

    public enum ArmorArea
    {
        None = -1,
        Head,
        Neck,
        Chest,
        LeftUpperArm,
        UpperArms,
        LeftLowerArm,
        LowerArms,
        LeftWrist,
        Wrists,
        LeftHand,
        LeftHandEquipped,   //used for the item held in the left hand
                            //Could add fingers here for rings
        Hands,
        RightUpperArm,
        RightLowerArm,
        RightWrist,
        RightHand,
        RightHandEquipped,  //used for the item held in the right hand
                            //Could add fingers here for rings
        Waist,
        Groin,
        LeftUpperLeg,
        UpperLegs,
        LeftKnee,
        Knees,
        LeftLowerLeg,
        LowerLegs,
        LeftFoot,
        Feet,
        RightUpperLeg,
        RightKnee,
        RightLowerLeg,
        RightFoot
    }

    public enum ItemType
    {
        Armor,
        Chest,
        Key,
        Potion,
        Scroll,
        Shield,
        Weapon,
        Money         //Placed out of order since we don't create money in the Editor
    }

    public enum TrapType
    {
        None,
        Missle,      //normal missle-type
        Gas,
        Magic,       //spell is cast
        Explosion
    }

    public enum ShieldSize
    {
        Small,
        Medium,
        Large
    }

    public enum PotionType
    {
        Heal,
        Poison,
        Speed,
        Strength,
        Dexterity
    }

    public enum AttackTypes
    {
        Normal,
        Parry,
        Riposte,
        CalledShot,
        LongRangeShot,
        Disengage
    }

}
