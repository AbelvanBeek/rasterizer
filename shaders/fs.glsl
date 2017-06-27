#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 worldPos;
in float spec;

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

	
	vec3 L0 = lightPos0 - worldPos.xyz;		//Vector from the vertex to the lightsource
	float dist0 = L0.length();				//Calculate the length of this vector
	L0 = normalize( L0 );					//now we can normalize the light vector
	vec3 materialColor0 = texture( pixels, uv ).xyz;	//get the pixel color from the texture
	float attenuation0 = 1.0f / (dist0 * dist0);		//calculate light attenuation

	//Phong part:
	vec3 R0 = normalize(-(-worldPos.xyz -2*(dot(-worldPos.xyz, normal.xyz)*normal.xyz))); //Reflect the ray from the camera to the vertex
			//Calculate the specular color dependending on how close the reflected vector is to the ligth vector, and raise by a certain power
	vec4 specColor0 = vec4(lightColor0, 1) * pow(max(dot(R0, L0), 0.0), 0.3*100) * (materialColor0, 1);
			//clamp the result to 1.0
	specColor0 = clamp(specColor0, 0.0, 1.0); 
			//add together all colors (Ambient, Phong, Diffuse)
	outputColor += vec4( materialColor0 * max( 0.0f, dot( L0, normal.xyz ) ) * attenuation0 * lightColor0, 1 ) + specColor0 * attenuation0 * spec;

	//repeat

	vec3 L1 = lightPos1 - worldPos.xyz;
	float dist1 = L1.length();
	L1 = normalize( L1 );
	vec3 materialColor1 = texture( pixels, uv ).xyz;
	float attenuation1 = 1.0f / (dist1 * dist1);
	vec3 R1 = normalize(-(-worldPos.xyz -2*(dot(-worldPos.xyz, normal.xyz)*normal.xyz)));
	vec4 specColor1 = vec4(lightColor1, 1) * pow(max(dot(R1, L1), 0.0), 0.3*100) * (materialColor1, 1);
	specColor1 = clamp(specColor1, 0.0, 1.0);
	outputColor += vec4( materialColor1 * max( 0.0f, dot( L1, normal.xyz ) ) * attenuation1 * lightColor1, 1 ) + specColor1  * attenuation0 * spec;

	vec3 L2 = lightPos2 - worldPos.xyz;
	float dist2 = L2.length();
	L2 = normalize( L2 );
	vec3 materialColor2 = texture( pixels, uv ).xyz;
	float attenuation2 = 1.0f / (dist2 * dist2);
	vec3 R2 = normalize(-(-worldPos.xyz -2*(dot(-worldPos.xyz, normal.xyz)*normal.xyz)));
	vec4 specColor2 = vec4(lightColor2, 1) * pow(max(dot(R2, L2), 0.0), 0.3*100) * (materialColor2, 1);
	specColor2 = clamp(specColor2, 0.0, 1.0);
	outputColor += vec4( materialColor2 * max( 0.0f, dot( L2, normal.xyz ) ) * attenuation2 * lightColor2, 1 ) + specColor2  * attenuation0 * spec;

	vec3 L3 = lightPos3 - worldPos.xyz;
	float dist3 = L3.length();
	L3 = normalize( L3 );
	vec3 materialColor3 = texture( pixels, uv ).xyz;
	float attenuation3 = 1.0f / (dist3 * dist3);
	vec3 R3 = normalize(-(-worldPos.xyz -2*(dot(-worldPos.xyz, normal.xyz)*normal.xyz)));
	vec4 specColor3 = vec4(lightColor3, 1) * pow(max(dot(R3, L3), 0.0), 0.3*100) * (materialColor3, 1);
	specColor3 = clamp(specColor3, 0.0, 1.0);
	outputColor += vec4( materialColor3 * max( 0.0f, dot( L3, normal.xyz ) ) * attenuation3 * lightColor3, 1 ) + specColor3 * attenuation0 * spec;

	outputColor += vec4(ambientColor * materialColor0, 1);
}