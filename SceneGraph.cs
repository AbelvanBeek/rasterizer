using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SceneGraph
{

    //public static List<GraphObjects> graphObjects;

    public SceneGraph()
    {
        //graphObjects = new List<GraphObjects>();
    }

    public void RenderHierarchy(GraphObjects obj)
    {
        obj.Render();

        foreach(GraphObjects child in obj.children)
        {
            RenderHierarchy(child);
        }
    }

}

