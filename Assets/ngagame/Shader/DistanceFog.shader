Shader "Custom/DistanceFog"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_FogColor("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
		_ExampleName("Start vector", Vector) = (0, 0, 0, 1)
		_FogMaxHeight("Fog Max Radius", Float) = 0.0
		_FogMinHeight("Fog Min Radius", Float) = -1.0
	}

		SubShader
		{
			ZWrite On
			CGPROGRAM

			#pragma surface surf Lambert finalcolor:finalcolor vertex:vert

			float4 _Color;
			sampler2D _MainTex;
			float4 _FogColor;
			float _FogMaxHeight;
			float _FogMinHeight;
			float4 _ExampleName;

			struct Input
			{
				float2 uv_MainTex;
				float4 pos;
				float dis;
			};

			void vert(inout appdata_full v, out Input o)
			{
				o.pos = mul(unity_ObjectToWorld, v.vertex);
				o.dis = length(o.pos - _ExampleName);
				o.uv_MainTex = v.texcoord.xy;
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				o.Albedo = _Color * tex2D(_MainTex, IN.uv_MainTex);
			}

			void finalcolor(Input IN, SurfaceOutput o, inout fixed4 color)
			{
				float lerpValue = clamp((IN.dis - _FogMinHeight) / (_FogMaxHeight - _FogMinHeight), 0, 1);
				color.rgb = lerp(color.rgb, _FogColor, lerpValue);
			}

			ENDCG
		}
		Fallback "Transparent/VertexLit"
			
}