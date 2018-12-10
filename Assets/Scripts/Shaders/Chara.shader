Shader "Custom/Chara" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		StructuredBuffer<float3> PositionBuffer;
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

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float3 rgb2hsv(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp(float4(c.b, c.g, K.w, K.z), float4(c.g, c.b, K.x, K.y), step(c.b, c.g));
			float4 q = lerp(float4(p.x, p.y, p.w, c.r), float4(c.r, p.y, p.z, p.x), step(p.x, c.r));

			float d = q.x - min(q.w, q.y);
			float e = 1.0e-10;
			return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		float3 random3(float3 c) {
			float j = 4096.0*sin(dot(c, float3(17.0, 59.4, 15.0)));
			float3 r;
			r.z = frac(512.0*j);
			j *= .125;
			r.x = frac(512.0*j);
			j *= .125;
			r.y = frac(512.0*j);
			return r - 0.5;
		}

		void vert(inout appdata_full v) {
	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			float size = 0.1+sin(_Time.x*unity_InstanceID*0.2)*0.05;
			float3 scale = float3(size, size, size);
			float3 verts = PositionBuffer[unity_InstanceID];
			float4x4 scaleMat = float4x4(
				scale.x, 0, 0, 0,
				0, scale.y, 0, 0,
				0, 0, scale.z, 0,
				0, 0, 0, 1
				);
			v.vertex.xyz = mul(v.vertex.xyz, scaleMat);
	#endif
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 res = tex2D(_MainTex, IN.uv_MainTex)*_Color;
			float3 col = float3(1, 1, 1);
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			col = random3(float3(unity_InstanceID*0.01, unity_InstanceID*0.01, unity_InstanceID*0.01))*2.0;
#endif
			o.Albedo = res.rgb*col;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = res.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
