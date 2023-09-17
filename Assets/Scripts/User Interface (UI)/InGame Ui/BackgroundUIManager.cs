using TMPro;
using UnityEngine;

public class BackgroundUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text freeDash;

    void Awake()
    {
        freeDash.enabled = false;
    }

    void Update()
    {
        if (Parry.IsGivenFreeDash)
        {
            if (!freeDash.enabled)
            {
                freeDash.enabled = true;
            }
        }
        else
        {
            if (freeDash.enabled)
            {
                freeDash.enabled = false;
            }
        }
    }    
}
