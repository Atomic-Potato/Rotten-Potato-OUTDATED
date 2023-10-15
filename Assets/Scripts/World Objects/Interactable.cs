using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    [SerializeField, Tooltip(
        "If set to false, the player has to use the interact button instead")] 
    bool interactOnContact;
    [SerializeField] bool showPrompt = true;
    [SerializeField] Canvas promptCanvas;
    [SerializeField] GameObject prompt;

    [SerializeField, Space(10), Tooltip("The event that is executed on interaction")]
    UnityEvent interactionEvent;

    bool _isInInteractionZone;
    UnityAction defaultAction;

    void Awake()
    {
        promptCanvas.worldCamera = Camera.main;
        prompt.SetActive(false);

        if (interactionEvent.GetPersistentEventCount() == 0)
        {
            defaultAction = DefaultInteractionMessage;
            interactionEvent.AddListener(defaultAction);
        }
    }

    void Update()
    {
        if (_isInInteractionZone)
        {
            if (showPrompt && !prompt.activeSelf)
                prompt.SetActive(true);

            if(interactOnContact)
                interactionEvent.Invoke();
            else if (PlayerInputManager.IsPerformedInteract)
                interactionEvent.Invoke();
        }
        else
        {
            if (prompt.activeSelf)
                prompt.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        string tag = other.gameObject.tag;
        if (tag == TagsManager.Tag_Player)
            _isInInteractionZone = true;
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        string tag = other.gameObject.tag;
        if (tag == TagsManager.Tag_Player)
            _isInInteractionZone = false;            
    }

    void DefaultInteractionMessage()
    {
        Debug.Log("Interacted with object: " + gameObject.name);
    }
}
