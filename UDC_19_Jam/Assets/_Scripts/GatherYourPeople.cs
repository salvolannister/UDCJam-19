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
    public enum cellState
    {
        EMPTY= 0,
        FULL =1
    }
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

    private enum eCombos
    {
        h_left = 0,
        h_right = 1,
        h_center = 3,
        vertical = 4,
        v_down = 5
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

    public Dictionary<Vector2, CharacterMovement> characterLocations;
    public int[,] grid;
    public int MAX_SIMILAR = 3;
    public int MAX_DIFF = 3;

    private const int GRID_DIM = 10;
    private bool combo = false;
    public static Action OnCombo;
    public void Awake()
    {
        S = this;

        characterLocations = new Dictionary<Vector2, CharacterMovement>();
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

    public static bool AddCharacterLocation(Vector3 position, CharacterMovement character)
    {
        int roundX = Mathf.RoundToInt(position.x);
        int roundY = Mathf.RoundToInt(position.y);


        if (S.grid[roundX, roundY] == 0)
        {
            S.grid[roundX, roundY] = 1;
            S.characterLocations.Add(new Vector2(roundX, roundY), character);
            S.CalculateCombo(new Vector2(roundX, roundY), character);

            return true;
        }
        return false;
    }

    private void CalculateCombo(Vector2 addedPos, CharacterMovement addedCharacter)
    {
        int totalSimilar = 0;
        int totalDifferent = 0;
        int X = (int)addedPos.x;
        int Y = (int)addedPos.y;
        Character character = addedCharacter.character;
        Character.tipology addedTipology = character.Tipology;
        // check if the combo is on the horizontal

        if (X == 0)
        {
            if (CheckCell(ref totalSimilar, ref totalDifferent, X + 1, Y, addedTipology) && CheckCell(ref totalSimilar, ref totalDifferent, X + 2, Y, addedTipology))
            {
                // Make combo and gain or loose points
                MakeCombo(eCombos.h_left, X, Y, addedCharacter);

            }
            else if (CheckCell(ref totalSimilar, ref totalDifferent, X, Y + 1, addedTipology) && CheckCell(ref totalSimilar, ref totalDifferent, X, Y + 2, addedTipology))
            {
                // make combo
                MakeCombo(eCombos.v_down, X, Y, addedCharacter);

            }

        }
        else if (X == GRID_DIM - 1)
        {
            if (CheckCell(ref totalSimilar, ref totalDifferent, X - 1, Y, addedTipology) && CheckCell(ref totalSimilar, ref totalDifferent, X - 2, Y, addedTipology))
            {
                // make combo 
                MakeCombo(eCombos.h_right, X, Y, addedCharacter);

            }


        }
        else
        {
            if (CheckCell(ref totalSimilar, ref totalDifferent, X - 1, Y, addedTipology) && CheckCell(ref totalSimilar, ref totalDifferent, X + 1, Y, addedTipology))
            {
                // make combo 
                MakeCombo(eCombos.h_center, X, Y, addedCharacter);
            }
            else if (CheckCell(ref totalSimilar, ref totalDifferent, X, Y + 1, addedTipology) && CheckCell(ref totalSimilar, ref totalDifferent, X, Y - 1, addedTipology))
            {
                // make combo 
                MakeCombo(eCombos.vertical, X, Y, addedCharacter);
            }
            else if (X - 2 >= 0)
            {
                if (CheckCell(ref totalSimilar, ref totalDifferent, X - 1, Y, addedTipology) && CheckCell(ref totalSimilar, ref totalDifferent, X - 2, Y, addedTipology))
                {
                    // make combo 
                    MakeCombo(eCombos.h_right, X, Y, addedCharacter);

                }
            }
            else if (X + 2 <= GRID_DIM - 1)
            {
                if (CheckCell(ref totalSimilar, ref totalDifferent, X + 1, Y, addedTipology) && CheckCell(ref totalSimilar, ref totalDifferent, X + 2, Y, addedTipology))
                {
                    // Make combo and gain or loose points
                    MakeCombo(eCombos.h_left, X, Y, addedCharacter);

                }
            }
        }



        // shift the icons and the character

        // calculate lifes and threats


        // TODO: Improve this part of code
        //if (X < GRID_DIM - 1)
        //{
        //    if (Y > 0)
        //        CheckCell(ref totalSimilar, ref totalDifferent, X + 1, Y - 1, addedTipology);
        //    CheckCell(ref totalSimilar, ref totalDifferent, X + 1, Y, addedTipology);
        //    if (Y < GRID_DIM - 1)
        //        CheckCell(ref totalSimilar, ref totalDifferent, X + 1, Y + 1, addedTipology);

        //}

        //if (Y > 0)
        //    CheckCell(ref totalSimilar, ref totalDifferent, X, Y - 1, addedTipology);
        //if (Y < GRID_DIM - 1)
        //    CheckCell(ref totalSimilar, ref totalDifferent, X, Y + 1, addedTipology);

        //if (X != 0)
        //{
        //    if (Y > 0)
        //        CheckCell(ref totalSimilar, ref totalDifferent, X - 1, Y - 1, addedTipology);
        //    CheckCell(ref totalSimilar, ref totalDifferent, X - 1, Y, addedTipology);
        //    if (Y < GRID_DIM - 1)
        //        CheckCell(ref totalSimilar, ref totalDifferent, X - 1, Y + 1, addedTipology);
        //}

        if (combo)
        {
            if (totalSimilar > MAX_SIMILAR)
            {
                LIFES -= addedCharacter.character.threat;
            }
            else if (totalDifferent >= MAX_DIFF)
            {
                LIFES += addedCharacter.character.societyGainMax;
            }
            else
            {
                LIFES += addedCharacter.character.societyGainMin;
            }

            combo = false;
        }
            


    }


    private void MakeCombo(eCombos comboType, int X, int Y, CharacterMovement character)
    {
        Vector2 tmp = new Vector2(X, Y);
        switch (comboType)
        {
            case (eCombos.h_left):
                {
                    grid[X, Y] = grid[X + 1, Y] = grid[X + 2, Y] = 0;

                    RemoveCharacter(tmp, character);
                    tmp.x = X + 1;

                    RemoveCharacter(tmp);
                    tmp.x = X + 2;
                    RemoveCharacter(tmp);
                    break;
                }
            case (eCombos.h_right):
                {
                    grid[X, Y] = grid[X - 1, Y] = grid[X - 2, Y] = 0;

                    RemoveCharacter(tmp, character);
                    tmp.x = X - 1;

                    RemoveCharacter(tmp);
                    tmp.x = X - 2;
                    RemoveCharacter(tmp);
                    break;
                }
            case (eCombos.h_center):
                {
                    grid[X, Y] = grid[X - 1, Y] = grid[X + 1, Y] = 0;

                    RemoveCharacter(tmp, character);
                    tmp.x = X - 1;

                    RemoveCharacter(tmp);
                    tmp.x = X + 1;
                    RemoveCharacter(tmp);
                    break;
                }
            case (eCombos.vertical):
                {
                    grid[X, Y] = grid[X, Y - 1] = grid[X, Y + 1] = 0;

                    RemoveCharacter(tmp, character);
                    tmp.y = Y + 1;

                    RemoveCharacter(tmp);
                    tmp.y = Y - 1;
                    RemoveCharacter(tmp);
                    break;
                }
            case (eCombos.v_down):
                {
                    grid[X, Y] = grid[X, Y + 1] = grid[X, Y + 2] = 0;

                    RemoveCharacter(tmp, character);
                    tmp.y = Y + 1;

                    RemoveCharacter(tmp);
                    tmp.y = Y + 2;
                    RemoveCharacter(tmp);
                    break;
                }


        }
        GatherYourPeople.OnCombo?.Invoke();
        combo = true;
    }

    private void RemoveCharacter(Vector2 tmp, CharacterMovement addedCharacter = null)
    {
        if (addedCharacter == null)
        {
            characterLocations.TryGetValue(tmp, out addedCharacter);

        }
        characterLocations.Remove(tmp);
        Destroy(addedCharacter.gameObject);

    }

    /// <summary>
    /// Check for the presence of the cell and the calculates the similarity
    /// </summary>
    /// <param name="totalSimilar"></param>
    /// <param name="totalDifferent"></param>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="addedTipology"></param>
    bool CheckCell(ref int totalSimilar, ref int totalDifferent, int X, int Y, Character.tipology addedTipology)
    {
        if (grid[X, Y] != 0)
        {
            if (CheckSimilarity(new Vector2(X, Y), addedTipology))
            {
                totalSimilar += 1;
            }
            else
            {
                totalDifferent += 1;
            }

            return true;
        }

        return false;
    }
    private bool CheckSimilarity(Vector2 values, Character.tipology myTipology)
    {
        if (characterLocations.TryGetValue(values, out CharacterMovement neighbor))
        {
            Debug.Log("Neig found " + neighbor.character.Tipology);
            if (neighbor.character.Tipology == myTipology)
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

        if (S.grid[roundX, roundY] == ((int)cellState.EMPTY))
        {
            return true;
        }
        return false;
    }
}
