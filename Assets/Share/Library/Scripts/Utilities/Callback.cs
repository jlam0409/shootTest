using UnityEngine;
using System.Collections;
using System;				// for Action

[System.Serializable]
public class Callback {
	// Message callback
	public GameObject callbackListener;
	public string callbackFunction;
	public string callbackArgument;	
	public MonoBehaviour callbackSourceScript;
	
	// Action callback
	public Action callbackAction;

	public Callback(Action callback){
		callbackAction = callback;
	}
	
	
	public Callback (Callback callback){
		callbackSourceScript = callback.callbackSourceScript;
		callbackListener = callback.callbackListener;
		callbackFunction = callback.callbackFunction;
		callbackArgument = callback.callbackArgument;
		callbackAction = callback.callbackAction;
	}
	
	public Callback(GameObject listener, string function){
		callbackListener = listener;
		callbackFunction = function;
		callbackArgument = null;	
		
		//Callback(listener, function, "");
	}
	
	public Callback(GameObject listener, string function, string argument){
//		callbackSourceScript = script;
		callbackListener = listener;
		callbackFunction = function;
		callbackArgument = argument;
	}
	
	public void Reset(){
		callbackListener = null;
		callbackFunction = null;
		callbackArgument = null;
	}
	/*
	public void SendCallback(){
		ExecSendCallback (callbackListener, callbackFunction, callbackArgument);
	}
	*/
	public void SendCallback(MonoBehaviour script){
		callbackSourceScript = script;
			
		if(callbackListener)															
			ExecSendCallback (callbackListener, callbackFunction, callbackArgument);
		else 
			if(callbackAction != null) 
				ExecSendCallback(callbackAction);
	}
	
	// runs callback method of type Action
	private void ExecSendCallback(Action callback)
	{
		callback();		
	}
	
	private void ExecSendCallback(GameObject listener_, string command_, string commandArgument_){
		if(listener_ != null){
			if (commandArgument_ != null && commandArgument_ != ""){
				switch (commandArgument_){
					case "this":	
						listener_.SendMessage (command_, callbackSourceScript);	
					break;
					case "gameObject":
					case "GameObject":	
						listener_.SendMessage (command_, callbackSourceScript.gameObject);	
					break;
					case "true":
						listener_.SendMessage (command_, true);
					break;
					case "false":
						listener_.SendMessage (command_, false);
					break;
					default: 
						listener_.SendMessage (command_, commandArgument_);		
					break;
				}			
			}else {
				listener_.SendMessage (command_);
			}
		}
	}
}
