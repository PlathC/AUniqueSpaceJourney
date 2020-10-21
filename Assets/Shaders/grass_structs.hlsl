struct Attributes {
	float4 positionOS   : POSITION;
	float3 normal		: NORMAL;
	float4 tangent		: TANGENT;
	float2 texcoord     : TEXCOORD0;
};

struct Varyings {
	float4 positionOS   : SV_POSITION;
	float3 positionWS	: TEXCOORD1;
	float3 normal		: NORMAL;
	float4 tangent		: TANGENT;
};

struct GeometryOutput {
	float4 positionCS	: SV_POSITION;
	float3 positionWS	: TEXCOORD1;
	float2 uv			: TEXCOORD0;
};
