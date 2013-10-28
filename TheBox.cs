using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;
using InfinityScript;

namespace Box
{
    public class Box : BaseScript
    {
        private static Random _rng = new Random();
        private int WallGunRange = 75;
        private Entity _airdropCollision;
        private string _mapname;
        public Box()
            : base()
        {
            Entity care_package = Call<Entity>("getent", "care_package", "targetname");
            _airdropCollision = Call<Entity>("getent", care_package.GetField<string>("target"), "targetname");
            _mapname = Call<string>("getdvar", "mapname");
            Call("setdvar", "allow_cheats", "1");
            //changeGametype("^5Infected 2.0");
            if (File.Exists("scripts\\maps\\" + _mapname + ".txt"))
                loadMapEdit(_mapname);

            PlayerConnected += new Action<Entity>(ent =>
            {
                ent.SetField("cash", 0);
                //createPlayerHud(ent);
                ShowGametype();
                showAliveHud();
                UsablesHud(ent);
                // Mystery(ent);
                ent.SpawnedPlayer += new Action(() =>
                {
                    if (ent.GetField<string>("sessionteam") == "axis")
                    {
                        //for (int i = 0; i < 20; i++) ent.TakeWeapon("throwingknife_mp");
                        ent.TakeAllWeapons();
                        ent.Call("giveweapon", "iw5_deserteagle_mp_tactical");
                        //ent.Call("giveMaxAmmo", "iw5_deserteagle_mp");
                        ent.Call("setweaponammoclip", "iw5_deserteagle_mp_tactical", 0);
                        ent.Call("setweaponammostock", "iw5_deserteagle_mp_tactical", 0);

                        ent.Call("SetOffhandPrimaryClass", "frag");
                        ent.GiveWeapon("frag_mp");
                        ent.Call("setweaponammoclip", "frag_mp", 1);

                        //ent.SetPerk("specialty_fastermelee", true, true);
                        //ent.SetPerk("specialty_fastmeleerecovery", true, true);
                        AfterDelay(100, () =>
                        {

                            ent.SwitchToWeaponImmediate("iw5_deserteagle_mp_tactical");
                            //specialty_fastermelee "0"
                            /*
                            string[] split = wep.Split('_');

                            string cw = split[1];

                            if (DIC.ContainsKey(cw))
                            {
                                double value = DIC[cw];
                            
                                // player.SetClientDvar("cg_gun_x", value.ToString());
                            }*/
                        });
                    }
                    ent.OnNotify("Rbox", (boxdude) =>
                    {
                        if (boxdude.GetField<string>("sessionteam") != "allies")
                        {
                            
                            return;
                        }
                        if (boxdude.GetField<int>("cash") == 30 || (boxdude.GetField<int>("cash") > 30))
                        {
                            if (boxdude.GetField<int>("canBox") == 1)
                            {
                                boxdude.TakeWeapon(boxdude.CurrentWeapon);
                                string gun = (_GunList[_rng.Next(_GunList.Length)]);
                                boxdude.GiveWeapon(gun);
                                boxdude.SwitchToWeaponImmediate(gun);
                                boxdude.SetField("cash", ent.GetField<int>("cash") - 30);
                                boxdude.Call("givemaxammo", gun);
                                boxdude.SetField("canBox", 0);
                                boxdude.Call("playlocalsound", "ammo_crate_use");
                            }
                        }
                    });
                });
            });
        }/*
        public List<string> PlayerStop = new List<string>();
        public bool Ammo(Entity player, int amount)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            var wep = player.CurrentWeapon;
            player.Call("setweaponammoclip", wep, amount);
            player.Call("setweaponammoclip", wep, amount, "left");
            player.Call("setweaponammoclip", wep, amount, "right");
            return true;
        }*/
        /*
        #region Change Gametype Appearance
        //From QCZM Sourcecode:
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool VirtualFree(IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

        public IntPtr alloc(int size)
        {
            return VirtualAlloc(IntPtr.Zero, (UIntPtr)size, 0x3000, 0x40);
        }

        public bool unalloc(IntPtr address, int size)
        {
            return VirtualFree(address, (UIntPtr)size, 0x8000);
        }

        bool _changed = false;
        IntPtr memory;
        private unsafe void changeGametype(string gametype)
        {
            byte[] gametypestring;
            if (_changed)
            {
                gametypestring = new System.Text.UTF8Encoding().GetBytes(gametype);
                if (gametypestring.Length >= 64) gametypestring[64] = 0x0; // null terminate if too large
                Marshal.Copy(gametypestring, 0, memory, gametype.Length > 64 ? 64 : gametype.Length);
                return;
            }
            memory = alloc(64);
            gametypestring = new System.Text.UTF8Encoding().GetBytes(gametype);
            if (gametypestring.Length >= 64) gametypestring[64] = 0x0; // null terminate if too large
            Marshal.Copy(gametypestring, 0, memory, gametype.Length > 64 ? 64 : gametype.Length);
            *(byte*)0x4EB983 = 0x68; // mov eax, 575D928h -> push stringloc
            *(int*)0x4EB984 = (int)memory;
            *(byte*)0x4EB988 = 0x90; // mov ecx, [eax+0Ch] -> nop
            *(byte*)0x4EB989 = 0x90;
            *(byte*)0x4EB98A = 0x90;
            *(byte*)0x4EB98B = 0x90; // push edx -> nop
            _changed = true;
        }
        #endregion

        */
        public static List<Entity> usables = new List<Entity>();
        public void UsablesHud(Entity player)
        {
            HudElem message = HudElem.CreateFontString(player, "hudbig", 0.7f);
            message.SetPoint("CENTER", "CENTER", 0, -50);

            OnInterval(100, () =>
            {
                bool _changed = false;
                foreach (Entity ent in usables)
                {
                    if (player.Origin.DistanceTo(ent.Origin) < WallGunRange)
                    {
                        switch (ent.GetField<string>("tipo"))
                        {
                            case "randombox":
                                message.SetText("Press ^3[{+activate}] ^7to use Mistery Box ^7(Cost: ^33 kills^7)");
                                player.Call("notifyonplayercommand", "Rbox", "+activate");
                                player.SetField("pickgun", 1);
                                player.SetField("canBox", 1);
                                break;
                            default:
                                message.SetText("");
                                break;
                        }
                        _changed = true;
                    }
                }
                if (!_changed)
                {
                    message.SetText("");
                    player.SetField("canBox", 0);
                }
                return true;
            });
        }
        /*
              private string[] weaponModels = { "weapon_smaw" , "weapon_xm25", "weapon_mp412", "weapon_desert_eagle_iw5",
                  "weapon_ak47_iw5", "weapon_scar_iw5", "weapon_mp5_iw5", "weapon_p90_iw5",  "weapon_m60_iw5", "weapon_as50_iw5",
                  "weapon_remington_msr_iw5",  "weapon_aa12_iw5", "weapon_model1887", "weapon_skorpion_iw5", "weapon_mp9_iw5",
              };
              private string[] weaponNames = { "iw5_smaw", "xm25", "iw5_mp412", "iw5_deserteagle",
                  "iw5_ak47", "iw5_scar", "iw5_mp5", "iw5_p90", "iw5_m60", "iw5_as50",
                  "iw5_msr", "iw5_aa12", "iw5_1887", "iw5_skorpion", "iw5_mp9",
              };

              */
        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (attacker.IsAlive)
            {
                attacker.SetField("cash", attacker.GetField<int>("cash") + 10);
            }
        }


        public static void ShowGametype()
        {
            HudElem gameType = HudElem.CreateServerFontString("default", 1.6f);
            gameType.Alpha = 1f;
            gameType.SetField("color", new Vector3(1f, 1f, 0f));
            gameType.Call("settext", "Infected 2.0");
            gameType.HideWhenInMenu = true;
            gameType.SetPoint("LEFT", "BOTTOMLEFT", 45, -62);
        }
        private void ShowAlive(Entity player)
        {
            HudElem moneytext = HudElem.CreateServerFontString("default", 1.4f);
            moneytext.Foreground = true;
            moneytext.HideWhenInMenu = true;
            moneytext.SetPoint("LEFT", "BOTTOMLEFT", 50, -25);
            /*HudElem money = HudElem.CreateFontString(player, "hudbig", 0.9f);
            money.SetPoint("TOP RIGHT", "TOP RIGHT", 0, 0); //25 original
            money.HideWhenInMenu = true;
             

            money.Call("setvalue", player.GetField<int>("cash"));*/

            HudElem lives = HudElem.CreateFontString(player, "hudbig", 0.9f);
            lives.SetPoint("TOP RIGHT", "TOP RIGHT", -10, 30); //50 original
            lives.HideWhenInMenu = true;

            HudElem livestext = HudElem.CreateFontString(player, "hudbig", 0.9f);
            livestext.SetPoint("TOP RIGHT", "TOP RIGHT", -50, 30); //50 original
            livestext.HideWhenInMenu = true;
            
                OnInterval(500, () =>
                {
                   
                        moneytext.Call("settext", "Cash: " + player.GetField<int>("cash"));
                        if (player.GetField<string>("sessionteam") == "axis")
                        {
                            livestext.SetText("");
                            lives.SetText("");
                        }
                        else
                        {
                            lives.SetText("");
                            livestext.SetText("");
                        }
                        return true;

                });
            
            /*
            OnInterval(100, () =>
            {
                moneytext.SetText("^3Cash ^7: ");
                money.Call("setvalue", player.GetField<int>("cash"));
                if (player.GetField<string>("sessionteam") == "axis")
                {
                    livestext.SetText("");
                    lives.SetText("");
                }
                else
                {
                    lives.SetText("");
                    livestext.SetText("");
                }
                return true;
            });*/


        }
        public static void showAliveHud()
        {
            HudElem bar1 = HudElem.NewHudElem();
            bar1.Parent = HudElem.UIParent;
            bar1.SetPoint(string.Empty, "BOTTOMLEFT", 80, -45);
            bar1.SetShader("white", 125, 15);
            bar1.Foreground = false;
            bar1.HideWhenInMenu = true;
            bar1.Alpha = .275f;/*
            HudElem bar2 = HudElem.NewHudElem();
            bar2.Parent = HudElem.UIParent;
            bar2.SetPoint(string.Empty, "BOTTOMLEFT", 80, -25);
            bar2.SetShader("white", 125, 15);
            bar2.Foreground = false;
            bar2.HideWhenInMenu = true;
            bar2.Alpha = .275f;*/
            

            HudElem Icon = HudElem.NewHudElem();
            Icon.Parent = HudElem.UIParent;
            Icon.SetPoint("LEFT", "BOTTOMLEFT", 0, -35);
            Icon.Foreground = true;
            Icon.HideWhenInMenu = true;
            Icon.SetShader("cardicon_skull_black", 55, 55);
        }
        private void createPlayerHud(Entity player)
        {

            HudElem money = HudElem.CreateFontString(player, "hudbig", 0.9f);
            money.SetPoint("TOP RIGHT", "TOP RIGHT", 0, 0); //25 original
            money.HideWhenInMenu = true;

            HudElem moneytext = HudElem.CreateFontString(player, "hudbig", 0.9f);
            moneytext.SetPoint("TOP RIGHT", "TOP RIGHT", -50, 0); //25 original
            moneytext.HideWhenInMenu = true;

            HudElem lives = HudElem.CreateFontString(player, "hudbig", 0.9f);
            lives.SetPoint("TOP RIGHT", "TOP RIGHT", -10, 30); //50 original
            lives.HideWhenInMenu = true;

            HudElem livestext = HudElem.CreateFontString(player, "hudbig", 0.9f);
            livestext.SetPoint("TOP RIGHT", "TOP RIGHT", -50, 30); //50 original
            livestext.HideWhenInMenu = true;

            OnInterval(100, () =>
            {
                moneytext.SetText("^3Cash ^7: ");
                money.Call("setvalue", player.GetField<int>("cash"));
                if (player.GetField<string>("sessionteam") == "axis")
                {
                    livestext.SetText("");
                    lives.SetText("");
                }
                else
                {
                    lives.SetText("");
                    livestext.SetText("");
                }
                return true;
            });
        }
        private Vector3 parseVec3(string vec3)
        {
            vec3 = vec3.Replace(" ", string.Empty);
            if (!vec3.StartsWith("(") && !vec3.EndsWith(")")) throw new IOException("Malformed MapEdit File!");
            vec3 = vec3.Replace("(", string.Empty);
            vec3 = vec3.Replace(")", string.Empty);
            String[] split = vec3.Split(',');
            if (split.Length < 3) throw new IOException("Malformed MapEdit File!");
            return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
        }/*
        public Entity randomWeaponCrate(Vector3 origin, Vector3 angles)
        {
            Entity crate = Call<Entity>("spawn", "script_model", new Parameter(origin));
            crate.Call("setmodel", "com_plasticcase_trap_friendly");
            crate.SetField("angles", new Vector3(0f, 90f, 0f));
            crate.Call(33353, _airdropCollision); // clonebrushmodeltoscriptmodel
            crate.SetField("state", "idle");
            crate.SetField("giveweapon", "");
            crate.SetField("player", "");
            crate.SetField("cash", 9);
            usables.Add(crate);
            return crate;
        }*/

        
        public Entity Mystery(Vector3 origin)
        {
            //RANDOM BOX
            Entity shit = Call<Entity>("spawn", "script_model", new Parameter(origin));
            shit.Call("setmodel", "com_plasticcase_trap_friendly");
            shit.Call("clonebrushmodeltoscriptmodel", _airdropCollision);
            shit.SetField("angles", new Vector3(0, 90, 0));
            shit.SetField("state", "idle");
            shit.SetField("tipo", "randombox");
            usables.Add(shit);
            //lipp(shit);
            return shit;

        }
        public int _flagCount = 0;
        public void lipp(Entity ent)
        {
            int curObjID = 12 - _flagCount++;
            Call(431, curObjID, "active"); // objective_add
            Call(435, curObjID, new Parameter(ent.Origin)); // objective_position
            Call(434, curObjID, "compass_waypoint_bomb"); // objective_icon
        }
    

    

        private void loadMapEdit(string mapname)
        {
            try
            {
                StreamReader map = new StreamReader("scripts\\maps\\" + mapname + ".txt");
                while (!map.EndOfStream)
                {
                    string line = map.ReadLine();
                    if (line.StartsWith("//") || line.Equals(string.Empty))
                    {
                        continue;
                    }
                    string[] split = line.Split(':');
                    if (split.Length < 1)
                    {
                        continue;
                    }
                    string type = split[0];
                    switch (type)
                    {

                        case "randombox":
                            split = split[1].Split(';');
                            if (split.Length < 2) continue;
                            Mystery(parseVec3(split[0]));
                            break;

                        default:
                            print("Unknown MapEdit Entry {0}... ignoring", type);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                print("error loading mapedit for map {0}: {1}", mapname, e.Message);
            }
        }
        private static void print(string format, params object[] p)
        {
            Log.Write(LogLevel.All, format, p);
        }
        /*
        void invisibl(Entity player)
        {
            AfterDelay(100, () =>
                        {
                            player.Call("iprintlnbold", "^3Invisibility On (15sec)");
                            player.Call("hide");

                        });
            player.AfterDelay(15000, entity =>
                                     {
                                         player.Call("iprintlnbold", "^1Invisibility Off");
                                         player.Call("show");
                                     });
        }

        void speed(Entity player)
        {
            AfterDelay(100, () =>
            {
                player.Call("setmovespeedscale", 2f);
                player.Call("iprintlnbold", "^3Speed On (20sec)");

            });
            player.AfterDelay(20000, entity =>
            {
                player.Call("setmovespeedscale", 1f);
                player.Call("iprintlnbold", "^1Speed Off");
            });
        }

        void AC130Strike(Entity player, Vector3 loc)
        {
            player.AfterDelay(300, e => Call(404, "ac130_105mm_mp", loc + new Vector3(0, 0, 6000), loc, player));
            player.AfterDelay(600, e => Call(404, "ac130_105mm_mp", loc + new Vector3(0, 0, 6000), loc + new Vector3(180, 0, 0), player));
            player.AfterDelay(900, e => Call(404, "ac130_40mm_mp", loc + new Vector3(0, 0, 6000), loc - new Vector3(180, 0, 0), player));
            player.AfterDelay(1200, e => Call(404, "ac130_105mm_mp", loc + new Vector3(0, 0, 6000), loc + new Vector3(0, 180, 0), player));
            player.AfterDelay(1500, e => Call(404, "ac130_105mm_mp", loc + new Vector3(0, 0, 6000), loc - new Vector3(0, 180, 0), player));
        }
        Random _rnd = new Random();

        List<Entity> USERS = new List<Entity>();
        void Suicide_strike(Entity player, Vector3 loc)
        {

            Vector3 sp = player.Origin + new Vector3(0, 0, 3000);

            Entity ent = Call<Entity>(85, "script_model", sp);

            ent.Call(32929, "vehicle_ac130_low_mp");

            ent.Call(33399, loc, 5f);

            switch (_rnd.Next(4))
            {
                case 0: ent.Call(33406, new Vector3(110, 50, 50), 5f); break;
                case 1: ent.Call(33406, new Vector3(50, 0, 110), 5f); break;
                case 2: ent.Call(33406, new Vector3(-90, 0, -90), 5f); break;
                case 3: ent.Call(33406, new Vector3(110, -200, -100), 5f); break;
            }

            player.AfterDelay(2000, e =>
            {
                AC130Strike(player, loc);

                player.Call(33466, "veh_b2_sonic_boom");

            });

            player.AfterDelay(5000, e =>
            {

                foreach (Entity user in USERS)
                {
                    if (user.Origin.DistanceTo2D(loc) < 500)
                    {
                        user.Call(33130, "ac130", 5f);

                        user.Call(33466, "exp_ac130_105mm_debris");

                        Call(355, 0.3f, 3, user.Origin, 800);
                    }
                }

            });

            player.AfterDelay(6000, e => ent.Call(32928));

        }

        void Get_strike(Entity player, int sel)
        {

            switch (sel)
            {
                case 0: selector(player, "invisibl"); break;
                case 1: selector(player, "suicide_strike"); break;
                case 2: selector(player, "speed"); break;
                //case 3: selector(player, "heli_flock"); break;
            }
        }
        void selector(Entity player, string gun)
        {

           // sound_air(player);

            switch (gun)
            {

                case "speed":
                    //player.SetField("pw", "1");
                    player.Call(33499, "map_artillery_selector", false);//beginlocationselection
                    break;
                case "suicide_strike":
                    //player.SetField("pw", "2");
                    player.Call(33499, "map_artillery_selector", false);
                    break;
                case "invisibl":
                    player.SetField("pw", "3");
                    player.Call(33499, "map_artillery_selector", false);
                    break;
                case "heli_flock":
                    player.SetField("pw", "4");
                    player.Call(33499, loc_selector, true);
                    break;

            }

            change_to_attach(player);

        }
        void change_to_attach(Entity player)
        {

            player.Call(33466, "mp_bonus_start");

            player.SetField("get_what", 1);

            player.Notify("+re");

        }*/
        public static string[] _GunList = new[]
        {
           // "Get_strikeSHJT I WANT TO CALL STREAKS HERE ASWELL",
            "iw5_44magnum_mp",
            "iw5_usp45_mp",
            "iw5_scar_mp",
            "iw5_acr_mp",
            "iw5_ak47_mp",
            "iw5_m4_mp",
            "iw5_g36c_mp",
            "iw5_fad_mp",
            "iw5_type95_mp",
            "iw5_m9_mp",
            "iw5_pp90m1_mp",
            "iw5_p90_mp",
            "iw5_mp7_mp",
            "iw5_spas12_mp",
            "iw5_striker_mp",
            "iw5_mk46_mp",
            "iw5_mg36_mp",
            "iw5_msr_mp_msrscopevz",
            "iw5_dragunov_mp_dragunovscopevz",
            "rpg_mp",
            "m320_mp"      
        };
    }
}
