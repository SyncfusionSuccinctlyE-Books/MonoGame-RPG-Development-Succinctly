using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RPGEngine.FrameworkExamples.CurrencyExample
{
    public class Denomination
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public float Value { get; set; }
        public int Amount { get; set; }
        public string Format = "<s><v>";

        public Denomination(string name, string symbol, float value, string format = null)
        {
            Name = name;
            Symbol = symbol;
            Value = value;

            if (format != null)
                Format = format;
        }

        public Denomination(Denomination denom)
        {
            Name = denom.Name;
            Symbol = denom.Symbol;
            Value = denom.Value;
            Format = denom.Format;
        }
    }

    public class Currency
    {
        public string Name { get; set; }
        public List<Denomination> Denominations { get; set; }
        public string delimiter = ".";

        public float Value { get; set; }

        public void SetAmount(params int[] amounts)
        {
            for (int d = 0; d < Denominations.Count; d++)
            {
                if (d < amounts.Length)
                    Denominations[d].Amount = amounts[d];

                Value += Denominations[d].Amount * Denominations[d].Value;
            }
        }

        public override string ToString()
        {
            string retVal = string.Empty;

            float denomValue = Value;

            // Make sure they are in decending order..
            Denominations = Denominations.OrderByDescending(d => d.Value).ToList();

            foreach (Denomination denom in Denominations)
            {
                retVal += $"{denom.Format.Replace("<s>", denom.Symbol).Replace("<v>", denom.Amount.ToString())}{delimiter}";
            }

            return retVal;
        }
    }

    public class USDollar : Currency
    {
        public USDollar()
        {
            Denominations = new List<Denomination>
            {
                new Denomination("Dollar","$",1),
                new Denomination("Cent", "c", .01f, "<v><s>"),
            };
        }
    }

    public class GBPound : Currency
    {
        public GBPound()
        {
            Denominations = new List<Denomination>
            {
                new Denomination("Pound", "£", 1),
                new Denomination("Penny", "p", .01f, "<v><s>"),
            };
        }
    }

    public class ThlorianFloop : Currency
    {
        public ThlorianFloop()
        {
            Denominations = new List<Denomination>
            {
                new Denomination("Platinum", "pp", 1, "<v><s>" ),
                new Denomination("Gold", "gp", .1f, "<v><s>"),
                new Denomination("Silver", "sp",.05f, "<v><s>"),
                new Denomination("Bronze", "bp",.01f, "<v><s>"),
            };

            delimiter = ", ";
        }
    }

    public class ItemExample
    {
        public string Name { get; set; }
        public virtual Currency Value { get; set; }

        // I think items should have a weight and a volume.
        /// <summary>
        /// Weight of the object, so we can restrict what can be carried by weight
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// Volume, so we can restrict the size of things going into containers etc.
        /// </summary>
        public float Volume { get; set; }
    }

    public class ThlorianSword : ItemExample
    {
        public override Currency Value
        {
            get
            {
                ThlorianFloop v = new ThlorianFloop();
                v.SetAmount(0, 12);

                return v;
            }
        }

        public ThlorianSword()
        {
            Name = "Thlorian Sword";
        }
    }

    public class UKToothpaste : ItemExample
    {
        public override Currency Value
        {
            get
            {
                GBPound v = new GBPound();
                v.SetAmount(1, 15);

                return v;
            }
        }

        public UKToothpaste()
        {
            Name = "UK toothpaste";
        }
    }

    public class USToothpaste : ItemExample
    {
        public override Currency Value
        {
            get
            {
                USDollar v = new USDollar();
                v.SetAmount(0, 99);

                return v;
            }
        }

        public USToothpaste()
        {
            Name = "US toothpaste";
        }
    }

    public class ShopExample
    {
        List<ItemExample> Items = new List<ItemExample>()
        {
            new ThlorianSword(),
            new UKToothpaste(),
            new USToothpaste(),
        };

        public ShopExample()
        {
            
        }

        public List<string> getItems()
        {
            List<string> retVal = new List<string>();

            foreach (ItemExample item in Items)
            {
                retVal.Add($"{item.Name} {item.Value}");
            }

            return retVal;
        }
    }
}

