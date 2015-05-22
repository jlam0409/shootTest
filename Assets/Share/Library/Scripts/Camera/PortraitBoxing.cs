using UnityEngine;
using System.Collections;

public class PortraitBoxing : MonoBehaviour {
	// attach this script to the UI Camera
	private readonly static float designedWidth = 640f;
	private readonly static float designedHeight = 960f;
	private readonly static float designedAspect = designedHeight / designedWidth;
	
	private void Start () {
		float ratio = Screen.height / (float)Screen.width;

		Camera cam = camera;
		if (!Mathf.Approximately (ratio, designedAspect)){
			if (ratio < 1.49f){ // wider than 960/640, apply pillarboxing
				CreatePillarbox();
			} else if (ratio > 1.51f) { // narrow than 960/640, apply letterboxing
				cam.orthographicSize = GetOrthorgraphicSize();
				CreateLetterbox();
			}
		}
	}
	
	private float GetOrthorgraphicSize(){
		float photoshopSize = designedHeight; // 960f
//		float designedAspect = 960f/640;
		float actualAspect = Screen.height / (float)Screen.width;		
		float size = photoshopSize * actualAspect / designedAspect / 200f;
		// size = 640 * Screen.height / (float) Screen.width / 200;
		return size;
	}
	
	private void CreatePillarbox(){
		Object prefab = Resources.Load ("Pillarbox");
		GameObject pillarbox = (GameObject)Instantiate (prefab);
//		float scaleMultiplier = (960f / pillarbox.renderer.bounds.size.y) * (Screen.height/960f)/2f; 
		pillarbox.transform.parent = transform;
//		pillarbox.transform.localScale *= scaleMultiplier;
		pillarbox.transform.localPosition = new Vector3 (-(6.4f/2), 0f,1f);
		
		pillarbox = (GameObject)Instantiate (prefab);
		pillarbox.transform.parent = transform;
//		pillarbox.transform.localScale *= scaleMultiplier;
		Vector3 oriScale = pillarbox.transform.localScale;
		oriScale.x *= -1f;
		pillarbox.transform.localScale = oriScale;
		pillarbox.transform.localPosition = new Vector3 ((6.4f/2), 0f,1f);
	}
	
	private void CreateLetterbox(){
		Object prefab = Resources.Load ("Letterbox");
		GameObject letterbox = (GameObject)Instantiate (prefab);
//		float scaleMultiplier = (640f / letterbox.renderer.bounds.size.x) * (Screen.width/640f)/2f; 
		letterbox.transform.parent = transform;
//		letterbox.transform.localScale *= scaleMultiplier;
		letterbox.transform.localPosition = new Vector3 (0f, (9.6f/2),1f);
		
		letterbox = (GameObject)Instantiate (prefab);
		letterbox.transform.parent = transform;
		Vector3 oriScale = letterbox.transform.localScale;
		oriScale.z *= -1f;
		letterbox.transform.localScale = oriScale;
		letterbox.transform.localPosition = new Vector3 (0f, -(9.6f/2),1f);
	}
}
