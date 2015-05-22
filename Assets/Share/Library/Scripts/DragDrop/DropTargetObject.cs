using UnityEngine;
using System.Collections;

public class DropTargetObject : MonoBehaviour {
	public bool holdDroppedObject = false;
	public GameObject droppedObject;
	
    public bool multipleDrags = false;

    public bool isTaken = false;

	public GameObject dropListener;
	public string dropCommand;
	public string dropArgument;
	
    /// <summary>
    /// Use this for initialization
    /// </summary>
    protected virtual void Start()
    {
    }

	public Vector3 DropObject(GameObject droppedObject_){
		if (holdDroppedObject){
			droppedObject = droppedObject_;
		}
		return DropObject();
	}
	
    public virtual Vector3 DropObject()
    {
		ExecSendCallback(dropListener, dropCommand, dropArgument);
		if (!isTaken)
        {
            PlaceObject();
            return this.transform.position;
        }
        return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    }

    private void PlaceObject()
    {
        if (!multipleDrags)
        {
            isTaken = true;
        }
    }

    public virtual void Reset()
    {
        isTaken = false;
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
}
