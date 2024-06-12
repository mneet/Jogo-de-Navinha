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

    public PoolManager EnemyParticlePool;
    public PoolManager SmallAsteroidPool;
    public PoolManager MediumAsteroidPool;
    public PoolManager BigAsteroidPool;
    public PoolManager ShooterPool;


    [SerializeField] private List<GameObject> powerUpList;

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
        GameObject powerUp = powerUpList[Random.Range(0, powerUpList.Count)];

        Instantiate(powerUp, position, Quaternion.identity);
    }
    public GameObject GetRandomEnemy(WaveManager.AsteroidTypes asteroidType) {

        GameObject obj = null;

        switch (asteroidType) {
            case WaveManager.AsteroidTypes.SMALL: 
                obj = SmallAsteroidPool.GetPoolObject(); 
                break;
            case WaveManager.AsteroidTypes.MEDIUM: 
                obj = MediumAsteroidPool.GetPoolObject(); 
                break;
            case WaveManager.AsteroidTypes.BIG: 
                obj = BigAsteroidPool.GetPoolObject(); 
                break;
            case WaveManager.AsteroidTypes.SHOOTER:
                obj = ShooterPool.GetPoolObject();
                break;
        }

        return obj;
    }
    public void PlayEnemieHitParticle(Vector3 position) {
        GameObject ptSys = EnemyParticlePool.GetPoolObject();
        ptSys.SetActive(true);
        ptSys.transform.position = position;
        ptSys.GetComponent<ParticleSystem>().Play();
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

    private void Start() {
        // Toca musica de background
        AudioController.Instance.PlayBgMusic(0);
    }
}
