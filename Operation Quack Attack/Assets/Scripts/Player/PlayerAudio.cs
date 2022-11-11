using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private Player player;

    [Header("Sound")]
    [SerializeField] private AudioClip[] quack = null;
    [SerializeField] private AudioClip[] movementSfx = null;
    private AudioSource[] _sources;
    private AudioSource quackSource;
    private AudioSource sfxSource;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
        if (player == null)
            Debug.LogError("Player Audio component requires a Player component.");

        _sources = GetComponents<AudioSource>();
        quackSource = _sources[0];
        sfxSource = _sources[1];

        if (quackSource == null)
            Debug.LogError("Quack Source is empty");
        else if (sfxSource == null)
            Debug.LogError("SFX source is empty");
        else
            quackSource.clip = quack[0];

        player.OnPlayerQuack += Quack;
        player.OnPlayerJump += Jump;
        player.OnPlayerAirJump += Jump;
        player.OnPlayerWallJump += WallJump;
        player.OnPlayerDash += Dash;
    }

    private void Quack()
    {
        //Play the quack sound effect when button is pressed
        int deepQuackPercentage = Random.Range(0, 100);

        if (deepQuackPercentage <= 10)
        {
            quackSource.PlayOneShot(quack[1]);
        }
        else if (deepQuackPercentage > 10)
        {
            quackSource.PlayOneShot(quack[0]);
        }
    }

    private void Jump()
    {
        sfxSource.PlayOneShot(movementSfx[0]);
    }
    private void WallJump()
    {
        sfxSource.PlayOneShot(movementSfx[1]);
    }
    private void Dash()
    {
        sfxSource.PlayOneShot(movementSfx[2]);
    }
}
