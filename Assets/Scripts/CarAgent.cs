using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class CarAgent : Agent
{
    public float speed = 10f;
    public float torque = 10f;

    public int score = 0;
    public bool resetOnCollision = true;

    private Transform _track;

    public override void InitializeAgent()
    {
        Raycast();
    }

    public override void AgentAction(float[] vectorAction)
    {
        float horizontal = vectorAction[0];
        float vertical = vectorAction[1];

        var lastPos = transform.position;
        MoveCar(horizontal, vertical, Time.fixedDeltaTime);

        int reward = Raycast();

        var moveVec = transform.position - lastPos;
        float direction = Vector3.Angle(moveVec, _track.forward);
        float bonus = 0.001f * (90f - direction) * vertical;
        AddReward(bonus + reward);

        score += reward;
    }

    public override void CollectObservations()
    {
        float angle = Vector3.SignedAngle(_track.forward, transform.forward, Vector3.up);

        AddVectorObs(angle / 180f);

        ObserveRay(1.5f, .5f, 25f);
        ObserveRay(1.5f, 0f, 0f);
        ObserveRay(1.5f, -.5f, -25f);
        ObserveRay(-1.5f, 0, 180f);
    }

    public override void AgentReset()
    {
        if (resetOnCollision)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    private void MoveCar(float horizontal, float vertical, float dt)
    {
        // Translated in the direction the car is facing
        float moveDist = speed * vertical;
        transform.Translate(dt * moveDist * Vector3.forward);

        // Rotate alongside it up axis 
        float rotation = horizontal * torque * 90f;
        transform.Rotate(0f, rotation * dt, 0f);
    }

    // Casts a ray from a point in a direction based on the car position
    // z: offset of the ray origin from the car centre on the z axis
    // x: as above but on the x
    // angle: direction of the ray from its origin
    private void ObserveRay(float z, float x, float angle)
    {
        var tf = transform;

        // Get the start position of the ray
        var raySource = tf.position + Vector3.up / 2f;
        const float RAY_DIST = 5f;
        var position = raySource + tf.forward * z + tf.right * x;

        // Get the angle of the ray
        var eulerAngle = Quaternion.Euler(0, angle, 0f);
        var dir = eulerAngle * tf.forward;

        
        // See if there is a hit in the given direction
        Physics.Raycast(position, dir, out var hit, RAY_DIST);
        
        Debug.DrawRay(position, dir * (hit.distance > 0 ? hit.distance : RAY_DIST), Color.yellow);
        AddVectorObs(hit.distance >= 0 ? hit.distance / RAY_DIST : -1f);
    }

    private int Raycast()
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
                var pos = _track.position + _track.forward * 10;
                float dist = Vector3.Distance(newHit.position, pos);
                reward = (dist < 1f) ? 1 : -1;
            }

            _track = hit.transform;
        }

        return reward;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("wall"))
        {
            SetReward(-1f);
            Done();
        }
    }
}