// Original by @Cyanilux
// Grass Geometry Shader, Written for Universal RP with help from https://roystan.net/articles/grass-shader.html

// Modified by @niuage to add tessellation. Also removed some code I didnt need in my own game, so feel free to revert to the original
// version from Cyanilux here: https://pastebin.com/Ey01tzLq

// Methods
float rand(float3 seed) {
	return frac(sin(dot(seed.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
}

// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
float3x3 AngleAxis3x3(float angle, float3 axis) {
	float c, s;
	sincos(angle, s, c);

	float t = 1 - c;
	float x = axis.x;
	float y = axis.y;
	float z = axis.z;

	return float3x3(
		t * x * x + c, t * x * y - s * z, t * x * z + s * y,
		t * x * y + s * z, t * y * y + c, t * y * z - s * x,
		t * x * z - s * y, t * y * z + s * x, t * z * z + c
	);
}

float3 _LightDirection;

float4 GetShadowPositionHClip(float3 positionWS, float3 normalWS) {
	float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

#if UNITY_REVERSED_Z
	positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#else
	positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#endif

	return positionCS;
}

float4 WorldToHClip(float3 positionWS, float3 normalWS) {
#ifdef SHADOW
	return GetShadowPositionHClip(positionWS, normalWS);
#else
	return TransformWorldToHClip(positionWS);
#endif
}

// Variables
CBUFFER_START(UnityPerMaterial) // Required to be compatible with SRP Batcher
float4 _Color;
float4 _Color2;
float _Width;
float _RandomWidth;
float _WindStrength;
float _Height;
float _RandomHeight;
CBUFFER_END

// Vertex, Geometry & Fragment Shaders

Varyings vert(Attributes input) {
	Varyings output = (Varyings)0;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

	VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
	// Seems like GetVertexPositionInputs doesn't work with SRP Batcher inside geom function?
	// Had to move it here, in order to obtain positionWS and pass it through the Varyings output.

	output.positionOS = input.positionOS; //vertexInput.positionCS; //
	output.positionWS = vertexInput.positionWS;
	output.normal = input.normal;
	output.tangent = input.tangent;
	output.fogCoord = ComputeFogFactor(output.positionOS.z);
	output.shadowCoord = GetShadowCoord(vertexInput);

	return output;
}

[maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
void geom(uint primitiveID : SV_PrimitiveID, triangle Varyings input[3], inout TriangleStream<GeometryOutput> triStream) {
	GeometryOutput output = (GeometryOutput)0;

	// Construct World -> Tangent Matrix (for aligning grass with mesh normals)
	float3 normal = input[0].normal;
	float4 tangent = input[0].tangent;
	float3 binormal = cross(normal, tangent.xyz) * tangent.w;

	float3x3 tangentToLocal = float3x3(
		tangent.x, binormal.x, normal.x,
		tangent.y, binormal.y, normal.y,
		tangent.z, binormal.z, normal.z
		);

	float3 positionWS = input[0].positionWS;

	float r = rand(positionWS.xyz);
	float3x3 randRotation = AngleAxis3x3(r * TWO_PI, float3(0, 0, 1));

	// Wind (based on sin / cos, aka a circular motion, but strength of 0.1 * sine)
	float2 wind = float2(sin(_Time.y + positionWS.x * 0.5), cos(_Time.y + positionWS.z * 0.5)) * _WindStrength * sin(_Time.y + r);
	float3x3 windMatrix = AngleAxis3x3((wind * PI).y, normalize(float3(wind.x, wind.y, 0)));

	float3x3 transformMatrix = mul(tangentToLocal, randRotation);
	float3x3 transformMatrixWithWind = mul(mul(tangentToLocal, windMatrix), randRotation);

	float bend = rand(positionWS.xyz) - 0.5;
	float width = _Width + _RandomWidth * (rand(positionWS.zyx) - 0.5);
	float height = _Height + _RandomHeight * (rand(positionWS.yxz) - 0.5);

	float3 normalWS = mul(transformMatrix, float3(0, 1, 0)); //?

	// Handle Geometry

	// Base 2 vertices
	output.positionWS = positionWS + mul(transformMatrix, float3(width, 0, 0));
	output.positionCS = WorldToHClip(output.positionWS, normalWS);
	output.uv = float2(0, 0);
	output.fogCoord = input[0].fogCoord;
	output.fogCoord = input[0].shadowCoord;
	triStream.Append(output);

	output.positionWS = positionWS + mul(transformMatrix, float3(-width, 0, 0));
	output.positionCS = WorldToHClip(output.positionWS, normalWS);
	output.uv = float2(0, 0);
	output.fogCoord = input[0].fogCoord;
	output.fogCoord = input[0].shadowCoord;
	triStream.Append(output);

	// Final vertex at top of blade
	output.positionWS = positionWS + mul(transformMatrixWithWind, float3(0, bend, height));
	output.positionCS = WorldToHClip(output.positionWS, normalWS);
	output.uv = float2(0, 1);
	output.fogCoord = input[0].fogCoord;
	output.fogCoord = input[0].shadowCoord;
	triStream.Append(output);

	triStream.RestartStrip();
}
