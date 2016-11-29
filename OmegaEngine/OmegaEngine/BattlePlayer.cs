using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace OmegaEngine
{
    class BattleCharacter
    {

        //Tony: Here's an enum for the character classes, and there's an enum for special moves.
        public enum characterClasses { Crusader, Marksman, Swordsman, StealthOp }
        public enum specialMoves { Protector, Sabotage, Counter, GroundPound }

        //Tony: Instantiates basic variables regarding an enemy unit's data with getters and setters.
        public String name { get; set; }
        public int HP { get; set; }
        public int maxHP { get; set; }
        public int FP { get; set; }
        public int maxFP { get; set; }
        public int str { get; set; }
        public int def { get; set; }
        public int spd { get; set; }
        public int faith { get; set; }
        public int exp { get; set; }
        public int lvl { get; set; }
        public characterClasses characterClass { get; set; }
        public specialMoves specialMove { get; set; }
        public Weapon weapon1 { get; set; }
        public Weapon weapon2 { get; set; }
        public Armor armor { get; set; }
        public string spriteFileName;

        //Tony: Instantiates variables regarding enemy unit's grid locaiton.
        public int spaceX { get; set; }
        public int spaceY { get; set; }

        //Tony: Status effects are marked by bools. Made with getters and setters.
        public bool Bleeding { get; set; }
        //Stat-up -- Program basic stat ups for certain items.
        //Stun -- Crossbow can cause this with a low % of success. 
        public bool Stun { get; set; }
        //Tony: Returns how many turns are left before you can't go.
        public int turnsLeftStunned { get; set; }
        public int statUp { get; set; }
        public int turnsLeftStatUp { set; get; }
        public string statUpType { set; get; }

        //Health Damage Poison -- Not in demo.
        //Nausea -- Not in demo.
        //Drugged -- Not in demo.
        //Confusion -- Not in demo.
        //Toxic -- Not in demo.
        //Healthy -- Not in demo.
        //Tough Skin -- Not in demo.

        //Tony: Weapons will be added a bit later, methinks

        public BattleCharacter()
        {

        }

        public BattleCharacter(String name, int maxHP, int maxFP, int str, int def, int spd, characterClasses characterClass, specialMoves specialMove, Globals.weaponTypes weapon1, Globals.weaponTypes weapon2, Globals.armorTypes armor, string spriteFileName)
        {
            this.name = name;
            this.HP = maxHP;
            this.maxHP = maxHP;
            this.FP = maxFP;
            this.maxFP = maxFP;
            this.str = str;
            this.def = def;
            this.spd = spd;
            this.characterClass = characterClass;
            this.specialMove = specialMove;
            this.weapon1 = new Weapon(weapon1);
            this.weapon2 = new Weapon(weapon2);
            this.armor = new Armor(armor);
            this.spriteFileName = spriteFileName;
            exp = 0;
            lvl = 1;
            Bleeding = false;
            Stun = false;
            turnsLeftStunned = -1;
            statUp = 0;
            turnsLeftStatUp = 0;
            statUpType = "0";

        }

        public BattleCharacter(BattleCharacter battleCharacter)
        {
            name = battleCharacter.name;
            HP = battleCharacter.HP;
            maxHP = battleCharacter.maxHP;
            FP = battleCharacter.FP;
            maxFP = battleCharacter.maxFP;
            str = battleCharacter.str;
            def = battleCharacter.def;
            spd = battleCharacter.spd;
            characterClass = battleCharacter.characterClass;
            specialMove = battleCharacter.specialMove;
            weapon1 = battleCharacter.weapon1;
            weapon2 = battleCharacter.weapon2;
            armor = battleCharacter.armor;
            spriteFileName = battleCharacter.spriteFileName;
            exp = battleCharacter.exp;
            lvl = battleCharacter.lvl;
            Bleeding = battleCharacter.Bleeding;
            Stun = battleCharacter.Stun;
            turnsLeftStunned = battleCharacter.turnsLeftStunned;
            statUp = battleCharacter.statUp;
            turnsLeftStatUp = battleCharacter.turnsLeftStatUp;
            statUpType = battleCharacter.statUpType;

        }

        public void loseHP(int damage)
        {
            HP -= damage;
        }

        public void gainHP(int HPGain)
        {
            HP += HPGain;
            if (HP > maxHP)
                HP = maxHP;
        }

        //Tony: Checks for Stun effect.
        public bool canGo()
        {
            if (turnsLeftStatUp > -1)
                turnsLeftStatUp--;
            if (turnsLeftStatUp == -1)
            {
                if (statUpType == "0")
                    str -= statUp;
                else if (statUpType == "1")
                    spd -= statUp;
                else if (statUpType == "2")
                    def -= statUp;
                statUp = 0;
                statUpType = "0";
            }
            if (Stun)
            {
                turnsLeftStunned--;
                if (turnsLeftStunned == 0)
                    Stun = false;
                return false;
            }
            else
                return true;
        }

        public void Stunned()
        {
            Stun = true;
            turnsLeftStunned = 3;
        }

        public void tempStatIncrease(int turnsLeftStat, int statUp, string rnd)
        {
            statUpType = rnd;
            switch (statUpType)
            {
                case "0":
                    {
                        str += statUp;
                        turnsLeftStatUp = turnsLeftStat;
                        return;
                    }
                case "1":
                    {
                        spd += statUp;
                        turnsLeftStatUp = turnsLeftStat;
                        return;
                    }
                case "2":
                    {
                        def += statUp;
                        turnsLeftStatUp = turnsLeftStat;
                        return;
                    }
                default:
                    {
                        str += statUp;
                        turnsLeftStatUp = turnsLeftStat;
                        return;
                    }
            }
        }

        public int[] LvlUp()
        {

            int[] statUps = new int[6];

            int maxHP_Up;
            int strUp;
            int spdUp;
            int defUp;
            int maxFP_Up;
            int faithUp;

            Random rnd = new Random();

            switch (characterClass)
            {
                case characterClasses.Crusader:
                    {
                        maxHP_Up = 3;
                        maxHP += maxHP_Up;
                        statUps[0] = maxHP_Up;

                        strUp = rnd.Next(2, 4);
                        str += strUp;
                        statUps[1] = strUp;

                        spdUp = rnd.Next(2, 4);
                        spd += spdUp;
                        statUps[2] = spdUp;

                        defUp = rnd.Next(2, 4);
                        def += defUp;
                        statUps[3] = defUp;

                        maxFP_Up = rnd.Next(2, 4);
                        maxFP += maxFP_Up;
                        statUps[4] = maxFP_Up;

                        faithUp = rnd.Next(2, 4);
                        faith += faithUp;
                        statUps[5] = faithUp;

                        return statUps;
                    }
                case characterClasses.Marksman:
                    {
                        maxHP_Up = rnd.Next(1, 3);
                        maxHP += maxHP_Up;
                        statUps[0] = maxHP_Up;

                        strUp = rnd.Next(1, 3);
                        str += strUp;
                        statUps[1] = strUp;

                        spdUp = rnd.Next(3, 5);
                        spd += spdUp;
                        statUps[2] = spdUp;

                        defUp = rnd.Next(1, 3);
                        def += defUp;
                        statUps[3] = defUp;

                        maxFP_Up = rnd.Next(3, 5);
                        maxFP += maxFP_Up;
                        statUps[4] = maxFP_Up;

                        faithUp = rnd.Next(3, 5);
                        faith += faithUp;
                        statUps[5] = faithUp;

                        return statUps;
                    }
                case characterClasses.Swordsman:
                    {
                        maxHP_Up = rnd.Next(1, 3);
                        maxHP += maxHP_Up;
                        statUps[0] = maxHP_Up;

                        strUp = rnd.Next(3, 5);
                        str += strUp;
                        statUps[1] = strUp;

                        spdUp = rnd.Next(1, 3);
                        spd += spdUp;
                        statUps[2] = spdUp;

                        defUp = rnd.Next(3, 5);
                        def += defUp;
                        statUps[3] = defUp;

                        maxFP_Up = rnd.Next(3, 5);
                        maxFP += maxFP_Up;
                        statUps[4] = maxFP_Up;

                        faithUp = rnd.Next(3, 5);
                        faith += faithUp;
                        statUps[5] = faithUp;

                        return statUps;
                    }
                case characterClasses.StealthOp:
                    {
                        maxHP_Up = rnd.Next(3, 5);
                        maxHP += maxHP_Up;
                        statUps[0] = maxHP_Up;

                        strUp = rnd.Next(1, 3);
                        str += strUp;
                        statUps[1] = strUp;

                        spdUp = rnd.Next(3, 5);
                        spd += spdUp;
                        statUps[2] = spdUp;

                        defUp = rnd.Next(3, 5);
                        def += defUp;
                        statUps[3] = defUp;

                        maxFP_Up = rnd.Next(1, 3);
                        maxFP += maxFP_Up;
                        statUps[4] = maxFP_Up;

                        faithUp = rnd.Next(1, 3);
                        faith += faithUp;
                        statUps[5] = faithUp;

                        return statUps;
                    }
                default:   //Default is Crusader's level up because wynaught?
                    {
                        maxHP_Up = 3;
                        maxHP += maxHP_Up;
                        statUps[0] = maxHP_Up;

                        strUp = rnd.Next(2, 4);
                        str += strUp;
                        statUps[1] = strUp;

                        spdUp = rnd.Next(2, 4);
                        spd += spdUp;
                        statUps[2] = spdUp;

                        defUp = rnd.Next(2, 4);
                        def += defUp;
                        statUps[3] = defUp;

                        maxFP_Up = rnd.Next(2, 4);
                        maxFP += maxFP_Up;
                        statUps[4] = maxFP_Up;

                        faithUp = rnd.Next(2, 4);
                        faith += faithUp;
                        statUps[5] = faithUp;

                        return statUps;
                    }
            }
        }

        public bool canLvlUp()
        {
            switch (lvl)
            {
                case 1:
                    {
                        if (exp >= 100)
                            return true;
                        else
                            return false;
                    }
                case 2:
                    {
                        if (exp >= 150)
                            return true;
                        else
                            return false;
                    }
                case 3:
                    {
                        if (exp >= 200)
                            return true;
                        else
                            return false;
                    }
                case 4:
                    {
                        if (exp >= 250)
                            return true;
                        else
                            return false;
                    }
                case 5:
                    {
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

    }
}