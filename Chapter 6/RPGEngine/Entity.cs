using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RPGEngine
{
    public delegate void EntityLevelUpDelegate(Entity entity);

    public class Entity
    {
        #region Members

        public EntityType Type { get; set; }

        public string Name { get; set; }
        public string ClassID { get; set; }
        public byte Level { get; set; }
        public int Experience { get; set; }

        public string RaceID { get; set; }

        public short BaseHP { get; set; }
        public short CurHP { get; set; }

        public byte DiseaseResistance { get; set; }
        public byte PoisonResistance { get; set; }
        public byte MagicResistance { get; set; }

        //Affects is empty for these
        private List<Damager> damageResistances { get; set; }
        private List<Damager> damageWeaknesses { get; set; }

        //Any misc, offensive, or defensive bonuses (ie. spells cast on character, items, etc. May be positive or negative)
        private List<Bonus> defBonuses { get; set; }
        private List<Bonus> offBonuses { get; set; }
        private List<Bonus> miscBonuses { get; set; }

        public EntityAlignment Alignment { get; set; }

        public EntitySex Sex { get; set; }
        public short Age { get; set; }

        private List<EntityStat> stats { get; set; }

        public string PortraitFileName { get; set; }

        public bool IsVendor { get; set; }

        public Money Coins { get; set; }

        public string SpriteFilename { get; set; }

        private Inventory items;

        //TODO: key is ID, value is amount of points in skill
        private Dictionary<int, int> skills;

        public int CurSP;
        public int BaseSP;

        //used for things like hiding and spells
        public bool IsVisible;

        public Object Target;

        private Dictionary<int, EntitySpell> spells;
        private Dictionary<int, int> spellbook; //key is spell ID, value is the page it's on

        public int BaseMana;
        public int CurMana;

        public bool InCombat;


        //requires a strength stat
        private float maxWeight;

        public AttackTypes SelectedAttack;
        #endregion

        public bool HasItem(int id)
        {
            return items.HasItem(id);
        }

        public int MaxWeight()
        {
            int str = 0;

            //find the strength stat
            foreach (EntityStat stat in stats)
            {
                if (stat.StatName.ToLower() == "strength")
                {
                    str = stat.CurrentValue;
                    break;
                }
            }

            return str * 3;
        }


        #region Bonus, Resistance, Weakness Functions

        public short GetTotalOffBonus()
        {
            short total = 0;
            Bonus bns;

            if (offBonuses != null)
            {
                for (short i = 0; i < offBonuses.Count; i++)
                {
                    bns = (Bonus)offBonuses[i];
                    total += bns.Amount;
                }
            }

            return total;
        }

        public short GetTotalMiscBonus()
        {
            short total = 0;
            Bonus bns;

            if (miscBonuses != null)
            {
                for (short i = 0; i < miscBonuses.Count; i++)
                {
                    bns = (Bonus)miscBonuses[i];
                    total += bns.Amount;
                }
            }

            return total;
        }

        public short GetTotalDefBonus()
        {
            short total = 0;
            Bonus bns;

            if (defBonuses != null)
            {
                for (short i = 0; i < defBonuses.Count; i++)
                {
                    bns = (Bonus)defBonuses[i];
                    total += bns.Amount;
                }
            }

            return total;
        }

        public void AddDefensiveBonus(Bonus bonus)
        {
            if (defBonuses == null)
                defBonuses = new List<Bonus>();

            defBonuses.Add(bonus);
        }

        public void SetDefensiveBonuses(List<Bonus> bonuses)
        {
            defBonuses = new List<Bonus>();

            if (bonuses != null)
            {
                foreach (Bonus bonus in bonuses)
                    defBonuses.Add(bonus);
            }
        }

        public void AddOffensiveBonus(Bonus bonus)
        {
            if (offBonuses == null)
                offBonuses = new List<Bonus>();

            offBonuses.Add(bonus);
        }

        public void SetOffensiveBonuses(List<Bonus> bonuses)
        {
            offBonuses = new List<Bonus>();

            if (bonuses != null)
            {
                foreach (Bonus bonus in bonuses)
                    offBonuses.Add(bonus);
            }
        }

        public void AddMiscBonus(Bonus bonus)
        {
            if (miscBonuses == null)
                miscBonuses = new List<Bonus>();

            miscBonuses.Add(bonus);
        }

        public void SetMiscBonuses(List<Bonus> bonuses)
        {
            miscBonuses = new List<Bonus>();

            if (bonuses != null)
            {
                foreach (Bonus bonus in bonuses)
                    miscBonuses.Add(bonus);
            }
        }

        public void AddDamageResistance(Damager resistance)
        {
            if (damageResistances == null)
                damageResistances = new List<Damager>();

            damageResistances.Add(resistance);
        }

        public void SetDamageResistances(List<Damager> resistances)
        {
            damageResistances = new List<Damager>();

            if (resistances != null)
            {
                foreach (Damager resistance in resistances)
                    damageResistances.Add(resistance);
            }
        }

        public void AddDamageWeakness(Damager weakness)
        {
            if (damageWeaknesses == null)
                damageWeaknesses = new List<Damager>();

            damageWeaknesses.Add(weakness);
        }

        public void SetDamageWeaknesses(List<Damager> weaknesses)
        {
            damageWeaknesses = new List<Damager>();

            if (weaknesses != null)
            {
                foreach (Damager weakness in weaknesses)
                    damageWeaknesses.Add(weakness);
            }
        }

        #endregion

        public void AddSkill(int id, int ranks)
        {
            if (skills == null)
                skills = new Dictionary<int, int>();

            skills.Add(id, ranks);
        }

        public void SetSkills(Dictionary<int, int> skills)
        {
            this.skills = skills;
        }

        public bool HasSkill(int key)
        {
            if (skills != null)
                return skills.ContainsKey(key);
            else
                return false;
        }

        public void Damage(int amount, DamageType type)
        {
            //reduce amount based on damagetype protection
            foreach (Damager damager in damageResistances)
            {
                if (damager.Type == type)
                    amount -= GlobalFunctions.GetRangeAmount(damager.DamageAmount);
            }

            CurHP -= (short)amount;

            if (CurHP <= 0)
            {
                //entity is dead
            }
        }

        public void Buff(int amount, BonusType type)
        {
            //switch (type)
            //{

            //}

        }

        public void UseSkill(int key, Difficulty difficulty)
        {
            int result = 0;
            Entity entity = this;

            Skill skill = GlobalFunctions.GetSkill(key);


            if (skill.Use(ref Target, ref entity, difficulty, ref result))
            {
                switch (skill.Type)
                {
                    case SkillType.Defensive:
                        {
                            if (Target is Entity)
                            {
                            }

                            break;
                        }
                    case SkillType.NonCombat:
                        {
                            if (Target is Entity)
                            {
                            }

                            break;
                        }
                    case SkillType.Offensive:
                        {
                            if (Target is Entity)
                            {
                            }


                            break;
                        }



                }
            }
        }

        public int GetSkillValueByID(int id)
        {
            if (HasSkillByID(id))
                return GlobalFunctions.CalculateSkillBonus(skills[id]);
            else
                return 0;
        }

        public bool HasSkillByID(int id)
        {
            foreach (int key in skills.Keys)
                if (key == id)
                    return true;

            return false;
        }

        public int GetCastingSkillBonus()
        {
            int amount = 0;

            foreach (int id in skills.Keys)
            {
                //TODO: copy function from backup
                if (GlobalFunctions.GetSkill(id).Name == "Spellcraft")
                {
                    amount = skills[id];
                    break;
                }
            }

            return amount;
        }

        public bool LearnSpell(int id, int skillPointsAllocated)
        {
            bool hasSpellcraftSkill = false;

            foreach (int key in skills.Keys)
            {
                //TODO: copy function from backup
                if (GlobalFunctions.GetSkill(key).Name == "Spellcraft")
                {
                    hasSpellcraftSkill = true;
                    break;
                }
            }

            if (!hasSpellcraftSkill)
                return false;

            spells[id].AllocatePoints(skillPointsAllocated);

            //get any bonuses for learning spells
            int amount = 0;


            return spells[id].Learn(amount);
        }

        public List<EntitySpell> GetPageSpells(int page)
        {
            List<EntitySpell> spells = new List<EntitySpell>();

            foreach (int key in spellbook.Keys)
            {
                if (spellbook[key] == page)
                    spells.Add(spells[key]);
            }

            return spells;
        }

        //TODO: spellbook member should probably be passed to GUI
        public void MoveSpell(int id, int page)
        {
            //check to see if page has an empty spot
            int count = 0;

            foreach (int key in spellbook.Keys)
            {
                if (spellbook[key] == page)
                    count++;
            }

            if (count < Spell.SpellsPerPage)
                spellbook[id] = page;
        }

        public void AddMoney(ref int amount)
        {
            //do check to make sure max weight isn't reached before adding
            float totalWeight = Coins.TotalWeight + Money.GetWeight(amount) + items.TotalWeight();

            if (totalWeight < maxWeight)
                Coins.Add(ref amount);
            else
            {
                //figure out how much can be added


            }
        }


        public void Update(GameTime gameTime)
        {
            //check for mana regain


        }

        public void SetAttackType(AttackTypes type)
        {
            SelectedAttack = type;
        }
    }
}
