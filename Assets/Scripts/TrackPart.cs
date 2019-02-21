using Pops.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPart : MonoBehaviour
{
    public float trackLenght = 200f;

    public static TrackPart CurrentPart
    {
        get { return _currentPart; }
        set
        {
            _prevPart = _currentPart;
            _currentPart = value;
            if (_prevPart != null)
            {
                _prevPart.Respawn();
            }
        }
    }
    static TrackPart _currentPart;
    static TrackPart _prevPart;

    public void Respawn()
    {
        transform.position = CurrentPart.transform.position.With(z: CurrentPart.transform.position.z + trackLenght);
        gameObject.SetActive(true);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
            CurrentPart = this;
    }
}
