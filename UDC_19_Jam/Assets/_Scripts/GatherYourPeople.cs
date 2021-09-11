using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class that will manage the states of the game, keep the reference to the scriptableObjects
/// and the spawned characters
/// </summary>
public class GatherYourPeople : MonoBehaviour
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

    public static GatherYourPeople S
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

    private static GatherYourPeople _S;

    [Header("Set in inspector")]
    public CharactersScriptableObject characters_SO;

    public int hearts;
    public int threats;

    public Dictionary<KeyValuePair<int, int>, Character> characterLocations;
    public int[,] grid;


    private const int GRID_DIM = 10;
    public void Awake()
    {
        S = this;

        characterLocations = new Dictionary<KeyValuePair<int, int>, Character>();
        grid = new int[GRID_DIM, GRID_DIM];
        InitGrid();
    }

    private void InitGrid()
    {
        for (int i = 0; i < GRID_DIM; i++)
        {
            for (int x = 0; x < GRID_DIM; x++)
            {
                grid[i, x] = 0;
            }
        }
    }

    public static bool AddCharacterLocation(Vector3 position, Character character)
    {
        int roundX = Mathf.RoundToInt(position.x);
        int roundY = Mathf.RoundToInt(position.y);


        if (S.grid[roundX, roundY] == 0)
        {
            S.grid[roundX, roundY] = 1;
            return true;
        }
        return false;
    }

    public static bool IsPositionValid(Vector3 position)
    {
        int roundX = Mathf.RoundToInt(position.x);
        int roundY = Mathf.RoundToInt(position.y);
        position.x = roundX;
        position.y = roundY;

        if (S.grid[roundX, roundY] == 0)
        {
            return true;
        }
        return false;
    }
}
