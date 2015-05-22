// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color
// - no culling (double sided)

Shader "VertexLit/Transparent Double Z" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,0.5)
		_SpecColor ("Spec Color", Color) = (1,1,1,1)
		_Emission ("Emmisive Color", Color) = (0,0,0,0)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.7
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		LOD 100
		ZWrite On
		Pass {
			AlphaTest Greater 0
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ColorMask RGB
			Material {
				Diffuse [_Color]
				Ambient [_Color]
				Shininess [_Shininess]
				Specular [_SpecColor]
				Emission [_Emission]
			}
			Lighting On
			SetTexture [_MainTex] {
				constantColor [_Color]
				Combine texture * primary DOUBLE, texture * constant
			}
		}
	}
}

