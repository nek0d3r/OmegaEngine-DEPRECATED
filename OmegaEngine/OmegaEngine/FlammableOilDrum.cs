using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmegaEngine
{
    class FlammableOilDrum
    {

        string name { get; set; }
        public int HP { get; set; }
        public int damageDealt { get; set; }
        public int spaceX { get; set; }
        public int spaceY { get; set; }

        public FlammableOilDrum()
        {

        }

        public FlammableOilDrum(string name, int HP, int damageDealt)
        {
            this.name = name;
            this.HP = HP;
            this.damageDealt = damageDealt;
        }

        public FlammableOilDrum(FlammableOilDrum flammableOilDrum)
        {
            name = flammableOilDrum.name;
            HP = flammableOilDrum.HP;
            damageDealt = flammableOilDrum.damageDealt;
        }

        public bool blowUp()
        {
            if (HP > 0)
                return true;
            else
                return false;
        }

        public void impacted(BattleEnemy enemy)
        {
            enemy.loseHP(damageDealt);
        }

        public void impacted(BattleCharacter character)
        {
            character.loseHP(damageDealt);
        }

        public FlammableOilDrum(Globals.flammableOilDrumTypes flammableOilDrumType)
        {
            switch (flammableOilDrumType)
            {
                case Globals.flammableOilDrumTypes.flammableOilDrum:

                    FlammableOilDrum flammableOilDrum = new FlammableOilDrum("Flammable Oil Drum", 5, 10);
                    this.copyFlammableOilDrum(flammableOilDrum);
                    break;

            }
        }

        public void copyFlammableOilDrum(FlammableOilDrum original)
        {
            name = original.name;
            HP = original.HP;
            damageDealt = original.damageDealt;
        }
    }
}