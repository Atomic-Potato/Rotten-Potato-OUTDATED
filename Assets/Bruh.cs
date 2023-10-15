using UnityEngine;

public class Bruh : MonoBehaviour
{
    [SerializeField] GameObject bruh;

    void Awake()
    {
        bruh.SetActive(false);
    }

    public void FrBruh()
    {
        bruh.SetActive(true);
    }
}
