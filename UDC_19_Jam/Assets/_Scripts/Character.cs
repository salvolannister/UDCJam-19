using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public enum tipology
    {
        queen = 0,
        lgbtquia = 1,
        white = 2,
        black = 3,
        thief = 4,
        standard = 5
    }

    public GameObject prefab;
    public tipology Tipology;
    public int societyGainMax;
    public int societyGainMin;
    public int threat;
    public GameObject potiveBindParticleEffect;
    public GameObject negativeBindParticleEffect;


}
