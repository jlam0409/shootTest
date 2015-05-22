using UnityEngine;
using System.Collections;
using System.Collections.Generic;	// for List
//using System.Drawing;				// for Point (unsupported)


/*!	\class LayoutAutoGridScript
 * 	\brief Provides a layout of containers for use in arranging scene elements
 * 
 * 	Layouts : None, Grid, Square, Circle, Triangle, Sine (n/a), Free (n/a)
 * 
 * 	\note 
 */
[RequireComponent (typeof (LayoutManagerScript))]
public class LayoutAutoGridScript : MonoBehaviour {
	
		
	//public static LayoutAutoGridScript instance;
	
	public enum GridType
	{	NONE,
		GRID,				// maybe just a group of lines
		//LINE,				// needs at least 2 points (but really just a grid)	
		SQUARE,				// needs at least 4 points
		CIRCLE,				// needs at least 4/5 points
		TRIANGLE,			// needs at least 3 points
		//SINE,
		FREE
	}
	
	public enum GridOrigin
	{	
		TOPLEFT,
		TOPRIGHT,
		BOTLEFT,
		BOTRIGHT
	}
	
	public enum GridDir
	{
		HORIZ,
		VERT
	}
	
	public enum LayoutAlignment
	{
		LEFT,
		CENTER,
		RIGHT,
		TOP,
		BOTTOM
	}
	
	

	
	public string id;
	public LayoutAlignment align;
	
	public List<GameObject> element;

	public Container[] containers = null;

	
	public GridType layoutType;
	



//	private float resolutionX;
//	private float resolutionY;	
	public float scalarX=1.0f;
	public float scalarY=1.0f;
	
	
	
//	private int buffer;				// deprecated
	
	//public int offsetX, offsetY;	// or take in locator.transform
	public GameObject originLocator;
	private Point origin;
	//public bool centered;
	private float radius;
	
	
	private int numItems=0;			// the number of items displayed

	
	// for Grid
	public int rows, columns;
	public GridOrigin gridOrigin = GridOrigin.BOTLEFT;
	public GridDir gridDirection = GridDir.HORIZ;
	
	
	//public string active;	// take in string of a or 
	
	//public List<GameObject> endPoints; 
	//public bool isConnected;
	
	//public bool animate;
	//public bool showPreviz;
	public Previz showPreviz;
	private bool enableGizmo=true;
	
	
	void Awake()
	{
		//instance = this;
		
		//resolutionX = 800f; //Screen.width;
		//resolutionY = 480f; //Screen.height;
		
		//scalar = 1.0f;		
		//rows = 1;
		//columns = 1;
		
		containers = null;
		origin = null;
				
		foreach(GameObject obj in element)
			if(obj != null)
				obj.transform.position = obj.transform.parent.gameObject.transform.position; //new Vector3(0,0,obj.transform.parent.gameObject.transform.position.Z + 4);
		
		enableGizmo = false;
		
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	private void SetResolution()
	{
		//if(!fillScreen)
		// make sure resolution doesn't go over screen ???
//		resolutionX = ((scalarX > 1.0f) ? 1.0f : scalarX) * Screen.width;	//resolutionX *= scalar;
//		resolutionY = ((scalarY > 1.0f) ? 1.0f : scalarY) * Screen.height;	//resolutionY *= scalar;
		
//		Debug.Log("RESOLUTION (" + resolutionX + ", " + resolutionY + ")");
		
	}
	
	
	
	// a bit troublesome to normalize origin (no size resolution, and pivot of bg is the center)
	// so origin is standardized to world point, but points are normalized
	//
	private void SetOrigin()
	{
		origin = new Point(0,0);
		
		if(originLocator != null)
		{		

			//Vector3 loc = Camera.mainCamera.ViewportToWorldPoint(originLocator.transform.position);
			Vector3 loc = originLocator.transform.position;
//			Debug.Log("LOC : " + loc.x + ", " + loc.y + ", " + loc.z);
			
			origin = new Point(loc.x, loc.y, loc.z);
		}
		else
		{		
			LayoutManagerScript parent = gameObject.GetComponent<LayoutManagerScript>();
			//Vector3 parentTrans = Camera.mainCamera.ViewportToWorldPoint(parent.Page.transform.position);
			Vector3 parentTrans = parent.Page.transform.position;			
//			Debug.Log("PARENT : " + parentTrans.x + ", " + parentTrans.y + ", " + parentTrans.z);

			origin = new Point(parentTrans.x, parentTrans.y, parentTrans.z);
			
		}
		
		//Vector3 test = Camera.mainCamera.WorldToViewportPoint(new Vector3(origin.x, origin.y, origin.z));		
		//origin = new Point(test.x, test.y, test.z);
		
//		Debug.Log("ORIGIN (" + origin.x + ", " + origin.y + ", " + origin.z + ")");
	}
	
	
	
	public void SetLayout()
	{	
		//foreach(GameObject obj in element)
		//		obj.transform.position = obj.transform.parent.gameObject.transform.position; //new Vector3(0,0,obj.transform.parent.gameObject.transform.position.Z + 4);
		
		
		SetResolution();		 
		//resolutionX = 800;
		//resolutionY = 480;
						
		SetOrigin();				
		
		Point[] points = DefineLayoutType();
		
		ResetElements();
		
		SetupContainers(points);
		UpdateElements();
			
	
// convert origin 
//centerElement.transform.position = new Vector3(origin.x, Camera.mainCamera.nearClipPlane, origin.z);


		Debug.Log("CONTAINERS");
		for(int i=0; i < containers.Length; i++)
		{	Debug.Log("Container " + i + "(" + containers[i].position.x + ", " + containers[i].position.y + ", " + containers[i].position.z + ")");
			Debug.Log(">> " + containers[i].content);	
		}
			
	}
	
	

	
	
	private Point[] DefineLayoutType()
	{
		Point[] points = new Point[0];
		
		numItems = element.Count;
		//numItems = 0;
		//foreach(GameObject el in element)
		//	if(el != null)
		//		numItems++;
			
//		Debug.Log("numItems : " + numItems);
//		Debug.Log("layoutType : " + layoutType);
		
		if(layoutType == GridType.GRID)
		{
			if(numItems < 2)
			{	//Debug.Log("GRID");				
//				Debug.Log("need more items");
				layoutType = GridType.FREE;				
			}
			else
			{
				points = GetGridPoints();
			}
			
		}
		else
		{	
			Point[] boundingPoints = new Point[0];
			
//			SetRadius(buffer);			// deprecate ???
						
			
			switch(layoutType)
			{
				case GridType.SQUARE 	:	if(numItems < 4)
											{	Debug.Log("need more items");
												layoutType = GridType.NONE;									
											}
											else
											{	//Debug.Log("SQUARE");
												// get bounding points
												//boundingPoints = GetCircleOfPoints(4);															
												//rotate bounds	(Square -45deg = -0.785f)										
												//boundingPoints = RotatePointSet(boundingPoints, -0.785f);		// rotates into proper position	
												boundingPoints = GetCircleOfPointsRotated(4, -0.785f);
											}
											break;
				case GridType.TRIANGLE	:	if(numItems < 3)
											{	//Debug.Log("need more items");
												layoutType = GridType.NONE;				
											}
											else 
											{	//Debug.Log("TRIANGLE");
					
												//boundingPoints = GetCircleOfPoints(3);				
												// rotate bounds (Triangle 90deg = 1.57f)
												//boundingPoints = RotatePointSet(boundingPoints, DegreeToRadian(90.0f)); 
												boundingPoints = GetCircleOfPointsRotated(3, DegreeToRadian(90.0f));
											}
											break;	
				case GridType.CIRCLE 	:	if(numItems < 4)
											{	//Debug.Log("need more items");
												layoutType = GridType.NONE;									
											}
											else
											{	//Debug.Log("CIRCLE");
												boundingPoints = GetCircleOfPoints(numItems);
											}
											break;
				default 				: 	//Debug.Log("FREE");											
											break;
			}
		
			//boundingPoints = ConvertPointToScreenSpace(boundingPoints);		// translates to Quadrant I
			//origin = GetCentroid(boundingPoints);							// updates origin
			//Debug.Log("cen (" + origin.X + ", " + origin.Y + ")");
			points = PopulatePointsBiasBounding(boundingPoints);	
			
//			Debug.Log("BOUNDING");
//			for(int i=0; i < boundingPoints.Length; i++)
//			{
//				Debug.Log("Bounding " + i + "(" + boundingPoints[i].x + ", " + boundingPoints[i].y + ")");
//			}
//			Debug.Log("RADIUS : " + radius);
			
		}//end if-else(layoutType == GridType.GRID)	
		
		
		return points;
	}
	
	
	
	// create Grid
	// 
	private Point[] GetGridPoints()
	{
//		Debug.Log("GetGridPoints()");
		
		// make sure there isn't a 'divide by zero' error
		rows = (rows == 0) ? 1 : rows;
		columns = (columns == 0) ? 1 : columns;
		
				
		// initialize points to number of rows & columns
		Point[] points = new Point[rows * columns];
		
		// Version A : determine normalized size of each row/col		
		//float resC = resolutionX / columns;
		//float resR = resolutionY / rows;
		
		// Version B : determine normalized size of each row/col
		float resC = 1.0f / columns;	
		float resR = 1.0f / rows;
		
		//Debug.Log("Resolution (" + resolutionX + ", " + resolutionY + ")");
		//Debug.Log("(" + resC + ", " + resR + ")");
		
		// get "radius" of each row/col
		float radC = resC / 2;
		float radR = resR / 2;
		
		// set radius to hypotenuse
		//radius = Mathf.RoundToInt(DistanceHypotenuse(resolutionX/2, resolutionY/2));
		//radius = resolutionX / 2;
		
		// reset radius
		radius = 0.0f;
		

		int first, second;
		if(gridDirection == GridDir.HORIZ)
		{	first = rows;
			second = columns;
		}
		else // GridDir.Vert
		{	first = columns;
			second = rows;
		}			
		// NORMALIZED offset values	
		// add 1 to center
		int i=0;
		for(int r=0; r < first; r++)
			for(int c=0; c < second; c++)
			{
				switch(gridOrigin)
				{
					case GridOrigin.TOPLEFT		:	if(gridDirection == GridDir.HORIZ)
														points[i] = new Point((c*resC+radC), -(r*resR+radR)+1);			
													else 
														points[i] = new Point((r*resC+radC), -(c*resR+radR)+1);	
													break;
					case GridOrigin.TOPRIGHT	:	if(gridDirection == GridDir.HORIZ)				
														points[i] = new Point(-(c*resC+radC)+1, -(r*resR+radR)+1);
													else
														points[i] = new Point(-(r*resC+radC)+1, -(c*resR+radR)+1);
													break;
					case GridOrigin.BOTLEFT		:	if(gridDirection == GridDir.HORIZ)
														points[i] = new Point(c*resC+radC, r*resR+radR);
													else
														points[i] = new Point(r*resC+radC, c*resR+radR);
													break;
					case GridOrigin.BOTRIGHT	:	if(gridDirection == GridDir.HORIZ)
														points[i] = new Point(-(c*resC+radC)+1, r*resR+radR);
													else
														points[i] = new Point(-(r*resC+radC)+1, c*resR+radR);
													break;
					default						:	points[i] = new Point(0,0); break;						
				}
		
				i++;				
			}
		
		

		return points;		
	}
	
	
		
	private void SetupContainers(Point[] points)
	{
//		Debug.Log("Setting up Containers");
	
		containers = new Container[points.Length];
		
//		Debug.Log("Screen (" + Screen.width + ", " + Screen.height + ")");
		
		int i=0;
		foreach(Point p in points)
		{	//Debug.Log("i : " + i);

			// convert view to world space
			Vector3 viewPoint = Camera.main.ViewportToWorldPoint(p.GetVector());	
//			Debug.Log("P (" + p.x + ", " + p.y + ", " + p.z + ")");					
//			Debug.Log("viewPoint (" + viewPoint.x + ", " + viewPoint.y + ", " + viewPoint.z + ")");	
			Vector3 v = new Vector3(origin.x + (viewPoint.x*scalarX), Camera.main.nearClipPlane, origin.z + (viewPoint.z*scalarY));
//			Debug.Log("V (" + v.x + ", " + v.y + ", " + v.z + ")");	
			
			containers[i] = new Container(v);
			
			containers[i].id = id;
			
			if(i < element.Count)	// test for Count since Null will throw 'ArgumentOutOfRangeException'			
				containers[i].content = element[i]; //.GetComponent<GameObject>();						
				//containers[i].active = true;	
			//else
				//containers[i].active = false;
				
			containers[i].order = i;
			containers[i].active = true;
			
//			Debug.Log("Container[" + i + "] (" + containers[i].position.x + ", " + containers[i].position.y + ", " + containers[i].position.z + ") : " + containers[i].content + "(order : " + containers[i].order + ")");
			i++;
		}	
	}
	
	
	// ok to use this?
	private void ResetElements()
	{
		foreach(GameObject obj in element)
			if(obj != null)
				obj.transform.position = obj.transform.parent.gameObject.transform.position; //new Vector3(0,0,obj.transform.parent.gameObject.transform.position.Z + 4);		
	}
	
	
	
	// adds and initializes containers at point 
	//
	private void SetupContainersOLD(Point[] points)
	{
		Debug.Log("Setting up Containers");
		
		containers = new Container[points.Length];
		//Debug.Log ("Containers length:" + containers.Length);
		
		
		// convert origin space (world -> view)		
		Vector3 originView = Camera.main.WorldToViewportPoint(new Vector3(origin.x, Camera.main.nearClipPlane, origin.z));
		Debug.Log("CONVERTED : " + originView.x + ", " + originView.y + ", " + originView.z);
			
		
		for(int i=0; i < points.Length; i++)
		{
			// do normalized calculations in viewspace then convert to world 
			Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3((originView.x + points[i].x)*scalarX, (originView.y + points[i].y)*scalarY, Camera.main.nearClipPlane ));
			containers[i] = new Container(p);
			
			containers[i].id = id;
			
			if(i < element.Count)	// test for Count since Null will throw 'ArgumentOutOfRangeException'			
				containers[i].content = element[i]; //.GetComponent<GameObject>();
				//containers[i].active = true;	
			//else
				//containers[i].active = false;
				
			containers[i].order = i;
			containers[i].active = true;
			
			Debug.Log("Container[" + i + "] (" + containers[i].position.x + ", " + containers[i].position.y + ", " + containers[i].position.z + ") : " + containers[i].content + "(order : " + containers[i].order + ")");
				
		}
		
	}
	
	// moves elements to containers
	// test for fillScreen???
	//
	public void UpdateElements()
	{
		for(int i=0; i < containers.Length; i++)
			if(containers[i].content != null)
				containers[i].content.transform.position = containers[i].position;
	
	}
	
	
	public void PopContent(Container con)
	{
	}		

	
	// gets (straight-path) slice (not arc length)
	//
	private float GetSlice(float totalLength, int segments)
	{		
		return totalLength / segments;
	}
	
	private float GetSliceNormalized(float totalLength, int segments)
	{
			float slice = totalLength / segments;
			return slice / totalLength;		
	}
	
	private float GetSliceNormalized(float totalLength, float slice)
	{
		return slice / totalLength;		
	}
	
	
	// sets the radius, but doesn't allow it to go off screen
	// takes in a buffer to "pad" radius
	//
	private void SetRadius(int buffer=0)
	{
		//radius = ((resolutionX > resolutionY) ? resolutionY-buffer : resolutionX-buffer) / 2;		
		radius = 1.0f;
	}
	
/*	
	// can get bounding points for square, triangle, circle
	private void GetCirclePoints(int points)	//, double radius)
	{	
		float slice = 2 * Mathf.PI / points;
		for(int i=0; i < points; i++)
		{
			float angle = slice * i;
			int newX = (int)(origin.X + radius * Mathf.Cos(angle));
			int newY = (int)(origin.Y + radius * Mathf.Sin(angle));
			Point p = new Point(newX, newY);
			
			Debug.Log("point (" + p.X + ", " + p.Y + ")");
		}
	}
*/
	
	
	// can get bounding points for square, triangle, circle
	// ...maybe ref boundingPoints[] in parameter?
	//

	// DEPRECATED
/*	private Point[] GetCircleOfPoints(int numPoints)	
	{	
		Point[] pBounds = new Point[numPoints];
		
		float slice = 2 * Mathf.PI / numPoints;		// radians
		for(int i=0; i < numPoints; i++)
		{
			float angle = slice * i;
			float newX = origin.x + radius * Mathf.Cos(angle);	// Mathf.RoundToInt
			float newY = origin.y + radius * Mathf.Sin(angle);	// Mathf.RoundToInt
			//Point p = new Point(newX, newY);
			
			pBounds[i] = new Point(newX, newY);
			
			//Debug.Log("point (" + p.X + ", " + p.Y + ")");
			Debug.Log(" > Bounds " + i + " (" + pBounds[i].x + ", " + pBounds[i].y + ")");
		}
		
		return pBounds;
	}
*/	
	
	
	private Point[] GetCircleOfPoints(int numPoints)	
	{	
		Point[] pBounds = new Point[numPoints];
		
		float slice = 2 * Mathf.PI / numPoints;		// radians
		for(int i=0; i < numPoints; i++)
		{
//			Debug.Log("ORI (" + origin.x + ", " + origin.y + ")");
			Vector3 o = Camera.main.ScreenToViewportPoint(new Vector3(origin.x, origin.y, origin.y));
//			Debug.Log("O (" + o.x + ", " + o.y + ", " + o.z + ")");
			
			float angle = slice * i;
			float newX = o.x + radius * Mathf.Cos(angle);	// Mathf.RoundToInt
			float newY = o.y + radius * Mathf.Sin(angle);	// Mathf.RoundToInt
			//Point p = new Point(newX, newY);
			
			//pBounds[i] = new Point(newX, newY);
			
			Vector3 v = Camera.main.WorldToViewportPoint(new Vector3(newX, newY, newY));
			pBounds[i] = new Point(v.x, v.y);
			
			//Debug.Log("point (" + p.X + ", " + p.Y + ")");
//			Debug.Log(" > Bounds " + i + " (" + pBounds[i].x + ", " + pBounds[i].y + ")");
		}
		
		return pBounds;
	}
	
	
	private Point[] GetCircleOfPointsRotated(int numPoints, float rotate)	
	{	
		Point[] pBounds = new Point[numPoints];
		
		float slice = 2 * Mathf.PI / numPoints;		// radians
		for(int i=0; i < numPoints; i++)
		{
//			Debug.Log("ORI (" + origin.x + ", " + origin.y + ")");
			Vector3 o = Camera.main.ScreenToViewportPoint(new Vector3(origin.x, origin.y, origin.y));
//			Debug.Log("O (" + o.x + ", " + o.y + ", " + o.z + ")");
			
			float angle = slice * i;
			float newX = o.x + radius * Mathf.Cos(angle+rotate);	// Mathf.RoundToInt
			float newY = o.y + radius * Mathf.Sin(angle+rotate);	// Mathf.RoundToInt
			//Point p = new Point(newX, newY);
			
			//pBounds[i] = new Point(newX, newY);
			
			Vector3 v = Camera.main.WorldToViewportPoint(new Vector3(newX, newY, newY));
			pBounds[i] = new Point(v.x, v.y);
			
			//Debug.Log("point (" + p.X + ", " + p.Y + ")");
//			Debug.Log(" > Bounds " + i + " (" + pBounds[i].x + ", " + pBounds[i].y + ")");
		}
		
		return pBounds;
	}

	
	// DEPRECATED
	// angle in radians
	// boundingPoints is smaller
	//
	private Point[] RotatePointSet(Point[] p, float angle)
	{
//		Debug.Log("Rotating");
		/*for(int i=0; i < points.Length; i++)
			points[i] = RotatePointAroundOrigin(points[i], angle);
			// rotate boundingPoints too?
		*/
		
		Point[] rotatedP = new Point[p.Length];
		
		for(int i=0; i < p.Length; i++)
			rotatedP[i] = RotatePointAroundOrigin(p[i], angle);
		
		return rotatedP;		
	}
	
	
	// DEPRECATED
	// counter-clockwise rotation
	// angle is theta, and in radians	
	// 45 = 0.785f
	// 90 = 1.57f
	// 120 = 2.094f
	// 180 = 3.141f
	// 270 = 4.712
	// 360 = 6.283
	//
	private Point RotatePointAroundOrigin(Point p, float angle)
	{
		//Debug.Log("Origin (" + origin.X + ", " + origin.Y + ")");
		//Debug.Log("Before (" + p.X + ", " + p.Y + ")");
		
		Point rotated = new Point(0, 0);
		
		// Mathf.Ceil or Mathf.RoundToInt
		rotated.x = Mathf.Cos(angle) * (p.x - origin.x) - Mathf.Sin(angle) * (p.y - origin.y) + origin.x; 
		rotated.y = Mathf.Sin(angle) * (p.x - origin.x) + Mathf.Cos(angle) * (p.y - origin.y) + origin.y; 
		
		//Debug.Log("Rotated (" + rotated.X + ", " + rotated.Y + ")");
		return rotated;
	}	
	// rotate around a given point
	//
	private Point RotatePointAroundPoint(Point p, float angle, Point center)
	{
//		Debug.Log("Center (" + center.x + ", " + center.y + ")");
//		Debug.Log("Before (" + p.x + ", " + p.y + ")");
		
		Point rotated = new Point(0, 0);
		
		// Mathf.Ceil ?
		rotated.x = Mathf.Cos(angle) * (p.x - center.x) - Mathf.Sin(angle) * (p.y - center.y) + center.x; 
		rotated.y = Mathf.Sin(angle) * (p.x - center.x) + Mathf.Cos(angle) * (p.y - center.y) + center.y; 
				
//		Debug.Log("Rotated (" + rotated.x + ", " + rotated.y + ")");
		return rotated;
	}
	
	
	
		
	// lays points down irrespective of bounding points
	// meaning that points will be uniformly spread out across the total length of the shape
	//
	private Point[] PopulatePointsEvenly(Point[] boundingPoints)
	{
		Point[] points = new Point[numItems];
		
		// get total length of all segments		
		float totalLength=0.0f;
		for(int i=0; i < boundingPoints.Length; i++)
			totalLength += DistanceBetweenPoints(boundingPoints[i], (i==(boundingPoints.Length-1) ? boundingPoints[0] : boundingPoints[i+1]));
		
		Debug.Log("totalLength : " + totalLength);
		Debug.Log("numItems : " + numItems);
		
		
		float slice = totalLength / numItems;	// the slice in terms of length
		Debug.Log("Slice : " + slice);
		
		float currentSlice = 0.0f;				// the slice to apply to a point
		int bCount = 0;							// index for boundingPoints
		for(int i=0; i < numItems; i++)
		{
			
			Point pointA = boundingPoints[bCount];
			Point pointB = (bCount==(boundingPoints.Length-1) ? boundingPoints[0] : boundingPoints[bCount+1]);
			
			float edgeLength = DistanceBetweenPoints(pointA, pointB);
			
			Debug.Log("CurrentSlice : " + currentSlice);
			float normSlice = GetSliceNormalized(edgeLength, currentSlice);
						
			if(normSlice > 1.0f)	// currentSlice runs past current boundingPoint
			{
				Debug.Log("over : " + normSlice);
				currentSlice -= edgeLength;
				bCount++;
				i--;				
			}
			else 					// within current bounding edge
			{
				points[i] = PointBetweenTwoPoints(pointA, pointB, normSlice);
				print("Points " + i + " (" + points[i].x + ", " + points[i].y + ")");
				currentSlice += slice;
			}
		}				
		
		return points;
	}
	
	
	
	// lays points down respecting bounding shape
	// bounding points are delineated 
	// can't take total (triangle with numItems==8, 5 points over total is every 0.5 eventually overlap with bounding points)	
	//	
	private Point[] PopulatePointsBiasBounding(Point[] boundingPoints)
	{
//		Debug.Log("Populating Points with Extreme Prejudice");
		
		Point[] points = new Point[numItems];		
		int numEdges = boundingPoints.Length;		// number of edges belonging to shape
	
		// iterates thru each edge, determines how many segment points (excluding bounding points) can be in this edge
		// then adds first bounding point (but not second...that will be added in the next segment or if its the last edge, not at all)
		//
		int pCount = 0;
		for(int i=0; i < numEdges; i++)
		{	
			Point pointA = boundingPoints[i];
			Point pointB = (i==(boundingPoints.Length-1) ? boundingPoints[0] : boundingPoints[i+1]);
			
			float edgeLength = DistanceBetweenPoints(pointA, pointB);
			
			int numEdgePoints = ((numItems-numEdges) + numEdges - (i + 1)) / numEdges;		// number of points in edge (exclusive of bounding points)		
//			Debug.Log("numSegPoints : " + numEdgePoints);
			
			float slice = edgeLength / (numEdgePoints + 1);		// the slice length, +1 to add first boundary point
			
			// assigns points, 0 is the boundary point
			for(int k=0; k <= numEdgePoints; k++)
			{
				float normSlice = GetSliceNormalized(edgeLength, k * slice);	// normalize slice, k is the position in segment
				
				points[pCount] = PointBetweenTwoPoints(pointA, pointB, normSlice);
				
//				Debug.Log("POINT (" + points[pCount].x + ", " + points[pCount].y + ", " + points[pCount].z + ")");
				
				pCount++;					
			}
		}	
		
		return points;
	}
				
			
			

		

		
		

	
/*
 * mirror experiment...across X axis
 * BUT can't always mirror (in case of odd numItems)
 * 
		for(int k=0; k < points.Length; k++)
		{
			if(points[k].Y >= 0.0)
			{
				
				Point p2 = new Point(-radius, 0);
				float pDist = DistanceBetweenPoints(points[k], p2);
				float angle = AngleBetweenPoints(points[k], p2);
								
				int newX = points[k].X; //(int)Mathf.Round(p2.X + pDist * Mathf.Cos(-angle));	// Mathf.RoundToInt
				int newY = (int)Mathf.Round(p2.Y + pDist * Mathf.Sin(-angle));	// Mathf.RoundToInt
			
				Debug.Log("p1 (" + points[k].X + ", " + points[k].Y + ")");
				Debug.Log("Mirror (" + newX + ", " + newY + ")");
				Debug.Log("p2 (" + p2.X + ", " + p2.Y + ")"); 
				Debug.Log("Angle " + k + " : " + RadianToDegree(angle));	
				
				
			}			
		}

*/	
	
	// returns angle in radians relative to X-axis
	//
	private float AngleBetweenPoints(Point p1, Point p2)
	{
		return Mathf.Atan2(p1.y - p2.y, p2.x - p1.x);
	}
		
	private float RadianToDegree(float radian)
	{	return radian * (180.0f / Mathf.PI);	}
	
	private float DegreeToRadian(float degree)
	{	return degree / (180.0f / Mathf.PI);	}
	
		
/*	
 	// breaks at 90+ degrees...	
	private float AngleBetweenVector(Point p1, Point p2)
	{
		float xz = p1.X * p2.X + 0.0f; // 0.0f for z
		float sqrP1 = p1.Y * p1.Y;
		float p1p2Y = p1.Y * p2.Y;
		float sqrP2 = p2.Y * p2.Y;
		
		float cosangle = (xz + p1p2Y) / Mathf.Sqrt((xz + sqrP1) * (xz + sqrP2));
		return Mathf.Acos(cosangle);
	}
*/		
	
	
/*	private float AngleBetweenPoints(Point p1, Point p2)
	{
		// negate x/y
		Point pNeg =  new Point(p2.X - p1.X, p2.Y - p1.Y);
		float angle = 0.0f;
		
		if(pNeg.X == 0.0f)
		{
			if(pNeg.X == 0.0f)
				angle = 0.0f;
			else
				if(pNeg.Y > 0.0)
					angle = Mathf.PI / 2.0f;
				else 
					angle = Mathf.PI * 3.0f / 2.0f;			
		}
		else
			if(pNeg.Y == 0.0f)
			{
				if(pNeg.X > 0.0f)
					angle = 0.0f;
				else				
					angle = Mathf.PI;
			}
			else
			{
				if(pNeg.X < 0.0f)
					angle = Mathf.Atan(pNeg.Y / pNeg.X) + Mathf.PI;
				else 
					if(pNeg.Y < 0.0f)
						angle = Mathf.Atan(pNeg.Y / pNeg.X) + (2 * Mathf.PI);
					else
						angle = Mathf.Atan(pNeg.Y / pNeg.X);
			}
			
		angle = angle * 180 / Mathf.PI;
		
		return angle;
	}
*/	
	
	
	
	// gets a point between two points given a distance (normalized)
	//
	private Point PointBetweenTwoPoints(Point p1, Point p2, float blend=0.0f)
	{
//		float distanceP1P2 = DistanceBetweenPoints(p1, p2);
		
		// [DEPRECATED] normalize byDistance 
		//float blend = byDistance / distanceP1P2;
		
		Point pBtwn = new Point(0,0);

		// make sure blend doesn't exceed distance from p1 to p2 		
		pBtwn.x = p1.x + ((blend > 1.0f) ? 1.0f : blend) * (p2.x - p1.x);
		pBtwn.y = p1.y + ((blend > 1.0f) ? 1.0f : blend) * (p2.y - p1.y);
				
		
		//Debug.Log("p1 (" + p1.X + ", " + p1.Y + ")");
		//Debug.Log("p2 (" + p2.X + ", " + p2.Y + ")");
		//Debug.Log("blend : " + blend );
		//Debug.Log("Between (" + pBtwn.X + ", " + pBtwn.Y + ")");
		
		return pBtwn;
	}

/*	private Point PointBetweenTwoPoints(Point p1, Point p2, float blend=0.0f)
	{
		float distanceP1P2 = DistanceBetweenPoints(p1, p2);
		
		// normalize byDistance
		//float blend = byDistance / distanceP1P2;
		
		Point pBtwn = new Point(0,0);

		// make sure blend doesn't exceed distance from p1 to p2
		pBtwn.X = (int)Mathf.Round(p1.X + ((blend > 1.0f) ? 1.0f : blend) * (p2.X - p1.X));
		pBtwn.Y = (int)Mathf.Round(p1.Y + ((blend > 1.0f) ? 1.0f : blend) * (p2.Y - p1.Y));
				
		
		Debug.Log("p1 (" + p1.X + ", " + p1.Y + ")");
		Debug.Log("p2 (" + p2.X + ", " + p2.Y + ")");
		Debug.Log("blend : " + blend + " (byDistance : " + byDistance + ")");
		Debug.Log("Between (" + pBtwn.X + ", " + pBtwn.Y + ")");
		
		return pBtwn;
	}
*/	
	 
	// gets distance between two points via...
	// pythagorean theorem...sqrt ( (x2-x1)^2 + (y2-y1)^2 )
	//
	private float DistanceBetweenPoints(Point p1, Point p2)
	{
		//float distance = Mathf.Sqrt(Mathf.Pow((p2.X-p1.X),2)+Mathf.Pow((p2.Y-p1.Y),2));	
		//Debug.Log("distance : " + distance);
		
		return Mathf.Sqrt(Mathf.Pow((p2.x-p1.x),2) + Mathf.Pow((p2.y-p1.y),2));	
	}
	// gets hypotenuse given two legs
	private float DistanceHypotenuse(float legA, float legB)
	{
		return Mathf.Sqrt(Mathf.Pow(legA,2) + Mathf.Pow(legB,2));		
	}
	
	
	// the centroid (center) of an assortment of points
	//
	private Point GetCentroid(Point[] p)
	{
		// Point centroidP = new Point(0,0);
		
		float x = 0.0f;
		float y = 0.0f;
		for(int i=0; i < p.Length; i++)
		{	
			//Debug.Log("CENTROID (" + p[i].x + ", " + p[i].y + ")");
			
			x += p[i].x;
			y += p[i].y;
		}
		
		
		
		return new Point(x/p.Length, y/p.Length);		
	}
	
	// the incenter (origin) of a triangle given three points
	// CENTROID will provide the same result
	//
	private Point TriangleInCenter(Point p1, Point p2, Point p3)
	{
		Point incenter = new Point(0,0);
		
		incenter.x = (p1.x + p2.x + p3.x) / 3;
		incenter.y = (p1.x + p2.x + p3.x) / 3;
		
		Debug.Log("Incenter (" + incenter.x + ", " + incenter.y + ")");
		return incenter;		
	}
	
	
	
	// translate values to quadrant I
	// maybe rename to TranslateToQuadrantI()
	//
	private Point[] ConvertPointToScreenSpace(Point[] p)
	{
		Point[] screenP = new Point[p.Length];
		
		for(int i=0; i < p.Length; i++)
			screenP[i] = new Point(p[i].x+radius, p[i].y+radius);		
		
		return screenP;
	}	
	
	private void ConvertPointToViewPort()
	{
		
	}

	
	
	
	//public void RaiseLayout(float height)
	//public void HideLayout()
	
	
	
////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////	
	
	
	public enum Previz
	{	
		NONE,
		GIZMO,
		TEXTURE
	}
	
	
	//private bool runonce;
	
	void ShowVisualization()
	{
//		Debug.Log("DRAWING GIZMO");

		Point[] gizPoints = new Point[0];
		
		SetResolution();
		SetOrigin();
		ResetElements();
		
		switch(showPreviz)
		{
			case Previz.NONE	:	
									do
									{
										//foreach(GameObject obj in element)
										//	if(obj != null)
										//		obj.transform.position = obj.transform.parent.gameObject.transform.position; //new Vector3(0,0,obj.transform.parent.gameObject.transform.position.Z + 4);		
										
//										Debug.Log("NONE");
									
										//runonce = false;
									}while(false);	
			
									break;
			
			case Previz.GIZMO	:	
									do
									{
										gizPoints = DefineLayoutType();
										
										//foreach(GameObject obj in element)
										//	if(obj != null)
										//		obj.transform.position = obj.transform.parent.gameObject.transform.position; //new Vector3(0,0,obj.transform.parent.gameObject.transform.position.Z + 4);		
										
//										Debug.Log("Screen (" + Screen.width + ", " + Screen.height + ")");
										
										Vector3 eSize = element[0].collider.bounds.size;
//										Debug.Log("ESIZE (" + eSize.x + ", " + eSize.y + ", " + eSize.z + ")");
				
//										if(eSize == null)
//											eSize = new Vector3(1,1,1);
				
										int i=0;
										foreach(Point p in gizPoints)
										{	//Debug.Log("i : " + i);
											Gizmos.color = new Color(1, 0, 0, 0.5F);
											Vector3 giz = Camera.main.ViewportToWorldPoint(p.GetVector());
//											Debug.Log("P (" + p.x + ", " + p.y + ", " + p.z + ")");					
//											Debug.Log("GIZ (" + giz.x + ", " + giz.y + ", " + giz.z + ")");	
											Vector3 t = new Vector3(origin.x + (giz.x*scalarX), Camera.main.nearClipPlane, origin.z + (giz.z*scalarY));
//											Debug.Log("T (" + t.x + ", " + t.y + ", " + t.z + ")");	
											//Gizmos.DrawCube( t, new Vector3(1,1,1));
											Gizmos.DrawCube( t, new Vector3(eSize.x,eSize.y,eSize.z));
											i++;
										}
				
										//runonce = false;
									}while(false);			
									
									break;
			
			case Previz.TEXTURE	:	
									do
									{
										gizPoints = DefineLayoutType();
				
										SetupContainers(gizPoints);
										UpdateElements();
														
										//runonce = false;
									}while(false);	
			
									break;
		}
		
		
		Gizmos.color = new Color(1, 1, 0, 0.75F);
		Vector3 gizOrigin = new Vector3(origin.x, Camera.main.nearClipPlane, origin.z);
		//Vector3 gizOrigin = Camera.main.WorldToViewportPoint(new Vector3(origin.x, origin.y, origin.z));
//		Debug.Log("GIZOrigin (" + gizOrigin.x + ", " + gizOrigin.y + ", " + gizOrigin.z + ")");
		Gizmos.DrawSphere(gizOrigin, 0.25f);
		
		
	}
	

	
	void OnDrawGizmos()
	//void OnDrawGizmosSelected()
	{	
		if(enableGizmo)
			ShowVisualization();
		
		
		
	}	
	

	
}
