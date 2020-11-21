struct Attributes {
	float4 positionOS   : POSITION;
	float3 normal		: NORMAL;
	float4 tangent		: TANGENT;
	float2 texcoord     : TEXCOORD0;

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings {
	float4 positionOS   : SV_POSITION;
	float2 texcoord     : TEXCOORD0;
	float  fogCoord     : TEXCOORD1;
	float4 shadowCoord  : TEXCOORD2;
	float3 positionWS	: TEXCOORD3;

	float3 normal		: NORMAL;
	float4 tangent		: TANGENT;

	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};

struct GeometryOutput {
	float4 positionCS	: SV_POSITION;
	float3 normal		: NORMAL;

	float2 uv			: TEXCOORD0;
	float  fogCoord		: TEXCOORD1;
	float4 shadowCoord  : TEXCOORD2;
	float3 positionWS	: TEXCOORD3;
};
