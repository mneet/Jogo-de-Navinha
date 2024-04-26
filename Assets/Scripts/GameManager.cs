using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    [SerializeField] private int score = 0;
    [SerializeField] private GameObject ShooterEnemy;
    [SerializeField] private GameObject RunnerEnemy;

    [SerializeField] private GameObject healPWRUP;
    [SerializeField] private GameObject bulletPWRUP;
    [SerializeField] private GameObject firePWRUP;
    [SerializeField] private GameObject speedPWRUP;

    [SerializeField] private int levelCap = 10;

    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject gameHUD;
    [SerializeField] private TMP_Text endScoreText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text hpText;
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
            if (score % levelCap == 0) {
                GameObject enemySpawn = ShooterEnemy;
                int dice = Random.Range(0, 100);
                if (dice > 40) enemySpawn = RunnerEnemy;

                Instantiate(enemySpawn, Vector3.zero, Quaternion.identity);
                levelCap *= 2;
            }
            scoreText.text = $"Pontos: {score}";

            if (score >= 80) {
                gameHUD.SetActive(false);
                gameEnded = true;
                endGameUI.SetActive(true);
                victoryText.SetActive(true);
                Time.timeScale = 0f;
                endScoreText.text = $"Pontuação: {score}";
            }
        }
    }

    public void UpdateHealthUI(int health) {
        hpText.text = $"Vida: {health}";
        if (health <= 0) {
            endGameUI.SetActive(true);
            endScoreText.text = $"Pontuação: {score}";
            gameEnded = true;
            gameHUD.SetActive(false);
            lossText.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RestartGame() {
        Debug.Log("Restarting game");
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }


    private void Awake() {

    }
}
