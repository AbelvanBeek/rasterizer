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
        transform = mainTransform;
        if (parent != null)
           transform *= parent.transform;
        if (mesh != null)
        {
                mesh.Render(shader, specness, transform , toWorld, texture);
        }

    }
}