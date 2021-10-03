using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{
    public class SpellBookPage
    {
        public int[] SpellIDs { get; set; }

        public SpellBookPage()
        {
            SpellIDs = new int[Spellbook.SpellsPerPage];
        }

        public bool AddSpell(int id)
        {
            bool ret = false;

            for (int i = 0;i < Spellbook.SpellsPerPage; i++)
            {
                if (SpellIDs[i] == 0)
                {
                    SpellIDs[i] = id;
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        public bool RemoveSpell(int id)
        {
            bool ret = false;

            for (int i = 0; i < Spellbook.SpellsPerPage; i++)
            {
                if (SpellIDs[i] == id)
                {
                    SpellIDs[i] = 0;
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        public bool HasSpell(int id)
        {
            bool ret = false;

            for (int i = 0; i < Spellbook.SpellsPerPage; i++)
            {
                if (SpellIDs[i] == id)
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }
    }

    public class Spellbook
    {
        public List<SpellBookPage> OffSpells;
        public List<SpellBookPage> DefSpells;
        public List<SpellBookPage> MiscSpells;

        public const int SpellsPerPage = 6;

        public Spellbook()
        {
        }

        public void AddSpell(int id, SpellType type)
        {
            bool ret = false;

            switch (type)
            {
                case SpellType.Offensive:
                {
                    if (OffSpells == null)
                        OffSpells = new List<SpellBookPage>();

                    foreach (SpellBookPage page in OffSpells)
                    {
                        if ((ret = page.AddSpell(id)) == true)
                            break;
                    }

                    //all the current pages are full so start a new one
                    if (!ret)
                    {
                        SpellBookPage page = new SpellBookPage();
                        page.AddSpell(id);
                        OffSpells.Add(page);
                    }

                    break;
                }
                case SpellType.Defensive:
                {
                    if (DefSpells == null)
                        DefSpells = new List<SpellBookPage>();

                    foreach (SpellBookPage page in DefSpells)
                    {
                        if ((ret = page.AddSpell(id)) == true)
                            break;
                    }

                    //all the current pages are full so start a new one
                    if (!ret)
                    {
                        SpellBookPage page = new SpellBookPage();
                        page.AddSpell(id);
                        DefSpells.Add(page);
                    }

                    break;
                }
                case SpellType.NonCombat:
                {
                    if (MiscSpells == null)
                        MiscSpells = new List<SpellBookPage>();

                    foreach (SpellBookPage page in MiscSpells)
                    {
                        if ((ret = page.AddSpell(id)) == true)
                            break;
                    }

                    //all the current pages are full so start a new one
                    if (!ret)
                    {
                        SpellBookPage page = new SpellBookPage();
                        page.AddSpell(id);
                        MiscSpells.Add(page);
                    }

                    break;
                }
            }
        }

        public bool MoveSpell(int id, SpellType type, int fromPage, int toPage)
        {
            bool ret = false;

            switch (type)
            {
                case SpellType.Defensive:
                {
                    //check to see if page exists
                    if (!(DefSpells.Count -1 < fromPage))
                        break;

                    //check to see if spell is actually on the fromPage
                    if (DefSpells[fromPage].HasSpell(id))
                    {
                        //return value tells us if toPage has room
                        //if not it's up to the calling code to handle
                        ret = DefSpells[toPage].AddSpell(id);
                    }

                    break;
                }
                case SpellType.NonCombat:
                {
                    //check to see if page exists
                    if (!(MiscSpells.Count - 1 < fromPage))
                        break;

                    //check to see if spell is actually on the fromPage
                    if (MiscSpells[fromPage].HasSpell(id))
                    {
                        //return value tells us if toPage has room
                        //if not it's up to the calling code to handle
                        ret = MiscSpells[toPage].AddSpell(id);
                    }

                    break;
                }
                case SpellType.Offensive:
                {
                    //check to see if page exists
                    if (!(OffSpells.Count - 1 < fromPage))
                        break;

                    //check to see if spell is actually on the fromPage
                    if (OffSpells[fromPage].HasSpell(id))
                    {
                        //return value tells us if toPage has room
                        //if not it's up to the calling code to handle
                        ret = OffSpells[toPage].AddSpell(id);
                    }

                    break;
                }
            }

            return ret;
        }
    }
}
