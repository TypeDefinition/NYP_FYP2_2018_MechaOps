// simple "dissolving" shader by genericuser (radware.wordpress.com)
// clips materials, using an image as guidance.
// use clouds or random noise as the slice guide for best results.
Shader "Custom Shaders/Dissolving" {
	Properties {
		_MainTex ("Texture (RGB)", 2D) = "white" {}
		_Noise ("Noise", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)

	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM

		#pragma surface surf Lambert addshadow
		struct Input {
		float2 uv_MainTex;
		float2 uv_Noise;

		};
		sampler2D _MainTex;
		sampler2D _Noise;
		half4 _Color;
		void surf (Input IN, inout SurfaceOutput o) {

			half4 noise = tex2D (_Noise, IN.uv_Noise);
			half s = (_SinTime[2] + 1) / 2;
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			half cut = abs(noise.r - s);
			if(cut < 0.03) {
				o.Emission = o.Albedo * _Color * 2;
		}

		}
		ENDCG
	} 
	Fallback "Diffuse"
}