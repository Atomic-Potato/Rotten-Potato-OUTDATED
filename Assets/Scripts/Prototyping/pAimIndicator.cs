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
    [SerializeField] BoxCollider2D collider;

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
        DrawBox(indicator.transform.position, new Vector3(collider.size.x/2f, collider.size.y/2f, 0f), Color.red);
        DrawBox(
            new Vector3(transform.position.x, transform.position.y -0.08f, 0f), 
            new Vector3(collider.size.x/2f, collider.size.y/2f, 0f), 
            Color.red);
    }

    Vector2 GetDirectionToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - transform.position;
        direction.Normalize();
        return direction;
    }

    void DrawBox(Vector3 position, Vector3 size, Color color){
        //TOP front
        Debug.DrawRay(new Vector3(position.x - size.x, position.y + size.y, position.z + size.z), new Vector3(size.x*2f, 0f, 0f), color);
        //TOP back
        Debug.DrawRay(new Vector3(position.x - size.x, position.y + size.y, position.z - size.z), new Vector3(size.x*2f, 0f, 0f), color);
        //BOTTOM front
        Debug.DrawRay(new Vector3(position.x - size.x, position.y - size.y, position.z + size.z), new Vector3(size.x*2f, 0f, 0f), color);
        //BOTTOM back
        Debug.DrawRay(new Vector3(position.x - size.x, position.y - size.y, position.z - size.z), new Vector3(size.x*2f, 0f, 0f), color);
        //LEFT front
        Debug.DrawRay(new Vector3(position.x - size.x, position.y - size.y, position.z + size.z), new Vector3(0f, size.y*2f, 0f), color);
        //LEFT back
        Debug.DrawRay(new Vector3(position.x - size.x, position.y - size.y, position.z - size.z), new Vector3(0f, size.y*2f, 0f), color);
        //RIGHT front
        Debug.DrawRay(new Vector3(position.x + size.x, position.y - size.y, position.z + size.z), new Vector3(0f, size.y*2f, 0f), color);
        //RIGHT back
        Debug.DrawRay(new Vector3(position.x + size.x, position.y - size.y, position.z - size.z), new Vector3(0f, size.y*2f, 0f), color);

        //LEFT SIDE
        //Top
        Debug.DrawRay(new Vector3(position.x - size.x, position.y + size.y, position.z - size.z), new Vector3(0f, 0f, size.z * 2f), color);
        //Bottom
        Debug.DrawRay(new Vector3(position.x - size.x, position.y - size.y, position.z - size.z), new Vector3(0f, 0f, size.z * 2f), color);
        //RIGHT SIDE
        //Top
        Debug.DrawRay(new Vector3(position.x + size.x, position.y + size.y, position.z - size.z), new Vector3(0f, 0f, size.z * 2f), color);
        //Bottom
        Debug.DrawRay(new Vector3(position.x + size.x, position.y - size.y, position.z - size.z), new Vector3(0f, 0f, size.z * 2f), color);
    }
}
