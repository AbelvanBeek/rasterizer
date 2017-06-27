#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)

// shader output
out vec3 outputColor;

// vignette variables
const float vignetteRadius = 0.8; const float vignetteSoftness = 0.5;

// chromatic abberation variables
const float chromaticIntensity = 1; const vec2 chromaticRed = vec2(0.01, 0); const vec2 chromaticGreen = vec2(0, 0.01); const vec2 chromaticBlue = vec2(0.01, 0.01);

void main()
{
	// screen center
	vec2 center = gl_FragCoord.xy/vec2(1920, 1000) - vec2(0.5);

	// retrieve input pixel
	outputColor = texture( pixels, uv ).rgb;

	// apply chromatic abberation
	vec3 rValue = texture(pixels, uv - chromaticRed * center * chromaticIntensity).rgb;
	vec3 gValue = texture(pixels, uv - chromaticGreen * center * chromaticIntensity).rgb;
	vec3 bValue = texture(pixels, uv - chromaticBlue * center * chromaticIntensity).rgb;
	outputColor =  vec3(rValue.r, gValue.g, bValue.b);
	
	//outputColor = vec3(1.0, 1.0, 1.0);

	// apply vignette
	float vignetteLength = length(center);
	float vignette = smoothstep(vignetteRadius, vignetteRadius - vignetteSoftness, vignetteLength);
	outputColor = mix(outputColor, outputColor * vignette, 0.6);
}

// EOF