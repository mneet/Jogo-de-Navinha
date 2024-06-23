using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static WaveManager;

public class WaveManager : MonoBehaviour {

    // System
    public static WaveManager Instance;

    // Timer entre cada wave
    [SerializeField] private float timerStartDelayMax = 1f;
    [SerializeField] private float timerStartDelay = 1f;
    private bool waveStartDelay = true;

    // Contador de waves
    public int waveCount = 0;

    // QUantidade de mobs por wave, cada indice indica quantos mobs serão spawnado de acordo com o contador
    public int[] waveMobAmount;

    // Generation variables
    [SerializeField] private List<GameObject> EnemiesList;
    [SerializeField] private GameObject bossPreFab;
    // Variáveis de Wave

    // Tipos de asteroides
    public enum AsteroidTypes {
        SMALL = 0,
        MEDIUM = 1,
        BIG = 2,
        SHOOTER = 3,
        LENGTH = 4,
        BOSS = 5
    }

    // Struct que armazena informações da Wave
    public struct Wave {
        public bool waveSpawned;            // Se a wave foi spawnada
        public List<AsteroidTypes> mobList; // Lista de mobs que deve ser spawnado
        public List<GameObject> mobObjList; // Referencias aos objetos spawnados para checagem da wave
        public bool waveRunning;            // Flag se a wave esta em progresso

        public void InitVariables() {       // Inicia variaveis do struct
            mobList = new List<AsteroidTypes>();
            mobObjList = new List<GameObject>();
            waveSpawned = false;
            waveRunning = true;
        }
    }
    
    // Wave atual
    public Wave currentWave;

    #region Generation Methods

    // Algumas partes do código estão em inglês, porque usei meus sistemas do TI como base e o projeto do TI foi programado todo em inglês

    // Generates a wave with mobs that run over the map
    private Wave WaveGenerator() {

        // Inicia struct de wave
        Wave wave = new Wave();
        wave.InitVariables();
        
        // Popula lista de mobs da wave com tipos de asteroid
        for (var i = 0; i < waveMobAmount[waveCount]; i++) {            
            AsteroidTypes mob = (AsteroidTypes)UnityEngine.Random.Range(0, (int)AsteroidTypes.LENGTH);
            wave.mobList.Add(mob);
        }

        // Retorna struct da wave gerada
        return wave;
    }

    // Retorna um valor aleatorio de uma lista fornecida
    private GameObject PickRandomList(List<GameObject> list) {
        int randomIndex = Mathf.Max(UnityEngine.Random.Range(0, list.Count()));
        return list[randomIndex];
    }

    #endregion

    #region Gameplay Management Methods

    // Spawna os mobs da wave
    private void SpawnWave() {

        // Inicia lista de objetos
        List<GameObject> mobObjList = new List<GameObject>();

        // Checa se wave atual já foi spawnada
        if (!currentWave.waveSpawned) {
            currentWave.waveSpawned = true;
            // Inicia spawn da wave
            for (var i = 0; i < currentWave.mobList.Count(); i++) {

                // Pega um mob da pool de mobs correspondente
                GameObject mob = GameManager.Instance.GetRandomEnemy(currentWave.mobList[i]);

                // Ativa objeto do mob e randomiza direção do mesmo
                mob.SetActive(true);
                MovementComponent movementComponent = mob.GetComponent<MovementComponent>();
                if (movementComponent != null) {
                    movementComponent.RandomizeDirection();
                }

                // Adiciona o mob na lista da wave
                mobObjList.Add(mob);
            }
        }

        // Seta nova lista de mobs spawnados no struct da wave
        currentWave.mobObjList = mobObjList;
    }

    // Checa se o mob fornecido pertence a lista da wave, caso sim remova-o da lista
    // Função chamada quando mob é destruido
    public void CheckWaveList(GameObject mob) {
        if (currentWave.mobObjList.Contains(mob)) {
            currentWave.mobObjList.Remove(mob);

            // Caso lista d emobs seja menor ou igual a 0, indica que a wave acabou
            if (currentWave.mobObjList.Count() <= 0) {
                currentWave.waveRunning = false;

                // Aumenta contador da wave
                waveCount++;
                waveCount = Mathf.Clamp(waveCount, 0, 5);
                GameManager.Instance.WaveCount(waveCount);
            }
        }
    }
    #endregion 

    // Controle de wave
    private void WaveControl() {
        // Se a wave esta em progresso
        if (currentWave.waveRunning) {
            // Se ainda não foi spawnada, chama o metodo de spawn
            if (!currentWave.waveSpawned) {
                SpawnWave();
            }
        }
        else { // Caso não esteja em progresso
            if (waveCount < 4) { // Caso seja menor que o limite de waves
                timerStartDelay -= Time.deltaTime; // Diminui timer de delay entre waves
                if (timerStartDelay <= 0) {
                    // Gera nova wave e reseta timer de delay
                    currentWave = WaveGenerator();
                    timerStartDelay = timerStartDelayMax;
                }
            }
            else { // Caso o limite de waves seja atingido, chama tela de vitoria
                if (waveCount == 4) {

                    // Iniciando wave do boss
                    currentWave = new Wave();
                    currentWave.InitVariables();
                    currentWave.mobList.Add(AsteroidTypes.BOSS);

                    GameObject boss = Instantiate(bossPreFab, new Vector3(0f, 0f, 10f), Quaternion.Euler(new Vector3(0f,180f,0f)));
                    currentWave.mobObjList.Add(boss);
                    currentWave.waveSpawned = true;

                    //waveCount++;
                    //GameManager.Instance.WaveCount(waveCount);

                    GameObject player = GameObject.Find("Player");
                    if (player != null) {
                        HealthComponent playerHealth = player.GetComponent<HealthComponent>();
                        playerHealth.TakeHeal(playerHealth.healthMax);
                    }
                }
                else {
                    GameManager.Instance.EndGameVictory();
                }
            }
        }
    }

    // DEBUG
    private void DebugSkipToBoss()
    {
        // Input da tecla F5
        if (Input.GetKeyDown(KeyCode.F5)) {
            // Loop percorrendo os mobs spawnados e desativando cada um
            for (var i = 0; i < currentWave.mobObjList.Count; i++)
            {
                currentWave.mobObjList[i].SetActive(false);
            }
            // Seta contador de wave para a wave do boss
            waveCount = 4;
            currentWave.waveRunning = false;
        }
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
        // Gera wave inicial
        currentWave = WaveGenerator();
    }
   
    private void Update() {
        // Metodo de controle de waves
        WaveControl();
        DebugSkipToBoss();
    }
}
