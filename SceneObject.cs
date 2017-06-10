using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SceneObject
{
    Mesh mesh;
    Shader shader;
    Texture texture;
    public Matrix4 transform;
    public Matrix4 toWorld;
    SceneObject parent;

    public List<SceneObject> children;

    public SceneObject(Mesh mesh, Shader shader, Texture texture, Matrix4 transform, Matrix4 toWorld, SceneObject parent)
    {
        children = new List<SceneObject>();

        this.mesh = mesh;
        this.shader = shader;
        this.texture = texture;
        this.toWorld = toWorld;

        if (parent != null)
        {
            this.transform = transform;// * parent.transform;
            this.parent = parent;

            parent.children.Add(this);
        }

        SceneGraph.sceneObjects.Add(this);
    }

    public void Render()
    {
        if (mesh != null)
        {
            transform = transform;// * parent.transform;
            mesh.Render(shader, transform, toWorld, texture);
        }

    }
}