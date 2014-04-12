Shader "Shaders/Cloth"
{
Properties {
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "black" {}
	_Wind("Wind",Vector) = (1,1,1,1)
	_Cloth_wind("Cloth_wind", float) = 0.5
	_Cloth_wind_Scale("Cloth_wind_Scale",float) = 0.5
}

	SubShader 
	{
		Tags
		{
"Queue"="Transparent"
"IgnoreProjector"="False"
"RenderType"="Transparent"

		}
	
	
	Cull Off
	ZWrite On
	ZTest LEqual
	ColorMask RGBA
	Fog{
	}
	
	
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "TerrainEngine.cginc"
	sampler2D _MainTex;
	float4 _MainTex_ST;
	samplerCUBE _ReflTex;
	
	#ifndef LIGHTMAP_OFF
	//float4 unity_LightmapST;
	//sampler2D unity_Lightmap;
	#endif
	
	float _Cloth_wind;
	float _Cloth_wind_Scale;

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		#ifndef LIGHTMAP_OFF
		float2 lmap : TEXCOORD1;
		#endif
		fixed3 spec : TEXCOORD2;
	};

inline float4 AnimateVertex2(float4 pos, float3 normal, float4 animParams,float4 wind,float2 time)
{	
	
	float fDetailAmp = 0.1f;
	float fBranchAmp = 0.3f;
	
	
	float fObjPhase = dot(_Object2World[3].xyz, 1);
	float fBranchPhase = fObjPhase + animParams.x;
	
	float fVtxPhase = dot(pos.xyz, animParams.y + fBranchPhase);
	
	
	float2 vWavesIn = time  + float2(fVtxPhase, fBranchPhase );
	
	
	float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0);
	
	vWaves = SmoothTriangleWave( vWaves );
	float2 vWavesSum = vWaves.xz + vWaves.yw;

	
	float3 bend = animParams.y * fDetailAmp * normal.xyz;
	bend.y = animParams.w * fBranchAmp;
	pos.xyz += ((vWavesSum.xyx * bend) + (wind.xyz * vWavesSum.y * animParams.w)) * wind.w; 

	
	pos.xyz += animParams.z * wind.xyz;
	
	return pos;
}


	
	v2f vert (appdata_full v)
	{
		v2f o;
		
		float4	wind;
		
		float			bendingFact	= v.color.a;
		
		wind.xyz	= mul((float3x3)_World2Object,_Wind.xyz);
		wind.w		= _Wind.w  * bendingFact;
		
		
		float4	windParams	= float4(0,_Cloth_wind,bendingFact.xx);
		float 		windTime 		= _Time.y * float2(_Cloth_wind_Scale,1);
		float4	mdlPos			= AnimateVertex2(v.vertex,v.normal,windParams,wind,windTime);
		
		o.pos = mul(UNITY_MATRIX_MVP,mdlPos);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		
		o.spec = v.color;
		
		#ifndef LIGHTMAP_OFF
		//o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		#endif
		
		return o;
	}
	ENDCG


	Pass {
		CGPROGRAM
		#pragma debug
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest		
		fixed4 frag (v2f i) : COLOR
		{
			fixed4 tex = tex2D (_MainTex, i.uv);
			
			fixed4 c;
			c.rgb = tex.rgb;
			c.a = tex.a;
			
			#ifndef LIGHTMAP_OFF
			//fixed3 lm = DecodeLightmap (tex2D(unity_Lightmap, i.lmap));
			//c.rgb *= lm;
			#endif
			
			return c;
		}
		
		ENDCG 
	}	
}
}


