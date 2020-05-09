
Shader "Face/FaceShaderNoise"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DisplaceTex ("Texture", 2D) = "white" {}		
		_Offset ("Offset", Vector) = (0,0,1,1)
		_Ratio ("Ratio", Range (0.0,1.0))= 3.0
		_Glossiness ("Smoothness", Range(0,10)) = 2.22
		_Occlusion  ("Occlusion" , Range(0, 10)) = 9.98
		_Metallic ("Metallic", Range(0,1)) = 0.3

	}
	
	SubShader
	{
	Pass
	{
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma target 3.0
	#include "./noise/SimplexNoise3D.hlsl"

	#pragma multi_compile_fwdbase

	#include "HLSLSupport.cginc"
    #include "UnityShaderVariables.cginc"
    #include "UnityShaderUtilities.cginc"
    #include "UnityCG.cginc"
    #include "Lighting.cginc"
    #include "UnityPBSLighting.cginc"
    #include "AutoLight.cginc"

	
	struct appdata_t
	{
		float4 vertex : POSITION;
		float4 color : COLOR;
		float2 texcoord : TEXCOORD0;
	};
	
	struct v2f
	{
		float4 vertex : POSITION;
		float4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float2 screenpos : TEXCOORD1;
	};

	//surface shader usage
	struct Input {
		float2 uv_MainTex;
	};

	struct v2f_surf {
        UNITY_POSITION(pos);
        float2 pack0 : TEXCOORD0;
        float3 worldNormal : TEXCOORD1;
        float3 worldPos : TEXCOORD2;
        UNITY_SHADOW_COORDS(5)
    };
	
	sampler2D _MainTex;
	sampler2D _DisplaceTex;
	
	
	float4x4 _displayMatrix;
	float4x4 _faceMatrix;

	float4 _Offset;
	float _Ratio;

	half _Glossiness;
	half _Occlusion;
	half _Metallic;
    fixed4 _Color;
    float _FRG;

    float _Volume;

    inline void Negative(inout float2 uv, inout float4 color)
    {
        color.rgb = 1 - color.rgb;
    }

	// vertex shader

		float mirrored(float v) {
			float m = fmod(v, 2.0);
			return lerp(m, 2.0 - m, step(1.0, m));
		}

	v2f vert(appdata_t IN)
	{
		v2f o;

		float4 v = IN.vertex;

		float4 deform = tex2Dlod (_DisplaceTex, float4(IN.texcoord.xy,0,0));
		float ratio = _Ratio * deform *_Volume * 2;
		v.x = v.x  +  0.5 * ratio * snoise(v*10.0 + _Time.y + v.y*5) ;
		v.z = v.z - ratio * 0.2 + ratio * 0.2 * snoise( v * 10.0 + _Time.y);

		o.vertex = UnityObjectToClipPos(v);
		o.texcoord = IN.texcoord;
		o.color = IN.color;

		float4 facePosWorld = mul(_faceMatrix,IN.vertex);
		float4 pos = mul( UNITY_MATRIX_VP, facePosWorld );
		float4 hoge = ComputeScreenPos(pos);
		float2 uv = hoge.xy/hoge.w;
		
		o.screenpos = uv;
		
		return o;
	}

			//surface shader
			void surf (Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = 2.05;
			}


	
	// fragment shader
	fixed4 frag(v2f IN) : SV_Target
	{
		SurfaceOutputStandard o;
		UNITY_INITIALIZE_OUTPUT(SurfaceOutputStandard, o);
		Input input;
		input.uv_MainTex = IN.texcoord;
		o.Emission = 0.0;
		o.Occlusion = 9.98;
		surf(input,o);


		float2 xy = IN.screenpos.xy;
		half4 c = tex2D(_MainTex, xy);// * IN.color;

		float2 uv = IN.texcoord;
		
		if (_FRG == 1.0) { Negative(uv, c); }//ポジネガ

		return c;
	}
	

	ENDCG
	}
	}


}
