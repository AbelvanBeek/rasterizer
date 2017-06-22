#version 330

// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 worldPos;

uniform sampler2D pixels;		// texture sampler
uniform vec3 ambientColor;		// ambient light color


// shader output
out vec4 outputColor;

// fragment shader
void main()
{
	outputColor = vec4(0, 0, 0, 0);

	vec3 materialColor = texture( pixels, uv ).xyz;

	outputColor += vec4( materialColor,1 ) ;
}

// EOF