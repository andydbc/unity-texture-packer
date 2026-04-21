Shader "Hidden/TexturePacker"
{
	Properties
	{
		_Input00Tex ("Input00", 2D) = "black" {}
		_Input01Tex ("Input01", 2D) = "black" {}
		_Input02Tex ("Input02", 2D) = "black" {}
		_Input03Tex ("Input03", 2D) = "black" {}

		_Input00In ("Input00 In", Vector) = (0,0,0,0)
		_Input01In ("Input01 In", Vector) = (0,0,0,0)
		_Input02In ("Input02 In", Vector) = (0,0,0,0)
		_Input03In ("Input03 In", Vector) = (0,0,0,0)

		_ChannelMask ("Channel Mask", Vector) = (1,1,1,1)
		_HasAlphaInput ("Has Alpha Input", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			ZTest Always
			ZWrite Off
			Cull Off
			ColorMask RGBA

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _Input00Tex;
			sampler2D _Input01Tex;
			sampler2D _Input02Tex;
			sampler2D _Input03Tex;

			float4 _Input00In;
			float4 _Input01In;
			float4 _Input02In;
			float4 _Input03In;

			float4 _Input00Inv;
			float4 _Input01Inv;
			float4 _Input02Inv;
			float4 _Input03Inv;

			float4x4 _Input00Out;
			float4x4 _Input01Out;
			float4x4 _Input02Out;
			float4x4 _Input03Out;

			// Per-slot channel selectors: each row is a unit vector picking R/G/B/A
			float4x4 _Input00Ch;
			float4x4 _Input01Ch;
			float4x4 _Input02Ch;
			float4x4 _Input03Ch;

			float4 _ChannelMask;
			float _HasAlphaInput;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			// Samples tex and routes channels according to ch (source selector),
			// e (enable mask), inv (invert mask), and out_m (output routing matrix).
			float4 PackChannels(sampler2D tex, float2 uv, float4 e, float4 inv, float4x4 out_m, float4x4 ch)
			{
				float4 inColor = tex2D(tex, uv);

				// dot(inColor, ch[i]) picks the channel the user chose for slot i
				float s0 = dot(inColor, ch[0]);
				float s1 = dot(inColor, ch[1]);
				float s2 = dot(inColor, ch[2]);
				float s3 = dot(inColor, ch[3]);

				float4 r = (inv[0] ? 1 - s0 : s0) * out_m[0] * e.r;
				float4 g = (inv[1] ? 1 - s1 : s1) * out_m[1] * e.g;
				float4 b = (inv[2] ? 1 - s2 : s2) * out_m[2] * e.b;
				float4 a = (inv[3] ? 1 - s3 : s3) * out_m[3] * e.a;

				return r + g + b + a;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float4 c00 = PackChannels(_Input00Tex, i.uv, _Input00In, _Input00Inv, _Input00Out, _Input00Ch);
				float4 c01 = PackChannels(_Input01Tex, i.uv, _Input01In, _Input01Inv, _Input01Out, _Input01Ch);
				float4 c02 = PackChannels(_Input02Tex, i.uv, _Input02In, _Input02Inv, _Input02Out, _Input02Ch);
				float4 c03 = PackChannels(_Input03Tex, i.uv, _Input03In, _Input03Inv, _Input03Out, _Input03Ch);
				float4 result = c00 + c01 + c02 + c03;
				result.a = lerp(1.0, result.a, _HasAlphaInput);

				// Unity-style preview: all channels on → full RGBA with alpha blending;
				// single channel on → show that channel tinted by its color (A is white).
				float n = dot(_ChannelMask, float4(1,1,1,1));
				float sumVal = dot(result, _ChannelMask);
				float3 tint = (_ChannelMask.a > 0.5) ? float3(1,1,1) : _ChannelMask.rgb;
				float4 single = float4(sumVal * tint, 1.0);
				return (n > 3.5) ? result : single;
			}
			ENDCG
		}
	}
}
