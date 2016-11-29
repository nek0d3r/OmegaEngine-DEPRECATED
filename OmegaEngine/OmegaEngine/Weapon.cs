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
    class Weapon
    {
        //Tony: Variables associated with weapons instantiated here.
        public String name { get; set; }
        public int baseDamage { get; set; }
        public int maxWeaponHP { get; set; }
        public int weaponHP { get; set; }
        public int durability { get; set; }
        public int buyingPrice { get; set; }
        public int sellingPrice { get; set; }
        public bool weaponDisabled { get; set; }
        public int turnsLeftDisabled { get; set; }
        public bool useByCrusader { get; set; }
        public bool useByMarksman { get; set; }
        public bool useBySwordsman { get; set; }
        public bool useByStealthOp { get; set; }
        public bool canBeDisabled { get; set; }
        public bool isHand { get; set; }


        //Tony: longrange being set to true makes the weapon long range. Setting it to false makes it short range.
        private bool longRange { get; set; }

        public Weapon()
        {

            name = "Hand";
            baseDamage = -1;
            maxWeaponHP = -1;
            durability = -1;
            buyingPrice = -1;
            sellingPrice = -1;
            weaponDisabled = false;
            canBeDisabled = false;
            isHand = true;

        }

        public Weapon(String name, int baseDamage, int maxWeaponHP, bool longRange, int durability, int buyingPrice, int sellingPrice, bool useByCrusader, bool useByMarksman, bool UseBySwordsman, bool useByStealthOp, bool canBeDisabled)
        {
            this.name = name;
            this.baseDamage = baseDamage;
            this.maxWeaponHP = maxWeaponHP;
            this.weaponHP = maxWeaponHP;
            this.longRange = longRange;
            this.durability = durability;
            this.buyingPrice = buyingPrice;
            this.sellingPrice = sellingPrice;
            this.useByCrusader = useByCrusader;
            this.useByMarksman = useByMarksman;
            this.useBySwordsman = useBySwordsman;
            this.useByStealthOp = useByStealthOp;
            this.canBeDisabled = canBeDisabled;
            this.isHand = isHand;
            weaponDisabled = false;
        }

        public Weapon(Weapon weapon)
        {
            name = weapon.name;
            baseDamage = weapon.baseDamage;
            maxWeaponHP = weapon.maxWeaponHP;
            weaponHP = weapon.weaponHP;
            longRange = weapon.longRange;
            durability = weapon.durability;
            buyingPrice = weapon.buyingPrice;
            sellingPrice = weapon.sellingPrice;
            useByCrusader = weapon.useByCrusader;
            useByMarksman = weapon.useByMarksman;
            useBySwordsman = weapon.useBySwordsman;
            useByStealthOp = weapon.useByStealthOp;
            canBeDisabled = weapon.canBeDisabled;
            isHand = weapon.isHand;
            weaponDisabled = false;
        }

        public void loseHP(int damage)
        {
            weaponHP -= damage;
            if (weaponHP == -1)
                weaponHP--;
        }

        public void gainHP(int HPGain)
        {
            weaponHP += HPGain;
        }

        public void useWeapon()
        {
            durability--;
            //Insert a line of code that removes this weapon from a player's/enemy's weaponList when weapon is destroyed by a durability of 0.
        }

        //Tony: This should be run once a player/enemy starts his/her turn. DO NOT CHECK IF A WEAPON CAN BE USED WITH THIS METHOD OTHERWISE. (If a weapon is disabled, you'll decrease the number of turns it is disabled by one every time you use it. Simply get the weaponDisabled value (and remember that said value being false means it can be used.)
        public bool canUse(BattleEnemy enemy)
        {
            if (durability == 0)
            {
                if (this == enemy.weapon1)
                    enemy.weapon1 = null;
                else if (this == enemy.weapon2)
                    enemy.weapon2 = null;
                return false;
            }
            if (weaponDisabled)
            {
                turnsLeftDisabled--;

                if (turnsLeftDisabled == 0)
                {
                    weaponHP = maxWeaponHP;
                    weaponDisabled = false;
                }

                return false;
            }
            else if (weaponHP <= 0)
            {
                weaponDisabled = true;
                turnsLeftDisabled = 3;
                return false;
            }
            else
                return true;
        }

        public bool canUse(BattleCharacter character)
        {
            if (durability == 0)
            {
                if (this == character.weapon1)
                    character.weapon1 = null;
                else if (this == character.weapon2)
                    character.weapon2 = null;
                return false;
            }
            if (weaponDisabled)
            {
                turnsLeftDisabled--;

                if (turnsLeftDisabled == 0)
                {
                    weaponHP = maxWeaponHP;
                    weaponDisabled = false;
                }

                return false;
            }
            else if (weaponHP <= 0)
            {
                weaponDisabled = true;
                turnsLeftDisabled = 3;
                return false;
            }
            else
                return true;
        }

        public bool canBeEquipped(BattleCharacter battleCharacter)
        {
            switch (battleCharacter.characterClass)
            {
                case BattleCharacter.characterClasses.Crusader:

                    return useByCrusader;

                case BattleCharacter.characterClasses.Marksman:

                    return useByMarksman;

                case BattleCharacter.characterClasses.Swordsman:

                    return useBySwordsman;

                case BattleCharacter.characterClasses.StealthOp:

                    return useByStealthOp;

                default:

                    return false;

            }
        }

        public Weapon(Globals.weaponTypes weaponType)
        {
            switch (weaponType)
            {
                case Globals.weaponTypes.hand:

                    Weapon none = new Weapon();
                    this.copyWeapon(none);
                    break;

                case Globals.weaponTypes.Spirit:

                    Weapon Spirit = new Weapon("Spirit", 5, -1, false, -1, -1, -1, true, false, false, false, false);
                    this.copyWeapon(Spirit);
                    break;

                case Globals.weaponTypes.homeforgedBlade:

                    Weapon homeforgedBlade = new Weapon("Homeforged Blade", 4, 30, false, 30, -1, 300, true, false, true, false, true);
                    this.copyWeapon(homeforgedBlade);
                    break;

                case Globals.weaponTypes.kunais:

                    Weapon kunais = new Weapon("Kunais", 4, 15, true, 10, 450, 225, true, false, true, false, true);
                    this.copyWeapon(kunais);
                    break;

                case Globals.weaponTypes.homeforgedPistol:

                    Weapon homeforgedPistol = new Weapon("Homeforged Pistol", 1, 30, true, 30, 400, 200, false, true, true, false, true);
                    this.copyWeapon(homeforgedPistol);
                    break;

                case Globals.weaponTypes.nightStick:

                    Weapon nightStick = new Weapon("Night Stick", 3, 30, false, 30, 500, 250, true, true, true, true, true);
                    this.copyWeapon(nightStick);
                    break;

                case Globals.weaponTypes.crossbow:

                    Weapon crossbow = new Weapon("Crossbow", 2, 35, true, 35, 500, 250, false, false, false, true, true);
                    this.copyWeapon(crossbow);
                    break;

                case Globals.weaponTypes.barrinNightStick:

                    Weapon barrinNightStick = new Weapon("Night Stick", 3, 30, false, 30, 500, 250, false, false, false, false, false);
                    this.copyWeapon(barrinNightStick);
                    break;

                case Globals.weaponTypes.ajaxPistol:

                    Weapon ajaxPistol = new Weapon("Homeforged Pistol", 1, 30, true, 30, 400, 200, false, false, false, false, false);
                    this.copyWeapon(ajaxPistol);
                    break;

            }

        }

        public void copyWeapon(Weapon original)
        {
            name = original.name;
            baseDamage = original.baseDamage;
            maxWeaponHP = original.maxWeaponHP;
            weaponHP = original.weaponHP;
            longRange = original.longRange;
            durability = original.durability;
            buyingPrice = original.buyingPrice;
            sellingPrice = original.sellingPrice;
            useByCrusader = original.useByCrusader;
            useByMarksman = original.useByMarksman;
            useBySwordsman = original.useBySwordsman;
            useByStealthOp = original.useByStealthOp;
            canBeDisabled = original.canBeDisabled;
            weaponDisabled = false;
        }
    }
}