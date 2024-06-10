using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    [SerializeField] private int score = 0;

    [SerializeField] private GameObject healPWRUP;
    [SerializeField] private GameObject bulletPWRUP;
    [SerializeField] private GameObject firePWRUP;
    [SerializeField] private GameObject speedPWRUP;

    [SerializeField] private int levelCap = 10;

    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject gameHUD;
    [SerializeField] private TMP_Text endScoreText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Slider hpBar;
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject lossText;
    public bool gameEnded = false;

    public void SpawnPowerUp(Vector3 position) {
        int dice = Random.Range(0, 100);
        if (dice < 20) Instantiate(bulletPWRUP, position, Quaternion.identity);
        else if (dice < 40) Instantiate(firePWRUP, position, Quaternion.identity);
        else if (dice < 60) Instantiate(speedPWRUP, position, Quaternion.identity);
        else Instantiate(healPWRUP, position, Quaternion.identity);
    }
    public void ScorePoint(int point) {
        if (!gameEnded) {
            score += point;
            scoreText.text = $"Pontos: {score}";
        }
    }

    public void UpdateHealthUI(int health) {
        hpBar.value = ((float)health / 100f) * 10f;
        if (health <= 0) {
            endGameUI.SetActive(true);
            endScoreText.text = $"Pontuação: {score}";
            gameEnded = true;
            gameHUD.SetActive(false);
            lossText.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void EndGameVictory() {
        gameHUD.SetActive(false);
        gameEnded = true;
        endGameUI.SetActive(true);
        victoryText.SetActive(true);
        Time.timeScale = 0f;
        endScoreText.text = $"Pontuação: {score}";
    }

    public void RestartGame() {
        Debug.Log("Restarting game");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        WaveManager.Instance.waveCount = 0;
        
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }
}
