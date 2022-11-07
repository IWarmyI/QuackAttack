using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDTimer : MonoBehaviour
{
    [SerializeField] private Player player;
    private TextMeshProUGUI hud;
    public static float timer = 0;
    private float water = 0;

    public bool Stop { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        hud = GetComponentInChildren<TextMeshProUGUI>();
        timer = 0;
        water = 0;
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

        water = player.currentWater;
    }

    private void LateUpdate()
    {
        int min = Mathf.FloorToInt(timer / 60.0f);
        int sec = Mathf.FloorToInt(timer - min * 60);
        int mil = Mathf.FloorToInt((timer - (min * 60 + sec)) * 100);

        string text = $"<mspace=0.9em>{min:00}</mspace>" +
                      $"'<mspace=0.9em>{sec:00}</mspace>" +
                      $"''<mspace=0.9em>{mil:00}</mspace>\n";
        text += $"Water {water}\n";
        hud.text = text;
    }
}
