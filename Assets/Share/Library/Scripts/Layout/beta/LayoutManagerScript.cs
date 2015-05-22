using UnityEngine;
using System.Collections;
using System.Collections.Generic;	// for List


// maybe make an abstract base class?
public class LayoutManagerScript : MonoBehaviour {
	
	//public static LayoutManagerScript instance;
	
	
	
	public GameObject Page;						// the page the layout(s) are associated with
	
	private LayoutAutoGridScript[] layouts;		
	
	private List<Container> allContainers;	
	
	
	public List<GameObject> testObjects;
	
	
	public GameObject defaultPosition;
	
	void Awake()
	{
		//instance = this;
		
	}
	
	
	// Use this for initialization
	public void Start () {
	
		//if(align)
		//	AlignLayouts();
		
		layouts = gameObject.GetComponents<LayoutAutoGridScript>();
		foreach(LayoutAutoGridScript lay in layouts)
			lay.SetLayout();
		
		allContainers = new List<Container>();
		GetAllContainers();
		
		//FindContainerByOrder("one", 2);
		//FindContainerByOrder("three", 1);
		//FindContainerByOrder("two", 4);
		
		//FindContainerByContent(testObjects[0]);
		//FindContainerByContent(testObjects[1]);
		
		
		//Debug.Log("SWAPPING");
		//SwapContents(testObjects[0], testObjects[1]);
		//PutInContainer(testObjects[0], "one", 7);
		//PutInFirstEmpty(testObjects[2]);
		//PutInFirstEmpty(testObjects[0], "two");
		//PutInContainer(testObjects[1], "two", 3);		
		//PutInFirstEmpty(testObjects[1], "two");
		//PutInContainer(testObjects[2], "one", 8);		
		//PutInContainer(testObjects[0], "one", 8);						
		UpdateLayouts();
		
		Container[] newArray = allContainers.ToArray();
		for(int i=0; i < newArray.Length; i++)
			Debug.Log("Container " + newArray[i].id + " : " + newArray[i].content + "(order: " + newArray[i].order + ")");	
		
	}
	
	
	public void UpdateLayouts()
	{
		foreach(LayoutAutoGridScript lay in layouts)
			lay.UpdateElements();
		
		Container[] newArray = allContainers.ToArray();
		for(int i=0; i < newArray.Length; i++)
			Debug.Log("Container " + newArray[i].id + " : " + newArray[i].content + "(order: " + newArray[i].order + ")");	
	}
		
	
	
	
	public void SwapContents(GameObject objA, GameObject objB) //string setA, int orderA, string setB, int orderB)
	{
		// swap contents, but leave container order/id alone
		Container foundA = FindContainerByContent(objA);
		Container foundB = FindContainerByContent(objB);
		
		GameObject tmp = foundA.content;
		
		foundA.content = foundB.content;
		foundB.content = tmp;
		
		foreach(LayoutAutoGridScript lay in layouts)
			lay.UpdateElements();		
	}
	
	
	
	
/*	// DEPRECATED
	public void MoveFromContainer(GameObject obj, string inSet, int ofOrder)
	{
		Container foundObj = FindContainerByContent(obj);
		Container foundCon = allContainers.Find( con => (con.id == inSet && con.order == ofOrder) );
		
		if(foundCon != null)
		{
			//foundCon.ResetContent(defaultPosition.transform.position);	// Pop old content
			
			foundCon.content = foundObj.content;
			foundObj.content = null;		
		}
		else
			Debug.Log("no container found");
	}
*/
	
	// move from container to container
	public void MoveFromContainer(Container conA, Container conB)
	{
		conB.ResetContent(defaultPosition.transform.position);	// Pop old content
		
		conB.content = conA.content;
		
		conA.content = null;		
	}
	
	// put in first empty spot
	public void PutInFirstEmpty(GameObject obj)
	{
		Container found = FindContainerByContent(obj);
		Container foundEmpty = allContainers.Find( con => (con.content == null) );
		
		if(foundEmpty != null)
		{
			if(found != null)	
				MoveFromContainer(found, foundEmpty);			
			else				
				foundEmpty.content = obj;	
		}
		else 
			Debug.Log("no empty containers found");			
		
	}
/*	
	// put in first empty spot RETURN bool
	public bool PutInFirstEmpty(GameObject obj)
	{
		Container found = FindContainerByContent(obj);
		Container foundEmpty = allContainers.Find( con => (con.content == null) );
		
		if(foundEmpty != null)
		{
			if(found != null)	
				MoveFromContainer(found, foundEmpty);			
			else				
				foundEmpty.content = obj;	
			
			return true;
		}
		else 
			Debug.Log("no empty containers found");	
		
		return false;
		
	}
*/	
	// put in first empty by set
	public void PutInFirstEmpty(GameObject obj, string inSet)
	{
		Container found = FindContainerByContent(obj);
				
		Container foundEmpty = allContainers.Find( con => (con.content == null && con.id == inSet) );		
		
		if(foundEmpty != null)
		{
			if(found != null)	
				MoveFromContainer(found, foundEmpty);			
			else				
				foundEmpty.content = obj;			
		}
		else 
			Debug.Log("no empty containers found");
	}
	
	
	
	// put in specified container
	public void PutInContainer(GameObject obj, string inSet, int ofOrder)
	{
		Container foundCon = FindContainerByContent(obj);		
		Container destCon = allContainers.Find( con => (con.id == inSet && con.order == ofOrder) );
		
		if(destCon != null)
		{
			//destCon.ResetContent(defaultPosition.transform.position);	// Pop old content
			
			if(foundCon != null)
				MoveFromContainer(foundCon, destCon);
			else	
				destCon.content = obj;		
		}
		else
			Debug.Log("no destination container found");
	}
	
	
	public void PutInContainer(GameObject obj, Container con)
	{
		Container foundCon = FindContainerByContent(obj);		
				
		if(con != null)
		{
			if(foundCon != null)
				MoveFromContainer(foundCon, con);
			else	
				con.content = obj;		
		}
		else
			Debug.Log("no destination container found");
	}
	
	
	
	
	
	// basic linear search
	public Container FindContainerByOrder(string inSet, int ofOrder)
	{
		Container found = allContainers.Find( con => (con.id == inSet && con.order == ofOrder) );		//(delegate(Container con) { return (con.id == inSet && con.order == ofOrder); });
		
		if(found != null)
			Debug.Log("found : " + found.content);
		else
			Debug.Log("not found");
		
		return found;
	}
		
	public Container FindContainerByContent(GameObject gObj)
	{
	
		Container found = allContainers.Find( con => (con.content == gObj) );		//(delegate(Container con) { return (con.content == gObj); });

		if(found != null)
			Debug.Log("found : " + found.content);
		else
			Debug.Log("not found");
		
		return found;
	}
	
	
	
	// Sort containers by order?
	//
	private void GetAllContainers()
	{
		foreach(LayoutAutoGridScript lay in layouts)
			allContainers.AddRange(lay.containers);
		
		// SORT!
		
		
		Container[] newArray = allContainers.ToArray();
		for(int i=0; i < newArray.Length; i++)
			Debug.Log("Container " + newArray[i].id + " : " + newArray[i].content + "(order: " + newArray[i].order + ")");					
	}
	
	
	private void AlignLayouts()
	{
		
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
		
}