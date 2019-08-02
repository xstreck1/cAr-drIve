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
        
    }

    void MoveCar(float horizontal, float vertical)
    {
        float distance = speed * vertical;
        transform.Translate(Vector3.forward * distance * Time.fixedDeltaTime);

        float rotation = horizontal * torque * 90f;
        transform.Rotate(0f, rotation * Time.fixedDeltaTime, 0f);
    }
    
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float horizontal = vectorAction[0];
        float vertical = vectorAction[1];

        Vector3 lastPos = transform.position;
        MoveCar(horizontal, vertical);
        Vector3 moveVect = (transform.position - lastPos) / Time.fixedDeltaTime;
        
        if (_currentTrack)
        {
            var desireVec = (Vector3.Scale(moveVect, _currentTrack.forward));
            AddReward(-0.001f + 0.001f * (desireVec.x + desireVec.y + desireVec.z));
        }

        int reward = Raycast();
        if (reward != 0)
        {
            AddReward(reward);
        }
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
        // Debug.Log(angle);
        AddVectorObs(angle / 180f);
        
        // Raycast
        ObserveRay(1.5f, .5f, 25f);
        ObserveRay(1.5f, 0f, 0f);
        ObserveRay(1.5f, -.5f, -25f);
        ObserveRay(-1.5f, 0, 180f);
    }

    void ObserveRay(float x, float y, float angle)
    {
        var raySource = transform.position + Vector3.up / 2f; 
        const float RAY_DIST = 5f;
        var position = raySource + transform.forward * x + transform.right * y;
        
        Vector3 direction = Quaternion.Euler(0, angle, 0f) * transform.forward;
        RaycastHit hit;
        Physics.Raycast(position, direction, out hit, RAY_DIST);
        Debug.DrawRay(position, direction * (hit.distance > 0 ? hit.distance : RAY_DIST), Color.yellow);
        AddVectorObs(hit.distance > 0 ? hit.distance / RAY_DIST : -1f);
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
            AddReward(-1f);
            Done();
        }
    }
}