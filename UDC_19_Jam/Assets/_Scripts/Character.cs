using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character 
{
    public enum tipology
    {
        comunist = 0,
        lgbtquia = 1,
        white = 2,
        black = 3,
        rightWingPolitician = 4,
        chineese = 5
    }

    public GameObject prefab;
    public tipology Tipology;
    public int societyGainMax;
    public int societyGainMin;
    public int threat;
    public GameObject potiveBindParticleEffect;
    public GameObject negativeBindParticleEffect;


}
