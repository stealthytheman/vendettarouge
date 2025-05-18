using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    [SerializeField] private Vector3 offset; // Offset from the player
    [SerializeField] private float dampening; // Smooth transition speed
    [SerializeField] private Transform target; // The target (player)

    private Vector3 vel = Vector3.zero;

    private void Start()
    {
        AssignPlayerAsTarget(); // Dynamically assign the player as the target
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            AssignPlayerAsTarget(); // Reassign the player if the target is missing
            if (target == null) return; // Exit if the target is still not assigned
        }

        // Smoothly follow the target (player)
        Vector3 targetPosition = target.position + offset;
        targetPosition.z = -10f; // Ensure Z stays at -10
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, dampening);
    }

    private void AssignPlayerAsTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Ensure the player is tagged as 'Player'.");
        }
    }
}
