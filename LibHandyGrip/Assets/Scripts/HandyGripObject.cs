using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HandyGripObject : MonoBehaviour
{
    private void Start()
    {
        // object must have a collider
        var coll = GetComponent<Collider>();
        Assert.IsNotNull(coll);
        
        // object must have a rigid body
        var rb = GetComponent<Rigidbody>();
        Assert.IsNotNull(rb);
    }
}
