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