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
    
    private void Start()
    {
        Assert.IsTrue(fingerCount > 0 && fingerCount <= _maximumFingers);
        _handyFingers = new List<GameObject>(_maximumFingers);
        GetHandyFingerReferences();
        SetFingerTransforms();
        if(_drawDebugRays) SetupDebugLines();
    }

    private void Update()
    {
        if(_drawDebugRays) DrawDebugRays();
    }

    private void GetHandyFingerReferences()
    {
        _handyThumb = transform.Find("Thumb").gameObject;
        _handyFingers.Insert((int)LibHandyGrip.FingerType.Index, transform.Find("Index").gameObject);
        _handyFingers.Insert((int)LibHandyGrip.FingerType.Middle, transform.Find("Middle").gameObject);
        _handyFingers.Insert((int)LibHandyGrip.FingerType.Ring, transform.Find("Ring").gameObject);
        _handyFingers.Insert((int)LibHandyGrip.FingerType.Pinky, transform.Find("Pinky").gameObject);
    }

    private void SetFingerTransforms()
    {
        _handyThumb.GetComponent<HandyGripFinger>().SetTransform(thumb);
        _handyFingers[(int)LibHandyGrip.FingerType.Index].GetComponent<HandyGripFinger>().SetTransform(index);
        _handyFingers[(int)LibHandyGrip.FingerType.Middle].GetComponent<HandyGripFinger>().SetTransform(middle);
        _handyFingers[(int)LibHandyGrip.FingerType.Ring].GetComponent<HandyGripFinger>().SetTransform(ring);
        _handyFingers[(int)LibHandyGrip.FingerType.Pinky].GetComponent<HandyGripFinger>().SetTransform(pinky);
    }

    private void DrawDebugRays()
    {
        Debug.DrawRay(_handyThumb.transform.position, _handyFingers[(int)LibHandyGrip.FingerType.Index].transform.position, Color.red, 0, true);
        Debug.DrawRay(_handyThumb.transform.position, _handyFingers[(int)LibHandyGrip.FingerType.Middle].transform.position, Color.red, 0, true);
        Debug.DrawRay(_handyThumb.transform.position, _handyFingers[(int)LibHandyGrip.FingerType.Ring].transform.position, Color.red, 0, true);
        Debug.DrawRay(_handyThumb.transform.position, _handyFingers[(int)LibHandyGrip.FingerType.Pinky].transform.position, Color.red, 0, true);
    }

    private void SetupDebugLines()
    {
        _debugLines = new List<GameObject>(_maximumFingers);

        for (int i = 0; i < _debugLines.Capacity; i++)
        {
            var line = new GameObject();
            line.transform.position = _handyThumb.transform.position;
            var lr = line.AddComponent<LineRenderer>();
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            lr.SetPosition(0, _handyThumb.transform.position);
            lr.SetPosition(1, _handyFingers[i].transform.position);
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            _debugLines.Insert(i, line);
        }
    }
}
