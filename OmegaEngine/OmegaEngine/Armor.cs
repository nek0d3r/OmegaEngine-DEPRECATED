using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmegaEngine
{
    class Armor
    {
        //Tony: Instantiates a few variables related to Armor.
        public String name { set; get; }
        public int baseDamageReduction { get; set; }

        //Tony: Some armor types increase stats, so every time information is requested, armor will have to be checked regarding stats. Remind me to program other stats if Joe says anything other than spd can be increased.
        public int spdUp { set; get; }

        public Armor()
        {

        }

        public Armor(String name, int baseDamageReduction, int spdUp)
        {
            this.name = name;
            this.baseDamageReduction = baseDamageReduction;
            this.spdUp = spdUp;

        }

        public Armor(Armor armor)
        {
            name = armor.name;
            baseDamageReduction = armor.baseDamageReduction;
            spdUp = armor.spdUp;

        }

        public Armor(Globals.armorTypes armorType)
        {
            switch (armorType)
            {
                case Globals.armorTypes.exoArmor:

                    Armor exoArmor = new Armor("Exo Armor", 1, 1);
                    this.copyArmor(exoArmor);
                    break;

                case Globals.armorTypes.superArmor:

                    Armor superArmor = new Armor("Super Armor", 20, -20);
                    this.copyArmor(superArmor);
                    break;

            }
        }

        public void copyArmor(Armor original)
        {
            name = original.name;
            baseDamageReduction = original.baseDamageReduction;
            spdUp = original.spdUp;
        }

    }
}