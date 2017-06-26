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
        this.id = id;
        this.position = position;
        this.intensity = intensity;
        this.shader = shader;

        // light preparation
        lightLocationID = GL.GetUniformLocation(shader.programID, "lightPos" + id);
        GL.UseProgram(shader.programID);
        GL.Uniform3(lightLocationID, position);

        // light preparation
        int lightColorID = GL.GetUniformLocation(shader.programID, "lightColor" + id);
        GL.UseProgram(shader.programID);
        GL.Uniform3(lightColorID, intensity);
    }

    public override void Render()
    {
        // light preparation
        lightLocationID = GL.GetUniformLocation(shader.programID, "lightPos" + id);
        GL.UseProgram(shader.programID);
        GL.Uniform3(lightLocationID, position);

        // light preparation
        int lightColorID = GL.GetUniformLocation(shader.programID, "lightColor" + id);
        GL.UseProgram(shader.programID);
        GL.Uniform3(lightColorID, intensity);
        transform = mainTransform;
        if (parent != null)
            transform *= parent.transform;
        GL.UseProgram(shader.programID);
        GL.Uniform3(lightLocationID, position);
    }
}
