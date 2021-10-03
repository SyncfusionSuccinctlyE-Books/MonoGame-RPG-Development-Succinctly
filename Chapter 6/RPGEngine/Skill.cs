using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RPGEngine
{
    public class Skill
    {
        public int ID;
        public string Name;
        public string Description;

        public SkillType Type;

        public int OpposingSkill;

        public bool AlwaysOn;

        private Dictionary<string, string> costs;
        private Dictionary<string, int> classBonuses;
        private List<MinMaxBonus> levelBonuses;
        private Dictionary<string, int> raceBonuses;
        private Dictionary<string, MinMaxBonus> statBonuses;

        public string IconFilename;

        public Texture2D Icon;

        public Skill()
        {

        }

        public Skill(int id)
        {
            ID = id;
            costs = new Dictionary<string, string>();
            classBonuses = new Dictionary<string, int>();
            raceBonuses = new Dictionary<string, int>();
            levelBonuses = new List<MinMaxBonus>();
            statBonuses = new Dictionary<string, MinMaxBonus>();
            AlwaysOn = false;
        }

#region Cost Functions 

        public bool AddCost(string id, string cost)
        {
            if (costs == null)
                costs = new Dictionary<string, string>();

            if (!costs.ContainsKey(id))
                costs.Add(id, cost);
            else
                return false;

            return true;
        }

        public void RemoveCost(string id)
        {
            if (costs != null)
                costs.Remove(id);
        }

        public string GetCost(string id)
        {
            string cost = "";

            if (costs != null)
                costs.TryGetValue(id, out cost);

            return cost;
        }

        public Dictionary<string, string> Costs
        {
            get { return costs; }
            set { costs = value; }
        }

#endregion

#region Class Bonus Functions

        public bool AddClassBonus(string id, int bonus)
        {
            if (classBonuses == null)
                classBonuses = new Dictionary<string, int>();

            if (!classBonuses.ContainsKey(id))
                classBonuses.Add(id, bonus);
            else
                return false;

            return true;

        }

        public void RemoveClassBonus(string id)
        {
            if (classBonuses != null)
                classBonuses.Remove(id);
        }

        public int GetClassBonus(string id)
        {
            int bonus = 0;

            if (classBonuses != null)
                classBonuses.TryGetValue(id, out bonus);

            return bonus;
        }

        public Dictionary<string, int> ClassBonuses
        {
            get { return classBonuses; }
            set { classBonuses = value; }
        }

#endregion

#region Race Bonus Functions

        public void AddRaceBonus(string id, int bonus)
        {
            if (raceBonuses == null)
                raceBonuses = new Dictionary<string, int>();

            if (!raceBonuses.ContainsKey(id))
                raceBonuses.Add(id, bonus);

        }

        public void RemoveRaceBonus(string id)
        {
            if (raceBonuses != null)
                raceBonuses.Remove(id);
        }

        public int GetRaceBonus(string id)
        {
            int bonus = 0;

            if (raceBonuses != null)
                raceBonuses.TryGetValue(id, out bonus);

            return bonus;
        }

        public Dictionary<string, int> RaceBonuses
        {
            get { return raceBonuses; }
            set { raceBonuses = value; }
        }

#endregion

#region Stat Bonus Functions

        public void AddStatBonus(string id, MinMaxBonus bonus)
        {
            if (statBonuses == null)
                statBonuses = new Dictionary<string, MinMaxBonus>();

            if (!statBonuses.ContainsKey(id))
                statBonuses.Add(id, bonus);

        }

        public void RemoveStatBonus(string id)
        {
            if (statBonuses != null)
                statBonuses.Remove(id);
        }

        public MinMaxBonus GetStatBonus(string id)
        {
            MinMaxBonus bonus = new MinMaxBonus();

            if (statBonuses != null)
                statBonuses.TryGetValue(id, out bonus);

            return bonus;
        }

        public Dictionary<string, MinMaxBonus> StatBonuses
        {
            get { return statBonuses; }
            set { statBonuses = value; }
        }

#endregion

#region Level Bonus Functions

        public void AddLevelBonus(MinMaxBonus bonus)
        {
            if (levelBonuses == null)
                levelBonuses = new List<MinMaxBonus>();

            levelBonuses.Add(bonus);

        }

        public void RemoveLevelBonus(int index)
        {
            if (levelBonuses != null)
                levelBonuses.RemoveAt(index);
        }

        public int GetLevelBonus(int level)
        {
            if (levelBonuses != null)
            {
                foreach (MinMaxBonus bonus in levelBonuses)
                {
                    if (bonus.IsValueInRange(level))
                        return bonus.Amount;
                }
            }

            return 0;
        }

        public List<MinMaxBonus> LevelBonuses
        {
            get { return levelBonuses; }
            set { levelBonuses = value; }
        }

#endregion

        public bool Use(ref Object target, ref Entity entity, Difficulty difficulty, ref int result)
        {
            int roll;
            int bns = 0;

            roll = GlobalFunctions.GetRandomNumber(DieType.d100);

            roll += (short)difficulty;

            if (OpposingSkill > 0)
            {
                if (((Entity)target).HasSkillByID(OpposingSkill))
                    roll -= ((Entity)target).GetSkillValueByID(OpposingSkill);
            }
            else
            {
                switch (Type)
                {
                    case SkillType.Defensive:
                        {
                            if (target is Entity)
                                roll -= ((Entity)target).GetTotalOffBonus();

                            break;
                        }
                    case SkillType.NonCombat:
                        {
                            if (target is Entity)
                                roll -= ((Entity)target).GetTotalMiscBonus();

                            break;
                        }
                    case SkillType.Offensive:
                        {
                            if (target is Entity)
                                roll -= ((Entity)target).GetTotalDefBonus();

                            break;
                        }
                }
            }

            foreach (KeyValuePair<string,int> kvp in raceBonuses)
            {
                if (entity.RaceID == kvp.Key)
                {
                    bns += entity.Level * kvp.Value;
                    break;
                }
            }

            foreach (KeyValuePair<string, int> kvp in classBonuses)
            {
                if (entity.ClassID == kvp.Key)
                {
                    bns += entity.Level * kvp.Value;
                    break;
                }
            }

            //Calculate level bonus
            if (levelBonuses == null)
            {
                short level = entity.Level;
                //use default
                switch (level)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    {
                        bns = (short)(10 * level);
                        break;
                    }
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    {
                        bns = (short)(50 + (5 * (level - 5)));
                        break;
                    }
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    {
                        bns = (short)(75 + (3 * (level - 10)));
                        break;
                    }
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    {
                        bns = (short)(90 + (2 * (level - 15)));
                        break;
                    }
                    default:
                    {
                        bns = (short)(100 + (1 * (level - 20)));
                        break;
                    }
                }
            }
            else
            {
                foreach (MinMaxBonus bonus in levelBonuses)
                {
                    if (bonus.IsValueInRange(entity.Level))
                    {
                        bns += bonus.Amount;
                        break;
                    }
                }
            }

            roll += bns;

            result = roll;

            return (roll >= 100);

        }
    }
}
