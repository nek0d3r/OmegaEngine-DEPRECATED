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
    class BattleEnemy
    {

        //Tony: Instantiates basic variables regarding an enemy unit's data with getters and setters.
        public String name { get; set; }
        public int HP { get; set; }
        public int maxHP { get; set; }
        public int str { get; set; }
        public int def { get; set; }
        public int spd { get; set; }
        public Weapon weapon1 { get; set; }
        public Weapon weapon2 { get; set; }
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

        public BattleEnemy()
        {

        }

        public BattleEnemy(String name, int maxHP, int str, int def, int spd, Globals.weaponTypes weapon1, Globals.weaponTypes weapon2, string spriteFileName)
        {
            this.name = name;
            this.HP = maxHP;
            this.maxHP = maxHP;
            this.str = str;
            this.def = def;
            this.spd = spd;
            this.weapon1 = new Weapon(weapon1);
            this.weapon2 = new Weapon(weapon2);
            this.spriteFileName = spriteFileName;
            Bleeding = false;
            Stun = false;
            turnsLeftStunned = 0;
            statUp = 0;
            turnsLeftStatUp = 0;
            statUpType = "0";

        }

        public BattleEnemy(BattleEnemy battleEnemy)
        {
            name = battleEnemy.name;
            HP = battleEnemy.HP;
            maxHP = battleEnemy.maxHP;
            str = battleEnemy.str;
            def = battleEnemy.def;
            spd = battleEnemy.spd;
            weapon1 = battleEnemy.weapon1;
            weapon2 = battleEnemy.weapon2;
            spriteFileName = battleEnemy.spriteFileName;
            Bleeding = battleEnemy.Bleeding;
            Stun = battleEnemy.Stun;
            turnsLeftStunned = battleEnemy.turnsLeftStunned;
            statUp = battleEnemy.statUp;
            turnsLeftStatUp = battleEnemy.turnsLeftStatUp;
            statUpType = battleEnemy.statUpType;

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

        public BattleEnemy(Globals.battleEnemyTypes battleEnemyType)
        {
            switch (battleEnemyType)
            {
                case Globals.battleEnemyTypes.patrolTrooperGround:

                    BattleEnemy patrolTrooperGround = new BattleEnemy("Patrol Trooper", 20, 2, 1, 5, Globals.weaponTypes.nightStick, Globals.weaponTypes.hand, "Assets/enemy");
                    this.copyBattleEnemy(patrolTrooperGround);
                    break;

                case Globals.battleEnemyTypes.patrolTrooperJetpack:

                    BattleEnemy patrolTrooperJetpack = new BattleEnemy("Jetpacked Patrol Trooper", 20, 2, 1, 5, Globals.weaponTypes.nightStick, Globals.weaponTypes.hand, "Assets/enemy");
                    this.copyBattleEnemy(patrolTrooperJetpack);
                    break;

                case Globals.battleEnemyTypes.riotTrooperGround:

                    BattleEnemy riotTrooperGround = new BattleEnemy("Riot Trooper", 25, 3, 2, 5, Globals.weaponTypes.nightStick, Globals.weaponTypes.homeforgedPistol, "Assets/enemy");
                    this.copyBattleEnemy(riotTrooperGround);
                    break;

                case Globals.battleEnemyTypes.riotTrooperJetpack:

                    BattleEnemy riotTrooperJetpack = new BattleEnemy("Riot Trooper", 25, 3, 2, 5, Globals.weaponTypes.nightStick, Globals.weaponTypes.homeforgedPistol, "Assets/enemy");
                    this.copyBattleEnemy(riotTrooperJetpack);
                    break;

                case Globals.battleEnemyTypes.ajax:

                    BattleEnemy ajax = new BattleEnemy("Ajax", 35, 7, 4, 17, Globals.weaponTypes.ajaxPistol, Globals.weaponTypes.hand, "Assets/enemy");
                    this.copyBattleEnemy(ajax);
                    break;

                case Globals.battleEnemyTypes.barrin:

                    BattleEnemy barrin = new BattleEnemy("Barrin", 36, 8, 5, 15, Globals.weaponTypes.barrinNightStick, Globals.weaponTypes.hand, "Assets/enemy");
                    this.copyBattleEnemy(barrin);
                    break;

            }
        }

        public void copyBattleEnemy(BattleEnemy original)
        {
            name = original.name;
            HP = original.HP;
            maxHP = original.maxHP;
            str = original.str;
            def = original.def;
            spd = original.spd;
            weapon1 = original.weapon1;
            weapon2 = original.weapon2;
            spriteFileName = original.spriteFileName;
            Bleeding = original.Bleeding;
            Stun = original.Stun;
            turnsLeftStunned = original.turnsLeftStunned;
            statUp = original.statUp;
            turnsLeftStatUp = original.turnsLeftStatUp;
            statUpType = original.statUpType;
        }
    }
}