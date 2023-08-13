using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSettings", menuName = "ScriptableObjects/MapSettings", order = 1)]
public class ScrObjMapSettings : ScriptableObject
{
    [SerializeField] [Range(2, 4)] public int countTeamWait = 2;
    [SerializeField] [Range(1, 20)] public int mapWidth;
    [SerializeField] [Range(1, 20)] public int mapHeight;
}
