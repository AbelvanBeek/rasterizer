using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SceneGraph
{

    public static List<SceneObject> sceneObjects;

    public SceneGraph()
    {
        sceneObjects = new List<SceneObject>();
    }

    public void RenderHierarchy(SceneObject obj)
    {
        obj.Render();

        foreach(SceneObject child in obj.children)
        {
            RenderHierarchy(child);
        }
    }

}

