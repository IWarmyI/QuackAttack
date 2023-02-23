using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteract : MonoBehaviour
{
    // move interactable x/y and pos/neg
    public GameObject interactable;
    private Vector3 interactablePos;
    public float interactableDist = 0.0f;
    public bool interactableMoveX = true;
    public bool interactableMoveNeg = false;

    // move button x/y and pos/neg
    public GameObject button;
    private Vector3 buttonPos;
    public bool buttonMoveX = true;
    public bool buttonMoveNeg = false;
    // button can only be hit once
    private bool hit = false;

    void Start()
    {
        buttonPos = button.transform.position;
        interactablePos = interactable.transform.position;
    }

    void Update()
    {
        button.transform.position = buttonPos;
        interactable.transform.position = interactablePos;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // check for projectile col and not hit
        if (col.gameObject.CompareTag("Projectile") && !hit) {
            // move button pos/neg in x dir
            if (buttonMoveX) {
                if (buttonMoveNeg) buttonPos.x -= 0.5f;
                else buttonPos.x += 0.5f;
            }
            // move button pos/neg in y dir
            else {
                if (buttonMoveNeg) buttonPos.y -= 0.5f;
                else buttonPos.y += 0.5f;
            }

            // move interactable pos/neg in x dir
            if (interactableMoveX) {
                if (interactableMoveNeg) interactablePos.x -= interactableDist;
                else interactablePos.x += interactableDist;
            }
            // move interactable pos/neg in y dir
            else {
                if (interactableMoveNeg) interactablePos.y -= interactableDist;
                else interactablePos.y += interactableDist;
            }
            hit = true;
        }
    }
}
