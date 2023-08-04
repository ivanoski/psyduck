using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFrogUI : MonoBehaviour
{
    public float angularVelocity = 0f;
    public float angularVelocityChange = 1f;
    private bool RightHeld = false;
    private bool LeftHeld = false;

    public void HoldRight()
    {
        RightHeld = true;
    }
    public void ReleaseRight()
    {
        RightHeld = false;
    }
    public void HoldLeft()
    {
        LeftHeld = true;
    }
    public void ReleaseLeft()
    {
        LeftHeld = false;
    }

    void Update()
    {
        if (RightHeld) angularVelocity -= angularVelocityChange;
        if (LeftHeld) angularVelocity += angularVelocityChange;
        transform.eulerAngles = transform.eulerAngles += new Vector3(0f, 0f, angularVelocity * Time.deltaTime);

    }
}
