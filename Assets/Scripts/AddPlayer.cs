using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlayer : MonoBehaviour
{
    private RuningMechanics _runingMechanics;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject addPlayer;
    [SerializeField] private GameObject InstatiatePlay;
    [SerializeField] private GameObject SpawnMark;
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            Destroy(addPlayer);
            InstatiatePlayer();
        }
    }
    private void InstatiatePlayer()
    {
        Instantiate(InstatiatePlay,SpawnMark.transform);
    }

}
