using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Tooltip("time after while the first character will be spawned")]
    public float startTime = 5;
    [Tooltip("Time every new character is spawn")]
    public float creationTimeRate = 10;

    public GameObject doorPosition;
    
    public void Awake()
    {
        S = this;
    }

    public void Start()
    {
        InvokeRepeating("GenerateNewCharacters", startTime, creationTimeRate);
    }

    public void GenerateNewCharacters()
    {
        GameObject character = Instantiate(characters_SO.GetCharacter());
        character.transform.position = doorPosition.transform.position;
    }

}
