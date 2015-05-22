// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color
// - no culling (double sided)

Shader "Unlit/Transparent Double" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		Blend SrcAlpha OneMinusSrcAlpha 
		LOD 100
		ZWrite Off
		Cull Off
		Pass {
			Lighting Off
			SetTexture [_MainTex] { combine texture } 
		}
	}
}

