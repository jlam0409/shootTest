// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/UnlitAlphaWithFade" {
	Properties {
		_Color ("Color Tint", Color) = (1,1,1,1)    
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white"{}
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		
		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 

		Pass {
			Lighting Off
			SetTexture [_MainTex] {
				ConstantColor [_Color]
				combine texture * constant
			} 
		}
	}
}

/*
Shader "Unlit/UnlitAlphaWithFade" 
{
    Properties 
    {
        _Color ("Color Tint", Color) = (1,1,1,1)    
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white"
    }

    Category 
    {
        Lighting Off
        ZWrite Off
                //ZWrite On  // uncomment if you have problems like the sprite disappear in some rotations.
        Cull back
        Blend SrcAlpha OneMinusSrcAlpha
                //AlphaTest Greater 0.001  // uncomment if you have problems like the sprites or 3d text have white quads instead of alpha pixels.
        Tags {Queue=Transparent}

        SubShader 
        {

             Pass 
             {
				Cull Back
				SetTexture [_MainTex] 
				{
                    ConstantColor [_Color]
                   Combine Texture * constant
                }
            }
        }
    }
}
*/