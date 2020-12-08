using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandyGripBone : MonoBehaviour
{
    private Transform _transform;
    public bool isActive;
    private void Start()
    {
        isActive = false;
    }
    
    private void Update()
    {
        if(isActive) transform.position = _transform.position;
    }

    public void SetTransform(Transform t)
    {
        _transform = t;
    }
}
