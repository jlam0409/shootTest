// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Texture Double" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off
		Pass {
			Lighting Off
			SetTexture [_MainTex] { combine texture } 
		}
	}
}
