using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossMachine : MonoBehaviour
{
    // State
    
    public enum BossStates {
        IDLE,
        SIDE_TO_SIDE_SHOOTING,   
        SPINNING_N_SHOOTING,
        FOUR_CORNERS,
        HEAL
    }
    [Header("State")]
    public BossStates state = BossStates.IDLE;
    private Queue<BossStates> stateQueue = new Queue<BossStates>();
    private bool stateSet = false;

    [SerializeField] private float stateTimer = 4f;
    [SerializeField] private float healSpawnTimer = 1f;
    private float healSpawnTimerMax;
    private float stateTimerMax;

    [SerializeField] private Slider healthBar;

    private float hp;
    private float maxHp;
    


    [Header("Movement")]
    // Movement
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxX;
    [SerializeField] private float minX;
    [SerializeField] private float maxZ;
    [SerializeField] private float minZ;
    private bool invertDirection;

    [SerializeField] private Vector3 moveDirVec = new Vector3(1, 0, 0);
    [SerializeField] private MovementComponent.MovementDirection direction = MovementComponent.MovementDirection.RIGHT;

    [Header("Positions")]
    // Positions
    [SerializeField] private Vector3 centerPosition;
    [SerializeField] private Vector3 topCenterPosition;

    [Header("Boss parts")]
    // Boss parts
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject bossCore;
    [SerializeField] private GameObject innerShield;
    [SerializeField] private GameObject outerShield;

    [SerializeField] private GameObject healerMob;


    private Queue<BossStates> defineStateQueue()
    {
        stateQueue = new Queue<BossStates>();
        Array enumValues = Enum.GetValues(typeof(BossStates));

        for (var i = 1; i < enumValues.Length; i++)
        {
            stateQueue.Enqueue((BossStates)enumValues.GetValue(i));
        }
        return stateQueue;

    }

    private BossStates pickStateFromQueue()
    {
        BossStates new_state = stateQueue.Dequeue();
        stateQueue.Enqueue(new_state);
        return new_state;
    }

    private void slideToPosition(Vector3 destPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, destPos, movementSpeed * Time.deltaTime);

    }

    private void rotateToPosition(Vector3 destPos)
    {
        transform.forward = Vector3.MoveTowards(transform.forward, -(transform.position - destPos), 1 * Time.deltaTime);

    }

    private void toggleSpinParts() {

        // Parent
        SpinBossPart parentSpin = gameObject.GetComponent<SpinBossPart>();
        parentSpin.rotateFlag = !parentSpin.rotateFlag;

        // RightArm
        SpinBossPart rightArmSpin = rightArm.GetComponent<SpinBossPart>();
        rightArmSpin.rotateFlag = !rightArmSpin.rotateFlag;

        // LeftArm
        SpinBossPart leftArmSpin = leftArm.GetComponent<SpinBossPart>();
        leftArmSpin.rotateFlag = !leftArmSpin.rotateFlag;

        // InnerShield
        SpinBossPart innerShieldSpin = innerShield.GetComponent<SpinBossPart>();
        innerShieldSpin.rotateFlag = !leftArmSpin.rotateFlag;

        // InnerShield
        SpinBossPart outerShieldSpin = outerShield.GetComponent<SpinBossPart>();
        outerShieldSpin.rotateFlag = !outerShieldSpin.rotateFlag;
    }

    private void toggleStopSpinParts() {

        // Parent
        SpinBossPart parentSpin = gameObject.GetComponent<SpinBossPart>();
        parentSpin.stopRotation = !parentSpin.stopRotation;

        // RightArm
        SpinBossPart rightArmSpin = rightArm.GetComponent<SpinBossPart>();
        rightArmSpin.stopRotation = !rightArmSpin.stopRotation;

        // LeftArm
        SpinBossPart leftArmSpin = leftArm.GetComponent<SpinBossPart>();
        leftArmSpin.stopRotation = !leftArmSpin.stopRotation;

        // InnerShield
        SpinBossPart innerShieldSpin = innerShield.GetComponent<SpinBossPart>();
        innerShieldSpin.stopRotation = !leftArmSpin.stopRotation;

        // InnerShield
        SpinBossPart outerShieldSpin = outerShield.GetComponent<SpinBossPart>();
        outerShieldSpin.stopRotation = !outerShieldSpin.stopRotation;
    }

    private void toggleShooting(bool flag) {

        // Parent
        ShooterComponent coreShoot = gameObject.GetComponent<ShooterComponent>();
        coreShoot.activateShoot = flag;

        // RightArm
        ShooterComponent rightArmShoot = rightArm.GetComponent<ShooterComponent>();
        rightArmShoot.activateShoot = flag;

        // LeftArm
        ShooterComponent leftArmShoot = leftArm.GetComponent<ShooterComponent>();
        leftArmShoot.activateShoot = flag;
    }

    private float getTotalHp() {

        float totalHp = 0;
        // Parent
        HealthComponent parentHp = gameObject.GetComponent<HealthComponent>();
        if (parentHp.isActiveAndEnabled) totalHp += parentHp.health;


        // RightArm
        HealthComponent rightArmHp = rightArm.GetComponent<HealthComponent>();
        if (rightArmHp.isActiveAndEnabled) totalHp += rightArmHp.health;

        // LeftArm
        HealthComponent leftArmHp = leftArm.GetComponent<HealthComponent>();
        if (leftArmHp.isActiveAndEnabled) totalHp += leftArmHp.health;

        // InnerShield
        HealthComponent innerShieldHp = innerShield.GetComponent<HealthComponent>();
        if (innerShieldHp.isActiveAndEnabled) totalHp += innerShieldHp.health;

        // InnerShield
        HealthComponent outerShieldHp = outerShield.GetComponent<HealthComponent>();
        if (outerShieldHp.isActiveAndEnabled) totalHp += outerShieldHp.health;

        return totalHp;
    }


    #region State Methods
    private void idleState()
    {
        if (!stateSet) {
            // Se move para posição incial
            slideToPosition(topCenterPosition);
            toggleShooting(false);
            if (transform.position == topCenterPosition) {
                stateSet = true;
            }
        }
        else {
            // Condição de troca de estado
            if (transform.position == topCenterPosition) {
                    state = pickStateFromQueue();
                    stateTimer = stateTimerMax;
                    stateSet = false;
            }
        }
    }    
    
    private void healState()
    {
        if (!stateSet) {
            slideToPosition(topCenterPosition);
            toggleShooting(false);
            if (transform.position == topCenterPosition) {             
                stateSet = true;
            }
        }
        else {

            healSpawnTimer -= Time.deltaTime;
            if (healSpawnTimer <= 0) {
                healSpawnTimer = healSpawnTimerMax;
                Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(minX, maxX), 0f, UnityEngine.Random.Range(-8f, -7.7f));
                GameObject mob = Instantiate(healerMob, spawnPos, Quaternion.identity);
                mob.GetComponent<HealEnemy>().boss = gameObject;
            }

            // Condição de troca de estado
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0 && direction == MovementComponent.MovementDirection.RIGHT) {
                state = pickStateFromQueue();
                stateTimer = stateTimerMax;
                stateSet = false;
            }
        }
    }

    private void sideToSideShootingState()
    {
        if (!stateSet) {
            slideToPosition(topCenterPosition);

            if (transform.position == topCenterPosition) {
                toggleShooting(true);
                stateSet = true;
            }
        }
        else {
            // Aplicando movimento
            transform.position += moveDirVec * (movementSpeed / 2) * Time.deltaTime;

            if (transform.position.x >= maxX || transform.position.x <= minX) {
                moveDirVec.x *= -1;
            }

            // Condição de troca de estado
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0) {
                state = pickStateFromQueue();
                stateTimer = stateTimerMax;
                stateSet = false;
            }
        }
    }

    private void fourCornersState() {
        if (!stateSet) {
            slideToPosition(topCenterPosition);
            SpinBossPart bossRotator = gameObject.GetComponent<SpinBossPart>();
            if (transform.position == topCenterPosition && transform.rotation == bossRotator.defaultRotation) {
                toggleStopSpinParts();
                moveDirVec = new Vector3(1, 0, 0);
                toggleShooting(true);
                stateSet = true;
            }
        }
        else {
            // Aplicando movimento
            transform.position += moveDirVec * (movementSpeed / 2) * Time.deltaTime;
            Vector3 clampVector = transform.position;
            clampVector.x = Mathf.Clamp(clampVector.x, minX, maxX);
            clampVector.z = Mathf.Clamp(clampVector.z, minZ, maxZ);
            transform.position = clampVector;

            if (transform.position.x >= maxX) {
                moveDirVec = new Vector3(0,0,-1);
            }

            switch (direction) {
                case MovementComponent.MovementDirection.RIGHT:
                    if (transform.position.x >= maxX) {
                        moveDirVec = new Vector3(0, 0, -1);
                        direction = MovementComponent.MovementDirection.DOWN;
                    }
                    break;
                case MovementComponent.MovementDirection.DOWN:
                    if (transform.position.z <= minZ) {
                        moveDirVec = new Vector3(-1, 0, 0);
                        direction = MovementComponent.MovementDirection.LEFT;
                    }
                    break;
                case MovementComponent.MovementDirection.LEFT:
                    if (transform.position.x <= minX) {
                        moveDirVec = new Vector3(0, 0, 1);
                        direction = MovementComponent.MovementDirection.UP;

                    }
                    break;
                case MovementComponent.MovementDirection.UP:
                    if (transform.position.z >= maxZ) {
                        moveDirVec = new Vector3(1, 0, 0);
                        direction = MovementComponent.MovementDirection.RIGHT;
                    }
                    break;
            }
            
            //transform.LookAt(new Vector3(0f, 0f, 0f));
            rotateToPosition(new Vector3(0f, 0f, 0f));

            // Condição de troca de estado
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0 && direction == MovementComponent.MovementDirection.RIGHT) {
                state = pickStateFromQueue();
                stateTimer = stateTimerMax;
                stateSet = false;

                toggleStopSpinParts();
            }
        }
    }

    private void spinningNShootingState()
    {
        if (!stateSet) {
            toggleSpinParts();
            toggleShooting(true);
            stateSet = true;
        }

        slideToPosition(centerPosition);

        // Condição de troca de estado
        if (transform.position == centerPosition)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                state = pickStateFromQueue();
                stateTimer = stateTimerMax;
                stateSet = false;
                toggleSpinParts();
            }
        }
    }
    #endregion
   
    private void stateMachineControl()
    {
        switch (state)
        {
            case BossStates.IDLE:
                idleState();
                break;

            case BossStates.SPINNING_N_SHOOTING:
                spinningNShootingState();
                break;

            case BossStates.SIDE_TO_SIDE_SHOOTING:
                sideToSideShootingState();
                break;

            case BossStates.HEAL:
                healState();
                break;

            case BossStates.FOUR_CORNERS:
                fourCornersState();
                break;
        }
    }

    private void Awake()
    {
        defineStateQueue();
        stateTimerMax = stateTimer;
        healSpawnTimerMax = healSpawnTimer;
    }

    private void Start() {
        hp = getTotalHp();
        maxHp = getTotalHp();
    }

    private void Update()
    {
        stateMachineControl();
        
        if (getTotalHp() != hp) {
            hp = getTotalHp();
            float hpPercent = hp / maxHp;
            Debug.Log(hpPercent);
            healthBar.value = hpPercent;
        }
    }
}
