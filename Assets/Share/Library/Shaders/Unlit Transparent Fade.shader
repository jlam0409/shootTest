// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color
// - no culling (double sided)

Shader "Unlit/Transparent Fade" {
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
			AlphaTest Greater 0
			SetTexture [_MainTex] {
				ConstantColor [_Color]
				combine texture * constant
			} 
		}
	}
}
