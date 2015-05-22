using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LayoutSpace {
	SCREEN,
	OBJECT,
	WORLD
}

public enum HorizontalOrigin{
	LEFT,
	CENTER,
	RIGHT
}
	
public enum VerticalOrigin{
	BOTTOM,
	CENTER,
	TOP
}
	
public class Layout : MonoBehaviour{
	private Vector3 origin;
	public float depthOffset;
	public LayoutSpace layoutSpace;
	public HorizontalOrigin horizontalOrigin;
	public VerticalOrigin verticalOrigin;
	
	public float cellWidth = 1f;
	public float cellHeight = 1f;
	
	protected Color gizmosColor = new Color(1, 0, 0, 0.5f);
	public bool enableGizmo = false;
	
	protected Vector3 GetOrigin(){
		switch (layoutSpace){
			case LayoutSpace.WORLD:	
				origin = Vector3.zero;
				break;
			case LayoutSpace.OBJECT:
				origin = gameObject.transform.position;
				break;
			case LayoutSpace.SCREEN:
				// deal with this later on, mainly trace what the position of the main camera and place it underneath
				break;
		}
		return origin;
	}
	
	
}
