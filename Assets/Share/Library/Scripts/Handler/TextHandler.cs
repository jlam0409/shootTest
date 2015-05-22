using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// How to use
/*
Normal set text with boundary width, if the maxLines is defined, the lines exceed the maxLine will be pruned.
TextHandler.SetText (TextMesh/exSpriteFont mesh, string context);
TextHandler.SetText (TextMesh/exSpriteFont mesh, string context, float/GameObject, [int maxLines]);

TextHandler.SetTextFitWidth (TextMesh/exSpriteFont mesh, string context, float/GameObject);
TextHandler.SetTextFitWidth (TextMesh/exSpriteFont mesh, stringcontext, GameObject, float scaleFactor, [SetTextFitOption=ONLYDOWN] ); 
*/
	
public enum SetTextFitOption{
	UPORDOWN,
	ONLYUP,
	ONLYDOWN
}

public static class TextHandler {
	private static Dictionary<char, float> charSizeDict = new Dictionary<char, float>();
	
	public static int SetText (TextMesh textMesh, string meshText, float wrapWidth=0f, int maxLines = -1, int startLine=0, bool showDots=false){
		int returnLines = 1;
		if (wrapWidth == 0f){
			textMesh.text = meshText;	
		} else {
			CacheCharacterSize (textMesh);
			//int lines = 1;
			int lines = GetParagraphBound (wrapWidth, ref meshText);
			if (maxLines < 0){
				returnLines = lines;
				textMesh.text = meshText;
			} else {
				returnLines = lines;
				meshText = TruncateText (meshText, maxLines, startLine, showDots);
				textMesh.text = meshText;
			}
		}
		return returnLines;
	}

	// overload function to support gameObject as width bounday
	public static int SetText (TextMesh textMesh, string meshText, GameObject wrapObject, int maxLines = -1, int startLine=0, bool showDots=false){
		if (wrapObject == null){
			return SetText (textMesh, meshText, 0f, maxLines, startLine, showDots);
		} else {
			float wrapWidth = GetGameObjectWidth (wrapObject);
			return SetText (textMesh, meshText, wrapWidth, maxLines, startLine, showDots);	
		}
	}

	public static float SetTextFitWidth (TextMesh textMesh, string meshText, float wrapWidth=0f, SetTextFitOption option=SetTextFitOption.ONLYDOWN){
		float scaleRatio = 1f;
		if (wrapWidth == 0f){
			textMesh.text = meshText;
		} else {
			CacheCharacterSize (textMesh);
			textMesh.text = meshText;
			
			float currentWidth = GetLongestWidth(meshText);	
			if (option == SetTextFitOption.ONLYUP){
				if (currentWidth < wrapWidth){
					scaleRatio = wrapWidth / currentWidth;
				}
			} else if (option == SetTextFitOption.ONLYDOWN){
				if (currentWidth > wrapWidth){
					scaleRatio = wrapWidth / currentWidth;	
				}
			} else {
				if (currentWidth != wrapWidth){ // have to scale up/down according to the ratio
					scaleRatio = wrapWidth / currentWidth;
				}	
			}
			if (scaleRatio != 1f){
		        Vector3 currentScale = textMesh.transform.localScale;
				Vector3 newScale = currentScale * scaleRatio;
				textMesh.transform.localScale = newScale;
			}
		}
		return scaleRatio;
	}
	
	// overload function
	public static float SetTextFitWidth (TextMesh textMesh, string meshText, GameObject wrapObject, float scaleFactor=1f, SetTextFitOption option=SetTextFitOption.ONLYDOWN){
		if (wrapObject == null){
			return SetTextFitWidth (textMesh, meshText, 0f);
		} else {
			float wrapWidth = GetGameObjectWidth (wrapObject) * scaleFactor;
			return SetTextFitWidth (textMesh, meshText, wrapWidth, option);	
		}
	}

	private static string TruncateText (string meshText, int maxLines, int startLine, bool showDots=true){
		string[] meshTextTok = meshText.Split('\n');
		string returnText = "";
//		for (int i=0; i<meshTextTok.Length; i++){
		int counter = 1;
		for (int i=startLine; i<meshTextTok.Length; i++){
			if (counter > maxLines)
				break;
			
			if (i != maxLines-1){
//			if (i != counter-1){
				// append current line if it is not the max line
				returnText += meshTextTok[i] + "\n";
			} else {
				// max line, check if there's any more lines cannot display, if yes, replace last 3 characters with "..."
				if (meshTextTok.Length > maxLines){
					// if the next line is null line, not do the replacement
					if (showDots){
						if (meshTextTok[i+1] != "" || meshTextTok[i+1].Length != 0){
							returnText += meshTextTok[i].Substring (0, meshTextTok[i].Length-2) + "...";
						} else {
							returnText += meshTextTok[i] + "\n";
						}
					} else {
						returnText += meshTextTok[i] + "\n";
					}
				} else {
					returnText += meshTextTok[i] + "\n";
				}
				
				break;
			}
			counter ++;
		}
		return returnText;
	}

	private static void CacheCharacterSize(TextMesh sample){
		TextMesh dummy = UnityEngine.Object.Instantiate (sample) as TextMesh;//, Vector3.zero, Quaternion.identity) as TextMesh;
		GameObject dummyObject = dummy.gameObject;
		Renderer dummyRenderer = dummy.renderer;
		
		Transform dummyXform = dummyObject.transform;
		Transform sampleXform = sample.transform;
		
		dummyXform.parent = sampleXform.parent;
		dummyXform.localPosition = sampleXform.localPosition;
		dummyXform.localEulerAngles = sampleXform.localEulerAngles;
		dummyXform.localScale = sampleXform.localScale;
		
		List<char> charList = new List<char>();
		foreach (char eachChar in sample.text.ToCharArray()){
			if (!charList.Contains(eachChar)){
				charList.Add (eachChar);
			}
		}
		
		for (int i=0; i<charList.Count; i++){
			char c = charList[i];
			dummy.text = c.ToString();
			if (charSizeDict.ContainsKey (c)){
				charSizeDict[c] = dummyRenderer.bounds.size.x;
			} else {
				charSizeDict.Add(c,dummyRenderer.bounds.size.x);
			}	
		}
		
		// cache A-Z characters size
		for (int i=33; i <=126; i++){
    		char c = Convert.ToChar(i);
			if (!char.IsControl(c)){
				dummy.text = c.ToString();
				if (charSizeDict.ContainsKey (c)){
					charSizeDict[c] = dummyRenderer.bounds.size.x;
				} else {
					charSizeDict.Add(c,dummyRenderer.bounds.size.x);
				}
			}
		}
		
		
//		int l = sample.font.characterInfo.Length;
//		Debug.Log ("sample.font.characterInfo.Length: " + l);
//		Debug.Log ("sample.font.characterInfo[Length-1]: " + sample.font.characterInfo[l-1].width);
		
		// special handle character - space: " "
		dummy.text = "a a";
		float dummyWidth = dummyRenderer.bounds.size.x;
		dummyWidth -= (charSizeDict['a']*2);
		if (charSizeDict.ContainsKey (' ')){
			charSizeDict[' '] = dummyWidth;
		} else {
			charSizeDict.Add (' ', dummyWidth);
		}

		UnityEngine.Object.Destroy (dummyObject);
	}

	private static float GetStringSize(string queryText) {
		float width = 0f;
		char[] allChar = queryText.ToCharArray();
		foreach (char eachChar in allChar) {
			if (!charSizeDict.ContainsKey (eachChar)) {
				DebugText.Log ("eachChar: " + eachChar + " id: " + Convert.ToInt32(eachChar) + " in string: " + queryText+" is not valid!");
				continue;
			}
			float eachWidth = charSizeDict[eachChar];
			width += eachWidth;
		}
		return width;
	}
	
	private static float GetLongestWidth(string meshText){
		float width = 0f;
		string[] meshTextTok = meshText.Split('\n');
		
		foreach (string eachLine in meshTextTok){
			float buffer = GetStringSize ( eachLine	);
			if (buffer > width){
				width = buffer;
			}
		}
		return width;
	}
	
	private static float GetGameObjectWidth(GameObject boundary){
		Renderer boundaryRenderer = boundary.renderer;
		return boundaryRenderer.bounds.size.x;
	}

	// core of the word wrap, it have two main features:
	// 1. if the word length is longer than the boundaryWidth, the word will split automatically
	// 2. if the word exceed the current line, it will goes to the next line automatically
	// 3. handle the paragragh 
	private static int GetParagraphBound(float boundaryWidth, ref string currentText) {		
		List<string> paragraph = new List<string>();
		
		float spaceWidth = GetStringSize(" ");
		string[] paragraphTok = currentText.Split ('\n');
		int line = 0;
		for (int p=0; p<paragraphTok.Length; p++){
			if (paragraphTok[p] == ""){
				paragraph.Add ("\n");
				line++;
				continue;	
			}

			string[] currentTextTok = paragraphTok[p].Split(' ');	
			//paragraph.Add (currentTextTok[0]);
			//float currentLineLength = GetStringSize(currentTextTok[0]);
			float currentLineLength = 0;
			for (int i=0; i<currentTextTok.Length; i++){
				float buffer = GetStringSize(currentTextTok[i]);
				if (buffer > boundaryWidth){	// the word is too long already, need to split the word
					
					string bufferString = currentTextTok[i];
					string splitString = "";
					string remainString = "";
					do {
						remainString = SplitWord (boundaryWidth, ref bufferString);
						splitString += bufferString;
						if (remainString != ""){
							splitString += "\n";
							bufferString = remainString;
						}
					}
					while (remainString != "");
						
					// append the paragraph with the split string
					string[] splitStringTok = splitString.Split('\n');
					
					// if the currentLineLength is not zero, then it means we have to increment the line id
					if (currentLineLength != 0f){
						//DebugText.Log ("Current Line full, switch to the next line");
						paragraph[line] += " \n";
						line++;
					}
					
					paragraph.Add (splitStringTok[0]);
					currentLineLength = GetStringSize(splitStringTok[0]);
					
					for (int w =1; w< splitStringTok.Length; w++){
						//DebugText.Log ("Current Line full, switch to the next line");
						paragraph[line] += "\n";
						line++;
						paragraph.Add (splitStringTok[w]);
						currentLineLength = GetStringSize(splitStringTok[w]);
					}
				} else {
					if (paragraph.Count <= line){	// first word and not exceed the width
						//DebugText.Log ("line: " + line + " first word in the line: " + currentTextTok[i]);
						paragraph.Add ( currentTextTok[i] );
						currentLineLength += buffer;
					} else {
						if (currentLineLength + spaceWidth + buffer < boundaryWidth){ // append current line
							//DebugText.Log ("line: " + line +" existing line: " + paragraph[line] + " add word: " + currentTextTok[i] );
							paragraph[line] += " " + currentTextTok[i];
							currentLineLength += (spaceWidth + buffer);
						} else { // current line is full, append to next line
							//DebugText.Log ("Current Line full, switch to the next line");
							paragraph[line] += " \n";
							line++;
							currentLineLength = 0f;
							// redo the word
							i--;
						}
					}
				}
			}
			
			if (p != paragraphTok.Length-1) {
				paragraph[line] += "\n";
			}
			line++;
		}
		
		// convert paragraph to string
		string finalString  = "";
		foreach (string eachLine in paragraph){
			finalString += eachLine;	
		}
		currentText = finalString;
		return (line+1);
	}
	
	private static string SplitWord (float boundaryWidth, ref string currentText){
		char[] allCharacters = currentText.ToCharArray();
		
		float currentWordLength = GetStringSize(allCharacters[0].ToString() );		
		string startString = allCharacters[0].ToString();
		string endString = "";
		
		int i = 1;
		bool full = false;		
		for (i=1; i<allCharacters.Length; i++){
			float buffer = GetStringSize(allCharacters[i].ToString());
			if (currentWordLength + buffer < boundaryWidth){ // append current string
				currentWordLength += buffer;
				startString += allCharacters[i];
			} else { // current string is full, append to next string
				full = true;
				break;
			}
		}
		
		if (full){
			for (int j=i; j<allCharacters.Length; j++){
				endString += allCharacters[j];
			}
		}
				
		currentText = startString;
		return endString;
	}
}
