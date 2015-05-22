using UnityEngine;
using UnityEditor;
using System.Collections;


/**
	\class Command
	\brief Editor class for command

	This is the detail description of the editor class Command
*/
public class Command : MonoBehaviour {
	/**
		\fn void Hide()
		\brief Deactive selected objects
	*/
	[MenuItem ("GameObject/Hide GameObject %h")]	 
	public static void Hide() {
		foreach (GameObject eachObj in Selection.gameObjects){
			//eachObj.renderer.enabled= false;
			eachObj.SetActive(false);
		}
	}
	
	/**
		Active selected objects
	*/
	[MenuItem ("GameObject/Show GameObject &h")]	 
	public static void Show(){
		foreach (GameObject eachObj in Selection.gameObjects){
			//eachObj.renderer.enabled = true;
			eachObj.SetActive(true);
		}
	}
	
	/**
		set the scale to match the texture		
	*/
	[MenuItem ("Command/Set Scale Match Texture")]
	public static void SetScaleMatchTexture(){
		foreach (Transform transform in Selection.transforms){
			GameObject eachObj = transform.gameObject;	
			Material mat = eachObj.renderer.sharedMaterial;
			Texture tex = mat.mainTexture;
			
			
			bool isRepeat = (tex.wrapMode == TextureWrapMode.Repeat);
			Vector2 scaleFactor = new Vector2 (1f, 1f);
			if (isRepeat){
				scaleFactor = mat.mainTextureScale;
			}
			/*
			float newScaleX = tex.width / 1000.0f * scaleFactor.x;
			float newScaleY = tex.height / 1000.0f * scaleFactor.y;
			Vector3 newScale = new Vector3 (newScaleX, 1.0f, newScaleY);
			*/
			Vector3 newScale = GetScaleMatchTexture (tex, scaleFactor, 100f);
			
			// parent to the world set the scale and parent back
			Transform parent = transform.parent;
			transform.parent = null;			
			transform.localScale  = newScale;
			if (parent != null)
				transform.parent = parent;
		}
	}
	
	[MenuItem ("Command/Set Scale One")]
	public static void SetScaleOne(){
		foreach (Transform transform in Selection.transforms){
			Transform parent = transform.parent;
			transform.parent = null;	
			transform.localScale = Vector3.one;
			if (parent != null)
				transform.parent = parent;
		}
	}
	
	public static Vector3 GetScaleMatchTexture(Texture tex, Vector2 texScaleFactor, float worldScaleFactor){	
		float newScaleX = tex.width / worldScaleFactor * texScaleFactor.x;
		float newScaleY = tex.height / worldScaleFactor * texScaleFactor.y;
		Vector3 newScale = new Vector3 (newScaleX, 1.0f, newScaleY);
		return newScale;
	}
	                               
	                               
	/**
		Group selected objects
	*/
	[MenuItem ("Command/Group %g")]
	public static GameObject Group(){				
		return Group (Selection.transforms);
	}
	
	/**
		Group input transform nodes
		\param[in]	xform	Transform nodes
		\return	GameObject grouped
	*/
	public static GameObject Group( Transform[] xform  ){
		GameObject newObj = new GameObject();
		newObj.name = "Group";
		
		// record the first xform and place the new group under the first selection
		Transform parent = xform[0].parent;
		
		foreach (Transform transform in xform){
			transform.parent = newObj.transform;
		}
		if (parent != null){
			newObj.transform.parent = parent;
		}
		
		return newObj;
	}
	
	/**
		Pending
	*/
	[MenuItem ("Command/Align/X Left")]
	public static void AlignX ( ){
		
	}
	

	private static Vector3[] tempPosition;// = new Vector3();	/*!< Detailed description of tempPosition */
	private static Vector3[] tempRotation;// = new Vector3();	/*!< Detailed description of tempRotation*/
	private static Vector3[] tempScale;// = new Vector3();		/*!< Detailed description of tempScale*/
	/**
		Save Transform Data to cache
	*/
	[MenuItem ("Command/Save Transform Data &1")]
	public static void SaveTransformData(){
		Transform[] selXform = Selection.transforms;
		tempPosition = new Vector3[selXform.Length];
		tempRotation = new Vector3[selXform.Length];
		tempScale = new Vector3[selXform.Length];

		for (int i=0; i<selXform.Length; i++){
			Debug.Log ("i: " + i + " " + selXform[i]);
			//selXform[i] = sel[i].transform;
			tempPosition[i] = selXform[i].localPosition;
			tempRotation[i] = selXform[i].localEulerAngles;
			tempScale[i] = selXform[i].localScale;
		}
		Debug.Log (selXform.Length + " transform data saved");
	}
		
	/**
		Load Transform Data from cache
	*/
	[MenuItem ("Command/Load Transform Data &2")]
	public static void LoadTransformData(){
		Transform[] selXform = Selection.transforms;
		for (int i=0; i<selXform.Length; i++){
			//selXform[i] = sel[i].transform;

			selXform[i].localPosition = tempPosition[i];
			selXform[i].localEulerAngles = tempRotation[i];
			selXform[i].localScale = tempScale[i];
		}
		Debug.Log (selXform.Length + " transform data loaded");
	}
	
	[MenuItem ("Command/Load First Transform Data To All &3")]
	public static void LoadAllTransformData(){
		Transform[] selXform = Selection.transforms;
		foreach (Transform eachXform in selXform){
			eachXform.localPosition = tempPosition[0];
			eachXform.localEulerAngles = tempRotation[0];
			eachXform.localScale = tempScale[0];
		}
		Debug.Log (selXform.Length + " transform data loaded from first transform");
	}
	
	/**
		Set the selected GameObject pivot to the left of the mesh
	*/
	[MenuItem ("Command/Pivot/Left")]
	public static void SetPivotLeft(){
		Transform selXform = Selection.activeTransform;
		Transform[] bufXform = {selXform};
		GameObject newGroup = Group ( bufXform );
		
		Vector3 selScale = selXform.localScale;
		
		Vector3 groupPos = selXform.localPosition;
		float groupOffsetPos = selScale.x/2 * -10;		
		groupPos.x += groupOffsetPos;
		
		Vector3 selPos = Vector3.zero;
		selPos.x += groupOffsetPos * -1;
		
		newGroup.transform.localPosition = groupPos;
		selXform.localPosition = selPos;
		
	}
	
	[MenuItem ("Command/Batch Rename")]
	public static void BatchRename(){
		string oldPattern = "Down";
		string newPattern = "Up";
		
		GameObject[] sel = Selection.gameObjects;
		foreach (GameObject each in sel){
			string newName = each.name.Replace ( oldPattern, newPattern);
			each.name = newName;
		}
	}
	
	[MenuItem ("Command/Batch Rename Numbers")]
	public static void BatchRenameNumbers(){
		GameObject[] sel = Selection.gameObjects;
		for (int i=0; i<sel.Length; i++){
			string newName = sel[i].name + i;
			sel[i].name = newName;
		}
	}
	/*
	[MenuItem ("Command/Select Missing Script GameObject")]
	public static void SelectMissingScriptGameObject(){
		Object[] allObject = GameObject.FindObjectsOfType(typeof (GameObject));
		foreach (Object eachObject in allObject){
			GameObject eachGameObject = (GameObject)eachObject;
			MonoScript[] allScript = eachGameObject.GetComponents<MonoScript>();
			foreach (MonoScript eachScript in allScript){
				Debug.Log ("GameObject: " + eachObject  + " have script named: " + eachScript.name);
			}
		}
	}
	*/
}

