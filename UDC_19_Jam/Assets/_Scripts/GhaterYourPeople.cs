using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class that will manage the states of the game, keep the reference to the scriptableObjects
/// and the spawned characters
/// </summary>
public class GhaterYourPeople : MonoBehaviour
{
    /// <summary>
    /// TODO: change the name of the gamestate with something more appropriate
    /// </summary>
    [System.Flags]
    public enum eGameState
    {
        // Decimal      // Binary
        none = 0,       // 00000000
        mainMenu = 1,   // 00000001
        preLevel = 2,   // 00000010
        level = 4,      // 00000100
        postLevel = 8,  // 00001000
        gameOver = 16,  // 00010000
        all = 0xFFFFFFF // 11111111111111111111111111111111
    }

    public static GhaterYourPeople S
    {
        get
        {
            return _S;
        }
        set
        {
            if (_S != null)
            {
                Debug.LogError(" an instance of GhaterYourPeople is already present !");
                return;
            }

            _S = value;
        }
    }

    private static GhaterYourPeople _S;

    [Header("Set in inspector")]
    public CharactersScriptableObject characters_SO;

    public int hearts;
    public int threats;

    public Dictionary<Vector3, GameObject> characterLocations;

    public void Awake()
    {
        S = this;
    }

    public static bool AddCharacterLocation(Vector3 position, GameObject gameObject)
    {
        if (S.characterLocations == null)
            S.characterLocations = new Dictionary<Vector3, GameObject>();
        return false;
    }

    public static bool IsValidPosition(Vector3 position)
    {
        return false;
    }
}
