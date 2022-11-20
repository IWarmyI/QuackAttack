using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private bool inheritRotation = true;
    private bool flag = false;

    private void Start()
    {
        flag = true;
    }

    private void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;
        if (!flag) return;

        GameObject effect = Instantiate(effectPrefab, transform.parent, true);
        effect.transform.position = transform.position;
        effect.transform.rotation = inheritRotation ? transform.rotation : Quaternion.identity;
    }
}
