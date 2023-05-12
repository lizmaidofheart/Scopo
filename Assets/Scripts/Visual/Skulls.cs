using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skulls : MonoBehaviour
{
    [SerializeField] float radiusToAnimate;
    [SerializeField] private Animator animator;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(PlayerReference.Instance.transform.position, transform.position) <= radiusToAnimate)
        {
            animator.SetTrigger("Float");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radiusToAnimate);
    }
}
