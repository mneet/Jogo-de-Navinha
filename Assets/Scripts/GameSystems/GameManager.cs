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

    // Pontuação do player
    [SerializeField] private int score = 0;

    // Pools de objetos e inimigos
    [Header("Particulas e Pools")]
    public PoolManager EnemyParticlePool;
    public PoolManager SmallAsteroidPool;
    public PoolManager MediumAsteroidPool;
    public PoolManager BigAsteroidPool;
    public PoolManager ShooterPool;

    // Lista de powerups a serem dropados
    [Header("GameObjects")]
    [SerializeField] private List<GameObject> powerUpList;
    [SerializeField] private GameObject bossPreFab;

    // Variaveis de controle da UI e HUD
    [Header("UI e HUD")]
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject gameHUD;
    [SerializeField] private TMP_Text endScoreText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private Slider hpBar;
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject lossText;

    // Flag de fim de jogo
    public bool gameEnded = false;

    // Spawna um powerup quando chamada
    public void SpawnPowerUp(Vector3 position) {
        // EScolhe um powerup da lista e o instancia
        GameObject powerUp = powerUpList[Random.Range(0, powerUpList.Count)];
        Instantiate(powerUp, position, Quaternion.identity);
    }
   
    // Retorna um objeto aleatorio da pool do tipo de inimigo fornecido
    public GameObject GetRandomEnemy(WaveManager.AsteroidTypes asteroidType) {

        GameObject obj = null;

        // Ecolhe pool com base no tipo de inimigo
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
            case WaveManager.AsteroidTypes.BOSS:
                obj = bossPreFab;
                break;
        }

        return obj;
    }

    // Instancia particulas de hit dos inimigos
    public void PlayEnemieHitParticle(Vector3 position) {
        // Retorna sistema de particulas da pool
        GameObject ptSys = EnemyParticlePool.GetPoolObject();
        ptSys.SetActive(true);

        // Seta posição das particulas e da play
        ptSys.transform.position = position;
        ptSys.GetComponent<ParticleSystem>().Play();
    }   
    
    // Marca um ponto para o player
    public void ScorePoint(int point) {
        if (!gameEnded) {
            score += point;
            scoreText.text = $"Pontos: {score}";
        }
    }

    // Atualiza contador de ondas
    public void WaveCount(int wave) {
        if (!gameEnded) {
            waveText.text = $"Waves Derrotadas: {wave}";
        }
    }


    // Atualiza Barra de vida da UI
    public void UpdateHealthUI(int health) {
        // Transforma valor fornecido em porcentagem
        hpBar.value = ((float)health / 100f) * 10f;

        // Se menor que 0, ativa tela de derrota
        if (health <= 0) {
            endGameUI.SetActive(true);
            endScoreText.text = $"Pontuação: {score}";
            gameEnded = true;
            gameHUD.SetActive(false);
            lossText.SetActive(true);
            Time.timeScale = 0f;
        }
    }
   
    // Ativa tela de vitoria
    public void EndGameVictory() {
        gameHUD.SetActive(false);
        gameEnded = true;
        endGameUI.SetActive(true);
        victoryText.SetActive(true);
        Time.timeScale = 0f;
        endScoreText.text = $"Pontuação: {score}";
    }
    
    // Reinicia jogo
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
