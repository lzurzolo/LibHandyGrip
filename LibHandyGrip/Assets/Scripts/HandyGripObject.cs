using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HandyGripObject : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private void Start()
    {
        // object must have a collider
        var coll = GetComponent<Collider>();
        Assert.IsNotNull(coll);
        
        // object must have a rigid body
        _rigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(_rigidbody);
    }

    private void Update()
    {
        if (!_rigidbody) Debug.Log("RB is null");
    }
    
    public void SetGrabPosition(Vector3 pos)
    {
        _rigidbody.isKinematic = true;
        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime);
    }

    public void ReleaseObject()
    {
        _rigidbody.isKinematic = false;
    }
}
