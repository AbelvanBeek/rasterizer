using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GraphObjects
{
    public Matrix4 transform;
    public Matrix4 toWorld;
    public Matrix4 mainTransform;

    public GraphObjects parent;

    public List<GraphObjects> children;

    public GraphObjects(Matrix4 transform, Matrix4 toWorld, GraphObjects parent)
    {
        this.toWorld = toWorld;

        if (parent != null)
        {
            this.parent = parent;

            parent.children.Add(this);
        }
            this.transform = transform;

        //mainTransform = transform;
        SceneGraph.graphObjects.Add(this);

        children = new List<GraphObjects>();
    }

    public virtual void Render()
    {
        transform = parent.transform * mainTransform;
    }
}
