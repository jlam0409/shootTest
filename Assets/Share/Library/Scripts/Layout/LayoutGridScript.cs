using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DepthAxis {
	X,
	Y,
	Z
}

public enum LayoutOrder
{
		RowBase,
		ColumnBase
}

public class LayoutGridScript : Layout {
	public DepthAxis depthAxis = DepthAxis.Z;
	
	public int row = 1;
	public int column = 1;
	
	public float horizontalSpacing;
	public float verticalSpacing;
	
	public LayoutOrder layoutOrder = LayoutOrder.RowBase;
	private List<Vector3> position;
	public void Start(){
		//position = CalculateAllPoint();
		//SetupLayout(row, column);
	}
	
	public void SetupLayout(int row, int column){
		this.row = row;
		this.column = column;
//		DebugText.Log ("Layout cache position");
		position = CalculateAllPoint();
	}
	
	public List<Vector3> GetAllPosition(){
		return position;	
	}
	
	public Vector3 GetPosition (int rowID, int columnID){
//		DebugText.Log ("GetPosition - row:"+rowID + "\tcolumn: " + columnID + "\tpos:" + position[columnID + (rowID*column)]);
		return position[columnID + (rowID*column)];
	}
	
	public Vector3 GetPosition (int id){
		return position[id];	
	}
	
	protected List<Vector3> CalculateAllPoint(){
//		DebugText.Log("startCalculateAllpoint: " + transform.position);
		
		Vector3 origin = GetOrigin();
		int firstOrder;
		int secondOrder;
		switch(layoutOrder)
		{
			case LayoutOrder.RowBase:
				firstOrder = row;
				secondOrder = column;
				break;
			case LayoutOrder.ColumnBase:
				firstOrder = column;
				secondOrder = row;
				break;
			default:
				firstOrder = row;
				secondOrder = column;
				break;
		}
		
		float originX = origin.x;
		float originZ = origin.z;
		
		if(layoutSpace == LayoutSpace.OBJECT)
		{
			if (depthAxis == DepthAxis.X) {
				originX = origin.y;
			} else if (depthAxis == DepthAxis.Y) {
				originZ = origin.z;
			} else{
				originZ = origin.y;
			}
		}
		
		List<Vector3> resultList = new List<Vector3>();
		for (int i=0; i<firstOrder; i++){
			for (int j=0; j<secondOrder; j++){
				Vector3 originOffset;
				float x, z;
				if(layoutOrder == LayoutOrder.RowBase)
				{
					originOffset = CalculateOriginOffset(row, column, i, j);
					x = originOffset.x + originX + (horizontalSpacing + cellWidth) * j;
					z = originOffset.z + originZ + (verticalSpacing + cellHeight) * i;
				}
				else
				{
					originOffset = CalculateOriginOffset(row, column, j, i);
					x = originOffset.x + originX + (horizontalSpacing + cellWidth) * i;
					z = originOffset.z + originZ + (verticalSpacing + cellHeight) * j;
				}

				Vector3 currentPosition = Vector3.zero;
				if (depthAxis == DepthAxis.X) {
					currentPosition = new Vector3 (transform.position.x+depthOffset, x, z);
				} else if (depthAxis == DepthAxis.Y) {
					currentPosition = new Vector3 (x, transform.position.y+depthOffset, z);
				} else {
					currentPosition = new Vector3 (x, z, transform.position.z+depthOffset);
				}
				
				// take cares about order
				resultList.Add (currentPosition);
			}
		}
		
		return resultList;
	}
	  
	protected Vector3 CalculateOriginOffset(int row, int column, int rowID, int columnID){
		Vector3 offset = Vector3.zero;
		switch (horizontalOrigin){
			case HorizontalOrigin.RIGHT:
				offset.x -=  (horizontalSpacing + cellWidth) * (column-1);
				break;
			case HorizontalOrigin.CENTER:
				offset.x -= (horizontalSpacing + cellWidth) * (column-1)/2;
				break;
			default:
			case HorizontalOrigin.LEFT:
				// we do nothing to left
				break;
		}
		
		switch (verticalOrigin){
			case VerticalOrigin.TOP:
				offset.z -= (verticalSpacing + cellHeight) * (row-1);
				break;
			case VerticalOrigin.CENTER:
				offset.z -= (verticalSpacing + cellHeight) * (row-1)/2;
				break;
			default:
			case VerticalOrigin.BOTTOM:	
				// we do nothing to bottom
				break;
		}
		
		return offset;
	}
	
	protected void DrawGizmoPreiew(){
		List<Vector3> allPoint = CalculateAllPoint();
		Gizmos.color = gizmosColor;
		for (int i=0; i<allPoint.Count; i++){
			if (depthAxis == DepthAxis.X) {
				Gizmos.DrawCube( allPoint[i], new Vector3(1f, cellWidth,cellHeight));
			} else if (depthAxis ==DepthAxis.Y) {
				Gizmos.DrawCube( allPoint[i], new Vector3(cellWidth,1f,cellHeight));
			} else {
				Gizmos.DrawCube( allPoint[i], new Vector3(cellWidth,cellHeight,1f));
			}
		}
	}
	
	public void OnDrawGizmos (){
		if(base.enableGizmo)
			DrawGizmoPreiew();	
	}
	
}
