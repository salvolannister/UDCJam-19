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
    private CharacterController characterController;

    public void Start()
    {
        characters_SO = GatherYourPeople.S.characters_SO;
        spawnerWidth = spawner.bounds.max.x;
        characterController = GenerateNewCharacters();
    }

    private void Update()
    {
        if (characterController  && characterController.hasReachedDestination)
        {
            characterController = GenerateNewCharacters();
        }
    }

    public CharacterController GenerateNewCharacters()
    {
        Character character = characters_SO.GetCharacter();
        GameObject characterPrefab = Instantiate(character.prefab);
        characterPrefab.transform.position = getNewSpawnPosition();

        CharacterController controller = characterPrefab.GetComponent<CharacterController>();
        controller.character = character;
        return controller;
    }

    private Vector3 getNewSpawnPosition()
    {
        float xRound = Mathf.Round(Random.Range(0, spawnerWidth));
        return new Vector3(xRound, spawner.bounds.center.y, 0);

    }
}
