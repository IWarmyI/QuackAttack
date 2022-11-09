using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;
    [SerializeField] private float[] timers;
    [SerializeField] private float respawnTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        timers = new float[enemy.Length];
        for (int i = 0; i < enemy.Length; i++)
        {
            timers[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i] == null) return;

            if (!enemy[i].activeInHierarchy)
            {
                if (timers[i] > respawnTime)
                {
                    enemy[i].SetActive(true);
                    timers[i] = 0;
                    return;
                }

                timers[i] += Time.deltaTime;
            }
        }
    }
}
