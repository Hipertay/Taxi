using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSDisplay3D : MonoBehaviour
{
	private float frequency = 1.0f;
	private int fps;
	public Text fpsString;

    int lastFrameCount;
    float lastTime;
    float timeSpan;
    int frameCount;

    void Start()
	{
		StartCoroutine(FPS());
	}
	private IEnumerator FPS() {
		for(;;){
			// Capture frame-per-second
			lastFrameCount = Time.frameCount;
			lastTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(frequency);
			timeSpan = Time.realtimeSinceStartup - lastTime;
			frameCount = Time.frameCount - lastFrameCount;

			// Display it

			fps =  Mathf.RoundToInt(frameCount / timeSpan);
		}
	}
	void Update()
	{
        fpsString.text = fps.ToString() + "fps";
	}
}