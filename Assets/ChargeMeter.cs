using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeMeter : MonoBehaviour
{

    public float percentageFull = 0f;

    [SerializeField] private Vector3 scale;

    void Start()
    {
        
    }

    void Update()
    {
        transform.localScale = new Vector3(scale.x, percentageFull * scale.y, scale.z);
    }
}
