using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HandyGripObject : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Quaternion _initialOrientation;
    private bool _isBeingMoved;
    
    private void Start()
    {
        // object must have a collider
        var coll = GetComponent<Collider>();
        Assert.IsNotNull(coll);
        
        // object must have a rigid body
        _rigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(_rigidbody);
        _initialOrientation = transform.rotation;
        _isBeingMoved = false;
    }

    private void Update()
    {
        if (!_rigidbody) Debug.Log("RB is null");
        if (!_isBeingMoved) _initialOrientation = transform.rotation;
    }
    
    public void SetGrabPosition(Vector3 pos)
    {
        _rigidbody.isKinematic = true;
        _isBeingMoved = true;
        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime);
    }

    public void SetGrabRotation(Vector3 dir)
    {
        Quaternion newOrientation = Quaternion.LookRotation(dir);
        newOrientation *= _initialOrientation;
        transform.rotation = newOrientation;
    }

    public void ReleaseObject()
    {
        _rigidbody.isKinematic = false;
        _isBeingMoved = false;
    }
}
