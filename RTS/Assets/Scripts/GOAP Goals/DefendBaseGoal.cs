﻿using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendBaseGoal : GoapGoal
{
    public DefendBaseGoal(string key, float multiplier = 1) : base(key, multiplier)
    {

    }

    public override void UpdateMultiplier(DataSet data)
    {
        // fancy function that lowers the multiplier if another player kills this AI often
    }
}