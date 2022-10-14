using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDTimer : MonoBehaviour
{
    [SerializeField] private Player player;
    private TextMeshProUGUI hud;
    private float timer = 0;

    public bool Stop { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        hud = GetComponentInChildren<TextMeshProUGUI>();
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if (player.HasStarted)
        {
            if (!Stop)
            {
                timer += Time.deltaTime;
            }
        }
    }

    private void LateUpdate()
    {
        int min = Mathf.FloorToInt(timer / 60.0f);
        int sec = Mathf.FloorToInt(timer - min * 60);
        int mil = Mathf.FloorToInt((timer - (min * 60 + sec)) * 100);

        if (mil >= 100) Debug.Log("heck");

        hud.text = $"{min.ToString("00")}:{sec.ToString("00")}.{mil.ToString("00")}";
    }
}
