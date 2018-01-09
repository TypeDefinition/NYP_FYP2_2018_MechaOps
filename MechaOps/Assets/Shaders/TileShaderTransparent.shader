Shader "Custom/TileShader (Transparent)"
{
	Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpTex("Normal Map", 2D) = "bump" {}

		_Smoothness ("Smoothness", Range(0,1)) = 1.0
		_Metallic ("Metallic", Range(0,1)) = 0.0

        _CanSee ("Can See", Int) = 1
        _SeenColor("Seen Color", Color) = (1, 1, 1, 1)
        _UnseenColor ("Unseen Color", Color) = (0.3, 0.3, 0.3, 1)

        // Rotate The Texture
        _TextureRotationSpeed("Texture Rotation Speed (Degrees)", Float) = 0
        // Offset The Texture
        _TextureOffSetSpeedU("Texture Offset Speed U", Float) = 0
        _TextureOffSetSpeedV("Texture Offset Speed V", Float) = 0
	}

    SubShader
    {
		Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
		LOD 200

        // This will write 2 to the stencil buffer when it passes.
        // It is set to always pass.
        Stencil
        {
            Ref 2 // This is a 8 bit integer. Let's use 2 for our tiles.
            Comp Always
            Pass replace
        }

        // Blend SrcAlpha, OneMinusSrcAlpha
        // Blend Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:blend

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

        // Allow Instancing
        #pragma multi_compile_instancing

		struct Input
        {
            float2 uv_MainTex; //Grab the uv of MainTex and store it here in the first uv thingy.
            float2 uv_BumpTex;
		};

        sampler2D _MainTex; //Our texture.
        sampler2D _BumpTex;

		half _Smoothness;
		half _Metallic;
		fixed4 _Color;

        half _TextureRotationSpeed;
        half _TextureOffSetSpeedU;
        half _TextureOffSetSpeedV;

        fixed4 _SeenColor;
        fixed4 _UnseenColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
            // UNITY_DEFINE_INSTANCED_PROP(half, _Smoothness)
            // UNITY_DEFINE_INSTANCED_PROP(half, _Metallic)
            // UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(int, _CanSee)
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Rotate the texture UVs.
            half Deg2Rad = 3.1415926535 / 180.0;
            half sinRotation = sin(_TextureRotationSpeed * _Time.y * Deg2Rad);
            half cosRotation = cos(_TextureRotationSpeed * _Time.y * Deg2Rad);
            half2x2 rotationMatrix = half2x2(cosRotation, -sinRotation, sinRotation, cosRotation);

            half2 uvTranslation = half2(0.5, 0.5);

            // UV Coordinates go from 0 to 1.
            // Therefore before rotation, we need to move the UV back to the orgin.
            IN.uv_MainTex -= uvTranslation;
            IN.uv_MainTex = mul(IN.uv_MainTex, rotationMatrix); // Rotate the UV Coordinates.
            IN.uv_MainTex += uvTranslation; // After rotation, we need to move the UV back.

            IN.uv_BumpTex -= uvTranslation;
            IN.uv_BumpTex = mul(IN.uv_BumpTex, rotationMatrix);
            IN.uv_BumpTex += uvTranslation;

            // Now that we've rotated the texture, let's offset it.
            IN.uv_MainTex += half2(_TextureOffSetSpeedU * _Time.y, _TextureOffSetSpeedV * _Time.y);
            IN.uv_BumpTex += half2(_TextureOffSetSpeedU * _Time.y, _TextureOffSetSpeedV * _Time.y);

            //It seems like I need to do this BEFORE doing Albedo for some weird reason.
            o.Normal = UnpackNormal(tex2D(_BumpTex, IN.uv_BumpTex));

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
            if (UNITY_ACCESS_INSTANCED_PROP(_CanSee) != 0)
            {
                c = c * _SeenColor;
            }
            else
            {
                c = c * _UnseenColor;
            }

            o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
