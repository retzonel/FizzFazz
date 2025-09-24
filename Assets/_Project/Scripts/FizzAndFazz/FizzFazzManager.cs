using System;
using System.Collections;
using System.Collections.Generic;
using Creotly.FizzFazz;
using UnityEngine;
using DG.Tweening;


public class AlienProgressGenerator : MonoBehaviour
{
    [Header("Prefabs & References")] public Transform alienAPrefab;
    public Transform alienBPrefab;
    public GameObject blockPrefab;

    [Header("Settings")] public int totalSteps = 3; // Number of connections/dots required
    public float blockSpacing = 1f; // Distance between each block

    private List<Transform> path = new List<Transform>();
    private Transform alienA;
    private Transform alienB;
    private int currentStep = 0;

    [SerializeField] private GameObject heartExplosion;

    [Header("SFX")] [SerializeField] private AudioClip stepSound, heartExplosionSound;

    [SerializeField] private GamesManager _gamesManager;
    [SerializeField] private GameplayManager gameplay;
    
    void Start()
    {
        totalSteps = _gamesManager.GetLevelElementsCount();
        gameplay.onColoursJoined.AddListener(OnPuzzleStepSolved);
        gameplay.onColoursSeparated.AddListener(OnPuzzleStepUnsolved);
        GeneratePath();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // forward
        {
            OnPuzzleStepSolved();
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) // backward
        {
            OnPuzzleStepUnsolved();
        }
    }


    void GeneratePath()
    {
        // Clear old path
        foreach (Transform t in path)
        {
            if (t != null) Destroy(t.gameObject);
        }

        path.Clear();

        // Get parent (this GameObject’s) position as origin
        Vector3 origin = transform.position;

        // Generate blocks horizontally, relative to origin
        for (int i = 0; i <= totalSteps; i++)
        {
            Vector3 pos = origin + new Vector3(i * blockSpacing, 0, 0);
            GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
            path.Add(block.transform);
        }

        // Spawn Alien A at start
        alienA = Instantiate(alienAPrefab, path[0].position, Quaternion.identity);

        // Spawn Alien B at goal
        alienB = Instantiate(alienBPrefab, path[path.Count - 1].position, Quaternion.identity);
    }

    public void OnPuzzleStepSolved()
    {
        if (currentStep >= totalSteps) return;

        currentStep++;
        MoveAlienToStep(currentStep);

        AudioManager.Instance.PlaySound(stepSound);

        if (currentStep >= totalSteps)
        {
            // delay slightly so jump finishes before "meet"
            DOVirtual.DelayedCall(0.35f, () => HandleAliensMeeting());
        }
    }

    public void OnPuzzleStepUnsolved()
    {
        if (currentStep <= 0) return;

        currentStep--;
        MoveAlienToStep(currentStep);

        AudioManager.Instance.PlaySound(stepSound);
    }

    private void MoveAlienToStep(int step)
    {
        alienA.DOJump(path[step].position, 0.5f, 1, 0.3f);
    }


    void HandleAliensMeeting()
    {
        //Destroy both aliens
        Destroy(alienB.gameObject);
        // Play explosion effect at Alien A's position
        GameObject explosion = Instantiate(heartExplosion, alienA.position, Quaternion.identity);
        AudioManager.Instance.PlaySound(heartExplosionSound);
        Destroy(alienA.gameObject);
        Destroy(explosion, 2f);
        // trigger any win conditions here
        GameManager.Instance?.GameOver(GameOverType.Win);
    }
}

