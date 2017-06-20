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
    float specness;

    public SceneObject(Mesh mesh, Shader shader, float specness, Texture texture, Matrix4 transform, Matrix4 toWorld, GraphObjects parent) : base(transform, toWorld, parent) 
    {
        this.mesh = mesh;
        this.shader = shader;
        this.texture = texture;
        this.specness = specness;
    }

    public override void Render()
    {
        if (mesh != null)
        {
            //transform = parent.transform * mainTransform;
            mesh.Render(shader, specness, parent.transform*transform , toWorld, texture);
        }

    }
}