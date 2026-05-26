using UnityEngine;

public class CentralPuzzleManager : MonoBehaviour
{
    [Header("Scanners")]
    [SerializeField] private HandScanner[] scanners;

    [Header("Central Progress Bar")]
    [SerializeField] private Transform centralBar;

    [SerializeField] private Vector3 startLocalPos;
    [SerializeField] private Vector3 endLocalPos;

    [Header("Progress")]
    [SerializeField] private float fillSpeed = 0.5f;
    [SerializeField] private float drainSpeed = 1f;

    private float progress = 0f;
    private bool puzzleCompleted = false;

    private void Start()
    {
        if (centralBar != null)
            centralBar.localPosition = startLocalPos;
    }

    private void Update()
    {
        if (puzzleCompleted)
            return;

        bool allCompleted = AreAllScannersCompleted();

        if (allCompleted)
            progress += Time.deltaTime * fillSpeed;
        else
            progress -= Time.deltaTime * drainSpeed;

        progress = Mathf.Clamp01(progress);

        if (centralBar != null)
        {
            centralBar.localPosition =
                Vector3.Lerp(startLocalPos, endLocalPos, progress);
        }

        if (progress >= 1f)
        {
            puzzleCompleted = true;
            Debug.Log("PUZZLE COMPLETE");
        }
    }

    private bool AreAllScannersCompleted()
    {
        foreach (var scanner in scanners)
        {
            if (!scanner.IsCompleted)
                return false;
        }
        return true;
    }

    public void ResetPuzzle()
    {
        puzzleCompleted = false;
        progress = 0f;

        if (centralBar != null)
            centralBar.localPosition = startLocalPos;

        foreach (var scanner in scanners)
            scanner.ResetScanner();
    }
}