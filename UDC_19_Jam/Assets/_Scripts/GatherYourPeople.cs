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

    public static int LIFES = 3;
    public static int THREATS;

    public Dictionary<Vector2, Character> characterLocations;
    public int[,] grid;
    public int MAX_SIMILAR = 3;
    private const int GRID_DIM = 10;
    public void Awake()
    {
        S = this;

        characterLocations = new Dictionary<Vector2, Character>();
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
            S.characterLocations.Add(new Vector2(roundX, roundY), character);
            S.CalculateLifesAndThreats(new Vector2(roundX, roundY), character);

            return true;
        }
        return false;
    }

    private void CalculateLifesAndThreats(Vector2 addedPos, Character addedCharacter)
    {
        // calculate lifes and threats
        int totalSimilar = 0;
        int totalDifferent = 0;
        int X = (int)addedPos.x;
        int Y = (int)addedPos.y;
        Character.tipology addedTipology = addedCharacter.Tipology;

        // TODO: Improve this part of code
        if (X < GRID_DIM - 1)
        {
            if (Y > 0)
                CheckCell(ref totalSimilar, ref totalDifferent, X + 1, Y - 1, addedTipology);
            CheckCell(ref totalSimilar, ref totalDifferent, X + 1, Y, addedTipology);
            if (Y < GRID_DIM - 1)
                CheckCell(ref totalSimilar, ref totalDifferent, X + 1, Y + 1, addedTipology);

        }

        if (Y > 0)
            CheckCell(ref totalSimilar, ref totalDifferent, X, Y - 1, addedTipology);
        if (Y < GRID_DIM - 1)
            CheckCell(ref totalSimilar, ref totalDifferent, X, Y + 1, addedTipology);

        if (X != 0)
        {
            if (Y > 0)
                CheckCell(ref totalSimilar, ref totalDifferent, X - 1, Y - 1, addedTipology);
            CheckCell(ref totalSimilar, ref totalDifferent, X - 1, Y, addedTipology);
            if (Y < GRID_DIM - 1)
                CheckCell(ref totalSimilar, ref totalDifferent, X - 1, Y + 1, addedTipology);
        }


        // udpate lifes and threats
        if (totalSimilar > MAX_SIMILAR)
        {
            THREATS += 1;
        }
        else
        {
            LIFES += addedCharacter.societyGainMin;
        }


    }


    void CheckCell(ref int totalSimilar, ref int totalDifferent, int X, int Y, Character.tipology addedTipology)
    {
        if (grid[X, Y] != 0)
        {
            if (CheckSimilarity(new Vector2(X - 1, Y + 1), addedTipology))
            {
                totalSimilar += 1;
            }
            else
            {
                totalDifferent += 1;
            }
        }
    }
    private bool CheckSimilarity(Vector2 values, Character.tipology myTipology)
    {
        if (characterLocations.TryGetValue(values, out Character neighbor))
        {
            Debug.Log("Neig found " + neighbor.Tipology);
            if (neighbor.Tipology == myTipology)
                return true;
            else
                return false;
        }
        else
        {
            Debug.LogWarning("no neigh found when cheking neighbourhood!!!");
            return false;
        }

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
