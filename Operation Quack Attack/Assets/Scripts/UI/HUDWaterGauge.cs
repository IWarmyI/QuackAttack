using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDWaterGauge : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image image;
    [SerializeField] private float speed = 10;

    [SerializeField] private Material flashMat;
    private Timer flashTimer;

    float toFill = 100;

    void Start()
    {
        flashTimer = new Timer(0.2f, () => { image.material = null; });
        toFill = player.MaxWater;

        player.OnPlayerDashReady += FlashFX;
    }

    private void Update()
    {
        flashTimer.Update();
    }

    private void LateUpdate()
    {
        UpdateBar();

        if (image.fillAmount != toFill)
        {
            image.fillAmount = Mathf.Lerp(image.fillAmount, toFill, speed * Time.deltaTime);

            if (Mathf.Abs(image.fillAmount - toFill) < 0.01)
                image.fillAmount = toFill;
        }
    }

    public void UpdateBar()
    {
        UpdateBar(player.Water, player.MaxWater);
    }
    public void UpdateBar(float currentWater, float maxWater)
    {
        toFill = Mathf.Clamp(currentWater / maxWater, 0, 1f);
    }

    public void FlashFX()
    {
        image.material = flashMat;
        flashTimer.Start();
    }
}
