using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProgressBlock : MonoBehaviour 
{
    [SerializeField] GameObject block;

    string _triggeredTag;
    public string TriggeredTag => _triggeredTag;
    bool _isOpen;
    public bool IsOpen => _isOpen;
    bool _isClosed;
    public bool IsClosed => _isClosed;

    void Awake()
    {
        _isOpen = block.activeSelf == false;
        _isClosed = block.activeSelf == true;
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        _triggeredTag = other.gameObject.tag;
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        _triggeredTag = null;
    }

    public void Open()
    {
        if (block.activeSelf == true)
        {
            block.SetActive(false);
            _isOpen = true;
            _isClosed = false;
        }
    }   

    public void Close()
    {
        if (block.activeSelf == false)
        {
            block.SetActive(true);
            _isOpen = false;
            _isClosed = true;
        }
    } 
}