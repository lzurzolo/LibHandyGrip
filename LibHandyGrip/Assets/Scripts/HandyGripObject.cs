using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class HandyGripObject : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Quaternion _initialOrientation;
    private bool _isBeingMoved;
    
    private void Start()
    {
        // object must have a collider
        var coll = GetComponent<Collider>();
        Assert.IsNotNull(coll);
        
        // object must have a rigid body
        _rigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(_rigidbody);
        
        _initialOrientation = transform.rotation;
        _isBeingMoved = false;

        StartCoroutine(SetCurrentOrientation());
    }

    public void SetGrabPosition(Vector3 pos)
    {
        _rigidbody.isKinematic = true;
        _isBeingMoved = true;
        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime);
    }

    public void SetGrabRotation(Vector3 dir)
    {
        Quaternion newOrientation = Quaternion.LookRotation(dir);
        newOrientation *= _initialOrientation;
        transform.rotation = newOrientation;
    }

    public void ReleaseObject()
    {
        _rigidbody.isKinematic = false;
        _isBeingMoved = false;
    }
    
    private IEnumerator SetCurrentOrientation()
    {
        WaitForSeconds waitTime = new WaitForSeconds(1);
        while (true)
        {
            // We need to ensure that the object's transform is buffered at all times when stationary
            // TODO : the object is stationary, why does this need to update? Perhaps move this to ReleaseObject()
            if (!_isBeingMoved)
            {
                _initialOrientation = transform.rotation;
            }
            yield return waitTime;
        }
    }
}