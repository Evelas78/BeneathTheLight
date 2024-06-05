using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class scr_Hat_Idle : scr_animations
{
    public void Awake()
    {
        spriteName = "sp_mainBody_Idle_Hat";
    }

    public override Sprite animationScript()
    {
        return frameArray[0];
    }
}
