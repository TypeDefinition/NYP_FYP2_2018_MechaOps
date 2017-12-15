Shader "Custom/Unlit Texture No Depth (Opaque)"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags
        {
            "RenderType"="Opaque"
            "Queue"="Overlay+1"
        }
		LOD 100

		Pass
		{
			ZTest Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
            // Allow Instancing
            #pragma multi_compile_instancing
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

            sampler2D _MainTex; //Our texture.
			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}