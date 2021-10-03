using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace RPGEngine
{
    public class GlobalData
    {
        //key is type, not id
        public static Dictionary<int, List<Skill>> Skills;
        public static Dictionary<int, List<Spell>> Spells;    
        public static Dictionary<int, List<Item>> Items;      
    }

    public class GlobalFunctions
    {
        static Random rnd = new Random();

        public static short GetRandomNumber(DieType die)
        {
            return (short)rnd.Next(1, (int)die + 1);
        }

        public static int GetRandomNumber(int min, int max)
        {
            return rnd.Next(min, max + 1);
        }

        public static int GetRangeAmount(String amount)
        {
            String min, max;

            min = amount.Substring(0, amount.IndexOf("-"));
            max = amount.Substring(amount.IndexOf("-")+1);

            return GetRandomNumber(Convert.ToInt32(min), Convert.ToInt32(max));
        }

        public static int GetRandomHPDieTotal(int level, DieType type)
        {
            int total = 0;

            for (int i = 1; i <= level; i++)
                total += GetRandomNumber(type);

            return total;
        }

        public static bool IsNumeric(object Expression)
        {
            bool isNum;

            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;

            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public static int GetSPForStat(int statValue)
        {
            int ret = 0;

            //TODO: placeholder for now
            ret = (int)(statValue / 20);

            return ret;

        }

        public static Item GetItem(int id)
        {
            foreach (List<Item> items in GlobalData.Items.Values)
            {
                foreach (Item item in items)
                    if (item.ID == id)
                        return item;
            }

            return null;
        }

        public static Skill GetSkill(int id)
        {
            foreach (List<Skill> skills in GlobalData.Skills.Values)
            {
                foreach (Skill skill in skills)
                    if (skill.ID == id)
                        return skill;
            }

            return null;
        }

        public static int GetIDForSkillName(string name)
        {
            int id = 0;

            foreach (List<Skill> skills in GlobalData.Skills.Values)
            {
                foreach(Skill skill in skills)
                    if (skill.Name == name)
                        return skill.ID;
            }

            return id;
        }

        public static int CalculateSkillBonus(int levels)
        {
            int ret = 0;

            //graduated value for levels
            if (levels <= 5)
                ret = levels * 5;
            else if (levels <= 10)
                ret = 25 + ((levels - 5) * 3);
            else if (levels <= 15)
                ret = 40 + ((levels - 10) * 2);
            else
                ret = 50 + (levels - 15);

            return ret;
        }

        public static DamageType GetDamageTypeForTrap(TrapType type)
        {
            DamageType damageType;

            switch (type)
            {
                case TrapType.Missle:
                    {
                        damageType = DamageType.Piercing;
                        break;
                    }
                case TrapType.Gas:
                    {
                        damageType = DamageType.Poison;
                        break;
                    }
                case TrapType.Explosion:
                    {
                        damageType = DamageType.Fire;
                        break;
                    }
                case TrapType.Magic:
                default:
                    {
                        damageType = DamageType.Magical;
                        break;
                    }
            }

            return damageType;
        }

        //public static Race LoadRace(string filename)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public static Stat LoadStat(string filename)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public static Entity LoadEntity(string filename)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}


        //public static EntityClass LoadClass(string filename)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

    }
}