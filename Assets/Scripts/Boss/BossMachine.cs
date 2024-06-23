using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossMachine : MonoBehaviour
{
    // Estados do Boss
    public enum BossStates {
        IDLE,
        SIDE_TO_SIDE_SHOOTING,   
        SPINNING_N_SHOOTING,
        FOUR_CORNERS,
        HEAL
    }

    // Variaveis relacionados ao controle de estados
    [Header("State")]
    
    // Estado atual
    public BossStates state = BossStates.IDLE;
    // Fila de estados
    private Queue<BossStates> stateQueue = new Queue<BossStates>();
    // Flag se a preparação do estado ja terminou
    private bool stateSet = false;

    // Tempo de duração de cada estado
    [SerializeField] private float stateTimer = 4f;
    private float stateTimerMax;

    // Cooldown do spawn de inimigos no estado de heal
    [SerializeField] private float healSpawnTimer = 1f;
    private float healSpawnTimerMax;
    
    // Barra de vida do boss
    [SerializeField] private Slider healthBar;

    // Vida atual e maxima total do boss
    private float hp;
    private float maxHp;
    
    // Variaveis relacionadas a movimentação
    [Header("Movement")]
    
    // Velocidade de movimento do boss
    [SerializeField] private float movementSpeed;

    // Limites da tela que o boss pode se movimentar
    [SerializeField] private float maxX;
    [SerializeField] private float minX;
    [SerializeField] private float maxZ;
    [SerializeField] private float minZ;

    // Flag para inverter direção do movimento
    private bool invertDirection;

    // Direção de movimento em um Vector3
    [SerializeField] private Vector3 moveDirVec = new Vector3(1, 0, 0);
    // Determina qual direção o boss esta se movendo
    [SerializeField] private MovementComponent.MovementDirection direction = MovementComponent.MovementDirection.RIGHT;

    [Header("Positions")]
    // Vector3 de posições padrão do boss
    [SerializeField] private Vector3 centerPosition;
    [SerializeField] private Vector3 topCenterPosition;

    [Header("Boss parts")]
    // Objetos das diferentes partes do boss
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject bossCore;
    [SerializeField] private GameObject innerShield;
    [SerializeField] private GameObject outerShield;

    // Prefab do inimigo curador
    [SerializeField] private GameObject healerMob;


    // Define a fila de estados do boss
    private Queue<BossStates> defineStateQueue()
    {
        // Iinicia fila
        stateQueue = new Queue<BossStates>();

        // Pega valores do enumerador de estados
        Array enumValues = Enum.GetValues(typeof(BossStates));

        // Preenche fila com valores
        for (var i = 1; i < enumValues.Length; i++)
        {
            stateQueue.Enqueue((BossStates)enumValues.GetValue(i));
        }
        return stateQueue;
    }

    // Retorna o primeiro valor da fila de estados
    private BossStates pickStateFromQueue()
    {
        BossStates new_state = stateQueue.Dequeue();

        // Posiciona o valor no final da fila
        stateQueue.Enqueue(new_state);
        return new_state;
    }

    // Move o  boss para dada posição
    private void slideToPosition(Vector3 destPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, destPos, movementSpeed * Time.deltaTime);

    }

    // Rotacionado o boss para determinado ponto
    private void rotateToPosition(Vector3 destPos)
    {
        transform.forward = Vector3.MoveTowards(transform.forward, -(transform.position - destPos), 1 * Time.deltaTime);

    }
    
    // Ativa ou desativa rotação das partes do boss
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

    // Desativa rotação das partes do boss 
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

    // Ativa ou desativa os tiros de todas as partes do boss
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

    // Retorna o valor total do HP do boss contando todas as partes
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

    // Estado idle, apenas se movimenta para posição topCenter e passa para proximo estado
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
    
    // Estado de cura
    private void healState()
    {
        if (!stateSet) { // Se movimenta para posição definida e entra no comportamento do estado após terminar movimento
            slideToPosition(topCenterPosition);
            toggleShooting(false);
            if (transform.position == topCenterPosition) {             
                stateSet = true;
            }
        }
        else {

            // DIminui timer de spawn dos curadores
            healSpawnTimer -= Time.deltaTime;
            if (healSpawnTimer <= 0) {
                // Caso timer chegue a zero, cria curador em posição aleatoria e reseta timer
                healSpawnTimer = healSpawnTimerMax;
                Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(minX, maxX), 0f, UnityEngine.Random.Range(-8f, -7.7f));
                GameObject mob = Instantiate(healerMob, spawnPos, Quaternion.identity);
                mob.GetComponent<HealEnemy>().boss = gameObject;
            }

            // Condição de troca de estado
            stateTimer -= Time.deltaTime; // Diminui timer do estado
            if (stateTimer <= 0) { // Quando timer chegar a zero, muda de estado
                state = pickStateFromQueue();
                stateTimer = stateTimerMax;
                stateSet = false;
            }
        }
    }

    private void sideToSideShootingState()
    {
        if (!stateSet) { // Se movimenta para posição definida e entra no comportamento do estado após terminar movimento
            slideToPosition(topCenterPosition);

            if (transform.position == topCenterPosition) {
                toggleShooting(true);
                stateSet = true;
            }
        }
        else {
            // Aplicando movimento
            transform.position += moveDirVec * (movementSpeed / 2) * Time.deltaTime;

            // Inverte movimento ao atingir limites da tela
            if (transform.position.x >= maxX || transform.position.x <= minX) {
                moveDirVec.x *= -1;
            }

            // Condição de troca de estado
            stateTimer -= Time.deltaTime; // Diminui timer do estado
            if (stateTimer <= 0) { // Quando timer chegar a zero, muda de estado
                state = pickStateFromQueue();
                stateTimer = stateTimerMax;
                stateSet = false;
            }
        }
    }

    private void fourCornersState() {
        if (!stateSet) { // Se movimenta para posição definida e entra no comportamento do estado após terminar movimento
            slideToPosition(topCenterPosition);
            SpinBossPart bossRotator = gameObject.GetComponent<SpinBossPart>();
            if (transform.position == topCenterPosition && transform.rotation == bossRotator.defaultRotation) { // Alem de posição, checa rotação padrão
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

            // Garantindo que não ultrapasse os limites da tela
            clampVector.x = Mathf.Clamp(clampVector.x, minX, maxX);
            clampVector.z = Mathf.Clamp(clampVector.z, minZ, maxZ);
            transform.position = clampVector;

            switch (direction) {
                // Se movimenta para a direita, caso chegue no limite, mude direção para baixo
                case MovementComponent.MovementDirection.RIGHT:
                    if (transform.position.x >= maxX) {
                        moveDirVec = new Vector3(0, 0, -1);
                        direction = MovementComponent.MovementDirection.DOWN;
                    }
                    break;
                // Se movimenta para a baixo, caso chegue no limite, mude direção para esquerda
                case MovementComponent.MovementDirection.DOWN:
                    if (transform.position.z <= minZ) {
                        moveDirVec = new Vector3(-1, 0, 0);
                        direction = MovementComponent.MovementDirection.LEFT;
                    }
                    break;

                // Se movimenta para a esquerda, caso chegue no limite, mude direção para cima
                case MovementComponent.MovementDirection.LEFT:
                    if (transform.position.x <= minX) {
                        moveDirVec = new Vector3(0, 0, 1);
                        direction = MovementComponent.MovementDirection.UP;

                    }
                    break;
                // Se movimenta para a cima, caso chegue no limite, mude direção para direita
                case MovementComponent.MovementDirection.UP:
                    if (transform.position.z >= maxZ) {
                        moveDirVec = new Vector3(1, 0, 0);
                        direction = MovementComponent.MovementDirection.RIGHT;
                    }
                    break;
            }
            
            // Rotaciona o boss para que ele fique voltado para o centro da tela
            rotateToPosition(new Vector3(0f, 0f, 0f));

            // Condição de troca de estado
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0 && direction == MovementComponent.MovementDirection.RIGHT) { // Apenas troca de estado quando esta voltado para direita
                state = pickStateFromQueue();
                stateTimer = stateTimerMax;
                stateSet = false;

                // Para rotação das partes
                toggleStopSpinParts();
            }
        }
    }

    private void spinningNShootingState()
    {
        if (!stateSet) {
            toggleSpinParts(); // Ativa rotação das partes
            toggleShooting(true); // Ativa tiro de todas as partes
            stateSet = true;
        }

        // Se movimenta para centro da tela
        slideToPosition(centerPosition);

        // Condição de troca de estado
        if (transform.position == centerPosition) // Se esta no centro da tela
        {
            stateTimer -= Time.deltaTime; // Diminui timer do estado
            if (stateTimer <= 0) // Quando timer chegar a zero, muda de estado
            {
                state = pickStateFromQueue();
                stateTimer = stateTimerMax;
                stateSet = false;
                toggleSpinParts(); // Desativa rotação
            }
        }
    }
    #endregion
   
    // Controla qual metodo sera chamado para cada estado ativo do boss
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
        // Define fila de estados e timers
        defineStateQueue();
        stateTimerMax = stateTimer;
        healSpawnTimerMax = healSpawnTimer;
    }

    private void Start() {
        // Define hp atual e total
        hp = getTotalHp();
        maxHp = getTotalHp();
    }

    private void Update()
    {
        // Controle de estados
        stateMachineControl();
        
        // Atualiza barra de vida
        if (getTotalHp() != hp) {
            hp = getTotalHp();
            float hpPercent = hp / maxHp;
            Debug.Log(hpPercent);
            healthBar.value = hpPercent;
        }
    }
}
