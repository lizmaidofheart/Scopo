using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptInArea : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI prompt;
    [SerializeField] float maxRadius = 5;
    [SerializeField] float minRadius = 3;
    Transform playerTransform;
    [SerializeField] KeyCode button;

    // Start is called before the first frame update
    void Start()
    {
        Color textColour = prompt.color;
        textColour.a = 0;
        prompt.color = textColour;

        playerTransform = PlayerReference.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // get opacity value based on player proximity to point (max distance to show = maxRadius)
        float distanceToPlayer = (playerTransform.position - transform.position).magnitude;
        float opacity;
        if (distanceToPlayer <= minRadius) opacity = 1;
        else if (distanceToPlayer > maxRadius) opacity = 0;
        else opacity = 1 - (distanceToPlayer - minRadius) / (maxRadius - minRadius);

        // change opacity of text based on player proximity to point
        Color textColour = prompt.color;
        textColour.a = opacity;
        prompt.color = textColour;

        // if player is close enough, do the action on button press
        if (Input.GetKeyDown(button))
        {
            Action();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // draw spheres for radii
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }

    public virtual void Action()
    {

    }
}
