using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

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
    // TODO : refactor fingers into structs
    [Header("Optional Hand Positions")] 
    public Transform wrist;
    public Transform palm;
    public Transform thumbDistal;
    public Transform thumbProximal;
    public Transform thumbMetacarpal;
    public Transform indexDistal;
    public Transform indexProximal;
    public Transform indexMetacarpal;
    public Transform middleDistal;
    public Transform middleProximal;
    public Transform middleMetacarpal;
    public Transform ringDistal;
    public Transform ringProximal;
    public Transform ringMetacarpal;
    public Transform littleDistal;
    public Transform littleProximal;
    public Transform littleMetacarpal;
    
    private GameObject _handyWrist;
    private GameObject _handyPalm;

    private GameObject _handyThumbDistal;
    private GameObject _handyThumbProximal;
    private GameObject _handyThumbMetacarpal;
    
    private GameObject _handyIndexDistal;
    private GameObject _handyIndexProximal;
    private GameObject _handyIndexMetacarpal;
    
    private GameObject _handyMiddleDistal;
    private GameObject _handyMiddleProximal;
    private GameObject _handyMiddleMetacarpal;
    
    private GameObject _handyRingDistal;
    private GameObject _handyRingProximal;
    private GameObject _handyRingMetacarpal;
    
    private GameObject _handyLittleDistal;
    private GameObject _handyLittleProximal;
    private GameObject _handyLittleMetacarpal;
    
    
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
    }

    private void FixedUpdate()
    {
        var thumbColl = _handyThumb.GetComponent<HandyGripThumb>().GetCurrentCollidedObject();
        var indexColl = _handyFingers[0].GetComponent<HandyGripFinger>().GetCurrentCollidedObject();

        if ((!thumbColl || !indexColl) || thumbColl != indexColl)
        {
            if(_lastHeldObject) _lastHeldObject.ReleaseObject();
            _lastHeldObject = null;
            return;
        }

        MoveObject(thumbColl);
        _lastHeldObject = thumbColl;
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

        _handyWrist = transform.Find("Wrist").gameObject;
        _handyPalm = transform.Find("Palm").gameObject;

        _handyThumbDistal = transform.Find("Thumb").transform.Find("ThumbDistal").gameObject;
        _handyThumbProximal = transform.Find("Thumb").transform.Find("ThumbDistal").transform.Find("ThumbProximal").gameObject;
        _handyThumbMetacarpal = transform.Find("Thumb").transform.Find("ThumbDistal").transform.Find("ThumbProximal").transform.Find("ThumbMetacarpal").gameObject;
        
        _handyIndexDistal = transform.Find("Index").transform.Find("IndexDistal").gameObject;
        _handyIndexProximal = transform.Find("Index").transform.Find("IndexDistal").transform.Find("IndexProximal").gameObject;
        _handyIndexMetacarpal = transform.Find("Index").transform.Find("IndexDistal").transform.Find("IndexProximal").transform.Find("IndexMetacarpal").gameObject;
        
        _handyMiddleDistal = transform.Find("Middle").transform.Find("MiddleDistal").gameObject;
        _handyMiddleProximal = transform.Find("Middle").transform.Find("MiddleDistal").transform.Find("MiddleProximal").gameObject;
        _handyMiddleMetacarpal = transform.Find("Middle").transform.Find("MiddleDistal").transform.Find("MiddleProximal").transform.Find("MiddleMetacarpal").gameObject;
        
        _handyRingDistal = transform.Find("Ring").transform.Find("RingDistal").gameObject;
        _handyRingProximal = transform.Find("Ring").transform.Find("RingDistal").transform.Find("RingProximal").gameObject;
        _handyRingMetacarpal = transform.Find("Ring").transform.Find("RingDistal").transform.Find("RingProximal").transform.Find("RingMetacarpal").gameObject;
        
        _handyLittleDistal = transform.Find("Pinky").transform.Find("PinkyDistal").gameObject;
        _handyLittleProximal = transform.Find("Pinky").transform.Find("PinkyDistal").transform.Find("PinkyProximal").gameObject;
        _handyLittleMetacarpal = transform.Find("Pinky").transform.Find("PinkyDistal").transform.Find("PinkyProximal").transform.Find("PinkyMetacarpal").gameObject;
    }

    private void SetFingerTransforms()
    {
        _handyThumb.GetComponent<HandyGripThumb>().SetTransform(thumb);
        _handyFingers[(int)LibHandyGrip.FingerType.Index].GetComponent<HandyGripFinger>().SetTransform(index);
        _handyFingers[(int)LibHandyGrip.FingerType.Middle].GetComponent<HandyGripFinger>().SetTransform(middle);
        _handyFingers[(int)LibHandyGrip.FingerType.Ring].GetComponent<HandyGripFinger>().SetTransform(ring);
        _handyFingers[(int)LibHandyGrip.FingerType.Pinky].GetComponent<HandyGripFinger>().SetTransform(pinky);

        if (wrist)
        {
            _handyWrist.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(wrist);
        }
        else
        {
            _handyWrist.SetActive(false);
        }

        if (palm)
        {
            _handyPalm.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(palm);
        }
        else
        {
            _handyPalm.SetActive(false);
        }

        if (thumbDistal) _handyThumbDistal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(thumbDistal);
        else _handyThumbDistal.SetActive(false);
        
        if (thumbProximal) _handyThumbProximal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(thumbProximal);
        else _handyThumbProximal.SetActive(false);
        
        if (thumbMetacarpal) _handyThumbMetacarpal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(thumbMetacarpal);
        else _handyThumbMetacarpal.SetActive(false);
        
        
        if (indexDistal) _handyIndexDistal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(indexDistal);
        else _handyIndexDistal.SetActive(false);
        
        if (indexProximal) _handyIndexProximal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(indexProximal);
        else _handyIndexProximal.SetActive(false);
        
        if (indexMetacarpal) _handyIndexMetacarpal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(indexMetacarpal);
        else _handyIndexMetacarpal.SetActive(false);
        
        
        if (middleDistal) _handyMiddleDistal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(middleDistal);
        else _handyMiddleDistal.SetActive(false);
        
        if (middleProximal) _handyMiddleProximal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(middleProximal);
        else _handyMiddleProximal.SetActive(false);
        
        if (middleMetacarpal) _handyMiddleMetacarpal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(middleMetacarpal);
        else _handyMiddleMetacarpal.SetActive(false);
        
        
        if (ringDistal) _handyRingDistal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(ringDistal);
        else _handyRingDistal.SetActive(false);
        
        if (ringProximal) _handyRingProximal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(ringProximal);
        else _handyRingProximal.SetActive(false);
        
        if (ringMetacarpal) _handyRingMetacarpal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(ringMetacarpal);
        else _handyRingMetacarpal.SetActive(false);
        
        
        if (littleDistal) _handyLittleDistal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(littleDistal);
        else _handyLittleDistal.SetActive(false);
        
        if (littleProximal) _handyLittleProximal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(littleProximal);
        else _handyLittleProximal.SetActive(false);
        
        if (littleMetacarpal) _handyLittleMetacarpal.GetComponent<HandyGripNonCollidableHandPosition>().SetTransform(littleMetacarpal);
        else _handyLittleMetacarpal.SetActive(false);
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
        
        var targetDirection = (_handyFingers[0].transform.position - _handyThumb.transform.position).normalized;
        hgo.SetGrabRotation(targetDirection);
    }
}
