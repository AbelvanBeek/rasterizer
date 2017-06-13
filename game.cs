using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

class Game
{
    // member variables
    public Surface screen;                  // background surface for printing etc.
    Mesh teapot, floor;                     // a mesh to draw using OpenGL
    const float PI = 3.1415926535f;         // PI
    float a = 0;                            // teapot rotation angle
    Stopwatch timer;                        // timer for measuring frame duration
    Shader shader;                          // shader to use for rendering
    Shader postproc;                        // shader to use for post processing
    Texture wood;                           // texture to use for rendering
    RenderTarget target;                    // intermediate render target
    ScreenQuad quad;                        // screen filling quad for post processing
    bool useRenderTarget = true;
    SceneGraph sceneGraph;                  // create new scenegraph
    SceneObject world;                      // main scene object containing others

    Matrix4 go;                             // initial matrix
    Matrix4 toWorld;                        // transform to world coordinates

    // initialize
    public void Init()
    {
        // load teapot
        teapot = new Mesh("../../assets/teapot.obj");
        floor = new Mesh("../../assets/floor.obj");
        // initialize stopwatch
        timer = new Stopwatch();
        timer.Reset();
        timer.Start();
        // create shaders
        shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
        postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
        // load a texture
        wood = new Texture("../../assets/wood.jpg");
        // create the render target
        target = new RenderTarget(screen.width, screen.height);
        quad = new ScreenQuad();
        // create scenegraph and main object
        sceneGraph = new SceneGraph();
        // working object location
        go = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
        toWorld = go;
        go *= Matrix4.CreateTranslation(0, -4, -15);
        go *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        // ambient light preparation
        int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
        GL.UseProgram(shader.programID);
        GL.Uniform3(ambientID, 0.4f, 0.1f, 0.0f);
        // prepare scene
        world = new SceneObject(null, null, null, go, toWorld, null);
        CreateScene();
    }

    // tick for background surface
    public void Tick()
    {
        screen.Clear(0);
        screen.Print("hello world", 2, 2, 0xffff00);
        //Console.WriteLine(GL.GetError());
    }

    // tick for OpenGL rendering code
    public void RenderGL()
    {
        // measure frame duration
        float frameDuration = timer.ElapsedMilliseconds;
        timer.Reset();
        timer.Start();

        // working object location
        go = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
        toWorld = go;
        go *= Matrix4.CreateTranslation(0, -4, -15);
        go *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);

        // prepare scene
        world = new SceneObject(null, null, null, go, toWorld, null);
        CreateScene();

        // update rotation
        a += 0.001f * frameDuration;
        if (a > 2 * PI) a -= 2 * PI;

        if (useRenderTarget)
        {
            // enable render target
            target.Bind();

            // render scene to render target
            sceneGraph.RenderHierarchy(world);

            // render quad
            target.Unbind();
            quad.Render(postproc, target.GetTextureID());
        }
        else
        {
            // render scene directly to the screen
            sceneGraph.RenderHierarchy(world);
        }
    }

    // compose a scene
    public void CreateScene()
    {

        SceneObject tp = new SceneObject(teapot, shader, wood, new Matrix4(1, 0, 0, 0,
                                                                           0, 1, 0, 0,
                                                                           0, 0, 1, 0,
                                                                           0, 0, 0, 1), toWorld, world);
        SceneObject fl = new SceneObject(floor, shader, wood, new Matrix4(1, 0, 0, 0,
                                                                          0, 1, 0, 0,
                                                                          0, 0, 1, 0,
                                                                          0, 0, 0, 1), toWorld, world);

        // sorry for the code
        Light light0 = new Light(0, new Vector3(10, 3, 0), new Vector3(10.0f, 10.0f, 10.0f), shader, new Matrix4(1, 0, 0, 0,
                                                                                                                 0, 1, 0, 0,
                                                                                                                 0, 0, 1, 0,
                                                                                                                 0, 0, 0, 1), toWorld, world);
        Light light1 = new Light(1, new Vector3(-10, 3, 0), new Vector3(0.0f, 0.0f, 10.0f), shader, new Matrix4(1, 0, 0, 0,
                                                                                                                0, 1, 0, 0,
                                                                                                                0, 0, 1, 0,
                                                                                                                0, 0, 0, 1), toWorld, world);
        Light light2 = new Light(2, new Vector3(0, 3, 10), new Vector3(0.0f, 10.0f, 0.0f), shader, new Matrix4(1, 0, 0, 0,
                                                                                                               0, 1, 0, 0,
                                                                                                               0, 0, 1, 0,
                                                                                                               0, 0, 0, 1), toWorld, world);
        Light light3 = new Light(3, new Vector3(0, 3, -10), new Vector3(10.0f, 0.0f, 0.0f), shader, new Matrix4(1, 0, 0, 0,
                                                                                                                0, 1, 0, 0,
                                                                                                                0, 0, 1, 0,
                                                                                                                0, 0, 0, 1), toWorld, world);
    }
}