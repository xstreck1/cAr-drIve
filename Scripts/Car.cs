using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float speed = 10f;
    public float torque = 10f;

    public int score = 0;

    private Transform _currentTrack;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        MoveCar(horizontal, vertical);
        score += Raycast();
    }

    void MoveCar(float horizontal, float vertical)
    {
        float moveDist = speed * vertical;
        transform.Translate(Vector3.forward * moveDist * Time.deltaTime);

        float rotDist = moveDist * horizontal * torque;
        transform.Rotate(0f, rotDist * Time.deltaTime, 0f);
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
            Debug.Log($"Currently on {_currentTrack.name}");
        }

        return reward;
    }
}