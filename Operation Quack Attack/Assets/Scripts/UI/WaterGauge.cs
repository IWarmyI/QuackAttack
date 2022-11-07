using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterGauge : MonoBehaviour
{
    public GameObject player;
    public float currWater;
    public Image image;
    void Start()
    {
        currWater = player.GetComponent<Player>().currentWater;
    }

    public void UpdateBar(float currentWater,float maxWater)
    {
        image.fillAmount = Mathf.Clamp(currentWater / maxWater, 0, 1f);
    }

  
}
