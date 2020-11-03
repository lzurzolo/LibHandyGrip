﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandyGripThumb : MonoBehaviour
{
    private Transform _transform;
    private List<HandyGripFinger> _fingers;

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

    public void SetFingerReferences(List<HandyGripFinger> fs)
    {
        _fingers = fs;
    }
    
    public void UpdatePotentiallyGrabbableSet()
    {
        RaycastHit[] hits;
        // TODO : modify this so it raycasts to all fingers
        Vector3 rayDir = (_fingers[0].transform.position - transform.position).normalized;
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
            if (hi.distanceFromFinger < 0.1f)
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
