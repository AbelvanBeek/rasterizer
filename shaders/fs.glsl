#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 worldPos;
uniform sampler2D pixels;		// texture sampler
uniform vec3 ambientColor;		// ambient light color

uniform vec3 lightPos0;			// light position
uniform vec3 lightColor0;		// light color

uniform vec3 lightPos1;			// light position
uniform vec3 lightColor1;		// light color

uniform vec3 lightPos2;			// light position
uniform vec3 lightColor2;		// light color

uniform vec3 lightPos3;			// light position
uniform vec3 lightColor3;		// light color


// shader output
out vec4 outputColor;

// fragment shader
void main()
{
	outputColor = vec4(0, 0, 0, 0);

	vec3 L0 = lightPos0 - worldPos.xyz;
	float dist0 = L0.length();
	L0 = normalize( L0 );
	vec3 materialColor0 = texture( pixels, uv ).xyz;
	float attenuation0 = 1.0f / (dist0 * dist0);
	outputColor += vec4( materialColor0 * max( 0.0f, dot( L0, normal.xyz ) ) * attenuation0 * lightColor0, 1 );

	vec3 L1 = lightPos1 - worldPos.xyz;
	float dist1 = L1.length();
	L1 = normalize( L1 );
	vec3 materialColor1 = texture( pixels, uv ).xyz;
	float attenuation1 = 1.0f / (dist1 * dist1);
	outputColor += vec4( materialColor1 * max( 0.0f, dot( L1, normal.xyz ) ) * attenuation1 * lightColor1, 1 );

	vec3 L2 = lightPos2 - worldPos.xyz;
	float dist2 = L2.length();
	L2 = normalize( L2 );
	vec3 materialColor2 = texture( pixels, uv ).xyz;
	float attenuation2 = 1.0f / (dist2 * dist2);
	outputColor += vec4( materialColor2 * max( 0.0f, dot( L2, normal.xyz ) ) * attenuation2 * lightColor2, 1 );

	vec3 L3 = lightPos3 - worldPos.xyz;
	float dist3 = L3.length();
	L3 = normalize( L3 );
	vec3 materialColor3 = texture( pixels, uv ).xyz;
	float attenuation3 = 1.0f / (dist3 * dist3);
	outputColor += vec4( materialColor3 * max( 0.0f, dot( L3, normal.xyz ) ) * attenuation3 * lightColor3, 1 );

	outputColor += vec4(ambientColor, 1);
}