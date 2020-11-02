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
    private void Start()
    {
        Assert.IsTrue(fingerCount > 0 && fingerCount <= _maximumFingers);
        _handyFingers = new List<GameObject>(_maximumFingers);
        Debug.Log(_handyFingers.Capacity);
        GetHandyFingerReferences();
        SetFingerTransforms();
    }

    private void Update()
    {
        
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
}
