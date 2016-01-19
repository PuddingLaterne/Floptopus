Shader "Custom/SeeThrough" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Emission("Emission", Range(0,1)) = 0.0
		_EmissionTex("Emission (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
		_Silhoutte("Silhoutte Colour", Color) = (1, 1, 1, 1)
	}
	SubShader 
	{

		Pass
		{
			Name "Behind"
            Tags { "RenderType"="transparent" "Queue" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Greater               // here the check is for the pixel being greater or closer to the camera, in which
            Cull Back                   // case the model is behind something, so this pass runs
            ZWrite Off
            LOD 200                    
               
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
               
            struct v2f 
			{
                float4 pos : SV_POSITION;
            };
               
            float4 _Silhoutte;
                             
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;                                               
            }
               
            half4 frag (v2f i) : COLOR
            {
                return _Silhoutte;
            }
            ENDCG
		}

		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _EmissionTex;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_EmissionTex;
			float3 viewDir;
		};

		float4 _RimColor;
		float _RimPower;
		half _Glossiness;
		half _Metallic;
		half _Emission;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Emission = tex2D(_EmissionTex, IN.uv_EmissionTex).rgb * _Emission;
			o.Occlusion = tex2D(_EmissionTex, IN.uv_EmissionTex) * _Emission;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
