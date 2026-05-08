using UnityEngine;

public class MetaButton : MonoBehaviour
{
    public Door1 door;

    public void Press()
    {
        Debug.Log("Button pressed → opening door");

        if (door != null)
        {
            door.Open();
        }
    }
}