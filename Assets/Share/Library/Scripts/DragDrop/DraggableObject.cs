using UnityEngine;
using System.Collections;

// need make sure only one Main Camera tag on the scene

public class DraggableObject : MonoBehaviour {

    public enum LimitMovement {
		X,
//		Y,
		Z,
//		XY,
		XZ,
//		YZ,
//		None
	};

    public LimitMovement limitMovement = LimitMovement.XZ;

    private bool isDraggable = true;
    private bool isUsed = false;
    protected bool moving = false;
    protected bool dragging;

//    protected float moveSpeed = 5.0f;

//    protected float dragPositionYTop = 7f;
//    protected float dragPositionYBottom = 0f;
//    public float dragDepthTop = 7.0f;
//    public float dragDepthBottom = 0.0f;
//    protected float dragDepthRatio;
	
	protected Transform objXform;
	
	protected int lastTouchId = -1;
	protected Vector3 lastTouchPos;
	
    protected Vector3 originalPosition;
    protected Vector3 targetPosition;
    protected Vector3 hitOffset;
	
	public GameObject startDragListener;
	public string startDragCommand;
	public string startDragArgument; // gameObject - send gameObject as argument

	public GameObject endDragListener;
	public string endDragCommand;
	public string endDragArgument; // gameObject - send gameObject as argument

	public GameObject dropListener;
	public string dropCommand;
	public string dropArgument; // gameObject - send gameObject as argument
	
	private DropTargetObject dropTargetObject = null;

    public bool IsUsed {
        get { return isUsed; }
        set { isUsed = value; }
    }

    public bool IsDraggable {
        set { isDraggable = value; }
		get { return isDraggable; }
    }
            
    protected void Start() {
		objXform = gameObject.transform;
		originalPosition = objXform.position;
//        dragDepthRatio = (dragDepthTop - dragDepthBottom) / (dragPositionYTop - dragPositionYBottom);
    }

    protected void Update() {
    }

    public void EnableDrag() {
        isDraggable = true;
    }

    public void DisableDrag() {
        isDraggable = false;
    }
	
	public void OnTouchDown(TouchData touchData_){
		if (!isDraggable) {
//			DebugText.Log ("Draggable object not draggable!");
			return;
		}
		
		
		if (lastTouchId < 0) {
			//DebugText.LogWarning ("Draggable object: OnTouchDown init succes!");	
			lastTouchId = touchData_.touchId;
			lastTouchPos = touchData_.GetWorldPosition();
		} else {
			//DebugText.LogError ("Draggable object: OnTouchDown init failed!");	
		}
	}

	public void OnTouchDrag (TouchData touchData_) {
		
		if (lastTouchId < 0) {
			//DebugText.Log ("Draggable object lastTouchId<0!");
			return;
		} else if (lastTouchId != touchData_.touchId) {
			//DebugText.Log ("Draggable object lastTouchId != touchId!");
			return;
		}
		
		//DebugText.Log ("Draggable object: OnTouchDrag");
		if (isDraggable) {
			if (!dragging) {
				hitOffset = lastTouchPos - objXform.position;
				if (dropTargetObject != null) {
					dropTargetObject.Reset();
					dropTargetObject = null;
				}
				ExecSendCallback(startDragListener, startDragCommand, startDragArgument);
			}
			dragging = true;
			if (touchData_.GetWorldPosition() == Vector3.zero) {
				Ray ray = Camera.main.ScreenPointToRay(touchData_.screenPosition);
				LimitDragMovements(ray.GetPoint(0.0f));
			} else {
	            LimitDragMovements(touchData_.GetWorldPosition());
			}
		}
	}
	
	public void OnTouchDragUp (TouchData touchData_) {
		if (!isDraggable || !dragging) {
			return;
		}
	
		dragging = false;
		lastTouchId = -1;
		lastTouchPos = Vector3.zero;
		
		bool droppedTarget = false;
		
		Ray ray = Camera.main.ScreenPointToRay(touchData_.screenPosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 200.0f);
		
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.collider.gameObject.name != this.gameObject.name)
            {                    
                DropTargetObject target = (DropTargetObject)hit.collider.gameObject.GetComponent(typeof(DropTargetObject));

                if (target != null)
                {
                    if (ObjectDropped(target)) {
						dropTargetObject = target;
						droppedTarget = true;
						//DebugText.Log ("Check drop listener dropped?:" + droppedTarget);
						ExecSendCallback(dropListener, dropCommand, dropArgument);
	                    break;
					}
                }               
            }
        }
		//DebugText.Log ("Check drop listener dropped?:" + droppedTarget);
		if (!droppedTarget) {
			ExecSendCallback(endDragListener, endDragCommand, endDragArgument);
		}
	}
	
	private void ExecSendCallback(GameObject listener_, string command_, string comandArgument_){
		if (listener_ == null) {
			return;
		}
		if (comandArgument_ != ""){
			switch (comandArgument_){
				case "this":	
					listener_.SendMessage (command_, this);	
				break;
				case "gameObject":
				case "GameObject":	
					listener_.SendMessage (command_, gameObject);	
				break;
				case "true":
					listener_.SendMessage (command_, true);
				break;
				case "false":
					listener_.SendMessage (command_, false);
				break;
				default: 
					listener_.SendMessage (command_, comandArgument_);		
				break;
			}			
		}else {
			listener_.SendMessage (command_);
		}
	}
	
	public void OnTouchUp (TouchData touchData_) {
		OnTouchDragUp (touchData_);
	}

 	public void OnTouchLeaveUp(TouchData touchData_) {
		OnTouchDragUp (touchData_);
	}
	
	protected void InputEnded()
    {
        dragging = false;
        moving = false;
    }

    protected virtual bool ObjectDropped(DropTargetObject target){		
		// send message to DropTargetObject to let it knows this game Obejct drops to there
		targetPosition = target.DropObject(gameObject);
		//targetPosition = target.DropObject();
			
		if (targetPosition != new Vector3(float.MaxValue, float.MaxValue, float.MaxValue)) {
	        moving = true;
	        objXform.position = targetPosition;
			return true;
		}
		return false;
    }
/*
    private float CalculateRayDistance(ref Vector2 position)
    {
        float rayDistance;

        if (position.y >= dragPositionYTop)
        {
            rayDistance = dragDepthTop;
        }
        else if (position.y <= dragDepthBottom)
        {
            rayDistance = dragDepthTop;
        }
        else
        {
            rayDistance = dragDepthBottom + (dragDepthRatio * (position.y - dragPositionYBottom));
        }

        return rayDistance;
    }
*/
    private void LimitDragMovements(Vector3 pos)
    {
//        float angle = Vector3.Angle(new Vector3(0, ray.direction.y, ray.direction.z), new Vector3(0, 0, -1));
        //angle = angle * Mathf.Deg2Rad;

        switch (limitMovement)
        {
            case LimitMovement.X:
                objXform.position = new Vector3(pos.x-hitOffset.x, objXform.position.y, objXform.position.z);
                break;
//            case LimitMovement.Y:
//                dist -= (objXform.position.z - pos.z) / Mathf.Cos(angle);
//                pos = ray.GetPoint(dist);
//                objXform.position = new Vector3(objXform.position.x, pos.y, objXform.position.z);
//                break;
            case LimitMovement.Z:
                objXform.position = new Vector3(objXform.position.x, objXform.position.y, pos.z-hitOffset.z);
                break;
//            case LimitMovement.XY:
//                objXform.position = new Vector3(pos.x-hitOffset.x, pos.y, objXform.position.z);
//                break;
            case LimitMovement.XZ:
                objXform.position = new Vector3(pos.x-hitOffset.x, objXform.position.y, pos.z-hitOffset.z);
                break;
//            case LimitMovement.YZ:
//                objXform.position = new Vector3(objXform.position.x, pos.y, pos.z);
//                break;
//            case LimitMovement.None:
//                objXform.position = new Vector3(pos.x, pos.y, pos.z);
//                break;
        }
    }    

    protected virtual void ReturnToStart()
    {
        targetPosition = originalPosition;
    }

    public virtual void Reset()
    {
		dropTargetObject = null;
        objXform.position = originalPosition;
        isUsed = false;
    }
}
