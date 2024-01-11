float4x4 WVP;
float Line;

float4x4 WorldInverseTranspose;
float3 DiffuseLightDirection;
float4 DiffuseLightColor;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Color    : COLOR0;
	float4 Normal   : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Color    : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, WVP);
	output.Color = input.Color;

	//float DiffuseIntensity = 7.0f;
	//float4 normal = mul(input.Normal, WorldInverseTranspose);
    //float lightIntensity = dot(normal, DiffuseLightDirection);
    //float4 light = saturate(DiffuseLightColor * DiffuseIntensity); // * lightIntensity);
	//output.Color = input.Color * light;
  
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	input.Color.a = 1.0f;
    return input.Color;
}

VertexShaderOutput BottomVertexShader(VertexShaderInput input)
{
	float LineThickness = Line;

    VertexShaderOutput Output;

	float4 lineColour = input.Color;
	lineColour = lineColour / 2;
	lineColour.a = 1.0f;

    Output.Position = mul(input.Position, WVP) - LineThickness;
	Output.Color = lineColour;
    return Output;
}

float4 BottomPixelShader(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}

VertexShaderOutput TopVertexShader(VertexShaderInput input)
{
	float LineThickness = Line;

    VertexShaderOutput Output;

    Output.Position = mul(input.Position, WVP) - LineThickness;
	Output.Color = float4(0,0,0,1);
    return Output;
}

float4 TopPixelShader(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}


technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 BottomVertexShader();
		PixelShader = compile ps_2_0 BottomPixelShader();
		CullMode = CCW;
	}
	pass Pass2
	{
		VertexShader = compile vs_2_0 TopVertexShader();
		PixelShader = compile ps_2_0 TopPixelShader();
		CullMode = CW;
	}

	pass Pass3
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
		CullMode = CCW;
    }
}
