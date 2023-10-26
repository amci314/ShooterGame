using UnityEngine;
using UnityEngine.UI;

public class FPScounter : MonoBehaviour
{
    [SerializeField] private Text counterText;
    [SerializeField] private int frequency = 1;

    private float timer = 0f;
    void Update()
    {
        var dt = Time.unscaledDeltaTime;

        timer += dt;

        if(timer >= 1f / frequency)
        {
            timer = 0f;
            counterText.text = "FPS " + Mathf.FloorToInt(1f / dt);
        }
    }
}
