using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandyGripFinger : MonoBehaviour
{
    private Transform _transform;
    private HandyGripThumb _thumb;
    private List<HandyGripObject> _objectsWithinGrasp;
    private HandyGripObject _currentlyCollidedObject;
    
    private void Start()
    {
        _objectsWithinGrasp = new List<HandyGripObject>();
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
            // TODO : check for collision
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
        hits = Physics.RaycastAll(transform.position, _thumb.transform.position);

        HashSet<HandyGripObject> tempObjectSet = new HashSet<HandyGripObject>();

        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            var hgo = hit.transform.gameObject.GetComponent<HandyGripObject>();
            if (hgo)
            {
                tempObjectSet.Add(hgo);
            }
        }

        if (_objectsWithinGrasp.Count == 0)
        {
            foreach (var t in tempObjectSet)
            {
                _objectsWithinGrasp.Add(t);
            }
        }
        else
        {
            for (int i = _objectsWithinGrasp.Count - 1; i >= 0; i--)
            {
                if (!tempObjectSet.Contains(_objectsWithinGrasp[i]))
                {
                    _objectsWithinGrasp.RemoveAt(i);
                }
            }
        }
    }

    public bool AreObjectsWithinGrasp()
    {
        return _objectsWithinGrasp.Count != 0;
    }
    
}
