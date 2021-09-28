using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class RuningMechanics : MonoBehaviour
{
    [SerializeField]public GameObject player;
    [SerializeField] private float speed = 10f;
    public Vector3 playerPosition;

    private void Start()
    {
        player.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
    private void Update()
    {
        player.transform.Translate(Vector3.up * speed);
        playerPosition = player.transform.position;
    }
}
