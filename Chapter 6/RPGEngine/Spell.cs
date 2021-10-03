using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RPGEngine
{
    public class SpellEffect
    {
        public EffectType Type;
        public DieType dieType;
        public int Amount;
    }

    public class Spell
    {
        private int id;
        private SpellType type;
        private string name;
        private string description;
        private Difficulty difficulty;
        private SpellEffect effect;
        private string range;
        private List<string> classesAllowed;
        private string duration;
        private byte levelRequired;
        private short manaCost;
        private AlignmentFlags alignmentsRequired;
        private SpellCastRate castRate;
        private List<Bonus> defBonuses;
        private List<Bonus> offBonuses;
        private List<Bonus> miscBonuses;

        //graphic used in the spellbook in-game
        [NonSerialized()]
        private Texture2D spellBookGraphic;
        private string spellBookGraphicFilename;
        public const int SpellsPerPage = 6;

#region Members
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public SpellType Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Name
        {
            get { return name; }
            set { if (!string.IsNullOrEmpty(value)) name = value; }
        }

        public string Description
        {
            get { return description; }
            set { if (!string.IsNullOrEmpty(value)) description = value; }
        }

        public SpellEffect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        public string Range
        {
            get { return range; }
            set { range = value; }
        }

        public List<string> ClassesAllowed
        {
            get { return classesAllowed; }
            set { classesAllowed = value; }
        }

        public string Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public byte LevelRequired
        {
            get { return levelRequired; }
            set { levelRequired = value; }
        }

        public short ManaCost
        {
            get { return manaCost; }
            set { manaCost = value; }
        }

        public AlignmentFlags AlignmentsRequired
        {
            get { return alignmentsRequired; }
            set { alignmentsRequired = value; }
        }

        public SpellCastRate CastRate
        {
            get { return castRate; }
            set { castRate = value; }
        }

        public List<Bonus> DefensiveBonuses
        {
            get { return defBonuses; }
            set { defBonuses = value; }
        }

        public List<Bonus> OffensiveBonuses
        {
            get { return offBonuses; }
            set { offBonuses = value; }
        }

        public List<Bonus> MiscBonuses
        {
            get { return miscBonuses; }
            set { miscBonuses = value; }
        }

        public string SpellBookGraphicFilename
        {
            get { return spellBookGraphicFilename; }
            set { spellBookGraphicFilename = value; }
        }

        public Difficulty LearnDifficulty
        {
            get { return difficulty; }
            set { difficulty = value; }
        }

#endregion

        public bool Cast(ref Object target, ref Entity caster)
        {


            return true;
        }

        public bool Cast(ref Entity target, ref Entity caster)
        {
            short mr = target.MagicResistance;
            short roll = GlobalFunctions.GetRandomNumber(DieType.d100);

            //Get caster's skill bonus
            roll += (short)caster.GetCastingSkillBonus();

            //resistance not applicable for any but offensive spells
            if (type == SpellType.Offensive)
                roll -= mr;

            if (roll >= 100)
            {
                int amount = 0;
                
                switch (type)
                {
                    case SpellType.Defensive:
                    {
                        //normally you would just use the defBonuses list but it can be done either way
                        if (defBonuses != null)
                        {
                            foreach(Bonus bonus in defBonuses)
                                ((Entity)target).AddDefensiveBonus(bonus);
                        }
                        else
                        {
                            Bonus bonus = new Bonus();

                            bonus.Amount = Convert.ToInt16(amount);
                            bonus.Type = BonusType.Magic;
                            bonus.Duration = Convert.ToInt32(duration);

                            ((Entity)target).AddDefensiveBonus(bonus);
                        }
                        break;
                    }
                    case SpellType.NonCombat:
                    {
                        if (!((Entity)target).InCombat)
                        {
                            //go through bonuses and add to target
                            foreach (Bonus bonus in defBonuses)
                                ((Entity)target).AddDefensiveBonus(bonus);

                            foreach (Bonus bonus in offBonuses)
                                ((Entity)target).AddOffensiveBonus(bonus);

                            foreach (Bonus bonus in miscBonuses)
                                ((Entity)target).AddMiscBonus(bonus);

                        }

                        break;
                    }

                    case SpellType.Offensive:
                    {
                        target.Damage(amount, DamageType.Magical);

                        break;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
