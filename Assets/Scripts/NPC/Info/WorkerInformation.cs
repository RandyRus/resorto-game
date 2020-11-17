﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerInformation : NPCInformation
{
    public WorkerInformation(string name, CharacterCustomization customization) : base(name, customization)
    {
        throw new System.NotImplementedException();
    }

    protected override GameObject ObjectToInitialize => ResourceManager.Instance.WorkerNpc;

    public override NPCSchedule CreateSchedule(NPCInstance instance)
    {
        throw new System.NotImplementedException();
    }
}
