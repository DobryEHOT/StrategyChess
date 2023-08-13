using Game.CameraController;
using Game.MapSystems;
using Game.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Baff
{
    public abstract class BaffObj : ScriptableObject
    {
        public bool isInfinity = false;

        public abstract void DoAction(Unit objectPawn);
    }

    
}

