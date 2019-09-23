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

    private Transform _currentTrack;

    public override void InitializeAgent()
    {
        Raycast();
    }
    
    void MoveCar(float horizontal, float vertical, float dt)
    {
        float distance = speed * vertical;
        transform.Translate(Vector3.forward * distance * dt);

        float rotation = horizontal * torque * 90f;
        transform.Rotate(0f, rotation * dt, 0f);
    }
    
    
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        var lastPos = transform.position;
        
        float horizontal = vectorAction[0];
        float vertical = vectorAction[1];

        float dt = Time.fixedDeltaTime;
        MoveCar(horizontal, vertical, dt);
        
        int reward = Raycast();

        var moveVec = (transform.position - lastPos) / (dt * speed);
        float dirReward = (moveVec + _currentTrack.forward).magnitude;
        AddReward(0.001f * (dirReward - 1f) + reward);
        score += reward;
    }
    
    public override void CollectObservations()
    {
        // Angle
        float angle = 0f;
        if (_currentTrack != null)
        {
            angle = Vector3.SignedAngle(_currentTrack.forward, transform.forward, Vector3.up);
        }
        AddVectorObs(angle / 180f);
        
        // Raycast
        ObserveRay(1.5f, .5f, 25f);
        ObserveRay(1.5f, 0f, 0f);
        ObserveRay(1.5f, -.5f, -25f);
        ObserveRay(-1.5f, 0, 180f);
    }

    // Casts a ray from a point in a direction based on the car position
    // z: offset of the ray origin from the car centre on the z axis
    // x: as above but on the x
    // angle: direction of the ray from its origin
    void ObserveRay(float z, float x, float angle)
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
        AddVectorObs(hit.distance >= 0 ? hit.distance / RAY_DIST : -1f);
        
        Debug.DrawRay(position, dir * (hit.distance > 0 ? hit.distance : RAY_DIST), Color.yellow);
    }
    
    int Raycast()
    {
        int reward = 0;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f))
        {
            Debug.DrawRay(transform.position + Vector3.up, Vector3.down * 2f, Color.white);
            var newHit = hit.transform;
            if (_currentTrack != null && newHit != _currentTrack)
            {
                reward = (Vector3.Distance(newHit.position, _currentTrack.position + _currentTrack.forward * 10) < 1f)
                    ? 1
                    : -1;
            }

            _currentTrack = hit.transform;
            Debug.Log($"{gameObject.name} currently on {_currentTrack.name}");
        }

        return reward;
    }

    public override void AgentReset()
    {
        if (resetOnCollision)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "wall")
        {
            SetReward(-1f);
            Done();
        }
    }
}