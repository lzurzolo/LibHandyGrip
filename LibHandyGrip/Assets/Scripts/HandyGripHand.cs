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
    public bool logData;
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

    private HandyDataWriter _dataWriter;
    
    private void Start()
    {
        Assert.IsTrue(fingerCount > 0 && fingerCount <= _maximumFingers);
        _handyFingers = new List<GameObject>(_maximumFingers);
        GetHandyFingerReferences();
        SetThumbReferences();
        SetFingerTransforms();
        if(_drawDebugRays) SetupDebugLines();
        if (logData)
        {
            _dataWriter = new HandyDataWriter("HandyLog", true, 
                new List<String>
                {
                    "Seconds",
                    "ThumbTip_x",
                    "ThumbTip_y",
                    "ThumbTip_z",
                    "ThumbDistal_x",
                    "ThumbDistal_y",
                    "ThumbDistal_z",
                    "ThumbProximal_x",
                    "ThumbProximal_y",
                    "ThumbProximal_z",
                    "ThumbMetacarpal_x",
                    "ThumbMetacarpal_y",
                    "ThumbMetacarpal_z",
                    "IndexTip_x",
                    "IndexTip_y",
                    "IndexTip_z",
                    "IndexDistal_x",
                    "IndexDistal_y",
                    "IndexDistal_z",
                    "IndexProximal_x",
                    "IndexProximal_y",
                    "IndexProximal_z",
                    "IndexMetacarpal_x",
                    "IndexMetacarpal_y",
                    "IndexMetacarpal_z",
                    "MiddleTip_x",
                    "MiddleTip_y",
                    "MiddleTip_z",
                    "MiddleDistal_x",
                    "MiddleDistal_y",
                    "MiddleDistal_z",
                    "MiddleProximal_x",
                    "MiddleProximal_y",
                    "MiddleProximal_z",
                    "MiddleMetacarpal_x",
                    "MiddleMetacarpal_y",
                    "MiddleMetacarpal_z",
                    "RingTip_x",
                    "RingTip_y",
                    "RingTip_z",
                    "RingDistal_x",
                    "RingDistal_y",
                    "RingDistal_z",
                    "RingProximal_x",
                    "RingProximal_y",
                    "RingProximal_z",
                    "RingMetacarpal_x",
                    "RingMetacarpal_y",
                    "RingMetacarpal_z",
                    "PinkyTip_x",
                    "PinkyTip_y",
                    "PinkyTip_z",
                    "PinkyDistal_x",
                    "PinkyDistal_y",
                    "PinkyDistal_z",
                    "PinkyProximal_x",
                    "PinkyProximal_y",
                    "PinkyProximal_z",
                    "PinkyMetacarpal_x",
                    "PinkyMetacarpal_y",
                    "PinkyMetacarpal_z"
                } );
        }
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

    private void LateUpdate()
    {
        if (logData)
        {


            List<float> data = new List<float>();

            if (thumb)
            {
                var thumbPos = thumb.position;
                data.Add(thumbPos.x);
                data.Add(thumbPos.y);
                data.Add(thumbPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (thumbDistal)
            {
                var thumbDistalPos = thumbDistal.position;
                data.Add(thumbDistalPos.x);
                data.Add(thumbDistalPos.y);
                data.Add(thumbDistalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (thumbProximal)
            {
                var thumbProximalPos = thumbProximal.position;
                data.Add(thumbProximalPos.x);
                data.Add(thumbProximalPos.y);
                data.Add(thumbProximalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (thumbMetacarpal)
            {
                var thumbMetacarpalPos = thumbMetacarpal.position;
                data.Add(thumbMetacarpalPos.x);
                data.Add(thumbMetacarpalPos.y);
                data.Add(thumbMetacarpalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (index)
            {
                var indexPos = index.position;
                data.Add(indexPos.x);
                data.Add(indexPos.y);
                data.Add(indexPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (indexDistal)
            {
                var indexDistalPos = indexDistal.position;
                data.Add(indexDistalPos.x);
                data.Add(indexDistalPos.y);
                data.Add(indexDistalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (indexProximal)
            {
                var indexProximalPos = indexProximal.position;
                data.Add(indexProximalPos.x);
                data.Add(indexProximalPos.y);
                data.Add(indexProximalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (indexMetacarpal)
            {
                var indexMetacarpalPos = indexMetacarpal.position;
                data.Add(indexMetacarpalPos.x);
                data.Add(indexMetacarpalPos.y);
                data.Add(indexMetacarpalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (middle)
            {
                var middlePos = middle.position;
                data.Add(middlePos.x);
                data.Add(middlePos.y);
                data.Add(middlePos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (middleDistal)
            {
                var middleDistalPos = middleDistal.position;
                data.Add(middleDistalPos.x);
                data.Add(middleDistalPos.y);
                data.Add(middleDistalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (middleProximal)
            {
                var middleProximalPos = middleProximal.position;
                data.Add(middleProximalPos.x);
                data.Add(middleProximalPos.y);
                data.Add(middleProximalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (middleMetacarpal)
            {
                var middleMetacarpalPos = middleMetacarpal.position;
                data.Add(middleMetacarpalPos.x);
                data.Add(middleMetacarpalPos.y);
                data.Add(middleMetacarpalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (ring)
            {
                var ringPos = ring.position;
                data.Add(ringPos.x);
                data.Add(ringPos.y);
                data.Add(ringPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (ringDistal)
            {
                var ringDistalPos = ringDistal.position;
                data.Add(ringDistalPos.x);
                data.Add(ringDistalPos.y);
                data.Add(ringDistalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (ringProximal)
            {
                var ringProximalPos = ringProximal.position;
                data.Add(ringProximalPos.x);
                data.Add(ringProximalPos.y);
                data.Add(ringProximalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (ringMetacarpal)
            {
                var ringMetacarpalPos = ringMetacarpal.position;
                data.Add(ringMetacarpalPos.x);
                data.Add(ringMetacarpalPos.y);
                data.Add(ringMetacarpalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (pinky)
            {
                var pinkyPos = pinky.position;
                data.Add(pinkyPos.x);
                data.Add(pinkyPos.y);
                data.Add(pinkyPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (littleDistal)
            {
                var littleDistalPos = littleDistal.position;
                data.Add(littleDistalPos.x);
                data.Add(littleDistalPos.y);
                data.Add(littleDistalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (littleProximal)
            {
                var littleProximalPos = littleProximal.position;
                data.Add(littleProximalPos.x);
                data.Add(littleProximalPos.y);
                data.Add(littleProximalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            if (littleMetacarpal)
            {
                var littleMetacarpalPos = littleMetacarpal.position;
                data.Add(littleMetacarpalPos.x);
                data.Add(littleMetacarpalPos.y);
                data.Add(littleMetacarpalPos.z);
            }
            else
            {
                data.Add(float.NaN);
                data.Add(float.NaN);
                data.Add(float.NaN);
            }

            _dataWriter.WriteFloats(Time.time, data);
        }
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

    private void OnDestroy()
    {
        _dataWriter.Close();
    }
}
