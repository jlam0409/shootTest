using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreatePlane : ScriptableWizard
{
    public enum Orientation {Horizontal, Vertical}
	public enum Pivot {BottomLeft, BottomRight, Center, TopCenter, BottomCenter, LeftCenter, RightCenter}
    public int widthSegments = 1;
    public int lengthSegments = 1;
    public float width = 1.0f;
    public float length = 1.0f;
    public Orientation orientation = Orientation.Horizontal;
	public Pivot pivot = Pivot.BottomLeft;
    public bool addCollider = false;
    public bool createAtOrigin = false;
    public string optionalName;
    static Camera cam;
    static Camera lastUsedCam;
    
    [MenuItem ("GameObject/Create Other/Custom Plane...")]
    static void CreateWizard()
    {
        cam = Camera.current;
        // Hack because camera.current doesn't return editor camera if scene view doesn't have focus
        if (!cam)
            cam = lastUsedCam;
        else
            lastUsedCam = cam;
        ScriptableWizard.DisplayWizard("Create Plane", typeof(CreatePlane));
    }
    
    void OnWizardUpdate ()
    {
        widthSegments = Mathf.Clamp(widthSegments, 1, 254);
        lengthSegments = Mathf.Clamp(lengthSegments, 1, 254);
    }
    
    void OnWizardCreate()
    {
        GameObject plane = new GameObject();
        
        if (!string.IsNullOrEmpty(optionalName))
            plane.name = optionalName;
        else
            plane.name = "Plane";
        
        if (!createAtOrigin && cam)
            plane.transform.position = cam.transform.position + cam.transform.forward * 5.0f;
        else
            plane.transform.position = Vector3.zero;
        
        //string meshPrefabPath = "Assets/Editor/" + plane.name + widthSegments + "x" + lengthSegments + "W" + width + "L" + length + (orientation == Orientation.Horizontal? "H" : "V") + ".asset";
		
		string pivotCode = "";
		switch (pivot){
			case Pivot.Center:			pivotCode = "CC";	break;
			case Pivot.LeftCenter:		pivotCode = "LC";	break;
			case Pivot.RightCenter:		pivotCode = "RC";	break;
			case Pivot.BottomRight:		pivotCode = "BR";	break;
			case Pivot.TopCenter:		pivotCode = "TC";	break;
			case Pivot.BottomCenter:	pivotCode = "BC";	break;
			default:
			case Pivot.BottomLeft:		pivotCode = "BL";	break;
		}
		string meshPrefabPath = "Assets/Editor/" + plane.name + widthSegments + "x" + lengthSegments + "W" + width + "L" + length + (orientation == Orientation.Horizontal? "H" : "V") + "P" + pivotCode + ".asset";
        MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
        plane.AddComponent(typeof(MeshRenderer));

        Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath(meshPrefabPath, typeof(Mesh));
                
        if (m == null)
        {
            m = new Mesh();
            m.name = plane.name;
        
            int hCount2 = widthSegments+1;
            int vCount2 = lengthSegments+1;
            int numTriangles = widthSegments * lengthSegments * 6;
            int numVertices = hCount2 * vCount2;
        
            Vector3[] vertices = new Vector3[numVertices];
            Vector2[] uvs = new Vector2[numVertices];
            int[] triangles = new int[numTriangles];
        
            int index = 0;
            float uvFactorX = 1.0f/widthSegments;
            float uvFactorY = 1.0f/lengthSegments;
            float scaleX = width/widthSegments;
            float scaleY = length/lengthSegments;
			
			Vector2 centerOffset = new Vector2 (hCount2 * scaleX/ 4.0f, vCount2 * scaleY / 4.0f);
            for (float y = 0.0f; y < vCount2; y++)
            {
                for (float x = 0.0f; x < hCount2; x++)
                {
					/*
                    if (orientation == Orientation.Horizontal)
                        vertices[index] = new Vector3(x*scaleX, 0.0f, y*scaleY);
                    else
                        vertices[index] = new Vector3(x*scaleX, y*scaleY, 0.0f);
                    */
					if (orientation == Orientation.Horizontal)
						switch (pivot){
							case Pivot.Center:
								vertices[index] = new Vector3 (x*scaleX - centerOffset.x, 0f, y*scaleY - centerOffset.y);
								break;
							case Pivot.LeftCenter:
								vertices[index] = new Vector3(x*scaleX, 0f, y*scaleY - centerOffset.y);
								break;
							case Pivot.RightCenter:
								vertices[index] = new Vector3(x*scaleX - centerOffset.x*2, 0f, y*scaleY - centerOffset.y);
								break;
							case Pivot.BottomRight:
								vertices[index] = new Vector3 (x*scaleX - centerOffset.x*2, 0f, y*scaleY);
								break;
							case Pivot.BottomLeft:
								vertices[index] = new Vector3(x*scaleX, 0.0f, y*scaleY);
								break;
							case Pivot.TopCenter:
								vertices[index] = new Vector3 (x*scaleX - centerOffset.x, 0f, y*scaleY - centerOffset.y*2);
								break;
							case Pivot.BottomCenter:
								vertices[index] = new Vector3 (x*scaleX - centerOffset.x, 0f, y*scaleY);
								break;
						}
					else {
						switch (pivot){
							case Pivot.Center:
								vertices[index] = new Vector3(x*scaleX-centerOffset.x, y*scaleY-centerOffset.y, 0.0f);
								break;
							case Pivot.LeftCenter:
								vertices[index] = new Vector3 (x*scaleX, y*scaleY-centerOffset.y, 0f);
								break;
							case Pivot.RightCenter:
								vertices[index] = new Vector3 (x*scaleX - centerOffset.x*2, y*scaleY-centerOffset.y, 0f);
								break;
							case Pivot.BottomRight:
								vertices[index] = new Vector3 (x*scaleX - centerOffset.x*2, y*scaleY, 0f);
								break;
							case Pivot.BottomLeft:
								vertices[index] = new Vector3(x*scaleX, y*scaleY, 0.0f);
								break;
							case Pivot.TopCenter:
								vertices[index] = new Vector3 (x*scaleX-centerOffset.x, y*scaleY - centerOffset.y*2, 0f);
								break;
							case Pivot.BottomCenter:
								vertices[index] = new Vector3 (x*scaleX - centerOffset.x, y*scaleY, 0f);
								break;
						}
					}
                    uvs[index++] = new Vector2(x*uvFactorX, y*uvFactorY);
                }
            }
            
            index = 0;
            for (int y = 0; y < lengthSegments; y++)
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    triangles[index]   = (y     * hCount2) + x;
                    triangles[index+1] = ((y+1) * hCount2) + x;
                    triangles[index+2] = (y     * hCount2) + x + 1;
        
                    triangles[index+3] = ((y+1) * hCount2) + x;
                    triangles[index+4] = ((y+1) * hCount2) + x + 1;
                    triangles[index+5] = (y     * hCount2) + x + 1;
                    index += 6;
                }
            }
        
            m.vertices = vertices;
            m.uv = uvs;
            m.triangles = triangles;
            m.RecalculateNormals();
            
            AssetDatabase.CreateAsset(m, meshPrefabPath);
            AssetDatabase.SaveAssets();
        }
        
        meshFilter.sharedMesh = m;
        m.RecalculateBounds();
        
        if (addCollider)
            plane.AddComponent(typeof(BoxCollider));
        
        Selection.activeObject = plane;
    }
}