using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 pos;
    private Vector2 input = Vector2.zero;
    private Vector2 oldInput = Vector2.zero;
    [SerializeField] private Vector2 vel = Vector2.zero;
    private float speed = 0;
    [SerializeField] private float baseSpeed = 5;
    [SerializeField] private float topSpeed = 10;
    [SerializeField] private float accelRate = 10;
    const float deccel = 0.75f;

    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        speed = baseSpeed;
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get current position
        pos = transform.position;

        // Lateral movement
        if (input.x != 0)
        {
            // Reset speed when changing directions
            if (input.x + oldInput.x == 0)
            {
                speed = baseSpeed;
            }
            // Accelerate speed, clamped from baseSpeed to topSpeed
            speed = Mathf.Clamp(speed + accelRate * Time.deltaTime, baseSpeed, topSpeed);
            // Update velocity
            vel.x = input.x * speed;
        }
        else
        {
            // Reset speed and deccelerate velocity
            speed = baseSpeed;
            vel.x *= deccel;
            if (vel.magnitude < baseSpeed * 0.1f) vel = Vector2.zero;
        }

        // Apply velocity
        pos += vel * Time.deltaTime;

        // Update position
        transform.position = pos;
    }

    private void OnMove(InputValue value)
    {
        oldInput = input;
        input = value.Get<Vector2>();
        input.y = 0;
        if (input != Vector2.zero) input.Normalize();
    }

    private void OnJump(InputValue value)
    {
        //vel.y += 10;
    }

    private void OnDash(InputValue value)
    {
        // TBD
    }
}
