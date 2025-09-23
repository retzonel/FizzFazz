using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }
    
    [SerializeField] private float gameplayTimer = 300f; // Example: 5 minutes
    public float GameplayTimer => gameplayTimer;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one instance of GameTimer found!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    
    private void Update()
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            gameplayTimer -= Time.deltaTime;
            if (gameplayTimer <= 0f)
            {
                gameplayTimer = 0f;
                GameManager.Instance?.GameOver(GameOverType.Lose);
            }
        }
    }
    
    public float GetTimeFormmated()
    {
        int minutes = Mathf.FloorToInt(gameplayTimer / 60f);
        int seconds = Mathf.FloorToInt(gameplayTimer % 60f);
        return minutes * 100 + seconds;
    }
}
