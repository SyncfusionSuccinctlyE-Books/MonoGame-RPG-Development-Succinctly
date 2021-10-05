using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;


namespace RPGEngine
{
    

    public static class Globals
    {
        public static StatGenerationType StatGenType = StatGenerationType.Points;
        public static int StatGenPoints = 20;
        public static List<Stat> Stats;
        public const int MaxWeightMultiplier = 3;
        public static int StatStartValue = 50;
        public static int MaxStatValue = 100;
        public static List<Race> Races;
        public static List<EntityClass> Classes;

        //hold object to call and their mapped methods for conversations
        //this needs to be in the RPGEngine namespace as classes that use it are here
        //and wouldn't know about the game code
        public static Dictionary<ConversationFunctions, object> FunctionClasses;
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

        public static void LoadStats()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(@"Content\Data\stats.xml", settings))
            {
                Stat stat;

                Globals.Stats = new List<Stat>();

                while (reader.Read())
                {
                    stat = new Stat();

                    try
                    {
                        reader.ReadToFollowing("Name");
                        stat.Name = reader.ReadElementContentAsString();
                        stat.Abbreviation = reader.ReadElementContentAsString();
                        stat.Description = reader.ReadElementContentAsString();

                        Globals.Stats.Add(stat);
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
        }

        public static void LoadRaces()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(@"Content\Data\races.xml", settings))
            {
                Race race;

                Globals.Races= new List<Race>();

                while (reader.Read())
                {
                    race = new Race();

                    try
                    {
                        reader.ReadToFollowing("Name");
                        race.Name = reader.ReadElementContentAsString();
                        race.Description = reader.ReadElementContentAsString();

                        Globals.Races.Add(race);
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
        }

        public static void LoadClasses()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            using (XmlReader reader = XmlReader.Create(@"Content\Data\classes.xml", settings))
            {
                EntityClass cls;

                Globals.Classes = new List<EntityClass>();

                while (reader.Read())
                {
                    cls = new EntityClass();

                    try
                    {
                        reader.ReadToFollowing("Name");
                        cls.Name = reader.ReadElementContentAsString();
                        cls.Description = reader.ReadElementContentAsString();

                        Globals.Classes.Add(cls);
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
        }

        public static void SaveCharacter(Character character)
        {
            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForDomain();

            IsolatedStorageFileStream fs = null;
            try
            {
                fs = savegameStorage.OpenFile(character.Name + ".dat", System.IO.FileMode.OpenOrCreate);
            }
            catch (IsolatedStorageException e)
            {

            }
            if (fs != null)
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    byte[] bytes = SerializeToBytes(character);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public static Character LoadCharacter(string name)
        {
            IsolatedStorageFileStream fs = null;

            try
            {
                IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForDomain();
                fs = savegameStorage.OpenFile(name + ".dat", System.IO.FileMode.Open);
            }
            catch (IsolatedStorageException e)
            {

            }
            if (fs != null)
            {

                using (BinaryReader reader = new BinaryReader(fs))
                {
                    byte[] bytes = new byte[fs.Length];

                    fs.Read(bytes, 0, (int)fs.Length);
                    return (Character)DeserializeFromBytes(bytes);
                }
            }
            else
            {
                return null;
            }
        }

        public static string[] GetCharacterNames()
        {
            IsolatedStorageFileStream fs = null;

            try
            {
                IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForDomain();
                return savegameStorage.GetFileNames("*.dat");
            }
            catch (IsolatedStorageException e)
            {
                return null;
            }
        }

        public static byte[] SerializeToBytes<T>(T item)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, item);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }
        public static object DeserializeFromBytes(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(bytes))
            {
                return formatter.Deserialize(stream);
            }
        }
    }
}