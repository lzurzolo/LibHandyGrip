using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace LibHandyGrip
{
    public enum FingerType
    {
        Index = 0,
        Middle,
        Ring,
        Pinky,
        Thumb
    }
};

public class HandyGripHand : MonoBehaviour
{
    public int fingerCount;
    private const int _maximumFingers = 4;
    public Transform thumb;
    public Transform index;
    public Transform middle;
    public Transform ring;
    public Transform pinky;

    private GameObject _handyThumb;
    private List<GameObject> _handyFingers;

    public bool _drawDebugRays;
    private List<GameObject> _debugLines;

    private HandyGripObject _lastHeldObject;
    
    private void Start()
    {
        Assert.IsTrue(fingerCount > 0 && fingerCount <= _maximumFingers);
        _handyFingers = new List<GameObject>(_maximumFingers);
        GetHandyFingerReferences();
        SetThumbReferences();
        SetFingerTransforms();
        if(_drawDebugRays) SetupDebugLines();
    }

    private void Update()
    {
        if(_drawDebugRays) UpdateDebugLines();
        var thumbColl = _handyThumb.GetComponent<HandyGripThumb>().GetCurrentCollidedObject();
        var indexColl = _handyFingers[0].GetComponent<HandyGripFinger>().GetCurrentCollidedObject();

        if (!thumbColl || !indexColl)
        {
            if(_lastHeldObject) _lastHeldObject.ReleaseObject();
            return;
        }

        if (thumbColl != indexColl)
        {
            if(_lastHeldObject) _lastHeldObject.ReleaseObject();
            return;
        }
        MoveObject(thumbColl);
        _lastHeldObject = thumbColl;
    }

    private void FixedUpdate()
    {

    }

    private void GetHandyFingerReferences()
    {
        _handyThumb = transform.Find("Thumb").gameObject;
        _handyFingers.Insert((int)LibHandyGrip.FingerType.Index, transform.Find("Index").gameObject);
        _handyFingers.Insert((int)LibHandyGrip.FingerType.Middle, transform.Find("Middle").gameObject);
        _handyFingers.Insert((int)LibHandyGrip.FingerType.Ring, transform.Find("Ring").gameObject);
        _handyFingers.Insert((int)LibHandyGrip.FingerType.Pinky, transform.Find("Pinky").gameObject);

        for (int i = 0; i < _handyFingers.Count; i++)
        {
            _handyFingers[i].GetComponent<HandyGripFinger>().SetFingerID((LibHandyGrip.FingerType)i);
        }
    }

    private void SetFingerTransforms()
    {
        _handyThumb.GetComponent<HandyGripThumb>().SetTransform(thumb);
        _handyFingers[(int)LibHandyGrip.FingerType.Index].GetComponent<HandyGripFinger>().SetTransform(index);
        _handyFingers[(int)LibHandyGrip.FingerType.Middle].GetComponent<HandyGripFinger>().SetTransform(middle);
        _handyFingers[(int)LibHandyGrip.FingerType.Ring].GetComponent<HandyGripFinger>().SetTransform(ring);
        _handyFingers[(int)LibHandyGrip.FingerType.Pinky].GetComponent<HandyGripFinger>().SetTransform(pinky);
    }
    
    private void SetThumbReferences()
    {
        var handyThumbScript = _handyThumb.GetComponent<HandyGripThumb>();
        foreach (var hgf in _handyFingers)
        {
            hgf.GetComponent<HandyGripFinger>().SetThumbReference(handyThumbScript);
        }

        var fingerScripts = new List<HandyGripFinger>(4);
        for (int i = 0; i < fingerScripts.Capacity; i++)
        {
            fingerScripts.Insert(i, _handyFingers[i].GetComponent<HandyGripFinger>());
        }
        
        handyThumbScript.SetFingerReferences(fingerScripts);
    }
    
    private void SetupDebugLines()
    {
        _debugLines = new List<GameObject>(_maximumFingers);

        for (int i = 0; i < _debugLines.Capacity; i++)
        {
            var line = new GameObject();
            line.transform.position = _handyThumb.transform.position;
            var lr = line.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Standard"));
            lr.material.color = Color.red;
            lr.SetPosition(0, _handyThumb.transform.position);
            lr.SetPosition(1, _handyFingers[i].transform.position);
            lr.startWidth = 0.001f;
            lr.endWidth = 0.001f;
            _debugLines.Insert(i, line);
        }
    }

    private void UpdateDebugLines()
    {
        for (int i = 0; i < _debugLines.Count; i++)
        {
            var lr = _debugLines[i].GetComponent<LineRenderer>();
            var finger = _handyFingers[i].GetComponent<HandyGripFinger>();
            if (finger.AreObjectsWithinGrasp())
            {
                lr.material.color = Color.green;
            }
            else
            {
                lr.material.color = Color.red;
            }
            lr.SetPosition(0, _handyThumb.transform.position);
            lr.SetPosition(1, _handyFingers[i].transform.position);
        }
    }

    private void MoveObject(HandyGripObject hgo)
    {
        var midpoint = 0.5f * (_handyThumb.transform.position + _handyFingers[0].transform.position);
        hgo.SetGrabPosition(midpoint);
    }
}
