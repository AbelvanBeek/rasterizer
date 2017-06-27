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
        transform = mainTransform; //make a copy of maintransform so that we don't transform into infinity
        if (parent != null)
           transform *= parent.transform; //if the sceneobject has a parent, apply its matrix to its own matrix
        if (mesh != null)
        {
            //if the object has a mesh, render it.
            mesh.Render(shader, specness, transform, texture);
        }

    }
}