using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    public Vector2 position = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        position = (Vector2)transform.position + Vector2.left;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            levelManager.Respawn(this);
            Debug.Log("TriggerEnter");
        }
    }
}
