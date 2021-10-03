using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;


namespace RPGEngine
{
	public abstract class Item : BaseItem
	{
		public ItemType Type;

		public int Cost;
		public bool Magical;
		public AlignmentFlags Alignments;

		public bool QuestItem;
		public byte QuestItemPiece;
		public int QuestID;
		public bool QuestReward;

		public bool Droppable;
		public bool Tradeable;

		public int BaseHP;

		public List<int> Spells;

		public List<Bonus> DefBonuses;
		public List<Bonus> OffBonuses;
		public List<Bonus> MiscBonuses;

		//Is item automatically put in character's hand when picked up
		public bool AutoReady;

		public bool Repairable;

		public bool Unique;

		//ID of the skill required to use this
		public int SkillRequired;

		//All classes that derive from this one must implement this function
		public abstract bool Use(ref Object target, ref Entity wielder);

		public new int ID 
		{ 
			get { return base.ID; } 
		}

		public new string Name;
		public new string Description;
		public new float Weight;
		public new byte StrengthRequired;

		//Used for different purposes depending on the type of derived item
		public new string Tag;

		public new string Filename;     //used to load picture
		public new Texture2D Picture;   //used in inventory/vendor dialogs

	}

	public class EntityItem
	{
		public int ID;
		public int CurHP;

		public EntityItem(int id, int curHP)
		{
			ID = id;
			CurHP = curHP;
		}

		public bool Useable()
		{
			return (CurHP > 0);
		}

		public void Damage(int amount)
		{
			CurHP -= amount;

			if (CurHP < 0)
				CurHP = 0;
		}

		public void Repair(int amount)
		{
			CurHP += amount;

			if (CurHP > GlobalFunctions.GetItem(ID).BaseHP)
				CurHP = GlobalFunctions.GetItem(ID).BaseHP;
		}

		//repair to a percentage of the BaseHP
		public void Repair(float percentage)
		{
			CurHP = (int)(GlobalFunctions.GetItem(ID).BaseHP * percentage);
		}
	}
}
