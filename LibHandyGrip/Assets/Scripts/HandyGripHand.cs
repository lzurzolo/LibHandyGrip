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

        public void SetActive(bool a)
        {
            _tip.isActive = a;
            foreach (var b in _bones)
            {
                b.isActive = a;
            }
        }
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

        public void SetTipTransform(ref Transform t)
        {
            _tip.SetTransform(t);
        }

        public void SetBoneTransform(BoneType b, ref Transform t)
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

        public void SetActive(bool a)
        {
            foreach (var b in _bones)
            {
                b.isActive = a;
            }
        }

        public void SetTipTransform(ref Transform t)
        {
            _tip.SetTransform(t);
        }

        public void SetBoneTransform(BoneType b, ref Transform t)
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
    private const int _minimumFingers = 1;
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
        Assert.IsTrue(fingerCount >= _minimumFingers && fingerCount <= _maximumFingers);
        _fingers = new List<HandyGripFinger>(fingerCount);
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
        _wrist = transform.Find("Wrist").GetComponent<HandyGripBone>();
        _palm = transform.Find("Palm").GetComponent<HandyGripBone>();
        
        _thumb = new HandyGripThumb(transform.Find("Thumb").GetComponent<HandyGripThumbTip>());
        
        var thumbDist = transform.Find("Thumb").transform.Find("ThumbDistal");
        _thumb.SetBone(BoneType.Distal, thumbDist.GetComponent<HandyGripBone>());
        var thumbProx = thumbDist.transform.Find("ThumbProximal");
        _thumb.SetBone(BoneType.Proximal, thumbProx.GetComponent<HandyGripBone>());
        _thumb.SetBone(BoneType.Metacarpal, thumbProx.transform.Find("ThumbMetacarpal").GetComponent<HandyGripBone>());
        _thumb.SetActive(true);


        string[] fingerTransforms =  {"Index", "Middle", "Ring", "Pinky"};

        for (int i = 1; i <= fingerCount; i++)
        {
            _fingers.Insert(i - 1, new HandyGripFinger(transform.Find(fingerTransforms[i - 1]).GetComponent<HandyGripFingerTip>(), (FingerType)i - 1));
            var distal = transform.Find(fingerTransforms[i - 1]).transform.Find(fingerTransforms[i - 1] + "Distal");
            _fingers[i - 1].SetBone(BoneType.Distal, distal.GetComponent<HandyGripBone>());
            var proximal = distal.transform.Find(fingerTransforms[i - 1] + "Proximal");
            _fingers[i - 1].SetBone(BoneType.Proximal, proximal.GetComponent<HandyGripBone>());
            _fingers[i - 1].SetBone(BoneType.Metacarpal, proximal.transform.Find(fingerTransforms[i - 1] + "Metacarpal").GetComponent<HandyGripBone>());
            _fingers[i - 1].SetActive(true);
        }
    }

    private void SetFingerTransforms()
    {
        SetTipReference(FingerType.Thumb, ref thumbTip);
        SetTipReference(FingerType.Index, ref indexTip);
        SetTipReference(FingerType.Middle, ref middleTip);
        SetTipReference(FingerType.Ring, ref ringTip);
        SetTipReference(FingerType.Pinky, ref littleTip);

        if (wrist) _wrist.SetTransform(wrist);
        else _wrist.GetComponent<Renderer>().enabled = false;

        if (palm) _palm.SetTransform(palm);
        else _palm.GetComponent<Renderer>().enabled = false;

        if (thumbDistal) _thumb.SetBoneTransform(BoneType.Distal, ref thumbDistal);
        else _thumb.SetBoneVisibility(BoneType.Distal, false);
        
        if (thumbProximal) _thumb.SetBoneTransform(BoneType.Proximal, ref thumbProximal);
        else _thumb.SetBoneVisibility(BoneType.Proximal, false);
        
        if (thumbMetacarpal) _thumb.SetBoneTransform(BoneType.Metacarpal, ref thumbMetacarpal);
        else _thumb.SetBoneVisibility(BoneType.Metacarpal, false);
        
        
        if(indexDistal) _fingers[(int)FingerType.Index].SetBoneTransform(BoneType.Distal, ref indexDistal);
        else _fingers[(int)FingerType.Index].SetBoneVisibility(BoneType.Distal, false);
        
        if(indexProximal) _fingers[(int)FingerType.Index].SetBoneTransform(BoneType.Proximal, ref indexProximal);
        else _fingers[(int)FingerType.Index].SetBoneVisibility(BoneType.Proximal, false);

        if(indexMetacarpal) _fingers[(int)FingerType.Index].SetBoneTransform(BoneType.Metacarpal, ref indexMetacarpal);
        else _fingers[(int)FingerType.Index].SetBoneVisibility(BoneType.Metacarpal, false);


        if (fingerCount >= 2)
        {
            if(middleDistal) _fingers[(int)FingerType.Middle].SetBoneTransform(BoneType.Distal, ref middleDistal);
            else _fingers[(int)FingerType.Middle].SetBoneVisibility(BoneType.Distal, false);
        
            if(middleProximal) _fingers[(int)FingerType.Middle].SetBoneTransform(BoneType.Proximal, ref middleProximal);
            else _fingers[(int)FingerType.Middle].SetBoneVisibility(BoneType.Proximal, false);

            if(middleMetacarpal) _fingers[(int)FingerType.Middle].SetBoneTransform(BoneType.Metacarpal, ref middleMetacarpal);
            else _fingers[(int)FingerType.Middle].SetBoneVisibility(BoneType.Metacarpal, false);
        }

        if (fingerCount >= 3)
        {
            if(ringDistal) _fingers[(int)FingerType.Ring].SetBoneTransform(BoneType.Distal, ref ringDistal);
            else _fingers[(int)FingerType.Ring].SetBoneVisibility(BoneType.Distal, false);
        
            if(ringProximal) _fingers[(int)FingerType.Ring].SetBoneTransform(BoneType.Proximal, ref ringProximal);
            else _fingers[(int)FingerType.Ring].SetBoneVisibility(BoneType.Proximal, false);

            if(ringMetacarpal) _fingers[(int)FingerType.Ring].SetBoneTransform(BoneType.Metacarpal, ref ringMetacarpal);
            else _fingers[(int)FingerType.Ring].SetBoneVisibility(BoneType.Metacarpal, false);
        }

        if (fingerCount == 4)
        {
            if(littleDistal) _fingers[(int)FingerType.Pinky].SetBoneTransform(BoneType.Distal, ref littleDistal);
            else _fingers[(int)FingerType.Pinky].SetBoneVisibility(BoneType.Distal, false);
        
            if(littleProximal) _fingers[(int)FingerType.Pinky].SetBoneTransform(BoneType.Proximal, ref littleProximal);
            else _fingers[(int)FingerType.Pinky].SetBoneVisibility(BoneType.Proximal, false);

            if(littleMetacarpal) _fingers[(int)FingerType.Pinky].SetBoneTransform(BoneType.Metacarpal, ref littleMetacarpal);
            else _fingers[(int)FingerType.Pinky].SetBoneVisibility(BoneType.Metacarpal, false);
        }
    }
    
    private void SetThumbReferences()
    {
        foreach (var hf in _fingers)
        {
            hf.GetTip().SetThumbReference(_thumb.GetTip());
        }

        var fingerScripts = new List<HandyGripFingerTip>(fingerCount);
        for (int i = 0; i < fingerScripts.Capacity; i++)
        {
            fingerScripts.Insert(i, _fingers[i].GetTip());
        }
        
        _thumb.GetTip().SetFingerReferences(fingerScripts);
    }
    
    private void InitDebugLines()
    {
        _debugLines = new List<GameObject>(fingerCount);

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

    public void SetBoneReference(FingerType ft, BoneType bt, ref Transform t)
    {
        if (ft == FingerType.Thumb)
        {
            if (bt == BoneType.Distal)
            {
                thumbDistal = t;
            }
            else if(bt == BoneType.Proximal)
            {
                thumbProximal = t;
            }
            else if (bt == BoneType.Metacarpal)
            {
                thumbMetacarpal = t;
            }
            
            _thumb.SetBoneTransform(bt, ref t);
        }
        else if (ft == FingerType.Index)
        {
            if (bt == BoneType.Distal)
            {
                indexDistal = t;
            }
            else if(bt == BoneType.Proximal)
            {
                indexProximal = t;
            }
            else if (bt == BoneType.Metacarpal)
            {
                thumbMetacarpal = t;
            }
            
            _fingers[(int)ft].SetBoneTransform(bt, ref t);
        }
        else if (ft == FingerType.Middle)
        {
            if (bt == BoneType.Distal)
            {
                middleDistal = t;
            }
            else if(bt == BoneType.Proximal)
            {
                middleProximal = t;
            }
            else if (bt == BoneType.Metacarpal)
            {
                middleMetacarpal = t;
            }
            
            _fingers[(int)ft].SetBoneTransform(bt, ref t);
        }
        else if (ft == FingerType.Ring)
        {
            if (bt == BoneType.Distal)
            {
                ringDistal = t;
            }
            else if(bt == BoneType.Proximal)
            {
                ringProximal = t;
            }
            else if (bt == BoneType.Metacarpal)
            {
                ringMetacarpal = t;
            }
            
            _fingers[(int)ft].SetBoneTransform(bt, ref t);
        }
        else if (ft == FingerType.Pinky)
        {
            if (bt == BoneType.Distal)
            {
                littleDistal = t;
            }
            else if(bt == BoneType.Proximal)
            {
                littleProximal = t;
            }
            else if (bt == BoneType.Metacarpal)
            {
                littleMetacarpal = t;
            }
            
            _fingers[(int)ft].SetBoneTransform(bt, ref t);
        }
        
    }

    public void SetTipReference(FingerType ft, ref Transform t)
    {
        if ((int) ft > fingerCount - 1 && ft != FingerType.Thumb) return;
        if (ft == FingerType.Thumb)
        {
            thumbTip = t;
            _thumb.SetTipTransform(ref t);
        }
        else if (ft == FingerType.Index)
        {
            indexTip = t;
            _fingers[(int)ft].SetTipTransform(ref t);
        }
        else if (ft == FingerType.Middle)
        {
            middleTip = t;
            _fingers[(int)ft].SetTipTransform(ref t);
        }
        else if (ft == FingerType.Ring)
        {
            ringTip = t;
            _fingers[(int)ft].SetTipTransform(ref t);
        }
        else if (ft == FingerType.Pinky)
        {
            littleTip = t;
            _fingers[(int)ft].SetTipTransform(ref t);
        }
    }
}

