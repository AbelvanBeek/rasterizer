#version 330
 
// shader input
in vec2 vUV;				// vertex uv coordinate
in vec3 vNormal;			// untransformed vertex normal
in vec3 vPosition;			// untransformed vertex position

// shader output
out vec4 normal;			// transformed vertex normal
out vec2 uv;	
out vec4 worldPos;
out float spec;			

uniform mat4 transform;		// fully transform --> model to screen space
uniform float specness;		// amount of gloss
uniform mat4 toWorld;		// world space --> model to world 
 
// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = transform * vec4(vPosition, 1.0);
	worldPos = toWorld * vec4(vPosition, 1.0);
	
	// forward normal and uv coordinate; will be interpolated over triangle
	normal = toWorld * vec4( vNormal, 0.0f );
	uv = vUV;
	spec = specness;
}