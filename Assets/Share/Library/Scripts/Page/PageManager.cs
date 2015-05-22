using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PageManager {
	public static List<Page> allPage = new List<Page>();
	public static List<Page> allPopupPage = new List<Page>();
	
	public static Page currentPage;
	private static Page currentPopupPage;
	private static Page loadingPage;
	private static Dictionary<Page, PageScript> managerScripts = new Dictionary<Page, PageScript>();
	
	public static Vector3 inPosition;
	public static Vector3 outPosition = new Vector3(0,0,10);
	
	public static void InitPage() {
		foreach(Page eachPage in allPage) {
			eachPage.SetPageState(PageState.OUT);	
		}
		foreach(Page eachPage in allPopupPage) {
			eachPage.SetPageState(PageState.OUT);	
		}
		if (loadingPage != null) {
			loadingPage.SetPageState(PageState.IN);
		}
	}
	
	public static void SetLoadingPage(PageName page){
		loadingPage = GetPage(page);
	}
	
	public static void SetLoadingPage(string pageName) {
		loadingPage = GetPage(pageName);
	}
	
	public static void AddPage(Page page) {
		allPage.Add (page);
	}
	
	public static void AddPopupPage(Page page) {
		allPopupPage.Add(page);	
	}
	
	public static void RemovePage(Page page){
		allPage.Remove(page);	
	}
	
	public static void GoToPage(PageName pageName){
//		Debug.Log ("GotoPage PageName called, pageName: " + pageName.ToString());
		if (loadingPage != null) {
			loadingPage.SetPageState(PageState.IN);
		}
		
		if (currentPage != null){
			currentPage.SetPageState (PageState.OUT);
			CallPageManagerScriptDestroy(currentPage);
		}
		
		currentPage = GetPage (pageName);
		
		if (currentPage != null) {
			CallPageManagerScriptInit(currentPage);
			if (loadingPage != null) {
				loadingPage.SetPageState(PageState.OUT);
			}
			currentPage.SetPageState (PageState.IN);
		} else {
			Debug.Log (pageName + " page cannot be found!");
		}
	}
	
	public static void GoToPage(string pageName) {
		if (loadingPage != null) {
			loadingPage.SetPageState(PageState.IN);
		}
		
		if (currentPage != null){
			currentPage.SetPageState (PageState.OUT);
			CallPageManagerScriptDestroy(currentPage);
		}
		
		currentPage = GetPage (pageName);
		
		if (currentPage != null) {
			CallPageManagerScriptInit(currentPage);
			if (loadingPage != null) {
				loadingPage.SetPageState(PageState.OUT);
			}
			currentPage.SetPageState (PageState.IN);
			
		} else {
			Debug.Log (pageName + " page cannot be found!");
		}
	}
	
	private static void CallPageManagerScriptDestroy(Page page){
		PageScript managerScript = page.gameObject.GetComponent<PageScript>() as PageScript;
		if (managerScript != null){
			managerScript.PageOut();
		}
	}
	
	private static void CallPageManagerScriptInit(Page page) {
		if (managerScripts.ContainsKey(page)) {
			managerScripts[page].Init();
			managerScripts[page].PageStart();
		} else {
			PageScript managerScript = page.gameObject.GetComponent<PageScript>() as PageScript;
			if (managerScript != null) {
				managerScripts.Add(page, managerScript);
				
				managerScript.Init();
				managerScript.PageStart();
			}
		}
	}
	
	private static Page GetPage(PageName pageName){
		foreach (Page eachPage in allPage){
			if (eachPage.pageName == pageName){
				return eachPage;
			}
		}
		
		foreach (Page eachPopupPage in allPopupPage){
			if (eachPopupPage.pageName == pageName){
				return eachPopupPage;	
			}
		}
		return null;	
	}
	
	private static Page GetPage(string pageName) {
		foreach (Page eachPage in allPage){
			if (eachPage.ToString().ToLower() == pageName.ToLower()){
				return eachPage;
			}
		}
		
		foreach (Page eachPopupPage in allPopupPage){
			if (eachPopupPage.ToString().ToLower() == pageName.ToLower() ){
				return eachPopupPage;
			}
		}
		return null;
	}
	
	public static void PopupPage ( string pageName ){
		currentPopupPage = GetPage (pageName);
		currentPopupPage.SetPageState (PageState.IN);
	}
	
	public static void PopoutPage ( ){
		if (currentPopupPage != null)
			currentPopupPage.SetPageState (PageState.OUT);
		currentPopupPage = null;
	}
}
