
using TMPro;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    public TextMeshProUGUI frameRateText;  // Assign a UI Text element if you want to display the FPS

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        float currentFrameRate = 1.0f / Time.deltaTime;
        frameRateText.text = "FPS: " + Mathf.RoundToInt(currentFrameRate).ToString();
    }
}
