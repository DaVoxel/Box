playerConnected += new Action<Entity>(entity =>
            {
                info = HudElem.CreateServerFontString("hudbig", 1.2f);//TYPE AND FONT SIZE
                info.SetPoint("TOPCENTER", "TOPCENTER", 0, 1);//COORDINATES AND LOCATION:CENTER, LEFT, RIGHT, TOP, BOTTOM
                info.HideWhenInMenu = true;
                info.SetText("^5message one");//HERE'S WHERE THE MESSAGE GO
            
                 info = HudElem.CreateServerFontString("hudbig", 1.0f);//TYPE AND FONT SIZE
                info.SetPoint("BOTTOMCENTER", "BOTTOMCENTER", 0, 1);//COORDINATES AND LOCATION:CENTER, LEFT, RIGHT, TOP, BOTTOM
                info.HideWhenInMenu = true;
                info.SetText("^5 message 2");//HERE'S WHERE THE MESSAGE GO

                info = HudElem.CreateServerFontString("hudbig", 0.6f);//TYPE AND FONT SIZE
                info.SetPoint("TOPRIGHT", "TOPRIGHT", 0, 1);//COORDINATES AND LOCATION:CENTER, LEFT, RIGHT, TOP, BOTTOM
                info.HideWhenInMenu = true;
                info.SetText("by: ^5 message 3");//HERE'S WHERE THE MESSAGE GO
