using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using LibHandyGrip;
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

    public enum BoneType
    {
        Distal = 0,
        Proximal,
        Metacarpal
    }

    public class HandyGripFinger
    {
        public HandyGripFinger(HandyGripFingerTip t, FingerType ft)
        {
            _tip = t;
            _whichFinger = ft;
            _bones = new List<HandyGripBone>(3);
        }
        
        private HandyGripFingerTip _tip;
        private List<HandyGripBone> _bones;
        private FingerType _whichFinger;

        public HandyGripBone GetBone(LibHandyGrip.BoneType b)
        {
            return _bones[(int)b];
        }
        
        public void SetBone(BoneType bt, HandyGripBone hgb)
        {
            _bones.Insert((int)bt, hgb);
        }

        public HandyGripFingerTip GetTip()
        {
            return _tip;
        }

        public void SetTipTransform(Transform t)
        {
            _tip.SetTransform(t);
        }

        public void SetBoneTransform(BoneType b, Transform t)
        {
            _bones[(int)b].SetTransform(t);
        }
        
        public void SetBoneVisibility(BoneType b, bool isVisible)
        {
            _bones[(int)b].GetComponent<Renderer>().enabled = isVisible;
        }
    }

    public class HandyGripThumb
    {
        public HandyGripThumb(HandyGripThumbTip t)
        {
            _tip = t;
            _whichFinger = FingerType.Thumb;
            _bones = new List<HandyGripBone>(3);
        }
        
        private HandyGripThumbTip _tip;
        private List<HandyGripBone> _bones;
        private FingerType _whichFinger;
        
        public HandyGripBone GetBone(LibHandyGrip.BoneType b)
        {
            return _bones[(int)b];
        }

        public void SetBone(BoneType bt, HandyGripBone hgb)
        {
            _bones.Insert((int)bt, hgb);
        }

        public HandyGripThumbTip GetTip()
        {
            return _tip;
        }

        public void SetTipTransform(Transform t)
        {
            _tip.SetTransform(t);
        }

        public void SetBoneTransform(BoneType b, Transform t)
        {
            _bones[(int)b].SetTransform(t);
        }

        public void SetBoneVisibility(BoneType b, bool isVisible)
        {
            _bones[(int)b].GetComponent<Renderer>().enabled = isVisible;
        }
    }
};

public class HandyGripHand : MonoBehaviour
{
    public bool logData;
    public int fingerCount;
    private const int _maximumFingers = 4;
    public Transform thumbTip;
    public Transform indexTip;
    public Transform middleTip;
    public Transform ringTip;
    public Transform littleTip;
    
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
    
    private HandyGripThumb _thumb;
    private List<HandyGripFinger> _fingers;

    public bool _drawDebugRays;
    private List<GameObject> _debugLines;
    
    private HandyGripBone _wrist;
    private HandyGripBone _palm;
    
    private HandyDataWriter _dataWriter;
    
    private void Start()
    {
        Assert.IsTrue(fingerCount > 0 && fingerCount <= _maximumFingers);
        //_handyFingers = new List<HandyGripFingerTip>(_maximumFingers);
        _fingers = new List<HandyGripFinger>(_maximumFingers);
        GetHandyFingerReferences();
        SetThumbReferences();
        SetFingerTransforms();
        if(_drawDebugRays) InitDebugLines();
        if (logData) InitLog();
    }

    private void Update()
    {
        if(_drawDebugRays) UpdateDebugLines();
    }

    private void FixedUpdate()
    {
        var thumbColl = _thumb.GetTip().GetCurrentCollidedObject();
        var indexColl = _fingers[0].GetTip().GetCurrentCollidedObject();

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
        if (logData) LogData();
    }

    private void GetHandyFingerReferences()
    {
        _thumb = new HandyGripThumb(transform.Find("Thumb").GetComponent<HandyGripThumbTip>());

        _fingers.Insert((int)FingerType.Index, new HandyGripFinger(transform.Find("Index").GetComponent<HandyGripFingerTip>(), FingerType.Index));
        _fingers.Insert((int)FingerType.Middle, new HandyGripFinger(transform.Find("Middle").GetComponent<HandyGripFingerTip>(), FingerType.Middle));
        _fingers.Insert((int)FingerType.Ring, new HandyGripFinger(transform.Find("Ring").GetComponent<HandyGripFingerTip>(), FingerType.Ring));
        _fingers.Insert((int)FingerType.Pinky, new HandyGripFinger(transform.Find("Pinky").GetComponent<HandyGripFingerTip>(), FingerType.Pinky));

        _wrist = transform.Find("Wrist").GetComponent<HandyGripBone>();
        _palm = transform.Find("Palm").GetComponent<HandyGripBone>();

        var thumbDist = transform.Find("Thumb").transform.Find("ThumbDistal");
        _thumb.SetBone(BoneType.Distal, thumbDist.GetComponent<HandyGripBone>());
        var thumbProx = thumbDist.transform.Find("ThumbProximal");
        _thumb.SetBone(BoneType.Proximal, thumbProx.GetComponent<HandyGripBone>());
        _thumb.SetBone(BoneType.Metacarpal, thumbProx.transform.Find("ThumbMetacarpal").GetComponent<HandyGripBone>());
        
        var indexDist = transform.Find("Index").transform.Find("IndexDistal");
        _fingers[(int)FingerType.Index].SetBone(BoneType.Distal, indexDist.GetComponent<HandyGripBone>());
        var indexProx = indexDist.transform.Find("IndexProximal");
        _fingers[(int)FingerType.Index].SetBone(BoneType.Proximal, indexProx.GetComponent<HandyGripBone>());
        _fingers[(int)FingerType.Index].SetBone(BoneType.Metacarpal, indexProx.transform.Find("IndexMetacarpal").GetComponent<HandyGripBone>());
        
        var middleDist = transform.Find("Middle").transform.Find("MiddleDistal");
        _fingers[(int)FingerType.Middle].SetBone(BoneType.Distal, middleDist.GetComponent<HandyGripBone>());
        var middleProx = middleDist.transform.Find("MiddleProximal");
        _fingers[(int)FingerType.Middle].SetBone(BoneType.Proximal, middleProx.GetComponent<HandyGripBone>());
        _fingers[(int)FingerType.Middle].SetBone(BoneType.Metacarpal, middleProx.transform.Find("MiddleMetacarpal").GetComponent<HandyGripBone>());
        
        var ringDist = transform.Find("Ring").transform.Find("RingDistal");
        _fingers[(int)FingerType.Ring].SetBone(BoneType.Distal, ringDist.GetComponent<HandyGripBone>());
        var ringProx = ringDist.transform.Find("RingProximal");
        _fingers[(int)FingerType.Ring].SetBone(BoneType.Proximal, ringProx.GetComponent<HandyGripBone>());
        _fingers[(int)FingerType.Ring].SetBone(BoneType.Metacarpal, ringProx.transform.Find("RingMetacarpal").GetComponent<HandyGripBone>());
        
        var littleDist = transform.Find("Pinky").transform.Find("PinkyDistal");
        _fingers[(int)FingerType.Pinky].SetBone(BoneType.Distal, littleDist.GetComponent<HandyGripBone>());
        var littleProx = littleDist.transform.Find("PinkyProximal");
        _fingers[(int)FingerType.Pinky].SetBone(BoneType.Proximal, littleProx.GetComponent<HandyGripBone>());
        _fingers[(int)FingerType.Pinky].SetBone(BoneType.Metacarpal, littleProx.transform.Find("PinkyMetacarpal").GetComponent<HandyGripBone>());
    }

    private void SetFingerTransforms()
    {
        _thumb.SetTipTransform(thumbTip);
        _fingers[(int)FingerType.Index].SetTipTransform(indexTip);
        _fingers[(int)FingerType.Middle].SetTipTransform(middleTip);
        _fingers[(int)FingerType.Ring].SetTipTransform(ringTip);
        _fingers[(int)FingerType.Pinky].SetTipTransform(littleTip);

        if (wrist)
        {
            _wrist.SetTransform(wrist);
        }
        else
        {
            _wrist.GetComponent<Renderer>().enabled = false;
        }

        if (palm)
        {
            _palm.SetTransform(palm);
        }
        else
        {
            _palm.GetComponent<Renderer>().enabled = false;
        }

        if (thumbDistal) _thumb.SetBoneTransform(BoneType.Distal, thumbDistal);
        else _thumb.SetBoneVisibility(BoneType.Distal, false);
        
        if (thumbProximal) _thumb.SetBoneTransform(BoneType.Proximal, thumbProximal);
        else _thumb.SetBoneVisibility(BoneType.Proximal, false);
        
        if (thumbMetacarpal) _thumb.SetBoneTransform(BoneType.Metacarpal, thumbMetacarpal);
        else _thumb.SetBoneVisibility(BoneType.Metacarpal, false);
        
        
        if(indexDistal) _fingers[(int)FingerType.Index].SetBoneTransform(BoneType.Distal, indexDistal);
        else _fingers[(int)FingerType.Index].SetBoneVisibility(BoneType.Distal, false);
        
        if(indexProximal) _fingers[(int)FingerType.Index].SetBoneTransform(BoneType.Proximal, indexProximal);
        else _fingers[(int)FingerType.Index].SetBoneVisibility(BoneType.Proximal, false);

        if(indexMetacarpal) _fingers[(int)FingerType.Index].SetBoneTransform(BoneType.Metacarpal, indexMetacarpal);
        else _fingers[(int)FingerType.Index].SetBoneVisibility(BoneType.Metacarpal, false);
        
        
        if(middleDistal) _fingers[(int)FingerType.Middle].SetBoneTransform(BoneType.Distal, middleDistal);
        else _fingers[(int)FingerType.Middle].SetBoneVisibility(BoneType.Distal, false);
        
        if(middleProximal) _fingers[(int)FingerType.Middle].SetBoneTransform(BoneType.Proximal, middleProximal);
        else _fingers[(int)FingerType.Middle].SetBoneVisibility(BoneType.Proximal, false);

        if(middleMetacarpal) _fingers[(int)FingerType.Middle].SetBoneTransform(BoneType.Metacarpal, middleMetacarpal);
        else _fingers[(int)FingerType.Middle].SetBoneVisibility(BoneType.Metacarpal, false);
        
        
        if(ringDistal) _fingers[(int)FingerType.Ring].SetBoneTransform(BoneType.Distal, ringDistal);
        else _fingers[(int)FingerType.Ring].SetBoneVisibility(BoneType.Distal, false);
        
        if(ringProximal) _fingers[(int)FingerType.Ring].SetBoneTransform(BoneType.Proximal, ringProximal);
        else _fingers[(int)FingerType.Ring].SetBoneVisibility(BoneType.Proximal, false);

        if(ringMetacarpal) _fingers[(int)FingerType.Ring].SetBoneTransform(BoneType.Metacarpal, ringMetacarpal);
        else _fingers[(int)FingerType.Ring].SetBoneVisibility(BoneType.Metacarpal, false);
        
        
        if(littleDistal) _fingers[(int)FingerType.Pinky].SetBoneTransform(BoneType.Distal, littleDistal);
        else _fingers[(int)FingerType.Pinky].SetBoneVisibility(BoneType.Distal, false);
        
        if(littleProximal) _fingers[(int)FingerType.Pinky].SetBoneTransform(BoneType.Proximal, littleProximal);
        else _fingers[(int)FingerType.Pinky].SetBoneVisibility(BoneType.Proximal, false);

        if(littleMetacarpal) _fingers[(int)FingerType.Pinky].SetBoneTransform(BoneType.Metacarpal, littleMetacarpal);
        else _fingers[(int)FingerType.Pinky].SetBoneVisibility(BoneType.Metacarpal, false);
    }
    
    private void SetThumbReferences()
    {
        foreach (var hf in _fingers)
        {
            hf.GetTip().SetThumbReference(_thumb.GetTip());
        }

        var fingerScripts = new List<HandyGripFingerTip>(4);
        for (int i = 0; i < fingerScripts.Capacity; i++)
        {
            fingerScripts.Insert(i, _fingers[i].GetTip());
        }
        
        _thumb.GetTip().SetFingerReferences(fingerScripts);
    }
    
    private void InitDebugLines()
    {
        _debugLines = new List<GameObject>(_maximumFingers);

        for (int i = 0; i < _debugLines.Capacity; i++)
        {
            var line = new GameObject();
            line.transform.position = _thumb.GetTip().transform.position;
            var lr = line.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Standard"));
            lr.material.color = Color.red;
            lr.SetPosition(0, _thumb.GetTip().transform.position);
            lr.SetPosition(1, _fingers[i].GetTip().transform.position);
            lr.startWidth = 0.001f;
            lr.endWidth = 0.001f;
            _debugLines.Insert(i, line);
        }
    }

    private void InitLog()
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
            }
        );
    }

    private void UpdateDebugLines()
    {
        for (int i = 0; i < _debugLines.Count; i++)
        {
            var lr = _debugLines[i].GetComponent<LineRenderer>();
            var finger = _fingers[i].GetTip();
            if (finger.AreObjectsWithinGrasp())
            {
                lr.material.color = Color.green;
            }
            else
            {
                lr.material.color = Color.red;
            }
            lr.SetPosition(0, _thumb.GetTip().transform.position);
            lr.SetPosition(1, _fingers[i].GetTip().transform.position);
        }
    }

    private void MoveObject(HandyGripObject hgo)
    {
        var midpoint = 0.5f * (_thumb.GetTip().transform.position + _fingers[0].GetTip().transform.position);
        hgo.SetGrabPosition(midpoint);
        
        var targetDirection = (_fingers[0].GetTip().transform.position - _thumb.GetTip().transform.position).normalized;
        hgo.SetGrabRotation(targetDirection);
    }

    private void OnDestroy()
    {
        if(logData) _dataWriter.Close();
    }

    private void LogData()
    {
        List<float> data = new List<float>();

            if (thumbTip)
            {
                var thumbPos = thumbTip.position;
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

            if (indexTip)
            {
                var indexPos = indexTip.position;
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

            if (middleTip)
            {
                var middlePos = middleTip.position;
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

            if (ringTip)
            {
                var ringPos = ringTip.position;
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

            if (littleTip)
            {
                var pinkyPos = littleTip.position;
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

