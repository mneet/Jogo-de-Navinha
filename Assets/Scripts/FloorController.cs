using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Transform followTarget;
    private void Update() {
        // Movimenta o piso para baixo
        transform.position += new Vector3(0f, 0f, moveSpeed) * Time.deltaTime;
    }
    private void LateUpdate() {
        // Checa se o piso saiu do limite da tela
        if (transform.position.z <= -35f) {
            // Caso tenha saido, se posiciona em cima do outro piso
            transform.position = new Vector3(0f, -1f, followTarget.position.z + 50f);
        }
    }
}
