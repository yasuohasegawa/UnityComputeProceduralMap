Shader "Custom/GodRayNormal" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		Cull Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert alpha fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);

			v.vertex.x += (cos(_Time.y + v.texcoord.x*0.001)*v.texcoord.y*0.1) - pow(v.texcoord.y*0.1, 1.5);
			v.vertex.z += sin(_Time.x + v.texcoord.x*0.01)*(v.texcoord.y*0.2);
		}

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {

			float2 uv = IN.uv_MainTex;
			uv.x = uv.x*2.0;

			float4 texCol = tex2D(_MainTex, uv);

			float4 sum = float4(0.0, 0.0, 0.0, 0.0);
			float2 tc = uv;

			float radius = 10.0;
			float resolution = 512.0;

			float samples = 8.0;
			float blur = radius / resolution / samples;

			float hstep = 0.8;
			float vstep = 0.8;

			for (float i = samples; i >= 1.0; i--) {
				sum += tex2D(_MainTex, float2(tc.x - i*blur*hstep, tc.y - i*blur*vstep)) * 0.0162162162;
			}

			sum += tex2D(_MainTex, float2(tc.x, tc.y)) * 0.2270270270;

			for (i = 1.0; i <= samples; i++) {
				sum += tex2D(_MainTex, float2(tc.x - i*blur*hstep, tc.y - i*blur*vstep)) * 0.0162162162;
			}

			o.Albedo = sum.rgb*_Color;
			o.Alpha = sum.a*min(1.0, _Time.w*0.1) * (0.3 + sin(_Time.w*0.08)*0.25);
		}
		ENDCG
	}
	FallBack "Diffuse"
}

