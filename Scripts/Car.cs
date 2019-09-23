using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float speed = 10f;
    public float torque = 10f;

    public int score = 0;

    private Transform _track;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float dt = Time.deltaTime;
        MoveCar(horizontal, vertical, dt);
        score += Raycast();
    }

    void MoveCar(float horizontal, float vertical, float dt)
    {
        // Translated in the direction the car is facing
        float moveDist = speed * vertical;
        transform.Translate(Vector3.forward * moveDist * dt);

        // Rotate alongside it up axis 
        float rotation = horizontal * torque * 90f;
        transform.Rotate(0f, rotation * dt, 0f);
    }

    int Raycast()
    {
        int reward = 0;
        RaycastHit hit;
        var carCenter = transform.position + Vector3.up;
        if (Physics.Raycast(carCenter, Vector3.down, out hit, 2f))
        {
            Debug.DrawRay(carCenter, Vector3.down * 2f, Color.white);
            var newHit = hit.transform;
            if (_track != null && newHit != _track)
            {
                Vector3 pos = _track.position + _track.forward * 10;
                float dist = Vector3.Distance(newHit.position,pos);
                reward = (dist < 1f) ? 1 : -1;
            }
            _track = hit.transform;
            Debug.Log($"Currently on {_track.name}");
        }

        return reward;
    }
}