using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationImpactWatcher : MonoBehaviour
{
    public event Action OnImpact;

    // called BY Animation
    private void Impact()
    {
        if (OnImpact != null)
        {
            OnImpact();
        }
    }
}