using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Transform followTarget;
    [SerializeField] private bool move;
    [SerializeField] private bool rotate;
    private void Update() {
        if (move) {
            // Movimenta o piso para baixo
            transform.position += new Vector3(0f, 0f, moveSpeed) * Time.deltaTime;
        }

        if (rotate) {
            transform.Rotate(new Vector3(0f, moveSpeed, 0f) * Time.deltaTime);
        }
    }
    private void LateUpdate() {
        if (move) {
            // Checa se o piso saiu do limite da tela
            if (transform.position.z <= -35f) {
                // Caso tenha saido, se posiciona em cima do outro piso
                transform.position = new Vector3(0f, -1f, followTarget.position.z + 50f);
            }
        }

    }
}
