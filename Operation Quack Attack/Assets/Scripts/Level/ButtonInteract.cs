using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteract : MonoBehaviour
{
    // interactable to change with button
    public GameObject interactable;
    // check for button hit
    public static bool hit = false;
    // indicate button has been pressed
    public GameObject button;
    private Vector3 buttonpos;
    private Vector3 interactablepos;

    void Start()
    {
        buttonpos = button.transform.position;
        interactablepos = interactable.transform.position;
        buttonpos.x -= 0.5f;
        interactablepos.y += 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            // for now set active, later animation?
            //interactable.SetActive(false);
            button.transform.position = buttonpos;
            interactable.transform.position = interactablepos;
        }
    }
}
