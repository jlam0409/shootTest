// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color
// - no culling (double sided)

Shader "Unlit/Transparent Double Z" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range (0,1)) = 0.5
	}
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		
		//Blend One Zero, OneMinusSrcAlpha SrcAlpha
		LOD 100
		ZWrite On
		Lighting Off
//		SeparateSpecular On
		Pass {
			AlphaTest Greater [_Cutoff]
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			SetTexture [_MainTex] { combine texture } 
		}
		
//		Pass {
//			Cull Back
//			Blend SrcAlpha OneMinusSrcAlpha
//			SetTexture [_MainTex] { 
//				combine texture 
//			} 
//		}
	}
}

