using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour {
    // System
    public static WaveManager Instance;

    [SerializeField] private float timerStartDelayMax = 1f;
    [SerializeField] private float timerStartDelay = 1f;
    private bool waveStartDelay = true;
    public int waveCount = 0;

    public int[] waveMobAmount;

    // Generation variables
    [SerializeField] private List<GameObject> EnemiesList;

    // Wave
    public struct Wave {
        public bool waveSpawned;
        public List<GameObject> mobList;
        public List<GameObject> mobObjList;
        public bool waveRunning;

        public void InitVariables() {
            mobList = new List<GameObject>();
            waveSpawned = false;
            waveRunning = true;
        }
    }
    public Wave currentWave;

    #region Generation Methods

    // Generates a wave with mobs that run over the map
    private Wave WaveGenerator() {
        Wave wave = new Wave();
        wave.InitVariables();

        waveCount++;

        for (var i = 0; i < waveMobAmount[waveCount]; i++) {

            GameObject mob = PickRandomList(EnemiesList);
            wave.mobList.Add(mob);
        }

        return wave;
    }

    private GameObject PickRandomList(List<GameObject> list) {

        int randomIndex = Mathf.Max(UnityEngine.Random.Range(0, list.Count()));
        Debug.Log(randomIndex);
        return list[randomIndex];
    }

    #endregion

    #region Gameplay Management Methods

    // Control Wave Spawn
    private void SpawnWave() {
        List<GameObject> mobObjList = new List<GameObject>();
        if (!currentWave.waveSpawned) {
            currentWave.waveSpawned = true;
            for (var i = 0; i < currentWave.mobList.Count(); i++) {
                GameObject mob = Instantiate(currentWave.mobList[i], transform.position, Quaternion.identity);
                mobObjList.Add(mob);
            }
        }
        currentWave.mobObjList = mobObjList;
    }

    // Check if given list contains the given mob and remove it from the list, returns false if the list is empty
    public void CheckWaveList(GameObject mob) {
        if (currentWave.mobObjList.Contains(mob)) {
            currentWave.mobObjList.Remove(mob);

            if (currentWave.mobObjList.Count() <= 0) {
                currentWave.waveRunning = false;
            }
        }
    }
    #endregion 

    private void WaveControl() {
        if (currentWave.waveRunning) {
            if (!currentWave.waveSpawned) {
                SpawnWave();
            }
        }
        else {
            if (waveCount < 4) { 
                timerStartDelay -= Time.deltaTime;
                if (timerStartDelay <= 0) {
                    currentWave = WaveGenerator();
                    timerStartDelay = timerStartDelayMax;
                }
            }
            else {
                GameManager.Instance.EndGameVictory();
            }
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
        currentWave = WaveGenerator();
    }
    void Update() {
        WaveControl();
    }
}
