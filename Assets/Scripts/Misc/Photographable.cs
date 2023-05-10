using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Photographable : MonoBehaviour
{
    // this is a generic component that can be applied to any object with a collider.
    // it designates the object as being important to photograph. if a photo is taken of this object, its info will be stored in the photograph info.
    // the more of these, the more checks that must be done every time a photo is taken, so don't apply this to literally everything - its for important stuff.

    [SerializeField] public float visibleRange = 100f;
    [SerializeField] public string identity = "Unknown Object";
    [SerializeField] public bool isClue = true;
    [Tooltip("Make sure the shown gizmo is within the object's collider")]
    [SerializeField] public Vector3 checkOffset = new Vector3();

    [SerializeField] UnityEvent gotPhotographed;

    // place self in a list of all photographable objects, which will be looped through later
    void Start()
    {
        PolaroidInfo.Instance.photographables.Add(this);
    }

    // when destroyed, remove self from list of all photographable objects
    private void OnDestroy()
    {
        PolaroidInfo.Instance.photographables.Remove(this);
    }

    public void IveBeenPhotographed()
    {
        gotPhotographed.Invoke();

        if (isClue)
        {
            PlayerReference.Instance.FindClue(identity);
            Monologue.Instance.PlayText(identity);
        }
        
    }

    void OnDrawGizmosSelected()
    {
        // draw a sphere for the visibleRange in the editor

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visibleRange);

        // draw a sphere for the checkoffset position

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + checkOffset, 0.2f);
        
    }
}
