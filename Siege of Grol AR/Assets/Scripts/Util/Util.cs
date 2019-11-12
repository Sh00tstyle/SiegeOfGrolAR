using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static float ClampAngle(float pAngle, float pMin, float pMax)
    {
        if (pAngle < -360F)
            pAngle += 360F;

        if (pAngle > 360F)
            pAngle -= 360F;

        return Mathf.Clamp(pAngle, pMin, pMax);
    }
}
