using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace OmegaEngine
{
    public class Map
    {
        static Vector2 mapSize;
        public Vector2 startPoint;

        Texture2D sprites;

        /* DEPRECATED. Also was getting in the way of the overloaded constructor. And that's kinda annoying.
        public Map(ContentManager c, SpriteBatch s, Vector2 screenBounds, Rectangle windowBounds, string spriteMap)
        {
            Globals.Content = c;
            Globals.spriteBatch = s;
            Globals.screenBounds = screenBounds;
            Globals.windowBounds = windowBounds;

            sprites = Globals.Content.Load<Texture2D>(spriteMap);

            Globals.map = new List<List<Vector2>>();
            for (int y = 0; y < 20; y++)
            {
                List<Vector2> newRow = new List<Vector2>();
                for (int x = 0; x < 20; x++)
                {
                    newRow.Add(new Vector2(10, 4));
                }
                Globals.map.Add(newRow);
            }

            Globals.startPoint = Vector2.Zero; // Change this ASAP
        }*/

        public Map(ContentManager c, SpriteBatch s, Vector2 screenBounds, Rectangle windowBounds, string spriteMap)
        {
            Globals.Content = c;
            Globals.spriteBatch = s;
            Globals.screenBounds = screenBounds;
            Globals.windowBounds = windowBounds;

            sprites = Globals.Content.Load<Texture2D>(spriteMap);

            Globals.map = new List<List<Vector2>>();
            Globals.enemies = new List<Enemy>();

            XmlDocument reader = new XmlDocument();
            XmlDocument overlayReader = new XmlDocument();
            string filename = "Maps/";
            if (Globals.fileOpen)
                filename = "Saves/" + Globals.slotOpen + "/" + filename;

            if (!File.Exists(filename + Globals.currentMap + "_new.xml"))
                reader.Load(filename + Globals.currentMap + ".xml");
            else
                reader.Load(filename + Globals.currentMap + "_new.xml");

            if (!File.Exists(filename + Globals.currentMap + "_events_new.xml"))
                overlayReader.Load(filename + Globals.currentMap + "_events.xml");
            else
                overlayReader.Load(filename + Globals.currentMap + "_events_new.xml");

            XmlNodeList mapx = reader.GetElementsByTagName("mapx");
            XmlNodeList mapy = reader.GetElementsByTagName("mapy");
            XmlNodeList tileset = reader.GetElementsByTagName("tileset");
            XmlNodeList overlayset = overlayReader.GetElementsByTagName("overlayset");

            mapSize = new Vector2(Convert.ToInt32(mapx[0].InnerText), Convert.ToInt32(mapy[0].InnerText));
            fillEmptyMap(mapSize);
            fillEmptyOverlayMap(mapSize);
            fillEmptyPerms(mapSize);
            fillEmptyOverlayPerms(mapSize);
            fillEmptyEvents(mapSize);
            fillEmptyThrows(mapSize);

            foreach (XmlNode i in tileset)
            {
                int x = Convert.ToInt32(i.Attributes["x"].Value);
                int y = Convert.ToInt32(i.Attributes["y"].Value);
                int tilex = Convert.ToInt32(i.SelectSingleNode("tilex").InnerText);
                int tiley = Convert.ToInt32(i.SelectSingleNode("tiley").InnerText);
                Globals.Permissions perm = (Globals.Permissions)Enum.Parse(typeof(Globals.Permissions), i.SelectSingleNode("perms").InnerText);

                Globals.map[y][x] = new Vector2(tilex, tiley);
                Globals.permissions[y][x] = perm;
            }

            foreach (XmlNode i in overlayset)
            {
                int x = Convert.ToInt32(i.Attributes["x"].Value);
                int y = Convert.ToInt32(i.Attributes["y"].Value);
                int tilex = Convert.ToInt32(i.SelectSingleNode("tilex").InnerText);
                int tiley = Convert.ToInt32(i.SelectSingleNode("tiley").InnerText);
                Globals.Permissions perm = (Globals.Permissions)Enum.Parse(typeof(Globals.Permissions), i.SelectSingleNode("perms").InnerText);

                if (i.SelectSingleNode("enemies") != null)
                {
                    XmlNodeList enemies = i.SelectSingleNode("enemies").ChildNodes;
                    string src = "Paul";
                    Globals.SpriteFace sprFace = Globals.SpriteFace.Down;
                    foreach (XmlAttribute j in i.SelectSingleNode("enemies").Attributes)
                    {
                        if (j.Name == "src")
                            src = j.Value;
                        if (j.Name == "facing")
                            sprFace = (Globals.SpriteFace)Enum.Parse(typeof(Globals.SpriteFace), j.Value);
                    }

                    for (int j = 0; j < enemies.Count; j++)
                    {
                        Globals.battleEnemyTypes type = (Globals.battleEnemyTypes)Enum.Parse(typeof(Globals.battleEnemyTypes), enemies[j].InnerText);
                        if (j == 0)
                            Globals.enemies.Add(new Enemy(src, new Vector2(x, y), sprFace, type));
                        else
                            Globals.enemies[Globals.enemies.Count - 1].addEnemy(type);
                    }
                }

                string evnt = i.SelectSingleNode("event").InnerText;
                foreach (XmlAttribute j in i.SelectSingleNode("event").Attributes)
                {
                    if (j.Name == "throw")
                        Globals.eventThrows[y][x] = (Globals.EventThrow)Enum.Parse(typeof(Globals.EventThrow), j.Value);
                }
                Globals.overlayMap[y][x] = new Vector2(tilex, tiley);
                Globals.overlayPerms[y][x] = perm;
                if (evnt != "nil")
                    Globals.events[y][x] = evnt;
            }
        }

        public static void changeMap(string name, Vector2 startPos)
        {
            Globals.currentMap = name;
            Globals.map = new List<List<Vector2>>();
            Globals.enemies = new List<Enemy>();

            XmlDocument reader = new XmlDocument();
            XmlDocument overlayReader = new XmlDocument();
            string filename = "Maps/";
            if (Globals.fileOpen)
                filename = "Saves/" + Globals.slotOpen + "/" + filename;

            if (!File.Exists(filename + Globals.currentMap + "_new.xml"))
                reader.Load(filename + Globals.currentMap + ".xml");
            else
                reader.Load(filename + Globals.currentMap + "_new.xml");

            if (!File.Exists(filename + Globals.currentMap + "_events_new.xml"))
                overlayReader.Load(filename + Globals.currentMap + "_events.xml");
            else
                overlayReader.Load(filename + Globals.currentMap + "_events_new.xml");

            XmlNodeList mapx = reader.GetElementsByTagName("mapx");
            XmlNodeList mapy = reader.GetElementsByTagName("mapy");
            XmlNodeList tileset = reader.GetElementsByTagName("tileset");
            XmlNodeList overlayset = overlayReader.GetElementsByTagName("overlayset");

            mapSize = new Vector2(Convert.ToInt32(mapx[0].InnerText), Convert.ToInt32(mapy[0].InnerText));
            fillEmptyMap(mapSize);
            fillEmptyOverlayMap(mapSize);
            fillEmptyPerms(mapSize);
            fillEmptyOverlayPerms(mapSize);
            fillEmptyEvents(mapSize);
            fillEmptyThrows(mapSize);

            foreach (XmlNode i in tileset)
            {
                int x = Convert.ToInt32(i.Attributes["x"].Value);
                int y = Convert.ToInt32(i.Attributes["y"].Value);
                int tilex = Convert.ToInt32(i.SelectSingleNode("tilex").InnerText);
                int tiley = Convert.ToInt32(i.SelectSingleNode("tiley").InnerText);
                Globals.Permissions perm = (Globals.Permissions)Enum.Parse(typeof(Globals.Permissions), i.SelectSingleNode("perms").InnerText);

                Globals.map[y][x] = new Vector2(tilex, tiley);
                Globals.permissions[y][x] = perm;
            }

            foreach (XmlNode i in overlayset)
            {
                int x = Convert.ToInt32(i.Attributes["x"].Value);
                int y = Convert.ToInt32(i.Attributes["y"].Value);
                int tilex = Convert.ToInt32(i.SelectSingleNode("tilex").InnerText);
                int tiley = Convert.ToInt32(i.SelectSingleNode("tiley").InnerText);
                Globals.Permissions perm = (Globals.Permissions)Enum.Parse(typeof(Globals.Permissions), i.SelectSingleNode("perms").InnerText);

                if (i.SelectSingleNode("enemies") != null)
                {
                    XmlNodeList enemies = i.SelectSingleNode("enemies").ChildNodes;
                    string src = "Paul";
                    Globals.SpriteFace sprFace = Globals.SpriteFace.Down;
                    foreach (XmlAttribute j in i.SelectSingleNode("enemies").Attributes)
                    {
                        if (j.Name == "src")
                            src = j.Value;
                        if (j.Name == "facing")
                            sprFace = (Globals.SpriteFace)Enum.Parse(typeof(Globals.SpriteFace), j.Value);
                    }

                    for (int j = 0; j < enemies.Count; j++)
                    {
                        Globals.battleEnemyTypes type = (Globals.battleEnemyTypes)Enum.Parse(typeof(Globals.battleEnemyTypes), enemies[j].InnerText);
                        if (j == 0)
                            Globals.enemies.Add(new Enemy(src, new Vector2(x, y), sprFace, type));
                        else
                            Globals.enemies[Globals.enemies.Count - 1].addEnemy(type);
                    }
                }

                string evnt = i.SelectSingleNode("event").InnerText;
                foreach (XmlAttribute j in i.SelectSingleNode("event").Attributes)
                {
                    if (j.Name == "throw")
                        Globals.eventThrows[y][x] = (Globals.EventThrow)Enum.Parse(typeof(Globals.EventThrow), j.Value);
                }

                Globals.overlayMap[y][x] = new Vector2(tilex, tiley);
                Globals.overlayPerms[y][x] = perm;

                if (evnt != "nil")
                    Globals.events[y][x] = evnt;
            }

            Globals.startPoint = startPos;
            setMapBounds();
            Player.updatePos();
        }

        public void mapSpriteMap(string spriteMap)
        {
            sprites = Globals.Content.Load<Texture2D>(spriteMap);
        }

        private static void fillEmptyMap(Vector2 size)
        {
            Globals.map = new List<List<Vector2>>();
            for (int y = 0; y < size.Y; y++)
            {
                List<Vector2> newRow = new List<Vector2>();
                for (int x = 0; x < size.X; x++)
                {
                    newRow.Add(Vector2.Zero);
                }
                Globals.map.Add(newRow);
            }
        }

        private static void fillEmptyOverlayMap(Vector2 size)
        {
            Globals.overlayMap = new List<List<Vector2>>();
            for (int y = 0; y < size.Y; y++)
            {
                List<Vector2> newRow = new List<Vector2>();
                for (int x = 0; x < size.X; x++)
                {
                    newRow.Add(Vector2.Zero);
                }
                Globals.overlayMap.Add(newRow);
            }
        }

        private static void fillEmptyPerms(Vector2 size)
        {
            Globals.permissions = new List<List<Globals.Permissions>>();
            for (int y = 0; y < size.Y; y++)
            {
                List<Globals.Permissions> newRow = new List<Globals.Permissions>();
                for (int x = 0; x < size.X; x++)
                {
                    newRow.Add(Globals.Permissions.Open);
                }
                Globals.permissions.Add(newRow);
            }
        }

        private static void fillEmptyOverlayPerms(Vector2 size)
        {
            Globals.overlayPerms = new List<List<Globals.Permissions>>();
            for (int y = 0; y < size.Y; y++)
            {
                List<Globals.Permissions> newRow = new List<Globals.Permissions>();
                for (int x = 0; x < size.X; x++)
                {
                    newRow.Add(Globals.Permissions.Open);
                }
                Globals.overlayPerms.Add(newRow);
            }
        }

        private static void fillEmptyEvents(Vector2 size)
        {
            Globals.events = new List<List<string>>();
            for (int y = 0; y < size.Y; y++)
            {
                List<string> newRow = new List<string>();
                for (int x = 0; x < size.X; x++)
                {
                    newRow.Add("");
                }
                Globals.events.Add(newRow);
            }
        }

        private static void fillEmptyThrows(Vector2 size)
        {
            Globals.eventThrows = new List<List<Globals.EventThrow>>();
            for (int y = 0; y < size.Y; y++)
            {
                List<Globals.EventThrow> newRow = new List<Globals.EventThrow>();
                for (int x = 0; x < size.X; x++)
                {
                    newRow.Add(Globals.EventThrow.None);
                }
                Globals.eventThrows.Add(newRow);
            }
        }

        public static void setMapBounds()
        {
            Globals.mapBounds = new Rectangle((int)(Globals.screenBounds.X / 2) - Globals.map[0].Count * (Globals.bitWidth / 2), (int)(Globals.screenBounds.Y / 2) - Globals.map.Count * (Globals.bitWidth / 2), Globals.map[0].Count * Globals.bitWidth, Globals.map.Count * Globals.bitWidth);
        }

        public static void modMapTile(string mapName, Vector2 pos, Vector2 newset, bool overlay)
        {
            XmlDocument doc = new XmlDocument();
            string filename = "Maps/" + mapName;
            if (Globals.fileOpen)
                filename = "Saves/" + Globals.slotOpen + "/" + filename;
            if (overlay)
                filename += "_events";
            if (!File.Exists(filename + "_new.xml"))
            {
                FileInfo fi = new FileInfo(filename + ".xml");
                fi.CopyTo(filename + "_new.xml");
            }
            filename += "_new";
            doc.Load(filename + ".xml");

            XmlNodeList docset;

            if (!overlay)
                docset = doc.GetElementsByTagName("tileset");
            else
                docset = doc.GetElementsByTagName("overlayset");

            bool xFound, yFound;
            foreach (XmlNode i in docset)
            {
                xFound = false;
                yFound = false;
                foreach (XmlAttribute j in i.Attributes)
                {
                    if (j.Name == "x" && j.Value == Convert.ToInt32(pos.X).ToString())
                        xFound = true;
                    if (j.Name == "y" && j.Value == Convert.ToInt32(pos.Y).ToString())
                        yFound = true;
                }
                if (xFound && yFound)
                {
                    foreach (XmlNode j in i.ChildNodes)
                    {
                        if (j.Name == "tilex")
                            j.InnerText = Convert.ToInt32(newset.X).ToString();
                        if (j.Name == "tiley")
                            j.InnerText = Convert.ToInt32(newset.Y).ToString();
                    }
                }
            }

            doc.Save(filename + ".xml");
        }

        public static void modMapPerm(string mapName, Vector2 pos, Globals.Permissions newperm, bool overlay)
        {
            XmlDocument doc = new XmlDocument();
            string filename = "Maps/" + mapName;
            if (Globals.fileOpen)
                filename = "Saves/" + Globals.slotOpen + "/" + filename;
            if (overlay)
                filename += "_events";
            if (!File.Exists(filename + "_new.xml"))
            {
                FileInfo fi = new FileInfo(filename + ".xml");
                fi.CopyTo(filename + "_new.xml");
            }
            filename += "_new";
            doc.Load(filename + ".xml");

            XmlNodeList docset;

            if (!overlay)
                docset = doc.GetElementsByTagName("tileset");
            else
                docset = doc.GetElementsByTagName("overlayset");

            bool xFound, yFound;
            foreach (XmlNode i in docset)
            {
                xFound = false;
                yFound = false;
                foreach (XmlAttribute j in i.Attributes)
                {
                    if (j.Name == "x" && j.Value == Convert.ToInt32(pos.X).ToString())
                        xFound = true;
                    if (j.Name == "y" && j.Value == Convert.ToInt32(pos.Y).ToString())
                        yFound = true;
                }
                if (xFound && yFound)
                {
                    foreach (XmlNode j in i.ChildNodes)
                    {
                        if (j.Name == "perms")
                            j.InnerText = newperm.ToString();
                    }
                }
            }

            doc.Save(filename + ".xml");
        }

        public static void modMapEvent(string mapName, Vector2 pos, string thrower, string eventscript)
        {
            XmlDocument doc = new XmlDocument();
            string filename = "Maps/" + mapName;
            if (Globals.fileOpen)
                filename = "Saves/" + Globals.slotOpen + "/" + filename;
            filename += "_events";
            if (!File.Exists(filename + "_new.xml"))
            {
                FileInfo fi = new FileInfo(filename + ".xml");
                fi.CopyTo(filename + "_new.xml");
            }
            filename += "_new";
            doc.Load(filename + ".xml");

            XmlNodeList docset;

            docset = doc.GetElementsByTagName("overlayset");

            bool xFound, yFound;
            foreach (XmlNode i in docset)
            {
                xFound = false;
                yFound = false;
                foreach (XmlAttribute j in i.Attributes)
                {
                    if (j.Name == "x" && j.Value == Convert.ToInt32(pos.X).ToString())
                        xFound = true;
                    if (j.Name == "y" && j.Value == Convert.ToInt32(pos.Y).ToString())
                        yFound = true;
                }
                if (xFound && yFound)
                {
                    foreach (XmlNode j in i.ChildNodes)
                    {
                        if (j.Name == "event")
                        {
                            foreach (XmlAttribute k in j.Attributes)
                            {
                                if (k.Name == "throw")
                                    k.Value = thrower;
                            }
                            j.InnerText = eventscript;
                        }
                    }
                }
            }

            doc.Save(filename + ".xml");
        }

        public static void modMapEnemy(string mapName, Vector2 pos, string src, Globals.SpriteFace sprFace, List<Globals.battleEnemyTypes> types)
        {
            XmlDocument doc = new XmlDocument();
            string filename = "Maps/" + mapName;
            if (Globals.fileOpen)
                filename = "Saves/" + Globals.slotOpen + "/" + filename;
            filename += "_events";
            if (!File.Exists(filename + "_new.xml"))
            {
                FileInfo fi = new FileInfo(filename + ".xml");
                fi.CopyTo(filename + "_new.xml");
            }
            filename += "_new";
            doc.Load(filename + ".xml");

            XmlNodeList docset;

            docset = doc.GetElementsByTagName("overlayset");

            bool xFound, yFound;
            foreach (XmlNode i in docset)
            {
                xFound = false;
                yFound = false;
                foreach (XmlAttribute j in i.Attributes)
                {
                    if (j.Name == "x" && j.Value == Convert.ToInt32(pos.X).ToString())
                        xFound = true;
                    if (j.Name == "y" && j.Value == Convert.ToInt32(pos.Y).ToString())
                        yFound = true;
                }
                if (xFound && yFound)
                {
                    if (i.SelectSingleNode("enemies") != null)
                        i.RemoveChild(i.SelectSingleNode("enemies"));
                    XmlNode newenemy = doc.CreateElement("enemies");

                    XmlAttribute newAttr = doc.CreateAttribute("src");
                    newAttr.Value = src;
                    newenemy.Attributes.Append(newAttr);

                    newAttr = doc.CreateAttribute("facing");
                    newAttr.Value = sprFace.ToString();
                    newenemy.Attributes.Append(newAttr);

                    foreach (Globals.battleEnemyTypes type in types)
                    {
                        XmlNode childenemy = doc.CreateElement("enemy");
                        childenemy.InnerText = type.ToString();
                        newenemy.AppendChild(childenemy);
                    }
                    i.AppendChild(newenemy);
                }
            }

            doc.Save(filename + ".xml");
        }

        public static void modMapEnemy(string mapName, Vector2 pos)
        {
            XmlDocument doc = new XmlDocument();
            string filename = "Maps/" + mapName;
            if (Globals.fileOpen)
                filename = "Saves/" + Globals.slotOpen + "/" + filename;
            filename += "_events";
            if (!File.Exists(filename + "_new.xml"))
            {
                FileInfo fi = new FileInfo(filename + ".xml");
                fi.CopyTo(filename + "_new.xml");
            }
            filename += "_new";
            doc.Load(filename + ".xml");

            XmlNodeList docset;

            docset = doc.GetElementsByTagName("overlayset");

            bool xFound, yFound;
            foreach (XmlNode i in docset)
            {
                xFound = false;
                yFound = false;
                foreach (XmlAttribute j in i.Attributes)
                {
                    if (j.Name == "x" && j.Value == Convert.ToInt32(pos.X).ToString())
                        xFound = true;
                    if (j.Name == "y" && j.Value == Convert.ToInt32(pos.Y).ToString())
                        yFound = true;
                }
                if (xFound && yFound)
                {
                    if (i.SelectSingleNode("enemies") != null)
                        i.RemoveChild(i.SelectSingleNode("enemies"));
                }
            }

            doc.Save(filename + ".xml");
        }

        /************************************************
         * DO NOT USE
         * This was used to test how to use XmlWriter class and methods.
         * Keep as a reference, but if you call it, you'll just create an 10x10 XML map with just 1 tile.
         * Gross.
         * **********************************************/
        public void makeMap()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            XmlWriter writer = XmlWriter.Create("Maps/map_001.xml", settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("map");

            writer.WriteStartElement("mapsize");
            writer.WriteElementString("mapx", "10");
            writer.WriteElementString("mapy", "10");
            writer.WriteEndElement();

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    writer.WriteStartElement("tileset");
                    writer.WriteAttributeString("x", x.ToString());
                    writer.WriteAttributeString("y", y.ToString());
                    writer.WriteElementString("tilex", "6");
                    writer.WriteElementString("tiley", "4");
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        public void Draw()
        {
            int k = 0, l = 0;
            foreach (List<Vector2> i in Globals.map)
            {
                foreach (Vector2 j in i)
                {
                    if (j.X != -1 && j.Y != -1)
                        Globals.spriteBatch.Draw(sprites, new Vector2(k * Globals.bitWidth + Globals.mapBounds.X + Globals.offset.X * Globals.bitWidth, l * Globals.bitWidth + Globals.mapBounds.Y + Globals.offset.Y * Globals.bitWidth), new Rectangle((int)j.X * Globals.bitWidth, (int)j.Y * Globals.bitWidth, Globals.bitWidth, Globals.bitWidth), Color.White);
                    k++;
                }
                l++;
                k = 0;
            }
        }

        public void DrawOverlay()
        {
            int k = 0, l = 0;
            foreach (List<Vector2> i in Globals.overlayMap)
            {
                foreach (Vector2 j in i)
                {
                    if (j.X != -1 && j.Y != -1)
                        Globals.spriteBatch.Draw(sprites, new Vector2(k * Globals.bitWidth + Globals.mapBounds.X + Globals.offset.X * Globals.bitWidth, l * Globals.bitWidth + Globals.mapBounds.Y + Globals.offset.Y * Globals.bitWidth), new Rectangle((int)j.X * Globals.bitWidth, (int)j.Y * Globals.bitWidth, Globals.bitWidth, Globals.bitWidth), Color.White);
                    k++;
                }
                l++;
                k = 0;
            }
        }

        public void DrawEnemies()
        {
            foreach (Enemy i in Globals.enemies)
            {
                Globals.spriteBatch.Draw(i.sprites, new Vector2(i.position.X * Globals.bitWidth + Globals.mapBounds.X + Globals.offset.X * Globals.bitWidth, i.position.Y * Globals.bitWidth + Globals.mapBounds.Y + Globals.offset.Y * Globals.bitWidth), new Rectangle((int)i.getFrameSrc().X * Globals.bitWidth, (int)i.getFrameSrc().Y * Globals.bitWidth, Globals.bitWidth, Globals.bitWidth), Color.White);
            }
        }
    }
    /*string ReadTextFromUrl(string url)
    {
    // WebClient is still convenient
    // Assume UTF8, but detect BOM - could also honor response charset I suppose
    using (var client = new WebClient())
    using (var stream = client.OpenRead(url))
    using (var textReader = new StreamReader(stream, Encoding.UTF8, true))
    {
        return textReader.ReadToEnd();
    }
    }*/
}
