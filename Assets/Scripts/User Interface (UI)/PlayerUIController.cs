using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] Text grappleIconTimerText;
    [SerializeField] Text dashIconTimerText;
    [SerializeField] PlayerController playerController;

    float grappleIconTimer;
    float originalGrappleIconTime;

    private void Start()
    {
        originalGrappleIconTime = playerController.grapplingDelay;
    }

    private void Update()
    {
        GrappleIcon();
        DashIcon();
    }

    void GrappleIcon()
    {
        if (!playerController.canGrapple)
        {
            grappleIconTimerText.text = grappleIconTimer.ToString("f1");
            grappleIconTimer -= Time.deltaTime;
        }
        else
        {
            grappleIconTimerText.text = grappleIconTimer.ToString("f1");
            grappleIconTimer = playerController.grapplingDelay;
        }
    }

    void DashIcon()
    {
        dashIconTimerText.text = playerController.dashesLeft.ToString();
    }
}
