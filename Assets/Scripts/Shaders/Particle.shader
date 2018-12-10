Shader "Custom/Particle" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}

		SubShader{
		Tags{ "Queue" = "Transparent" }
		LOD 100

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		StructuredBuffer<float3> PositionBuffer;
		StructuredBuffer<float4x4> RotationMatrixBuffer;
	#endif

		void setup() {
	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			float3 verts = PositionBuffer[unity_InstanceID];
			float x = verts.x;
			float y = verts.y;
			float z = verts.z;
			unity_ObjectToWorld._14_24_34_44 = float4(x, y, z, 1);
	#endif
		}

		fixed4 _Color;

		void vert(inout appdata_full v) {
	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			float size = 0.1 + sin(_Time.x*cos(unity_InstanceID)*2.0)*0.05;
			float3 scale = float3(size, size, size);
			float3 verts = PositionBuffer[unity_InstanceID];
			float4x4 scaleMat = float4x4(
				scale.x, 0, 0, 0,
				0, scale.y, 0, 0,
				0, 0, scale.z, 0,
				0, 0, 0, 1
				);

			float4x4 rotMat = RotationMatrixBuffer[unity_InstanceID];
			v.vertex.xyz = mul(v.vertex.xyz, rotMat);
			v.vertex.xyz = mul(v.vertex.xyz, scaleMat);
	#endif
		}

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 res = tex2D(_MainTex, IN.uv_MainTex)*_Color;
			o.Albedo = res.rgb;
			o.Alpha = res.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
