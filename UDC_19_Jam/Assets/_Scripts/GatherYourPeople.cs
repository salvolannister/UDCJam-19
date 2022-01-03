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
        EMPTY = 0,
        FULL = 1
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
        v_center = 4,
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

    private const int GRID_DIM_Y = 10;
    private const int GRID_DIM_X = 7;
    private bool combo = false;
    public static Action OnCombo;
    public void Awake()
    {
        S = this;

        characterLocations = new Dictionary<Vector2, CharacterMovement>();
        grid = new int[GRID_DIM_Y, GRID_DIM_Y];
        InitGrid();
    }

    private void InitGrid()
    {
        int emptyState = (int)cellState.EMPTY;
        for (int i = 0; i < GRID_DIM_Y; i++)
        {
            for (int x = 0; x < GRID_DIM_X; x++)
            {
                grid[i, x] = emptyState;
            }
        }
    }

    public static bool AddCharacterLocation(Vector3 position, CharacterMovement character)
    {
        int roundX = Mathf.RoundToInt(position.x);
        int roundY = Mathf.RoundToInt(position.y);


        if (S.grid[roundX, roundY] == ((int)cellState.EMPTY))
        {
            Vector2 roundedPos = new Vector2(roundX, roundY);
            S.grid[roundX, roundY] = ((int)cellState.FULL);
            S.characterLocations.Add(roundedPos, character);
            //  S.CalculateCombo(roundedPos, character);
            Vector2 foundPos = Vector2.zero;
            // checks for similar character
            int equalFound = S.CheckComboHorizontal(roundedPos, roundedPos, character.character.Tipology, 0, ref foundPos);

            //Debug.Log($" Equal found {equalFound}");
            if (equalFound == 3)
            {
                //make the characters disappear
                S.MakeCombo(eCombos.h_center, (int)foundPos.x, (int)foundPos.y, character);

            }
            else
            {
                equalFound = S.CheckComboVertical(roundedPos, roundedPos, character.character.Tipology, 0, ref foundPos);
                if (equalFound == 3)
                    S.MakeCombo(eCombos.v_center, (int)foundPos.x, (int)foundPos.y, character);
            }
            return true;
        }
        return false;
    }
    /// <summary>
    ///  check if the current cell is full and then moves the cell and all the above downstairs
    /// </summary>
    /// <param name="X">X of the cell you want to check to see if it can go down</param>
    /// <param name="Y">Y of the cell you want to check to see if it can go down</param>
    private void CheckAndScrollDown(float X, float Y)
    {
        Vector2 position = new Vector2(X, Y);
        int _X = (int)X;
        int _Y = (int)Y;
        // check if position is full
        if (grid[_X, _Y] == ((int)cellState.FULL))
        {
            // move down
            if (characterLocations.TryGetValue(position, out CharacterMovement character))
            {
                // remove from position
                characterLocations.Remove(position);
                grid[_X, _Y] = ((int)cellState.EMPTY);
                // move one pos under
                grid[_X, _Y - 1] = ((int)cellState.FULL);
                Vector2 newPosition = position;
                newPosition.y -= 1;
                characterLocations.Add(newPosition, character);
                Vector3 newGOposition = new Vector3(position.x, newPosition.y, 0);
                character.gameObject.transform.position = newGOposition;

                //Debug.Log($" Character in postion X {position.x} and Y: {position.y} removed");
                // move GO above
                if (Y + 1 < GRID_DIM_Y)
                    CheckAndScrollDown(X, Y + 1);
            }
            else
            {
                Debug.LogError($"Inconsistence between grid and character locations in x:{X} and y:{Y}");
            }
        }

    }

    public int CheckComboHorizontal(Vector2 currentPos, Vector2 caller, Character.tipology tipology, int similarFound, ref Vector2 foundPos)
    {

        //Debug.Log(" checking position " + currentPos + "with similar found " + similarFound);

        if (similarFound < 3 && CheckSimilarity(currentPos, tipology))
        {
            similarFound++;
        }
        else
        {
            return similarFound;
        }


        if (similarFound == 3)
        {
            // segni la posizione del simile tra i due
            foundPos = caller;
            //Debug.Log(" Match found: it s working  " + foundPos.x + " " + foundPos.y);
            return similarFound;
        }



        // check right side
        Vector2 nextPos = currentPos;
        nextPos.x += 1;
        if (!IsOutOfBorder(nextPos.x, nextPos.y) && caller != nextPos)
        {
            similarFound = CheckComboHorizontal(nextPos, currentPos, tipology, similarFound, ref foundPos);
        }

        //check left side
        nextPos = currentPos;
        nextPos.x -= 1;
        if (!IsOutOfBorder(nextPos.x, nextPos.y) && caller != nextPos)
        {
            similarFound = CheckComboHorizontal(nextPos, currentPos, tipology, similarFound, ref foundPos);
        }

        return similarFound;
    }

    public int CheckComboVertical(Vector2 currentPos, Vector2 caller, Character.tipology tipology, int similarFound, ref Vector2 foundPos)
    {

        //Debug.Log(" checking position " + currentPos + "with similar found " + similarFound);

        if (similarFound < 3 && CheckSimilarity(currentPos, tipology))
        {
            similarFound++;
        }
        else
        {
            return similarFound;
        }


        if (similarFound == 3)
        {
            foundPos = caller;
            //Debug.Log(" Match found: it s working  " + foundPos.x + " " + foundPos.y);
            return similarFound;
        }

        // check right side
        Vector2 nextPos = currentPos;
        nextPos.y += 1;
        // check only if the position is inside the boreders and nextPos is not the caller pos (avoid checking the pos already checked) 
        if (!IsOutOfBorder(nextPos.x, nextPos.y) && caller != nextPos)
        {
            similarFound = CheckComboVertical(nextPos, currentPos, tipology, similarFound, ref foundPos);
        }

        //check left side
        nextPos = currentPos;
        nextPos.y -= 1;
        if (!IsOutOfBorder(nextPos.x, nextPos.y) && caller != nextPos)
        {
            similarFound = CheckComboVertical(nextPos, currentPos, tipology, similarFound, ref foundPos);
        }

        return similarFound;
    }



    /// <summary>
    /// Determines whether [the cell is valid] giving the a position in the grid.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>
    ///   <c>true</c> if [is cell valid] [the specified x]; otherwise, <c>false</c>.
    /// </returns>
    bool IsOutOfBorder(float x, float y)
    {
        if ((x == GRID_DIM_X || x < 0) || (y == GRID_DIM_Y || y < 0))
            return true;
        else
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
        else if (X == GRID_DIM_Y - 1)
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
                MakeCombo(eCombos.v_center, X, Y, addedCharacter);
            }
            else if (X - 2 >= 0)
            {
                if (CheckCell(ref totalSimilar, ref totalDifferent, X - 1, Y, addedTipology) && CheckCell(ref totalSimilar, ref totalDifferent, X - 2, Y, addedTipology))
                {
                    // make combo 
                    MakeCombo(eCombos.h_right, X, Y, addedCharacter);

                }
            }
            else if (X + 2 <= GRID_DIM_Y - 1)
            {
                if (CheckCell(ref totalSimilar, ref totalDifferent, X + 1, Y, addedTipology) && CheckCell(ref totalSimilar, ref totalDifferent, X + 2, Y, addedTipology))
                {
                    // Make combo and gain or loose points
                    MakeCombo(eCombos.h_left, X, Y, addedCharacter);

                }
            }
        }





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

            case (eCombos.h_center):
                {
                    grid[X, Y] = grid[X - 1, Y] = grid[X + 1, Y] = 0;

                    RemoveCharacter(tmp);
                    CheckAndScrollDown(tmp.x, tmp.y + 1);
                    tmp.x = X - 1;
                    RemoveCharacter(tmp);
                    CheckAndScrollDown(tmp.x, tmp.y + 1);
                    tmp.x = X + 1;
                    RemoveCharacter(tmp);
                    CheckAndScrollDown(tmp.x, tmp.y + 1);

                    break;
                }
            case (eCombos.v_center):
                {
                    grid[X, Y] = grid[X, Y - 1] = grid[X, Y + 1] = 0;

                    RemoveCharacter(tmp);
                    tmp.y = Y + 1;
                    CheckAndScrollDown(tmp.x, tmp.y);
                    RemoveCharacter(tmp);
                    tmp.y = Y - 1;
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
        if (grid[X, Y] != ((int)cellState.EMPTY))
        {
            if (CheckSimilarity(new Vector2(X, Y), addedTipology))
                return true;
        }

        return false;
    }

    bool CheckCell(int X, int Y, Character.tipology addedTipology)
    {
        if (grid[X, Y] != ((int)cellState.EMPTY))
        {
            if (CheckSimilarity(new Vector2(X, Y), addedTipology))
                return true;
        }

        return false;
    }



    private bool CheckSimilarity(Vector2 values, Character.tipology myTipology)
    {
        if (characterLocations.TryGetValue(values, out CharacterMovement neighbor))
        {
            //Debug.Log("Neig found " + neighbor.character.Tipology);
            if (neighbor.character.Tipology == myTipology)
                return true;
            else
                return false;
        }
        else
        {
            Debug.LogWarning("No neigh found when cheking neighbourhood!!!");
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
