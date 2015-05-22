// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color
// - no culling (double sided)

Shader "Unlit/Transparent Z Fade For Shadow" {
	Properties {
		_Color ("Color Tint", Color) = (1,1,1,1)    
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white"{}
		_Cutoff ("Alpha cutoff", Range (0,1)) = 0
	}

	SubShader {
		Tags {"Queue"="Transparent+10" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		
		ZWrite On
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha 

		Pass {
			Lighting Off
			AlphaTest Greater [_Cutoff]
			SetTexture [_MainTex] {
				ConstantColor [_Color]
				combine texture * constant
			} 
		}
	}
}
