// Update Date 8 May 2012
Shader "Transparent/VertexLitWithZ" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader {
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
		// Render into depth buffer only
//		Pass {
//			ColorMask 0
//		}
		// Render normally
		Pass {
			ZWrite On
			AlphaTest Greater 0
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Material {
				Diffuse [_Color]
				Ambient [_Color]
			}
			Lighting On
			SetTexture [_MainTex] {
				Combine texture * primary DOUBLE, texture * primary
			}
		}
	}
}
