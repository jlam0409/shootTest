using UnityEngine;
using UnityEditor;

public class AdjustCameraMask : MonoBehaviour{
    [MenuItem ("Command/Adjust Camera Mask")]
    public static void AdjustCameraMaskValue(){
		foreach (Transform transform in Selection.transforms){
			GameObject eachObj = transform.gameObject;	
			CameraMaskScript cameraMaskScript = eachObj.GetComponent("CameraMaskScript") as CameraMaskScript;
			cameraMaskScript.CalculateWorldValue();
			cameraMaskScript.AdjustCameraValue();
		}
    }
}