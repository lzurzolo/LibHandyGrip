using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using LibHandyGrip;
using UnityEngine;

public class HandyHitInfo
{
    public HandyHitInfo(float distance, float offset)
    {
        distanceFromFinger = distance;
        contactOffset = offset;
    }

    public void UpdateHitInfo(float distance, float offset)
    {
        distanceFromFinger = distance;
        contactOffset = offset;
    }    
    public float distanceFromFinger;
    public float contactOffset;
};

public class HandyObjectList
{
    public List<HandyGripObject> objectsWithinGrasp;
    public List<HandyHitInfo> hitInfos;

    public HandyObjectList()
    {
        objectsWithinGrasp = new List<HandyGripObject>();
        hitInfos = new List<HandyHitInfo>();
    }

    public HandyGripObject GetObject(int i)
    {
        return objectsWithinGrasp[i];
    }

    public HandyHitInfo GetHitInfo(int i)
    {
        return hitInfos[i];
    }
    
    public void AddRecord(HandyGripObject hgo, HandyHitInfo hhi)
    {
        objectsWithinGrasp.Add(hgo);
        hitInfos.Add(hhi);
    }
    public void RemoveRecord(int i)
    {
        objectsWithinGrasp.RemoveAt(i);
        hitInfos.RemoveAt(i);
    }

    public bool IsEmpty()
    {
        return objectsWithinGrasp.Count == 0;
    }

    public int GetCount()
    {
        return objectsWithinGrasp.Count;
    }
}

public class HandyGripFingerTip : MonoBehaviour
{
    public bool isActive;
    private Transform _transform;
    private HandyGripThumbTip _thumbTip;

    private HandyObjectList _objectList;
    private HandyGripObject _currentlyCollidedObject;

    private LibHandyGrip.FingerType _whichFinger;
    
    private void Start()
    {
        isActive = false;
        _objectList = new HandyObjectList();
    }
    private void Update()
    {
        if(isActive) transform.position = _transform.position;
    }

    private void FixedUpdate()
    {
        if (!isActive) return;
        if (AreObjectsWithinGrasp())
        {
            _currentlyCollidedObject = SetObjectCollision();
        }
        else
        {
            _currentlyCollidedObject = null;
        }
        UpdatePotentiallyGrabbableSet();
    }

    public void SetTransform(Transform t)
    {
        _transform = t;
    }

    public void SetThumbReference(HandyGripThumbTip t)
    {
        _thumbTip = t;
    }

    public void UpdatePotentiallyGrabbableSet()
    {
        RaycastHit[] hits;
        Vector3 rayDir = (_thumbTip.transform.position - transform.position).normalized;
        hits = Physics.RaycastAll(transform.position, rayDir);

        HandyObjectList tempList = new HandyObjectList();

        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            var hgo = hit.transform.gameObject.GetComponent<HandyGripObject>();
            if (hgo)
            {
                var hi = new HandyHitInfo(hit.distance, hit.collider.contactOffset);
                tempList.AddRecord(hgo, hi);
            }
        }

        if (_objectList.IsEmpty())
        {
            for (int i = 0; i < tempList.objectsWithinGrasp.Count; i++)
            {
                _objectList.AddRecord(tempList.GetObject(i), tempList.GetHitInfo(i));
            }
        }
        else
        {
            for (int i = _objectList.objectsWithinGrasp.Count - 1; i >= 0; i--)
            {
                if (!tempList.objectsWithinGrasp.Contains(_objectList.objectsWithinGrasp[i]))
                {
                    _objectList.RemoveRecord(i);
                }
                else
                {
                    var tObjectRef = tempList.objectsWithinGrasp.IndexOf(_objectList.objectsWithinGrasp[i]);
                    _objectList.hitInfos[i].UpdateHitInfo(tempList.hitInfos[tObjectRef].distanceFromFinger, tempList.hitInfos[tObjectRef].contactOffset);
                }
            }
        }
    }

    public bool AreObjectsWithinGrasp()
    {
        return !_objectList.IsEmpty();
    }

    public HandyGripObject SetObjectCollision()
    {
        int objectCount = _objectList.GetCount();
        for (int i = 0; i < objectCount; i++)
        {
            var hi = _objectList.GetHitInfo(i);
            if (hi.distanceFromFinger < 0.03f)
            {
                return _objectList.GetObject(i);
            }
        }
        return null;
    }

    public HandyGripObject GetCurrentCollidedObject()
    {
        return _currentlyCollidedObject;
    }
    
    public void SetFingerID(LibHandyGrip.FingerType f)
    {
        _whichFinger = f;
    }
}
