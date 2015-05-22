using UnityEngine;


/*!	\class Point
 * 	\brief A point class (x, y, z)
 * 
 * 	A simple utility class, representing a point in space
 * 
 * 	\note could've used a Vector3 or Vector2, but wanted to be explicit
 */
public class Point
{
	public float x, y, z;			
	
	public Point(float inputX=0.0f, float inputY=0.0f)
	{
		x = inputX;
		y = inputY;		
		z = 0;
	}
	
	public Point(float inputX, float inputY, float inputZ)
	{	
		x = inputX;
		y = inputY;
		z = inputZ;
	}	
	
	public static Point operator +(Point p1, Point p2)
	{
		return new Point(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
	}
	
	public static Point operator -(Point p1, Point p2)
	{
		return new Point(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
	}
	
	public Vector3 GetVector()
	{
		return new Vector3(x,y,z);	
	}
	
}


/*!	\class Container
 * 	\brief A container for holding "content"
 * 
 * 	\note maybe deprecate x,y,z and leave position.x/y/z
 */
public class Container 
{
	public string id;
	
	public GameObject content;
	
	public int order;
	
	public float screenX, screenY;			// Cartesian
	public float viewX, viewY;				// normalized 
	public float worldX, worldY, worldZ;		// Euclidean 
	//public float defaultX, defaultY, defaultZ;
	
	public float x, y, z;		// maybe Deprecate?
	public Vector3 position
	{
		get {	return new Vector3(x, y, z);	}
		set	{	x = value.x; 
				y = value.y;
				z = value.z;
			}		
	}
	//public Vector3 defaultPosition;
		
	public bool active;	
	
	public Container(){}
	
	// Deprecated
	public Container(float x, float y, float Z)
	{
		Debug.Log ("constructed a container");
		worldX = x;
		worldY = y;
		worldZ = z;
	}
	
	public Container(Vector3 v)
	{
		//Debug.Log ("constructed a container");
		//x = v.x;
		//y = v.y;
		//z = v.z;
		position = v;
	}
	
	public void addContent()
	{
		
	}

	/*!	\brief resets position of container content
	 * 	\note maybe illogical or not necessary
	 */
	public void ResetContent(Vector3 defaultPos)
	{
		//Debug.Log("RESET Def: " + content + " (" + defaultPos.x + ", " + defaultPos.y + ", " + defaultPos.z + ")");
		//Debug.Log("RESET Pos: " + content + " (" + content.transform.position.x + ", " + content.transform.position.y + ", " + content.transform.position.z + ")");
		
		if(content != null)
			content.transform.position = defaultPos;
			//content.transform.position = defaultPosition;	//new Vector3(0,0,0);
		
		//content.transform.position = new Vector3(0, 1, 4);
		
		//Debug.Log("AFTERMath: " + content + " (" + content.transform.position.x + ", " + content.transform.position.y + ", " + content.transform.position.z + ")");
		
		content = null;
	}
	
}

