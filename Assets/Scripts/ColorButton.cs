using UnityEngine;

public class ColorButton : MonoBehaviour
{
    public string colorName;
    public PuzzleManager manager;

    private bool cooldown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (cooldown) return;

        if (other.CompareTag("Player"))
        {
            cooldown = true;

            manager.PressButton(colorName);

            Invoke(nameof(ResetCooldown), 0.5f);
        }
    }

    void ResetCooldown()
    {
        cooldown = false;
    }
}