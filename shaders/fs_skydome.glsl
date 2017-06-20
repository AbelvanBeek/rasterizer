#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 worldPos;

uniform sampler2D pixels;		// texture sampler

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
	outputColor = vec4(texture( pixels, uv ).xyz, 1);
}