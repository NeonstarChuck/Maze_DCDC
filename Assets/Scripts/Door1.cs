using UnityEngine;

public class Door1 : MonoBehaviour
{
    public void Open()
    {
        Debug.Log("Door opened");
        gameObject.SetActive(false);
    }
}