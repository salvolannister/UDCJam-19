using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [Tooltip("time after while the first character will be spawned")]
    public float startTime = 5;
    [Tooltip("Time every new character is spawn")]
    public float creationTimeRate = 10;
    [Tooltip(" Collider from where you would like to spawn the characters")]
    public Collider2D spawner;
    private float spawnerWidth;
    private CharactersScriptableObject characters_SO;
    public void Start()
    {
        characters_SO = GhaterYourPeople.S.characters_SO;
        spawnerWidth = spawner.bounds.max.x;
        InvokeRepeating("GenerateNewCharacters", startTime, creationTimeRate);
    }

    public void GenerateNewCharacters()
    {
        GameObject character = Instantiate(characters_SO.GetCharacter());
        character.transform.position = getNewSpawnPosition();
    }

    private Vector3 getNewSpawnPosition()
    {

        return new Vector3(Random.Range(0, spawnerWidth), spawner.bounds.center.y, 0);

    }
}
