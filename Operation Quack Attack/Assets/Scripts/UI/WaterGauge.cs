using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterGauge : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image image;
    [SerializeField] private float speed = 10;

    [SerializeField] private Material flashMat;
    private const float flashTime = 0.2f;
    private float flashTimer = 0.2f;

    float toFill = 100;

    void Start()
    {
        toFill = player.maxWater;
    }

    private void Update()
    {
        if (flashTimer < flashTime)
        {
            flashTimer += Time.deltaTime;

            if (flashTimer >= flashTime)
            {
                image.material = null;
            }
        }

    }

    private void LateUpdate()
    {
        UpdateBar(player.currentWater, player.maxWater);

        if (image.fillAmount != toFill)
        {
            image.fillAmount = Mathf.Lerp(image.fillAmount, toFill, speed * Time.deltaTime);

            if (Mathf.Abs(image.fillAmount - toFill) < 0.01)
                image.fillAmount = toFill;
        }
    }

    public void UpdateBar(float currentWater, float maxWater)
    {
        toFill = Mathf.Clamp(currentWater / maxWater, 0, 1f);
    }

    public void PlayFlash()
    {
        image.material = flashMat;
        flashTimer = 0;
    }
}
