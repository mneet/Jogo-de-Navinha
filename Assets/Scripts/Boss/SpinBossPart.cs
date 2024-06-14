using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinBossPart : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 rotationAxis;
    [SerializeField] private Quaternion defaultRotation;
    public bool rotateFlag;
    public bool stopRotation = false;

    // Rotaciona o objeto em torno do eixo definido

    private void RotateBody() {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }

    private void Update() {
        if (rotateFlag) {
            RotateBody();
        }
        else {
            if (!stopRotation) { 
                transform.rotation = Quaternion.RotateTowards(transform.rotation, defaultRotation, (rotationSpeed * 2) * Time.deltaTime);
            }
        }
    }
}
