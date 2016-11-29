using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace OmegaEngine
{
    /***************************************************************************************
     * This class is dedicated entirely to variables accessible to all classes in the project.
     * If you need to add any, just throw them in here with a get/set.
     * ************************************************************************************/
    public static class Globals
    {
        public enum Permissions { Open, Impassable };			// Permissions available
        public enum SpriteFace { Up, Down, Left, Right };       // SpriteFace identifies the direction a specific sprite is facing for source rectangle purposes.
        public enum EventThrow { None, Interact, Stand, Push }; // What requires an event script to run. Interact if acting next to an object, Stand if you have to stand on entity.

        public static SpriteBatch spriteBatch;                  // SpriteBatch object passed up from main driver
        public static ContentManager Content;                   // ContentManager object passed up from main driver

        public static Rectangle windowBounds;                   // Will contain the bounds of the window, set to box to highest resolution possible and center
        public static Vector2 screenBounds;                     // Contains bounds of entire screen resolution
        public static Rectangle mapBounds;                      // Contains bounds of the map in pixels, if necessary (smaller than window bounds)
        public static int bitWidth;                             // Contains the size of the sprites used

        public static List<List<Vector2>> map;                  // Matrix of tile visuals
        public static List<List<Vector2>> overlayMap;           // Matrix of overlay tile visuals
        public static List<List<Permissions>> permissions;      // Matrix of tile permissions
        public static List<List<Permissions>> overlayPerms;     // Matrix of overlay tile permissions
        public static List<List<string>> events;                // Matrix of event handler scripts
        public static List<List<EventThrow>> eventThrows;       // Matrix of event throwers
        public static Fade fade = new Fade();					// Control for color/alpha fading
        public static bool pause = false;                       // Determine if pause occurs between lines
        public static bool sleep = false;						// Determine if sleep occurs between lines

        public static Events scripts;                           // Event handler script processor

        public static bool isBattling = false;                  // Whether or not battle is going on
        public static bool isWon = false;                       // Whether or not Markiplier is allowed to scream "DID I DOOOOOONE IT? DID I WOOOOOOON?"

        public static Dictionary<string, bool> keyItems =       // Inventory of key items for progression
            new Dictionary<string, bool>
            {
                { "Prison_Key", false },
                { "Paul", false }
            };
        public static Dictionary<string, bool> flags =          // Flags for progression
            new Dictionary<string, bool>
            {
                { "First_Enemy", false },
                { "Cells_Checked", false },
                { "Boss_Fought", false },
                { "Prison_Finish", false }
            };
        public static int money;                                // Monetization

        public static bool fileOpen = false;                    // Denotes if slot is open
        public static int slotOpen;								// Which slot is open

        public static KeyboardState keyPress;                   // Keyboard input
        public static KeyboardState prevKeyPress;               // Previous keyboard input
        public static GamePadState gamePad;                     // Gamepad input
        public static GamePadState prevGamePad;                 // Previous gamepad input

        public static Vector2 startPoint = new Vector2(9, 9);   // Start point (Change later maybe?)
        public static string currentMap = "starting_room/starting_room";
        public static string currentPlayer;
        public static SpriteFace spriteFace = SpriteFace.Down;	// Direction player faces
        public static Vector2 position;                         // Current player position
        public static Vector2 offset;                           // Offset player's position to map position

        public static List<Enemy> enemies = new List<Enemy>();	// All enemies currently on-screen

        public static List<Vector2> spriteSrcUp = new List<Vector2>(new[]
        {
            new Vector2(3, 0),
            new Vector2(4, 0),
            new Vector2(5, 0)
        });
        public static List<Vector2> spriteSrcDown = new List<Vector2>(new[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(2, 0)
        });
        public static List<Vector2> spriteSrcLeft = new List<Vector2>(new[]
        {
            new Vector2(9, 0),
            new Vector2(10, 0),
            new Vector2(11, 0)
        });
        public static List<Vector2> spriteSrcRight = new List<Vector2>(new[]
        {
            new Vector2(6, 0),
            new Vector2(7, 0),
            new Vector2(8, 0)
        });

        // Initialize timers and counters for delay and animation. stepDelay is the delay in milliseconds between each frame. Each step takes approximately 50 ms
        public static Stopwatch timer = new Stopwatch();
        public const int STEP_DELAY = 40;
        public static bool moving;
        public static int step;


        /*******************************************************
        * All code from other programmers here
        * Classes from other programmers includes:
        * - Armor.cs
        * - BattleEnemy.cs
        * - BattlePlayer.cs
        * - Item.cs
        * - Weapon.cs
        * I will review these classes later as well as the code below and move/modify accordingly in order to keep efficiency, organization, and ease of use.
        *******************************************************/

        //Tony: Battle Global variables

        //Tony: Here's the list for the player's items/equipment/armor/etc.
        private static List<Object> playerOwns = new List<Object>();

        //Tony: Joe wants the party to start out with these items.
        public static void generatePlayerOwnsListFirstTime()
        {
            playerOwns.Add(Item(itemTypes.smallRationPack));
            playerOwns.Add(Item(itemTypes.smallRationPack));
            playerOwns.Add(Item(itemTypes.smallRationPack));
            playerOwns.Add(Item(itemTypes.smallRationPack));
        }

        //Tony: These let you add items and instantiate them at the same time. Quite useful, maybe.
        private static object Weapon(weaponTypes weaponType)
        {
            Weapon weapon = new Weapon(weaponType);
            return weapon;
        }

        private static object Armor(armorTypes armorType)
        {
            Armor armor = new Armor(armorType);
            return armor;
        }


        private static object Item(itemTypes itemType)
        {
            Item item = new Item(itemType);
            return item;
        }

        private static object BattleEnemy(battleEnemyTypes battleEnemyType)
        {
            BattleEnemy battleEnemy = new BattleEnemy(battleEnemyType);
            return battleEnemy;
        }

        //Tony: Here are a few methods that sort through the playerOwns list for different types of items. Make note that each return a new list. Each list type is Object.
        public static List<Object> sortPlayerOwnsWeapons()
        {
            List<Object> weaponsPlayerOwns = new List<Object>();
            Weapon weapon = new Weapon();
            foreach (Object commodity in playerOwns)
                if (commodity.GetType() == weapon.GetType())
                    weaponsPlayerOwns.Add(commodity);
            return weaponsPlayerOwns;
        }

        public static List<Object> sortPlayerOwnsArmor()
        {
            List<Object> armorPlayerOwns = new List<Object>();
            Armor armor = new Armor();
            foreach (Object commodity in playerOwns)
                if (commodity.GetType() == armor.GetType())
                    armorPlayerOwns.Add(commodity);
            return armorPlayerOwns;
        }
        public static List<Object> sortPlayerOwnsItems()
        {
            List<Object> itemsPlayerOwns = new List<Object>();
            Item item = new Item();
            foreach (Object commodity in playerOwns)
                if (commodity.GetType() == item.GetType())
                    itemsPlayerOwns.Add(item);
            return itemsPlayerOwns;
        }
        //Tony: This list stores the battle enemy types. Glorious shocker.       
        //private static List<BattleEnemy> battleEnemyTypes = new List<BattleEnemy>();

        //Tony: To instantiate a new BattleEnemy, just use the BattleEnemy constructor with one of these glorious enum types.
        public enum battleEnemyTypes { patrolTrooperGround, patrolTrooperJetpack, riotTrooperGround, riotTrooperJetpack, ajax, barrin }

        //Tony: To instantiate a new weapon, just use the Weapon constructor with one of these glorious enum types.
        public enum weaponTypes { hand, Spirit, homeforgedBlade, kunais, homeforgedPistol, nightStick, crossbow, barrinNightStick, ajaxPistol }

        //Tony: To instantiate a new armor, just use the Armor consructor with one of these glorious enum types.
        public enum armorTypes { exoArmor, superArmor }

        //Tony: To instantiate a new item, just use the Item constructor with one of these glorious enum types.
        public enum itemTypes { smallRationPack, staleBread, jerky, oldVitamins, minorStatUpPillStrUp, minorStatUpPillSpdUp, minorStatUpPillDefUp }

        //Tony: BattleCharacters here. Only load its method up the first time the game is played. If we can't save, don't worry too much about it.
        private static List<BattleCharacter> battleCharacters = new List<BattleCharacter>();

        private static BattleCharacter Joseph = new BattleCharacter("Joseph", 15, 5, 3, 1, 9, BattleCharacter.characterClasses.Crusader, BattleCharacter.specialMoves.Protector, weaponTypes.Spirit, weaponTypes.kunais, armorTypes.exoArmor, "N/A");
        private static BattleCharacter Aaron = new BattleCharacter("Aaron", 15, 5, 2, 0, 10, BattleCharacter.characterClasses.Marksman, BattleCharacter.specialMoves.Sabotage, weaponTypes.homeforgedPistol, weaponTypes.hand, armorTypes.exoArmor, "N/A");
        private static BattleCharacter Thomas = new BattleCharacter("Thomas", 15, 6, 3, 2, 8, BattleCharacter.characterClasses.Swordsman, BattleCharacter.specialMoves.Counter, weaponTypes.homeforgedBlade, weaponTypes.hand, armorTypes.exoArmor, "N/A");
        private static BattleCharacter Grayson = new BattleCharacter("Grayson", 15, 5, 3, 1, 8, BattleCharacter.characterClasses.StealthOp, BattleCharacter.specialMoves.GroundPound, weaponTypes.crossbow, weaponTypes.hand, armorTypes.exoArmor, "N/A");

        public static void generateStartingParty()
        {
            battleCharacters.Add(Joseph);
            battleCharacters.Add(Aaron);
            battleCharacters.Add(Thomas);
            battleCharacters.Add(Grayson);
        }


        //Tony: To instantiate a new ProtectiveHazard, just use the ProtectiveHazard constructor with one of these glorious enum types.
        public enum protectiveHazardTypes { woodenCrate, metalCrate, safeOilDrum }

        //Tony: Here's the single FlammableOilDrum type. No list necessary, I suppose. Here's an enum for easy instantiation anyways, like the other instantiation methods.
        public enum flammableOilDrumTypes { flammableOilDrum }

        //Tony: Here's the single LightFixture type. No list necessary, I suppose. Here's an enum for easy instantiation anyways, like the other instantiation methods.
        public enum lightFixtureTypes { lightFixture }

        //private static LightFixture lightFixture = new LightFixture("Light Fixture", 3, 8);
    }
}
