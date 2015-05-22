using UnityEngine;
using System.Collections;
using System.Text;

/*
 * Description, if you want show up the debug log
 * In the Build Settings dialog there is a check box called "Development Build".
 * In the editor debug log always be true.
 */
public class DebugText : MonoBehaviour
{
    public bool toggleDebug = true;
    
    private Vector2 scrollViewVector = Vector2.zero;
    private GUIStyle debugBoxStyle;
    
    private float debugWidth = 1000.0f;
    private float buttonWidth = 60.0f;
    private float buttonHeight = 30.0f;
    private Rect rectButtonDebugOn;
    private Rect rectButtonClear;  

    private bool showDebug = false;
    private string debugButtonText = "Show";
    private static bool debugIsOn = false;
    private static StringBuilder GUIText = new StringBuilder(5000);

    public bool enableFPSCounter = false;
    public float updateInterval = 0.5f;
    private float accum = 0;
    private int frames = 0;
    private float timeLeft;
    private string fpsString = string.Empty;

    void Start()
    {
        CheckDebug();       
        
        if (debugIsOn)
        {
            debugBoxStyle = new GUIStyle();
            debugBoxStyle.alignment = TextAnchor.UpperLeft;
            rectButtonDebugOn = new Rect(10f, Screen.height - buttonHeight - 5.0f, buttonWidth, buttonHeight);
            rectButtonClear = new Rect(80f, Screen.height - buttonHeight - 5.0f, buttonWidth, buttonHeight);
        }

        useGUILayout = false;

        timeLeft = updateInterval;
    }

    private void CheckDebug()
    {
        if (toggleDebug && Debug.isDebugBuild)
        {
            debugIsOn = true;
        }
        else
        {
            debugIsOn = false;
        }
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeLeft <= 0.0f)
        {
            float fps = accum / frames;
            fpsString = System.String.Format("{0:F2} FPS", fps);

            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    void OnGUI()
    {
        if (debugIsOn)
        {
            if (showDebug)
            {
                GUI.depth = 0;

                GUI.BeginGroup(new Rect(10.0f, 10.0f, debugWidth, Screen.height - 10.0f));
                scrollViewVector = GUI.BeginScrollView(new Rect(0, 0.0f, debugWidth, Screen.height - 65.0f), scrollViewVector, new Rect(0.0f, 0.0f, 400.0f, 2000.0f));
                GUI.Box(new Rect(0, 0.0f, debugWidth - 20.0f, 2000.0f), GUIText.ToString(), debugBoxStyle);
                GUI.EndScrollView();
                GUI.EndGroup();
            }

            if (GUI.Button(rectButtonDebugOn, debugButtonText))
            {
                if (showDebug)
                {
                    showDebug = false;
                    debugButtonText = "Show";
                }
                else
                {
                    showDebug = true;
                    debugButtonText = "Hide";
                }
            }

            if (GUI.Button(rectButtonClear, "Clear"))
            {
                GUIText.Remove(0, GUIText.Length);
            }
        }

        if (enableFPSCounter)
        {
            GUI.Label(new Rect(Screen.width - 100, 10, 100, 100), fpsString, debugBoxStyle);
        }
    }

    public static void Log(string debugString)
    {
        if (debugIsOn)
        {
            GUIText.Insert(0, debugString + "\n");            
        }

        if (Debug.isDebugBuild)
        {
            Debug.Log(debugString );
        }
    }

    public static void Log(string debugString, Object context)
    {
        if (debugIsOn)
        {
            GUIText.Insert(0, debugString + "\n");            
        }

        if (Debug.isDebugBuild)
        {
            Debug.Log(debugString, context);
        }
    }

    public static void LogWarning(string debugString)
    {
        if (debugIsOn)
        {
            GUIText.Insert(0, "*W*" + debugString + "\n");            
        }

        if (Debug.isDebugBuild)
        {
            Debug.LogWarning(debugString);
        }
    }

    public static void LogWarning(string debugString, Object context)
    {
        if (debugIsOn)
        {
            GUIText.Insert(0, "*W*" + debugString + "\n");            
        }

        if (Debug.isDebugBuild)
        {
            Debug.LogWarning(debugString, context);
        }
    }

    public static void LogError(string debugString)
    {
        if (debugIsOn)
        {
            GUIText.Insert(0, "*E*" + debugString + "\n");            
        }

        if (Debug.isDebugBuild)
        {
            Debug.LogError(debugString);
        }
    }

    public static void LogError(string debugString, Object context)
    {
        if (debugIsOn)
        {
            GUIText.Insert(0, "*E*" + debugString + "\n");            
        }

        if (Debug.isDebugBuild)
        {
            Debug.LogError(debugString, context);
        }
    }
}
