using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SceneGraph
{

    public SceneGraph()
    {
    }

    public void RenderHierarchy(GraphObjects obj)
    {
        //object renders itself
        obj.Render();

        foreach(GraphObjects child in obj.children)
        {
            //render every child
            RenderHierarchy(child);
        }
    }

}

