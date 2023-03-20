using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBInteractible : MonoBehaviour
{
    private Outline outline;
    private bool outlineEnabled = false;

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void SwitchOutline(bool on)
    {
        if (!outlineEnabled && on)
        {
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outlineEnabled = true;
        }
        if (outlineEnabled && !on)
        {
            outline.OutlineMode = Outline.Mode.SilhouetteOnly;
            outlineEnabled = false;
        }
    }

    public virtual void Interact()
    {

    }
}
