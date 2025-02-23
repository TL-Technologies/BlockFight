Shader "Custom/VerticalFogColor"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_FogColor("Fog Color", Color) = (1, 1, 1, 1)
		_FogMaxHeight("Fog Max Height", Float) = 0.0
		_FogMinHeight("Fog Min Height", Float) = -1.0
	}

		SubShader
		{
			Tags { "RenderType" = "Opaque"}
			LOD 200
			CGPROGRAM

			#pragma surface surf Lambert finalcolor:finalcolor vertex:vert
			#pragma multi_compile_fog

			float4 _Color;
			float4 _FogColor;
			float _FogMaxHeight;
			float _FogMinHeight;

			struct Input
			{
				float4 pos;
				UNITY_FOG_COORDS(4)
				float lerpValue;
			};

			void vert(inout appdata_full v, out Input o)
			{
				o.pos = mul(unity_ObjectToWorld, v.vertex);  
				o.lerpValue = clamp((o.pos.y - _FogMinHeight) / (_FogMaxHeight - _FogMinHeight), 0, 1);
				float4 clipPos = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o, clipPos);
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = _Color;
			}

			void finalcolor(Input IN, SurfaceOutput o, inout fixed4 color)
			{
				
#ifndef UNITY_PASS_FORWARDADD
				UNITY_APPLY_FOG(IN.fogCoord, color);
				color.rgb = lerp(_FogColor.rgb, color.rgb, IN.lerpValue);
#endif
			}

			ENDCG
		}
		Fallback "Diffuse"
			
}