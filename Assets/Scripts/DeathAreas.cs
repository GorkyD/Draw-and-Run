using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class DeathAreas : MonoBehaviour
{
    private RuningMechanics _runingMechanics;
    [SerializeField] private ParticleSystem particle;
    private void OnCollisionEnter(Collision other)
    {
        Destroy(other.collider.gameObject);
        particle.Play();
    }
}
