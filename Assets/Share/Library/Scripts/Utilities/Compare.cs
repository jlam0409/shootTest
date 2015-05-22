using UnityEngine;
using System.Collections;

public class Compare : Object{
	private static float defaultTolerance = 0.01f;
	
	public static bool  EqualsFloat(float firstFloat, float targetFloat){
		 return EqualsFloat(firstFloat, targetFloat, defaultTolerance);
	}

	public static bool EqualsFloat(float firstFloat, float targetFloat, float tolerance){
		return (Mathf.Abs (firstFloat - targetFloat) <= tolerance);
	}
		
	public static bool EqualsVector2 (Vector3 firstVector2, Vector3 secondVector2){
		return EqualsVector2 (firstVector2, secondVector2, defaultTolerance);
	}
	
	public static bool EqualsVector2 (Vector3 firstVector2, Vector3 secondVector2, float tolerance){
		return 	(
			EqualsFloat (firstVector2.x, secondVector2.x, tolerance) &&
			EqualsFloat (firstVector2.y, secondVector2.y, tolerance) );
	}
	
	public static bool EqualsVector3 (Vector3 firstVector3, Vector3 secondVector3){
		return EqualsVector2 (firstVector3, secondVector3, defaultTolerance);
	}
	
	public static bool EqualsVector3 (Vector3 firstVector3, Vector3 secondVector3, float tolerance){
		return 	(
			EqualsFloat (firstVector3.x, secondVector3.x, tolerance) &&
			EqualsFloat (firstVector3.y, secondVector3.y, tolerance) &&
			EqualsFloat (firstVector3.z, secondVector3.z, tolerance) );
	}
}
