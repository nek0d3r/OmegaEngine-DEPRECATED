using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmegaEngine
{
    class Item
    {

        //Tony: Here's an enum for the item types.
        public enum itemTypes { HP_Up, Food, RandomStatUp, StrUp, SpdUp, DefUp }

        //Tony: Sets up basic variables regarding items. The enum Type should say which it is.

        public string name { get; set; }
        public int upBy { get; set; }
        public int index { get; set; }
        public itemTypes itemType { get; set; }
        public int turns;
        public int buyingPrice { get; set; }
        public int sellingPrice { get; set; }

        public Item()
        {

        }

        public Item(string name, int upBy, int index, int turns, int buyingPrice, int sellingPrice)
        {
            this.name = name;
            this.upBy = upBy;
            this.index = index;
            itemType = (itemTypes)index;
            this.turns = turns;
            this.buyingPrice = buyingPrice;
            this.sellingPrice = sellingPrice;
        }

        public Item(Item item)
        {
            name = item.name;
            upBy = item.upBy;
            index = item.index;
            itemType = item.itemType;
            turns = item.turns;
            buyingPrice = item.buyingPrice;
            sellingPrice = item.sellingPrice;

        }

        //Tony: For when an item is used by an enemy, use this method:
        public void useByEnemy(BattleEnemy enemy)
        {
            switch (this.itemType)
            {
                case itemTypes.HP_Up:
                    {
                        enemy.gainHP(upBy);
                        return;
                    }
                case itemTypes.Food:
                    {
                        enemy.gainHP(upBy);
                        return;
                    }
                case itemTypes.RandomStatUp:
                    {
                        Random i = new Random(3);
                        if (i.ToString() == "0")
                        {
                            enemy.tempStatIncrease(turns, upBy, i.ToString());
                            return;
                        }
                        else if (i.ToString() == "1")
                        {
                            enemy.tempStatIncrease(turns, upBy, i.ToString());
                            return;
                        }
                        else if (i.ToString() == "2")
                        {
                            enemy.tempStatIncrease(turns, upBy, i.ToString());
                            return;
                        }
                        else return;
                    }
                case itemTypes.StrUp:
                    {
                        enemy.tempStatIncrease(turns, upBy, "0");
                        return;
                    }
                case itemTypes.SpdUp:
                    {
                        enemy.tempStatIncrease(turns, upBy, "1");
                        return;
                    }
                case itemTypes.DefUp:
                    {
                        enemy.tempStatIncrease(turns, upBy, "2");
                        return;
                    }
                default:
                    {
                        enemy.gainHP(upBy);
                        return;
                    }
            }
        }

        public bool enemyCanUseItem(BattleEnemy enemy)
        {
            if (enemy.turnsLeftStatUp > -1)
                return false;
            else
                return true;
        }

        public Item(Globals.itemTypes itemType)
        {
            switch (itemType)
            {
                case Globals.itemTypes.smallRationPack:

                    Item smallRationPack = new Item("Small Ration Pack", 5, 0, -1, 50, 25);
                    this.copyItem(smallRationPack);
                    break;

                case Globals.itemTypes.staleBread:

                    Item staleBread = new Item("Stale Bread", 8, 1, -1, 75, 38);
                    this.copyItem(staleBread);
                    break;

                case Globals.itemTypes.jerky:

                    Item jerky = new Item("Jerky", 10, 1, -1, 100, 50);
                    this.copyItem(jerky);
                    break;

                case Globals.itemTypes.oldVitamins:

                    Item oldVitamins = new Item("Old Vitamins", 4, 2, 3, 500, 250);
                    this.copyItem(oldVitamins);
                    break;

                case Globals.itemTypes.minorStatUpPillStrUp:

                    Item minorStatUpPillStrUp = new Item("Minor Stat-up Pill", 2, 3, 2, 200, 100);   //Index 3 is for StrUp
                    this.copyItem(minorStatUpPillStrUp);
                    break;

                case Globals.itemTypes.minorStatUpPillSpdUp:

                    Item minorStatUpPillSpdUp = new Item("Minor Stat-up Pill", 2, 4, 2, 200, 100);   //Index 4 is for SpdUp
                    this.copyItem(minorStatUpPillSpdUp);
                    break;

                case Globals.itemTypes.minorStatUpPillDefUp:

                    Item minorStatUpPillDefUp = new Item("Minor Stat-up Pill", 2, 5, 2, 200, 100);   //Index 5 is for DefUp
                    this.copyItem(minorStatUpPillDefUp);
                    break;
            }
        }

        public void copyItem(Item original)
        {
            name = original.name;
            upBy = original.upBy;
            index = original.index;
            itemType = original.itemType;
            turns = original.turns;
            buyingPrice = original.buyingPrice;
            sellingPrice = original.sellingPrice;
        }
    }
}