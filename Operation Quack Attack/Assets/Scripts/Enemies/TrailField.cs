using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailField : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (TryGetComponent(out ParticleSystem ps)) {
            ParticleSystemForceField ff;
            GameObject pl = GameObject.FindWithTag("Player");
            try
            {
                ff = pl.GetComponentInChildren<ParticleSystemForceField>();
                ps.externalForces.SetInfluence(0, ff);
            }
            catch
            {
                Debug.LogWarning($"No player particle force field found in \"{pl.name}\".");
            }
        }
    }
}
