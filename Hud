using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;

namespace HideSeek
{
    class CustomHud
    {
        static Dictionary<int, HudElem[]> ClientHud = new Dictionary<int, HudElem[]>();

        public static void HudInit(Entity ent)
        {

            HudElem[] list = new HudElem[3];
            list[0] = CreateInfoText(ent);
            int entNum = ent.Call<int>("getEntityNumber");
            HudElem[] temp = CreateAmmoDisplay(ent);
            list[1] = temp[0];
            list[2] = temp[1];

            if (ClientHud.ContainsKey(entNum))
                ClientHud[entNum] = list;
            else
                ClientHud.Add(entNum, list);

            ent.OnNotify("weapon_fired", (entity, weaponName) =>
                {
                    UpdateAmmoVal(entity);
                });

            ent.OnNotify("reload", entity =>
                {
                    UpdateAmmoVal(entity);
                });

            ent.OnNotify("weapon_change", (entity, newWep) =>
                {
                    UpdateAmmoVal(entity);
                });
            Credits(ent);
        }

        public static void UpdateHud(Entity ent, int type)
        {
            int entNum = ent.Call<int>("getEntityNumber");
            if(!ClientHud.ContainsKey(entNum))
                return;
            HudElem[] list = ClientHud[entNum];
            for (int i = 0; i < list.Length; i++)
            {
                if (type == 1)
                    list[i].Call("destroy");
                else if (type == 2)
                    list[i].Alpha = 0f;
                else
                    list[i].Alpha = 1f;
            }
            if (type == 1)
                ClientHud.Remove(entNum);
        }

        public static void OnSpawned(Entity ent)
        {
            if (!ent.HasField("hnsType"))
                return;
            int entNum = ent.Call<int>("getEntityNumber");
            if (!ClientHud.ContainsKey(entNum))
                return;
            UpdateHud(ent, 3);
            HudElem[] list = ClientHud[entNum];

            if (ent.GetField<string>("hnsType") == "hider")
            {
                list[0].Call("settext", "Press [{+smoke}] to open Model Menu | Press [{+actionslot 5}] to see Info | Press [{+actionslot 3}] to toggle 3rd Person");
            }
            else if (ent.GetField<string>("hnsType") == "seeker")
            {
                list[0].Call("settext", "Press [{+actionslot 4}] to attach/detach Silencer | Press [{+actionslot 5}] to see Info | Press [{+actionslot 3}] to toggle 3rd Person");
            }

            UpdateAmmoTxt(ent);
            UpdateAmmoVal(ent);
        }

        public static void ShowGametype()
        {
            HudElem gameType = HudElem.CreateServerFontString("default", 1.6f);
            gameType.Alpha = 1f;
            gameType.SetField("color", new Vector3(1f, 1f, 0f));
            gameType.Call("settext", "Hide & Seek");
            gameType.HideWhenInMenu = true;
            gameType.SetPoint("LEFT", "BOTTOMLEFT", 45, -62);
        }

        public static void showAliveHud()
        {
            HudElem bar1 = HudElem.NewHudElem();
            bar1.Parent = HudElem.UIParent;
            bar1.SetPoint(string.Empty, "BOTTOMLEFT", 80, -45);
            bar1.SetShader("white", 125, 15);
            bar1.Foreground = false;
            bar1.HideWhenInMenu = true;
            bar1.Alpha = .275f;
            HudElem bar2 = HudElem.NewHudElem();
            bar2.Parent = HudElem.UIParent;
            bar2.SetPoint(string.Empty, "BOTTOMLEFT", 80, -25);
            bar2.SetShader("white", 125, 15);
            bar2.Foreground = false;
            bar2.HideWhenInMenu = true;
            bar2.Alpha = .275f;

            HudElem Icon = HudElem.NewHudElem();
            Icon.Parent = HudElem.UIParent;
            Icon.SetPoint("LEFT", "BOTTOMLEFT", 0, -35);
            Icon.Foreground = true;
            Icon.HideWhenInMenu = true;
            Icon.SetShader("cardicon_skull_black", 55, 55);
        }

        private static HudElem CreateInfoText(Entity ent)
        {
            HudElem info = HudElem.CreateFontString(ent, "default", 1.7f);
            info.SetPoint("CENTER", "TOP", 0, 10);
            info.Call("settext", "Press [{+actionslot 5}] to see Info | Press [{+actionslot 3}] to toggle 3rd Person");

            return info;
        }

        private static HudElem[] CreateAmmoDisplay(Entity ent)
        {
            HudElem[] list = new HudElem[2];
            HudElem value = HudElem.NewClientHudElem(ent);
            value.Parent = HudElem.UIParent;
            value.FontScale = 2.2f;
            value.SetPoint("RIGHT", "BOTTOMRIGHT", -43, -37);
            value.HideWhenInMenu = true;
            list[0] = value;
            //label doesn't work
            HudElem stock = HudElem.CreateFontString(ent, "default", 1.7f);
            stock.SetPoint("LEFT", "BOTTOMRIGHT", -40, -33);
            stock.HideWhenInMenu = true;
            list[1] = stock;

            return list;
        }

        private static void UpdateAmmoTxt(Entity ent)
        {
            int stock = -1;
            ent.OnInterval(100, entity =>
                {
                    if (entity == null || !entity.IsAlive)
                        return false;
                    int entNum = ent.Call<int>("getEntityNumber");
                    if (!ClientHud.ContainsKey(entNum))
                        return false;
                    HudElem ammoTxt = ClientHud[entNum][2];

                    int newStock = entity.Call<int>("getWeaponAmmoStock", ent.CurrentWeapon);
                    if (newStock != stock)
                    {
                        stock = newStock;
                        ammoTxt.Call("settext", "/ " + stock);
                    }
                    return true;
                });
        }

        private static void UpdateAmmoVal(Entity ent)
        {
            if (ent == null || !ent.IsAlive)
                return;
            int entNum = ent.Call<int>("getEntityNumber");
            if (!ClientHud.ContainsKey(entNum))
                return;

            HudElem ammoVal = ClientHud[entNum][1];
            int ammo = ent.Call<int>("getWeaponAmmoClip", ent.CurrentWeapon);
            ammoVal.Call("setvalue", (float)ammo);
            if (ammo == 0)
                ammoVal.SetField("color", new Vector3(1f, 0.23f, 0f));
            else
                ammoVal.SetField("color", new Vector3(1f, 1f, 1f));
        }

        private static void Credits(Entity ent)
        {
            HudElem credits = HudElem.CreateFontString(ent, "hudbig", 1.2f);
            credits.SetPoint("CENTER", "BOTTOM", 0, -70);
            credits.Call("settext", "Hide&Seek Mod by zxz0O0");
            credits.Alpha = 0f;
            credits.SetField("glowcolor", new Vector3(0f, 0f, 1f));
            credits.GlowAlpha = 1f;

            ent.OnNotify("tab", entity =>
            {
                credits.Alpha = 1f;
            });

            ent.OnNotify("-tab", entity =>
            {
                credits.Alpha = 0f;
            });
        }
    }
}
