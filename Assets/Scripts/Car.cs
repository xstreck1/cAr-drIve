using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float speed = 10f;
    public float torque = 1f;

    public int score = 0;

    private Transform _track;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float dt = Time.deltaTime;
        MoveCar(horizontal, vertical, dt);
        score += GetTrackIncrement();
    }

    private  void MoveCar(float horizontal, float vertical, float dt)
    {
        // Translated in the direction the car is facing
        float moveDist = speed * vertical;
        transform.Translate(dt * moveDist * Vector3.forward);

        // Rotate alongside it up axis 
        float rotation = horizontal * torque * 90f;
        transform.Rotate(0f, rotation * dt, 0f);
    }

    private int GetTrackIncrement()
    {
        int reward = 0;
        var carCenter = transform.position + Vector3.up;

        // Find what tile I'm on
        if (Physics.Raycast(carCenter, Vector3.down, out var hit, 2f))
        {
            var newHit = hit.transform;
            // Check if the tile has changed
            if (_track != null && newHit != _track)
            {
                float angle = Vector3.Angle(_track.forward, newHit.position - _track.position);
                reward = (angle < 90f) ? 1 : -1;
            }

            _track = newHit;
        }

        return reward;
    }
}