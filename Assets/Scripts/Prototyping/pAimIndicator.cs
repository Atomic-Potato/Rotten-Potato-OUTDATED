using UnityEngine;

// TODO:
// - Make indicator distance be the same distance as the dash
// - Block indicator by solid objects

public class pAimIndicator : MonoBehaviour
{
    [Range(0f, 20f)]
    [SerializeField] float distanceFromPlayer = 5f;
    [SerializeField] GameObject indicator;

    [Space]
    [Header("Gizmos")]
    [SerializeField] bool gizmos;
    [SerializeField] bool gizmosDisplayMouseDirection;
    [SerializeField] bool gizmosDisplayIndicatorDirection;

    void OnDrawGizmos()
    {
        if (!gizmos)
        {
            return;
        }


        if (gizmosDisplayMouseDirection)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float distanceFromPlayer = Vector3.Distance(transform.position, mousePosition);
            
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, GetDirectionToMouse() * distanceFromPlayer);
        }

        if (gizmosDisplayIndicatorDirection)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, GetDirectionToMouse() * distanceFromPlayer);
        }
    }

    void Update()
    {
        indicator.transform.position = (Vector2)transform.position + GetDirectionToMouse() * distanceFromPlayer;
    }

    Vector2 GetDirectionToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - transform.position;
        direction.Normalize();
        return direction;
    }
}
