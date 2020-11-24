using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticles : MonoBehaviour
{
    [SerializeField]
    private PooledMonoBehaviour deathParticlePrefab;

    private IDie entity;

    private void Awake()
    {
        entity = GetComponent<IDie>();
    }
    private void OnEnable()
    {
        entity.OnDied += Character_OnDied;
    }

    private void Character_OnDied(IDie entity)
    {
        entity.OnDied -= Character_OnDied;
        deathParticlePrefab.Get<PooledMonoBehaviour>(transform.position, Quaternion.identity);
    }

    private void OnDisable()
    {
        entity.OnDied -= Character_OnDied;
    }
}