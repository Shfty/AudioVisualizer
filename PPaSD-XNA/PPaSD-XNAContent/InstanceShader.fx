
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
	// PER VERTEX DATA
    float4 Position : POSITION0;

	// PER INSTANCE DATA
	float4x4 World : TEXCOORD5;
	float4 InstanceColour : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Colour : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	// Use per instance World matrix to get World Pos
    float4 worldPosition = mul(input.Position, transpose(input.World));

	// Transform with camera view and projection to get screen pos
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	// Height-based brightness
	// CONFIG
	const float offset = 0.5;
	const float multiplier = 0.35;
	const float minFactor = 0.5;
	const float maxFactor = 1.5; // The shader of shader artists
	// END CONFIG
	float factor = min(max(((abs(worldPosition.y) * multiplier) + offset), minFactor), maxFactor); //SO MUCH MATHS
	float4 brightHeightColour = input.InstanceColour * factor;

	// Is the block white (1.0, 1.0, 1.0)? If so, it should be full-bright
	float avgColour = input.InstanceColour.x + input.InstanceColour.y + input.InstanceColour.z;

	// IknowIknowIknow. if statement in the vertex shader. Worry not, I got this!
	// This [flatten] compiler hint will un-if the if using conditional move statements
	[flatten]
	if(avgColour == 3.0) {
		output.Colour = input.InstanceColour;
	}
	else {
		output.Colour =  brightHeightColour;
	}


    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return input.Colour;
}

technique InstancePositionColour
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
