using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    public enum WeaponType
    {
        OneHandSword,
        TwoHandSword,
        Dagger,
        Staff,
        Mace,
        Bow,
        Crossbow
    }

    public class Weapon : Item
    {
        //array of Damager objects
        public List<Damager> Damagers;
        public WeaponType WeaponType;

        public void AddDamager(Damager item)
        {
            if (Damagers == null)
              Damagers = new List<Damager>();

            Damagers.Add(item);
        }

        public void AddDamager(DamageType type, DieType amount, String affects)
        {
            Damager item = new Damager();

            item.Affects = affects;
            item.DamageAmount = amount.ToString();
            item.Type = type;

			if (Damagers == null)
				Damagers = new List<Damager>();
				
			Damagers.Add(item);
        }

        protected int GetDamage(byte damageType)
        {
            int damage = 0;

            for (int i = 0; i < Damagers.Count; i++)
            {
                if (Damagers[i].Type == (DamageType)damageType)
                {
                    damage = GlobalFunctions.GetRangeAmount(Damagers[i].DamageAmount);
                    break;
                }
            }

            return damage;
        }

        public override bool Use(ref Object target, ref Entity wielder)
        {

			  //not used as attacking with a weapon is handled
			  //by the combat manager

            return true;
        }
    }
}
