using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmegaEngine
{
    class LightFixture
    {

        string name { get; set; }
        public int HP { get; set; }
        public int damageDealt { get; set; }
        public int spaceX { get; set; }
        public int spaceY { get; set; }

        public LightFixture()
        {

        }

        public LightFixture(string name, int HP, int damageDealt)
        {
            this.name = name;
            this.HP = HP;
            this.damageDealt = damageDealt;
        }

        public LightFixture(LightFixture lightFixture)
        {
            name = lightFixture.name;
            HP = lightFixture.HP;
            damageDealt = lightFixture.damageDealt;
        }

        public bool willFall()
        {
            if (HP > 0)
                return true;
            else
                return false;
        }

        public void fell(BattleEnemy enemy)
        {
            enemy.loseHP(damageDealt);
            enemy.Stun = true;
        }

        public void fell(BattleCharacter character)
        {
            character.loseHP(damageDealt);
            character.Stun = true;
        }

        public LightFixture(Globals.lightFixtureTypes lightFixtureType)
        {
            switch (lightFixtureType)
            {
                case Globals.lightFixtureTypes.lightFixture:

                    LightFixture lightFixture = new LightFixture("Light Fixture", 3, 6);
                    this.copyLightFixture(lightFixture);
                    break;

            }
        }

        public void copyLightFixture(LightFixture original)
        {
            name = original.name;
            HP = original.HP;
            damageDealt = original.damageDealt;
        }
    }
}