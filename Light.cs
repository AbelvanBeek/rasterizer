using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

class Light : GraphObjects
{
    int lightLocationID;
    int id;
    Vector3 position;
    Vector3 intensity;
    Shader shader;

    public Light(int id, Vector3 position, Vector3 intensity, Shader shader, Matrix4 transform, Matrix4 toWorld, GraphObjects parent) : base(transform, toWorld, parent)
    {
        //we actually don't use the matrices because we have a static position right now.

        this.id = id;
        this.position = position;
        this.intensity = intensity;
        this.shader = shader;

        // light adds its position to the shader
        lightLocationID = GL.GetUniformLocation(shader.programID, "lightPos" + id);
        GL.UseProgram(shader.programID);
        GL.Uniform3(lightLocationID, position);

        // light adds its Color/intensity to the shader
        int lightColorID = GL.GetUniformLocation(shader.programID, "lightColor" + id);
        GL.UseProgram(shader.programID);
        GL.Uniform3(lightColorID, intensity);
    }
}
