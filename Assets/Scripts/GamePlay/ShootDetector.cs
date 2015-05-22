using UnityEngine;
using System.Collections;

public class ShootDetector : MonoBehaviour {
	public GameController gameController;
	public Camera gameCamera;
	public void OnTouchUp (TouchData touchData_){
		Vector3 position = gameCamera.ScreenToWorldPoint(new Vector3(touchData_.screenPosition.x, touchData_.screenPosition.y, gameCamera.nearClipPlane));

		gameController.ShootBullet(position.x);

	}
}
