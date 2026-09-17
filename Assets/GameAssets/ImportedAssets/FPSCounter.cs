using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsText;

    private int avgFrameRate;
    private float currTime;

    void Update()
    {
        if (currTime > 1f)
        {
            _fpsText.text = avgFrameRate + " FPS";
            currTime = 0f;
            avgFrameRate = 0;
        }
        else
        {
            avgFrameRate++;
        }

        currTime += Time.deltaTime;
    }
}
