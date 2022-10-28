using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private List<Bullet> bulletTypes = new List<Bullet>();
    [SerializeField] private List<Bullet> bulletList = new List<Bullet>();
    [SerializeField] private Bounds bounds;

    public List<Bullet> BulletList { get { return bulletList; } }
    // Start is called before the first frame update
    void Start()
    {
        foreach(Bullet bullet in bulletTypes)
        {
            for (int i = 0; i < 10; i++)
            {
                bulletList.Add(Instantiate(bullet, Vector2.zero, Quaternion.identity, transform));
                bulletList[bulletList.Count - 1].gameObject.SetActive(false);
            }
        }

        bounds.center += transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        foreach (Bullet bullet in bulletList)
        {
            if (bullet.gameObject.activeSelf)
            {
                if (!bounds.Contains(bullet.transform.position))
                {
                    bullet.gameObject.SetActive(false);
                }
            }
        }
    }

    public Bullet SpawnBullet(GameObject owner, int bulletType, Vector2 pos, Vector2 dir)
    {
        // Create projectile instance
        foreach (Bullet bullet in bulletList)
        {
            if (bullet.gameObject.activeSelf || bullet.Id != bulletType)
            {
                continue;
            }
            else
            {
                bullet.Owner = owner;
                bullet.gameObject.SetActive(true);
                bullet.Reinstantiate(pos, dir);
                return bullet;
            }
        }

        Bullet newBullet = Instantiate(bulletTypes[bulletType], transform);
        newBullet.Owner = owner;
        newBullet.gameObject.SetActive(true);
        newBullet.Reinstantiate(pos, dir);
        return newBullet;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
