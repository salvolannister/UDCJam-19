using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/CharactersSO", fileName = "CharactersSO.asset")]
[System.Serializable]
public class CharactersScriptableObject : ScriptableObject
{
    [SerializeField]
    public Character[] characterPrefabs;

    public Character GetCharacter()
    {
        int n = Random.Range(0, characterPrefabs.Length);
        return characterPrefabs[n];
    }
}
