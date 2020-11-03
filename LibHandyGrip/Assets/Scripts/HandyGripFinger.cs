using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandyHitInfo
{
    public HandyHitInfo(float distance)
    {
        distanceFromFinger = distance;
    }
    public float distanceFromFinger;
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

public class HandyGripFinger : MonoBehaviour
{
    private Transform _transform;
    private HandyGripThumb _thumb;

    private HandyObjectList _objectList;
    private HandyGripObject _currentlyCollidedObject;
    
    private void Start()
    {
        _objectList = new HandyObjectList();
    }
    private void Update()
    {
        transform.position = _transform.position;
    }

    private void FixedUpdate()
    {
        UpdatePotentiallyGrabbableSet();
        if (AreObjectsWithinGrasp())
        {
            _currentlyCollidedObject = GetObjectCollision();
        }
    }

    public void SetTransform(Transform t)
    {
        _transform = t;
    }

    public void SetThumbReference(HandyGripThumb t)
    {
        _thumb = t;
    }

    public void UpdatePotentiallyGrabbableSet()
    {
        RaycastHit[] hits;
        Vector3 rayDir = (_thumb.transform.position - transform.position).normalized;
        hits = Physics.RaycastAll(transform.position, rayDir);

        HandyObjectList tempList = new HandyObjectList();

        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            var hgo = hit.transform.gameObject.GetComponent<HandyGripObject>();
            if (hgo)
            {
                var hi = new HandyHitInfo(hit.distance);
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
            }
        }
    }

    public bool AreObjectsWithinGrasp()
    {
        return !_objectList.IsEmpty();
    }

    public HandyGripObject GetObjectCollision()
    {
        int objectCount = _objectList.GetCount();
        for (int i = 0; i < objectCount; i++)
        {
            var hi = _objectList.GetHitInfo(i);
            if (hi.distanceFromFinger < 1.1f)
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
}
