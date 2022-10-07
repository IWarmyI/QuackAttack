using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;
    [SerializeField] private float respawnTime = 10;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i] == null) return;

        if (!enemy[i].activeInHierarchy)
        {
            if (timer > respawnTime)
            {
                enemy[i].SetActive(true);
                enemy[i].GetComponent<IDamageable>().Health = 1;
                timer = 0;
                return;
            }

            timer += Time.deltaTime;
        }
        }
    }
}
