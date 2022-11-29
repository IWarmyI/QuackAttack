using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class HUDTimer : MonoBehaviour
{
    public static bool RestartFlag = true;

    [SerializeField] private Player player;
    private TextMeshProUGUI hud;
    public static float timer = 0;
    private float water = 0;

    public bool Stop { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        hud = GetComponentInChildren<TextMeshProUGUI>();
        water = 0;
        if (RestartFlag)
        {
            timer = 0;
            RestartFlag = false;
        }
    }

    // Restart timer
    public static void Initialize()
    {
        RestartFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        if (LevelManager.IsLoading) return;

        if (player.HasStarted)
        {
            if (!Stop && Time.timeScale != 0)
            {
                timer += Time.deltaTime * 100;
            }
        }

        water = player.Water;
    }

    private void LateUpdate()
    {
        int min = (int)(timer / 6000);
        int sec = (int)(timer / 100 - min * 60);
        int dec = (int)(timer - (min * 6000 + sec * 100));

        string text = $"<mspace=0.95em>{min:00}</mspace>" +
                      $"'<mspace=0.95em>{sec:00}</mspace>" +
                      $"''<mspace=0.95em>{dec:00}</mspace>\n";
        //text += $"Water {(int)water}\n";
        hud.text = text;
    }
}
