using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;

namespace SpellHack
{
    internal class Program
    {
        public struct SpellStruct
        {
            public string ChampionName;
            public SpellSlot AvailableSpell;

        }
        public static List<SpellStruct> Spells = new List<SpellStruct>();
        public static Menu Config;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Spells.Add(new SpellStruct
            {
                ChampionName = "Annie",
                AvailableSpell = SpellSlot.E
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "Ashe",
                AvailableSpell = SpellSlot.Q
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "Blitzcrank",
                AvailableSpell = SpellSlot.W
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "ChoGath",
                AvailableSpell = SpellSlot.E
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "Jax",
                AvailableSpell = SpellSlot.E
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "Singed",
                AvailableSpell = SpellSlot.R
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "Sion",
                AvailableSpell = SpellSlot.W
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "Tristana",
                AvailableSpell = SpellSlot.Q
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "TwistedFate",
                AvailableSpell = SpellSlot.W //fun fact it DOES work but TF's W is unusable and champion can't AA if W active
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "MasterYi",
                AvailableSpell = SpellSlot.R
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "Vayne",
                AvailableSpell = SpellSlot.R
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "Tryndamere",
                AvailableSpell = SpellSlot.R
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "Teemo",
                AvailableSpell = SpellSlot.W
            });

            Spells.Add(new SpellStruct
            {
                ChampionName = "Zilean",
                AvailableSpell = SpellSlot.W
            });

            /*
             * Ashe:
             * -> Fiora: OnAttack: Instant ultimate / no duration limit / less damage / can be attacked
             * -> Twitch: OnAttack: Cast's W without CD except of AA
             * -> TwistedFate: OnAttack: Always shoots with red card
             * -> Ezreal: OnAttack: E particle, ways less damage, ways less attackspeed
             * -> Lucian: OnAttack: R particle, goes throguh enemys, ways less damage, ways less attackspeed
             * -> Brand: OnAttack: Ultimate
             * -> Pantheon: Weird shit.
             * -> Gragas: OnAttack: Ultimate with a cd of 10-15sec
             * -> Varus: Uses the area Damage on attack
             * -> Jax: Possible to stun everyone
             * -> Lulu: OnAttack: Lulu AA becomes her Q and Pix also CS
             */

            Config = new Menu("SpellHack", "SpellHack", true);
            Config.AddSubMenu(new Menu("Champions", "Champions"));
            Config.AddSubMenu(new Menu("Tower", "Tower"));
            foreach (var turret in ObjectManager.Get<Obj_AI_Turret>())
            {
                Config.SubMenu("Tower")
                    .AddItem(
                        new MenuItem(turret.Name, "Hack " + turret.Name.Replace("_T1_", "_BLUE_").Replace("_T2_", "_RED_").Replace("_", "")).SetValue(false));
                Config.Item(turret.Name).SetValue(false);
            }
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => !hero.IsMe))
            {
                Config.SubMenu("Champions")
                    .AddItem(
                        new MenuItem(hero.ChampionName, "Hack " + hero.ChampionName).SetValue(false));
                Config.Item(hero.ChampionName).SetValue(false);
            }
            Config.AddToMainMenu();

            Game.PrintChat("<font color=\"#00BFFF\">SpellHack</font> <font color=\"#FFFFFF\">loaded! Press SHIFT to customize ^_^");
            Game.PrintChat("Found available spells:");
            bool flag = false;
            foreach (var spell in Spells)
            {
                if (spell.ChampionName == ObjectManager.Player.ChampionName)
                {
                    Game.PrintChat(spell.ChampionName + " " + spell.AvailableSpell);
                    flag = true;
                }
            }
            if (!flag) Game.PrintChat("None");
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        static int tick = 0;
        private static void Game_OnGameUpdate(EventArgs args)
        {
            foreach (var spell in Spells)
            {
                if (spell.ChampionName == ObjectManager.Player.ChampionName)
                {
                    foreach (var turret in from turret in ObjectManager.Get<Obj_AI_Turret>() 
                                           let isEnabled = Config.Item(turret.Name).GetValue<bool>()
                                           where turret.Name == Config.Item(turret.Name).Name & isEnabled && !turret.IsDead
                                           select turret)
                    {
                        /*Game.PrintChat(turret.Name);
                        Game.PrintChat("curr:" + ObjectManager.Player.Position);
                        Game.PrintChat("pos: " + turret.Position);*/
                        Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(turret.NetworkId, spell.AvailableSpell)).Send();
                    }
                    foreach (var hero in from hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => !hero.IsMe)
                                         let isEnabled = Config.Item(hero.ChampionName).GetValue<bool>()
                                         let championName = Config.Item(hero.ChampionName).Name
                                         where hero.ChampionName == championName & isEnabled && !hero.IsDead
                                         select hero)
                    {
                        Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(hero.NetworkId, spell.AvailableSpell)).Send();
                    }
                }
            }
        }
    }
}
