﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandyGripBone : MonoBehaviour
{
    private Transform _transform;
    private void Start()
    {
        
    }
    
    private void Update()
    {
        transform.position = _transform.position;
    }

    public void SetTransform(Transform t)
    {
        _transform = t;
    }
}