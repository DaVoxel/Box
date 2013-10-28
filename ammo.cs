// Basic idea is give ammo for whatever AFC get from player
// So idea lead to this logic: Every time a player change weapon, save the weapon name to a list
// You can ask what about secondary weapon, if he doesn't change to secondary weapon, how can we get the name of it
// We can't, and we don't need it, because of the simple logic: Until he change to secondary weapon, that weapon remains full ammo
// But that is the reason why it doesn't work with Scavenger specialist pointstreak, after the player earns Scavenger, stock max ammo increased
// But AFC can't catch that, so !ammo won't give him full ammo
/////////////////////////////////////////
// About the GiveAmmo issue, why I can't use it along with AFC
// Both script use !ammo, GiveAmmo will give full ammo, but AFC only give start ammo if the player doesn't have Scavenger Pro
// I can change my script's name to a name that has the first letter after 'G' in alphabet
// But I prefer the name AFC.dll, it just like KFC, with out a K pronoun :)
using System;
using System.Collections.Generic;
using InfinityScript;

namespace AFC
{
    public class AFC : BaseScript
    {
        List<string> LauncherList = new List<string>(new string[] { "m320_mp", "rpg_mp", "iw5_smaw_mp", "stinger_mp", "javelin_mp", "xm25_mp" });

        public AFC()
            : base()
        {
            Call(44, "scr_fullscavenger", 1);
            Call(44, "scr_supply_max", 3);
            Call(44, "scr_supply_axis", 1);
            PlayerConnected += OnPlayerConnected;
        }

        void OnPlayerConnected(Entity player)
        {
            var weaponList = new List<string>();
            player.SetField("supply_count", 0);
            player.SetField("weapons", new Parameter(weaponList));
            player.OnNotify("joined_team", e =>
            {
                if (player.GetField<string>("sessionteam") == "allies" || Call<int>("scr_supply_axis") != 0)
                    Utilities.RawSayTo(player, "[^2AFC^7] You can type !ammo in chat to fully refill your ammo");
            });
            player.OnNotify("weapon_change", (e, w) =>
            {
                var weapon = w.As<string>();
                var list = player.GetField<List<string>>("weapons");
                if (!list.Contains(weapon))
                {
                    list.Add(weapon);
                    player.SetField("weapons", new Parameter(list));
                }
            });

            player.OnNotify("scavenger_pickup", e =>
            {
                if (Call<int>(48, "scr_fullscavenger") != 0)
                {
                    var offhand = GetPrimaryOffhandWeapon(player);
                    if (offhand != "none")
                        player.Call(33468, offhand, 1);
                    offhand = GetSecondaryOffhandWeapon(player);
                    if (offhand != "none")
                    {
                        if (offhand == "concussion_grenade_mp" || offhand == "flash_grenade_mp")
                        {
                            var ammoCount = player.Call<int>(33127, offhand);
                            if (ammoCount < 2)
                                player.Call(33468, offhand, ammoCount + 1);
                        }
                        else player.Call(33468, offhand, 1);
                    }
                    var list = player.GetField<List<string>>("weapons");
                    LauncherList.ForEach(weapon =>
                    {
                        if (HasWeapon(player, weapon))
                        {
                            if (!list.Contains(weapon))
                                list.Add(weapon);
                            var stockAmmo = player.Call<int>(33471, weapon);
                            if (stockAmmo < Call<int>(439, weapon))
                                player.Call(33469, weapon, stockAmmo + 1);
                        }
                    });
                    player.SetField("weapons", new Parameter(list));
                }
            });
        }

        public override void OnSay(Entity player, string name, string message)
        {
            if (message == "!ammo")
            {
                if (Call<int>(48, "scr_supply_axis") == 0 && player.GetField<string>("sessionteam") == "axis")
                    return;
                var maxsupply = Call<int>(48, "scr_supply_max");
                var supplycount = player.GetField<int>("supply_count");
                if (maxsupply > 0 && supplycount == maxsupply)
                    return;
                var list = player.GetField<List<string>>("weapons");
                var removeWeapons = new List<string>();
                list.ForEach(weapon =>
                {
                    if (HasWeapon(player, weapon))
                        GiveFullAmmo(player, weapon, player.Call<int>(33394, "specialty_extraammo") != 0);
                    else
                        removeWeapons.Add(weapon);
                });
                removeWeapons.ForEach(weapon => list.Remove(weapon));
                player.SetField("weapons", new Parameter(list));
                supplycount++;
                player.SetField("supply_count", supplycount);
                var supplyleft = maxsupply - supplycount;
                if (supplyleft > 0)
                    player.Call(33344, "You have " + supplyleft + " ammo supply left");
                else if (supplyleft == 0)
                    player.Call(33344, "You have reach the maximum ammo supply count");
                Call(362, "[^2AFC^7] ^3" + player.Name + " ^7just replenished his ammo");
            }
        }

        string GetPrimaryOffhandWeapon(Entity player)
        {
            var offhandclass = player.Call<string>(33542);
            if (offhandclass == "frag")
                return "frag_grenade_mp";
            if (offhandclass == "throwingknife")
                return "throwingknife_mp";
            if (offhandclass == "other")
                foreach (var offhand in new string[] { "c4_mp", "semtex_mp", "claymore_mp", "bouncingbetty_mp" })
                    if (HasWeapon(player, offhand))
                        return offhand;
            return "none";
        }

        string GetSecondaryOffhandWeapon(Entity player)
        {
            var offhandclass = player.Call<string>(33498);
            if (offhandclass == "smoke")
            {
                if (player.Call<int>(33493, "concussion_grenade_mp") != 0)
                    return "concussion_grenade_mp";
                return "smoke_grenade_mp";
            }
            if (offhandclass == "flash")
                foreach (var offhand in new string[] { "flash_grenade_mp", "emp_grenade_mp", "trophy_mp", "flare_mp", "scrambler_mp", "portable_radar_mp" })
                    if (HasWeapon(player, offhand))
                        return offhand;
            return "none";
        }

        void GiveFullAmmo(Entity player, string weapon, bool extraAmmo)
        {
            if (extraAmmo)
                player.Call(33523, weapon);
            else
            {
                var startAmmo = Call<int>(438, weapon);
                if (player.Call<int>(33471, weapon) < startAmmo) // Fix decrease ammo after ammo-pickup
                    player.Call(33469, weapon, startAmmo);
            }
            var maxBulletsInClip = Call<int>(388, weapon);
            player.Call(33468, weapon, maxBulletsInClip, "right");
            if (weapon.Contains("akimbo"))
                player.Call(33468, weapon, maxBulletsInClip, "left");
        }

        bool HasWeapon(Entity player, string weapon)
        {
            return player.Call<int>(33493, weapon) != 0;
        }
    }
}
