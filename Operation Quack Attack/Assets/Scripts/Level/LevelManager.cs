using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();
    
    public Player Player { get => player; }
    public List<Checkpoint> Checkpoints { get => checkpoints; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn(Checkpoint checkpoint)
    {
        Player.Respawn(checkpoint.position);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
