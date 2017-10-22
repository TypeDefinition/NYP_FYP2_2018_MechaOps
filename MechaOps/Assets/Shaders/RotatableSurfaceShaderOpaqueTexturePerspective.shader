Shader "Custom/RotateUV Surface Shader (Opaque Textures)" {

	Properties {
		//_varName("Label", type) = defaultValue;
		//Types: 2D, Cube, Color, Range, Float, Int, Vector
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpTex("Normal Map", 2D) = "bump" {}
		_Color("Color", Color) = (1,1,1,1)
		_Brightness("Brightness", Float) = 1
		_RotationSpeed("Rotation Speed", Float) = 0
	}

	SubShader {
		//Tells Unity what we're about to do. In this case we're about to render an opaque texture.
		Tags {
			"RenderType" = "Opaque"
		}

		//Some Level-of-Detail stuff. Not sure how it works.
		LOD 200

		//Tell Unity we are about to use the CGProgramming language to do stuff.
		CGPROGRAM

		//Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard
		//Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			//There's a built in list of possible variables. Use to link and scroll near the bottom of the page.
			//https://docs.unity3d.com/Manual/SL-SurfaceShaders.html
			//The input structure Input generally has any texture coordinates needed by the shader.
			//Texture coordinates must be named “uv” followed by texture name (or start it with “uv2” to use second texture coordinate set).
			float2 uv_MainTex; //Grab the uv of MainTex and store it here in the first uv thingy.
			float2 uv2_MainTex; //Grab the 2nd uv of MainTex and store it here in the first uv thingy.
			float2 uv_BumpTex;
			float2 uv2_BumpTex;

			float4 color : COLOR;
			INTERNAL_DATA float3 viewDir;
			//INTERNAL_DATA float3 worldNormal;
		};

		//Define our properties.
		sampler2D _MainTex; //Our texture.
		sampler2D _BumpTex;
		fixed4 _Color;
		float _Brightness;
		float _RotationSpeed;
		
		//This is our output.
		void surf(Input IN, inout SurfaceOutputStandard o) {
			// Rotate the texture UVs.
			float Deg2Rad = 3.1415926535 / 180.0;
			float sinRotation = sin(_RotationSpeed * _Time.y * Deg2Rad);
			float cosRotation = cos(_RotationSpeed * _Time.y * Deg2Rad);
			float2x2 rotationMatrix = float2x2(cosRotation, -sinRotation, sinRotation, cosRotation);

			float2 uvTranslation = float2(0.5, 0.5);

			// UV Coordinates go from 0 to 1.
			// Therefore before rotation, we need to move the UV back to the orgin.
			IN.uv_MainTex -= uvTranslation;
			// Rotate the UV Coordinates.
			IN.uv_MainTex = mul(IN.uv_MainTex, rotationMatrix);
			// After rotation, we need to move the UV back.
			IN.uv_MainTex += uvTranslation;

			IN.uv2_MainTex -= uvTranslation;
			IN.uv2_MainTex = mul(IN.uv2_MainTex, rotationMatrix);
			IN.uv2_MainTex += uvTranslation;

			IN.uv_BumpTex -= uvTranslation;
			IN.uv_BumpTex = mul(IN.uv_BumpTex, rotationMatrix);
			IN.uv_BumpTex += uvTranslation;

			IN.uv2_BumpTex -= uvTranslation;
			IN.uv2_BumpTex = mul(IN.uv2_BumpTex, rotationMatrix);
			IN.uv2_BumpTex += uvTranslation;

			//It seems like I need to do this BEFORE doing Albedo for some weird reason.
			o.Normal = UnpackNormal(tex2D(_BumpTex, IN.uv_BumpTex));

			//Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * _Color;
			o.Alpha = c.a;
		}

		ENDCG
	}

	FallBack "Diffuse" //What shader to use if the above subshader(s) doesn't work.

}