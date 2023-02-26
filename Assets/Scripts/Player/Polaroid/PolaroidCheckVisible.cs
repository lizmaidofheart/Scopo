using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolaroidCheckVisible : MonoBehaviour
{
    // this is a singleton for retrieving all photographable objects in a photograph

    private static PolaroidCheckVisible _instance;
    public static PolaroidCheckVisible Instance { get { return _instance; } }

    [SerializeField] private LayerMask raycastLayers;

    [SerializeField] Photographable testObj;

    private void Awake() // singleton setup
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    // return a list of all visible photographables to the camera
    public List<Photographable> visiblePhotographables(Camera cam)
    {
        List<Photographable> output = new List<Photographable>();
        foreach (Photographable obj in PolaroidInfo.Instance.photographables)
        {
            if (ICanSee(cam, obj)) { output.Add(obj); }
        }
        return output;
    }

    // check whether a particular object is visible to the camera
    public bool ICanSee(Camera cam, Photographable obj)
    {
        if (obj.isActiveAndEnabled) // make sure object is active
        {
            Collider objCollider = obj.gameObject.GetComponent<Collider>();

            // gets the area the camera's outer planes contain the specified object
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

            // checks whether the object is within the camera area
            if (GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
            {
                // check if the camera has a clean line of sight to the object, within a max range specified by the object's photographable component.
                // right now this checks for the object's centre. this may become an issue later on for large objects that go around or behind other objects.
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, (obj.transform.position - cam.transform.position).normalized, out hit, obj.visibleRange, raycastLayers))
                {
                    // make sure that what we hit was what we're looking for
                    Photographable hitPhotographable = hit.collider.gameObject.GetComponent<Photographable>();
                    if (hitPhotographable != null)
                    {
                        if (hitPhotographable == obj)
                        {
                            Debug.DrawRay(transform.position, (obj.transform.position - cam.transform.position).normalized * obj.visibleRange, Color.red, 2f);
                            Debug.Log(obj.identity);

                            return true;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }
        else return false;
        
    }
}
