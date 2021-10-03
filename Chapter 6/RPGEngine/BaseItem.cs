using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RPGEngine
{
    public enum ItemGenerationTypes
    {
        Money,
        Items,
        Both
    }

    abstract public class BaseItem : Object
    {
        protected int ID;

        protected string Name;
        protected string Description;
        protected float Weight;
        protected byte StrengthRequired;
        
		//Used for different purposes depending on the type of derived item
        protected string Tag;

        protected string Filename;     //used to load picture
        protected Texture2D Picture;   //used in inventory/vendor dialogs
	 }

	public class Money : BaseItem
    {
        public const int CoinsPerPound = 10;

        private int amount;

        //Overrides since the strength required changes as the amount changes
        new public byte StrengthRequired
        {
            get { return (byte)((amount / CoinsPerPound) / Stat.PoundsPerStatPoint); }
        }

        public static byte GetStrengthRequired(int amount)
        {
            return (byte)((amount / CoinsPerPound) / Stat.PoundsPerStatPoint);
        }

        public int Total()
        {
            return amount;
        }
        
        public Money()
        {

        }

        public static int GetWeight(int amount)
        {
            return (int)(amount / CoinsPerPound);
        }

        public int TotalWeight
        {
            get { return GetWeight(amount); }
        }

        //Called for things like buying items
        public void Subtract(int amount)
        {
            if (amount <= this.amount)
                this.amount -= amount;
        }

        //Parameter is declared as ref so we can modify it to let the calling code know
        //that we couldn't hold the full amount. This should never happen however
        //as the weight alone should limit what an entity can carry
        public void Add(ref int amount)
        {
            if (this.amount + amount <= int.MaxValue)
            {
                this.amount += amount;
            }
            else
            {
                this.amount = (int)(int.MaxValue - amount);
                amount = int.MaxValue;
            }
        }

        //Called for things like dropping on the ground etc
        //Added simply for making sense in the context for which it's used
        public void Drop(int amount)
        {
            Subtract(amount);
        }

        public void Give(ref int amount, ref Entity target)
        {
            if (this.amount <= amount)
            {
                target.AddMoney(ref this.amount);
            }
        }
    }
	
	public class RPGKey : Item
	{
		override public bool Use(ref object target, ref Entity wielder)
		{
			return ((Chest)target).KeyRequired == this.ID;

		}
	}

	public class Scroll : Item
	{
		override public bool Use(ref object target, ref Entity wielder)
		{
			throw new NotImplementedException();
		}
	}

	public class Shield : Item
	{
		public ShieldSize Size;
        public int DefensiveValue;

		override public bool Use(ref object target, ref Entity wielder)
		{
			throw new NotImplementedException();
		}

	}

	public class Potion : Item
	{
        public PotionType PotionType;
        public int Amount;
        public int Duration;

		override public bool Use(ref object target, ref Entity wielder)
		{
			throw new NotImplementedException();
		}
	}
}
