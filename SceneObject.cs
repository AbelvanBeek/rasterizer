using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SceneObject : GraphObjects
{
    Mesh mesh;
    Shader shader;
    Texture texture;

    public SceneObject(Mesh mesh, Shader shader, Texture texture, Matrix4 transform, Matrix4 toWorld, GraphObjects parent) : base(transform, toWorld, parent) 
    {
        this.mesh = mesh;
        this.shader = shader;
        this.texture = texture;
    }

    public override void Render()
    {
        if (mesh != null)
        {
            transform = parent.transform * mainTransform;
            mesh.Render(shader, transform, toWorld, texture);
        }

    }
}