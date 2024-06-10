using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float maxX;
    [SerializeField] private float minX;
    [SerializeField] private float maxZ;
    [SerializeField] private float minZ;

    [SerializeField] private LayerMask mouseLayer;

    public float moveSpeed = 8f;

    private void HandleMovement() {

        // Variaveis de controle
        Vector3 movementVec = new Vector3();
        Vector3 position = transform.position;

        // Pegando input do player
        movementVec.x = Input.GetAxis("Horizontal");
        movementVec.z = Input.GetAxis("Vertical");

        // Normalizando vetor caso a magnitude seja maior que 1
        if (movementVec.magnitude > 1) {
            movementVec.Normalize();
        }

        // Aplicando movimento
        position += movementVec * moveSpeed * Time.deltaTime;


        // Teleporte limite movimento horizontal
        if (position.x < minX - 3) {
            position.x = maxX + 2;
        } else if (position.x > maxX + 3) {
            position.x = minX - 2;
        }

        // Teleporte limite movimento vertical
        if (position.z < minZ - 3) {
            position.z = maxZ + 2;
        }
        else if (position.z > maxZ + 3) {
            position.z = minZ - 2;
        }

        // Aplicando posição ao objeto
        transform.position = position;
    }

    public Vector3 GetMouseDirectionTopDown() {
        // Obter a posição do mouse na tela
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mousePosition = new Vector3();
        
        // Checar colisão do mouse com camada 
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, mouseLayer)) {
            // Coletando ponto de colisão do mouse
            mousePosition = raycastHit.point;
        }
        
        // Zera eixo Y da posição (Movimento 2D)
        mousePosition.y = 0;

        // Returna posição do mouse
        return mousePosition;
    }

    // Rotate player
    private void RotatePlayerMouse() {
        // Coleta posição do mouse
        Vector3 mousePosition = GetMouseDirectionTopDown();

        // Aponta o objeto para direção do mouse
        transform.forward = -(transform.position - mousePosition);
    }

    // Update is called once per frame
    void Update() {
        // Metodo que controla movimento
        HandleMovement();
        // Metodo que controla rotação
        RotatePlayerMouse();
    }
}
