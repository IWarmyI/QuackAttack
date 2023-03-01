using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteract : MonoBehaviour
{
    // move interactable x/y and pos/neg
    public GameObject interactable;
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

    // lerp
    public float durationTime = 3.0f;
    private float currentTime = 0.0f;
    private Vector3 startPos; 
    private Vector3 endPos;

    void Start()
    {
        buttonPos = button.transform.position;
        startPos = interactable.transform.position;
        endPos = interactable.transform.position;

        // move interactable pos/neg in x dir
        if (interactableMoveX) {
            if (interactableMoveNeg) endPos.x -= interactableDist;
            else endPos.x += interactableDist;
        }
        // move interactable pos/neg in y dir
        else {
            if (interactableMoveNeg) endPos.y -= interactableDist;
            else endPos.y += interactableDist;
        }
    }

    void Update()
    {
        button.transform.position = buttonPos;
        if (hit)
        {
            currentTime += Time.deltaTime;
            interactable.transform.position = Vector3.Lerp(startPos, endPos, (currentTime / durationTime));
        }
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
            
            hit = true;
        }
    }
}
