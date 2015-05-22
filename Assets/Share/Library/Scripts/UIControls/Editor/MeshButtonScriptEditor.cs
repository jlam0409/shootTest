using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MeshButtonScript))]
public class MeshButtonScriptEditor : Editor {
    public override void OnInspectorGUI () {
		MeshButtonScript script = (MeshButtonScript) target;
		EditorGUIUtility.LookLikeInspector();
		
		script.callbackAtEvent = (CallbackTouchEvent)EditorGUILayout.EnumPopup ("Callback at Event", script.callbackAtEvent);
		if (script.callbackAtEvent != CallbackTouchEvent.None){
			script.callback.callbackListener = (GameObject)EditorGUILayout.ObjectField (new GUIContent("Callback Listener"), script.callback.callbackListener, typeof(GameObject), true);
			if (script.callback.callbackListener != null){
				script.callback.callbackFunction = EditorGUILayout.TextField ("Callback Function", script.callback.callbackFunction);
				if (script.callback.callbackFunction != ""){
					script.callback.callbackArgument = EditorGUILayout.TextField ("Callback Argument", script.callback.callbackArgument);
				}
			}
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		script.touchTransfer = EditorGUILayout.Toggle ("Enable Touch Transfer", script.touchTransfer);
		if (script.touchTransfer){
			script.touchTransferListener = (GameObject)EditorGUILayout.ObjectField (new GUIContent("Touch Transfer Listener"), script.touchTransferListener, typeof(GameObject), true);
			EditorGUILayout.Space();
			EditorGUILayout.Space();
		}
		
		
		script.enableTouchConverter = EditorGUILayout.Toggle ("Enable Touch Converter", script.enableTouchConverter);
		if (script.enableTouchConverter){
			script.onTouchClickConverter = (TouchEventConverter)EditorGUILayout.EnumPopup("OnTouchClick Converter", script.onTouchClickConverter);
			script.onTouchHoldConverter = (TouchEventConverter)EditorGUILayout.EnumPopup("OnTouchHold Converter", script.onTouchHoldConverter);
			script.onTouchDragConverter = (TouchEventConverter)EditorGUILayout.EnumPopup("OnTouchDrag Converter", script.onTouchDragConverter);
			script.onTouchDragUpConverter = (TouchEventConverter)EditorGUILayout.EnumPopup("OnTouchDragUp Converter", script.onTouchDragUpConverter);
			EditorGUILayout.Space();
			EditorGUILayout.Space();
		}
		
		
		script.mode = (ButtonStateChangeMode)EditorGUILayout.EnumPopup("Mode", script.mode);
		script.dragDisplayState = (MeshButtonState)EditorGUILayout.EnumPopup("Drag Display State", script.dragDisplayState);
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		switch (script.mode){
			case ButtonStateChangeMode.SetTexture:
				script.upTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("Up Texture"), script.upTexture, typeof(Texture), true);
				script.downTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("Down Texture"), script.downTexture, typeof(Texture), true);
				if (script.dragDisplayState == MeshButtonState.Drag)
					script.dragTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("Drag Texture"), script.dragTexture, typeof(Texture), true);
				script.lockTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("Lock Texture"), script.lockTexture, typeof(Texture), true);
				break;
			case ButtonStateChangeMode.SetValue:	
				script.upValue = EditorGUILayout.IntField("Up Value", script.upValue);
				script.downValue = EditorGUILayout.IntField("Down Value", script.downValue);
				if (script.dragDisplayState == MeshButtonState.Drag)
					script.dragValue = EditorGUILayout.IntField("Drag Value", script.dragValue);
				script.lockValue = EditorGUILayout.IntField("Lock Value", script.lockValue);
				break;
			case ButtonStateChangeMode.SetTextureValue:	
				script.upTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("Up Texture"), script.upTexture, typeof(Texture), true);
				script.downTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("Down Texture"), script.downTexture, typeof(Texture), true);
				if (script.dragDisplayState == MeshButtonState.Drag)
					script.dragTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("Drag Texture"), script.dragTexture, typeof(Texture), true);
				script.lockTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("Lock Texture"), script.lockTexture, typeof(Texture), true);
				
				script.upValue = EditorGUILayout.IntField("Up Value", script.upValue);
				script.downValue = EditorGUILayout.IntField("Down Value", script.downValue);
				if (script.dragDisplayState == MeshButtonState.Drag)
					script.dragValue = EditorGUILayout.IntField("Drag Value", script.dragValue);
				script.lockValue = EditorGUILayout.IntField("Lock Value", script.lockValue);
				break;
			case ButtonStateChangeMode.SetSaturation:	
				script.upValue = EditorGUILayout.IntField("Up Value", script.upValue);
				script.downValue = EditorGUILayout.IntField("Down Value", script.downValue);
				if (script.dragDisplayState == MeshButtonState.Drag)
					script.dragValue = EditorGUILayout.IntField("Drag Value", script.dragValue);
				script.lockValue = EditorGUILayout.IntField("Lock Value", script.lockValue);
				break;
			default:
			case ButtonStateChangeMode.None:
				break;
		}
		EditorGUILayout.Space();
		
		script.sound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Sound"), script.sound, typeof(AudioClip), true);
		
		
        if (GUI.changed){
            EditorUtility.SetDirty (target);
		}
    }

}
