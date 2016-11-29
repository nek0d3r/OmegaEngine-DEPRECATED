using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace OmegaEngine
{
    public class Save
    {
        XmlDocument reader;

        public Save(int slot)
        {
            Globals.slotOpen = slot;
            saveExists();
        }

        public bool saveExists()
        {
            Directory.CreateDirectory("Saves/1");
            Directory.CreateDirectory("Saves/2");
            Directory.CreateDirectory("Saves/3");

            return File.Exists("Saves/" + Globals.slotOpen + "/save.xml");
        }

        public void newFile()
        {
            Directory.Delete("Saves/" + Globals.slotOpen, true);
            Directory.CreateDirectory("Saves/" + Globals.slotOpen + "/Maps");
            File.Copy("savedefault.xml", "Saves/" + Globals.slotOpen + "/save.xml");

            DirectoryInfo diSource = new DirectoryInfo("Maps");
            DirectoryInfo diTarget = new DirectoryInfo("Saves/" + Globals.slotOpen + "/Maps");
            CopyAll(diSource, diTarget);
        }

        public void openFile()
        {
            if (saveExists())
            {
                reader = new XmlDocument();
                reader.Load("Saves/" + Globals.slotOpen + "/save.xml");

                XmlNodeList keyItems = reader.GetElementsByTagName("item");
                XmlNodeList flags = reader.GetElementsByTagName("flag");
                XmlNodeList money = reader.GetElementsByTagName("money");
                XmlNodeList location = reader.GetElementsByTagName("location");

                foreach (XmlNode i in keyItems)
                {
                    if (Globals.keyItems.ContainsKey(i.InnerText))
                        Globals.keyItems[i.InnerText] = true;
                }
                foreach (XmlNode i in flags)
                {
                    if (Globals.flags.ContainsKey(i.InnerText))
                        Globals.flags[i.InnerText] = true;
                }
                Globals.money = Convert.ToInt32(money[0].InnerText);
                Item item = new Item();
                foreach (XmlAttribute i in location[0].Attributes)
                {
                    if (i.Name == "x")
                        Globals.startPoint.X = Convert.ToInt32(i.Value);
                    if (i.Name == "y")
                        Globals.startPoint.Y = Convert.ToInt32(i.Value);
                    if (i.Name == "facing")
                        Globals.spriteFace = (Globals.SpriteFace)Enum.Parse(typeof(Globals.SpriteFace), i.Value);
                }
                Globals.currentMap = location[0].InnerText;

                Globals.fileOpen = true;
            }
        }

        public void saveFile()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            XmlWriter writer = XmlWriter.Create("Saves/" + Globals.slotOpen + "/save.xml", settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("save");

            writer.WriteStartElement("keyitems");
            foreach (KeyValuePair<string, bool> i in Globals.keyItems)
            {
                if (i.Value)
                    writer.WriteElementString("item", i.Key);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("flags");
            foreach (KeyValuePair<string, bool> i in Globals.flags)
            {
                if (i.Value)
                    writer.WriteElementString("flag", i.Key);
            }
            writer.WriteEndElement();

            writer.WriteElementString("money", Globals.money.ToString());

            writer.WriteStartElement("location");
            writer.WriteAttributeString("x", Convert.ToInt32(Globals.position.X).ToString());
            writer.WriteAttributeString("y", Convert.ToInt32(Globals.position.Y).ToString());
            writer.WriteAttributeString("facing", Globals.spriteFace.ToString());
            writer.WriteString(Globals.currentMap);
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            OverwriteMaps(new DirectoryInfo("Saves/" + Globals.slotOpen + "/Maps"));
            DeleteTempMaps();
        }

        public void DeleteTempMaps()
        {
            if (saveExists())
                DeleteTempMapsRecurse(new DirectoryInfo("Saves/" + Globals.slotOpen + "/Maps"));
        }

        public void DeleteTempMapsRecurse(DirectoryInfo source)
        {
            foreach (FileInfo fi in source.GetFiles())
            {
                if (fi.Name.EndsWith("_new.xml"))
                    fi.Delete();
            }
            foreach (DirectoryInfo di in source.GetDirectories())
                DeleteTempMapsRecurse(di);
        }

        public void OverwriteMaps(DirectoryInfo source)
        {
            foreach (FileInfo fi in source.GetFiles())
            {
                if (fi.Name.EndsWith("_new.xml"))
                    File.Copy(fi.FullName, fi.FullName.Substring(0, fi.FullName.Length - 8) + ".xml", true);
            }
            foreach (DirectoryInfo di in source.GetDirectories())
                OverwriteMaps(di);
        }

        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}

