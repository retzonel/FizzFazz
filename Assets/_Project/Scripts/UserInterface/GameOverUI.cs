using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button next_retry_Button;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI gameoverBtnText;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        next_retry_Button.onClick.AddListener(() =>
        {
            if (GameManager.Instance.GetGameOverType() == GameOverType.Lose)
                SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
        menuButton.onClick.AddListener(() =>
        {
           SceneManager.LoadScene(0); // Load the main menu
        });
        Hide();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Show()
    {
        UpdateUI();
        GameManager.Instance.StartCoroutine(ActivateAfterDelay(1f)); // Delay activation by 1 second
    }
    
    IEnumerator ActivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(true);
    }

    void UpdateUI()
    {
        if (GameManager.Instance.GetGameOverType() == GameOverType.Lose)
        {
            gameOverText.text = "Game Over";
            gameoverBtnText.text = "Retry";
        }
        else
        {
            gameOverText.text = "Level Complete!";
            gameoverBtnText.text = "Next";
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}