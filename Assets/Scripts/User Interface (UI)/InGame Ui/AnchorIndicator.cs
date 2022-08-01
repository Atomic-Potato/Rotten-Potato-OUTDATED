using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorIndicator : MonoBehaviour
{
    [SerializeField] GameObject indicator;
    [SerializeField] GameObject target;
    [SerializeField] LayerMask cameraLayer;
    [SerializeField] PlayerController playerController;

    Renderer rendererComponent; //Had to rename it because of an error, renderer was already something idk

    private void Start()
    {
        rendererComponent = GetComponent<Renderer>();
    }

    void Update()
    {
        if (!rendererComponent.isVisible && Vector2.Distance(transform.position, target.transform.position) <= playerController.grappleDistance)
        {
            indicator.SetActive(true);

            //Spawning a ray from the anchor to the target
            Vector2 direction = target.transform.position - transform.position;
            float distance = Vector3.Distance(transform.position, target.transform.position);
            RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, distance, cameraLayer);

            //Rotating the Anchors
            float indicatorAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            indicator.transform.rotation = Quaternion.AngleAxis(indicatorAngle, Vector3.forward);
            //Debug.DrawLine(target.transform.position, ray.point);

            if (ray.collider != null)
            {
                indicator.transform.position = ray.point;
            }
        }
        else if (rendererComponent.isVisible || Vector2.Distance(transform.position, target.transform.position) > playerController.grappleDistance)
            indicator.SetActive(false);

        //Debugging
        //Debug.Log("Player to anchor distance : " + Vector2.Distance(transform.position, target.transform.position));
        //Debug.Log("Anchor is visible: " + rendererComponent.isVisible);
    }
}
