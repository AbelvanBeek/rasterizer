using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using OpenTK.Input;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

class Game
{
    // member variables
    public Surface screen;                  // background surface for printing etc.
    Mesh teapot, floor, sphere;             // a mesh to draw using OpenGL
    const float PI = 3.1415926535f;         // PI
    float a = 0;                            // teapot rotation angle

    Stopwatch timer;                        // timer for measuring frame duration
    Shader shader;                          // shader to use for rendering
    Shader skyshader;                       // shader for skydome
    Shader postproc;                        // shader to use for post processing

    Texture wood;                           // texture to use for rendering
    Texture skytex;

    RenderTarget target;                    // intermediate render target
    ScreenQuad quad;                        // screen filling quad for post processing
    bool useRenderTarget = true;

    SceneGraph sceneGraph;                  // create new scenegraph
    SceneObject camera;                     // camera on top of the hierarchy
    Matrix4 cameraMatrix;                   // moving the camera
    SceneObject world;                      // main scene object containing others
    Matrix4 worldMatrix;                    // moving the world

    Matrix4 toWorld;                        // transform to world coordinates

    Matrix4 rotation;                       // rotate world
    float transFX = 0, transFY = 0, transFZ = 0, transLX = 0, transLY = 0, transLZ = 0;

    // initialize
    public void Init()
    {
        // load teapot
        teapot = new Mesh("../../assets/teapot.obj");
        floor = new Mesh("../../assets/floor.obj");
        sphere = new Mesh("../../assets/sphere.obj");

        // initialize stopwatch
        timer = new Stopwatch();
        timer.Reset();
        timer.Start();

        // create shaders
        shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
        skyshader = new Shader("../../shaders/vs_skydome.glsl", "../../shaders/fs_skydome.glsl");
        postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");

        // load a texture
        wood = new Texture("../../assets/wood.jpg");
        skytex = new Texture("../../assets/space.jpg");

        // create the render target
        target = new RenderTarget(screen.width, screen.height);
        quad = new ScreenQuad();

        // create scenegraph and main object
        sceneGraph = new SceneGraph();

        // initialize matrices
        cameraMatrix = Matrix4.Identity;
        worldMatrix = Matrix4.Identity;
        rotation = Matrix4.Identity;
        toWorld = cameraMatrix;
        // ambient light preparation
        int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
        GL.UseProgram(shader.programID);
        GL.Uniform3(ambientID, 0.4f, 0.1f, 0.0f);

        // prepare scene
        camera = new SceneObject(null, null, 0, null, cameraMatrix, toWorld, null);
        world = new SceneObject(null, null, 0, null, worldMatrix, toWorld, camera);
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
        HandleInput();

        // measure frame duration
        float frameDuration = timer.ElapsedMilliseconds;
        timer.Reset();
        timer.Start();


        world.transform = worldMatrix * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a) * Matrix4.CreateTranslation(0 + transLX, -5 + transLY, -15 + transLZ) * rotation * Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        //CreateScene();

        // update rotation
        //a += 0.001f * frameDuration;
        if (a > 2 * PI) a -= 2 * PI;

        if (useRenderTarget)
        {
            // enable render target
            target.Bind();

            // render scene to render target
            sceneGraph.RenderHierarchy(camera);

            // render quad
            target.Unbind();
            quad.Render(postproc, target.GetTextureID());
        }
        else
        {
            // render scene directly to the screen
            sceneGraph.RenderHierarchy(camera);
        }
    }

    // compose a scene
    public void CreateScene()
    {
        // objects
        SceneObject tp = new SceneObject(teapot, shader, 1, wood, Matrix4.Identity, toWorld, world);
        SceneObject fl = new SceneObject(floor, shader, 1, wood, Matrix4.Identity, toWorld, world);

        // skydome
        SceneObject skydome1 = new SceneObject(sphere, skyshader, 0, skytex, Matrix4.Identity, toWorld, world);

        //  lights
        Light light0 = new Light(0, new Vector3(0, 0, 5), new Vector3(2.0f, 2.0f, 2.0f), shader, Matrix4.Identity, toWorld, world);
        Light light1 = new Light(1, new Vector3(-10, 3, 0), new Vector3(0.0f, 0.0f, 10.0f), shader, Matrix4.Identity, toWorld, world);
        Light light2 = new Light(2, new Vector3(0, 3, 10), new Vector3(0.0f, 10.0f, 0.0f), shader, Matrix4.Identity, toWorld, world);
        Light light3 = new Light(3, new Vector3(0, 3, -10), new Vector3(10.0f, 0.0f, 0.0f), shader, Matrix4.Identity, toWorld, world);
    }

    public void HandleInput()
    {
        KeyboardState k = Keyboard.GetState();
        //working left right up down zoom in zoom out
       	if (k.IsKeyDown(Key.Up))
            transLY -= 0.1f;
        if (k.IsKeyDown(Key.Down))
            transLY += 0.1f;
        if (k.IsKeyDown(Key.Left))
            transLX += 0.1f;
        if (k.IsKeyDown(Key.Right))
            transLX -= 0.1f;
        if (k.IsKeyDown(Key.Plus))
            transLZ += 0.1f;
        if (k.IsKeyDown(Key.Minus))
            transLZ -= 0.1f;
        if (k.IsKeyDown(Key.W) || k.IsKeyDown(Key.A) || k.IsKeyDown(Key.S) || k.IsKeyDown(Key.D))
       {
            rotation = Matrix4.Identity;
            if (k.IsKeyDown(Key.W))
                transFX -= 0.01f;
            if (k.IsKeyDown(Key.A))
                transFY -= 0.01f;
            if (k.IsKeyDown(Key.S))
                transFX += 0.01f;
            if (k.IsKeyDown(Key.D))
                transFY += 0.01f;
            rotation *= Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), transFX);
            rotation *= Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), transFY);
        }
        
    }
}