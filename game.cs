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
    Mesh teapot, floor;                     // a mesh to draw using OpenGL
    const float PI = 3.1415926535f;         // PI
    float a = 0;                            // teapot rotation angle
    Stopwatch timer;                        // timer for measuring frame duration
    Shader shader, skyshader;                          // shader to use for rendering
    Shader postproc;                        // shader to use for post processing
    Texture wood;                           // texture to use for rendering
    Texture skytex;
    RenderTarget target;                    // intermediate render target
    ScreenQuad quad;                        // screen filling quad for post processing
    bool useRenderTarget = true;

    SceneGraph sceneGraph;                  // create new scenegraph
    public static SceneObject camera, world, tp, fl, skypot;      // Used sceneobjects
    Matrix4 cameraMatrix;                   // moving the camera                    

    Light light0, light1, light2, light3;
    Matrix4 worldMatrix;                    // moving the world

    public static Matrix4 toWorld;                        // transform to world coordinates

    Matrix4 rotation;                       // rotate world

    float transFX = 0, transFY = 0, transFZ = 0, transLX = 0, transLY = 0, transLZ = 0;

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
        worldMatrix = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
        rotation = Matrix4.Identity;
        // ambient light preparation
        int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
        GL.UseProgram(shader.programID);
        GL.Uniform3(ambientID, 0.4f, 0.1f, 0.0f);
        // prepare scene
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

        // working object location
        //split up for easier debugging
        Matrix4 transform = Matrix4.Identity;
        transform *= Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
        transform *= Matrix4.CreateTranslation(0, -5, -15);
        transform *= rotation; // rotation before movement makes the player move in the direction the camera is facing.
        transform *= Matrix4.CreateTranslation(transLX, transLY, transLZ);
        toWorld = transform;
        transform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        world.mainTransform = transform;
        UpdateScene();

        // update rotation
        a += 0.001f * frameDuration;
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
        camera = new SceneObject(null, null, 0, null, cameraMatrix, toWorld, null);
        world = new SceneObject(null, null, 0, null, worldMatrix, toWorld, camera);

        tp = new SceneObject(teapot, shader, 1, wood, Matrix4.Identity, toWorld, world);
        fl = new SceneObject(floor, shader, 1, wood, Matrix4.CreateScale(1), toWorld, world);

        skypot = new SceneObject(teapot, skyshader, 0, skytex, Matrix4.CreateScale(100), toWorld, world);

        // sorry for the code
        light0 = new Light(0, new Vector3(0, 10, -10), new Vector3(10.0f, 10.0f, 10.0f), shader, Matrix4.Identity, toWorld, world);
        light1 = new Light(1, new Vector3(0, 0, 0), new Vector3(0.0f, 0.0f, 10.0f), shader, Matrix4.Identity, toWorld, world);
        light2 = new Light(2, new Vector3(-5, 0, 0), new Vector3(0.0f, 10.0f, 0.0f), shader, Matrix4.Identity, toWorld, world);
        light3 = new Light(3, new Vector3(5, 0, 0), new Vector3(10.0f, 0.0f, 0.0f), shader, Matrix4.Identity, toWorld, world);
    }

    public void UpdateScene()
    {
        skypot.mainTransform = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -a) * Matrix4.CreateTranslation(0, -3, 0) * Matrix4.CreateScale(10) ;
    }

    public void HandleInput()
    {
        KeyboardState k = Keyboard.GetState();
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
        if (k.IsKeyDown(Key.W) || k.IsKeyDown(Key.A) || k.IsKeyDown(Key.S) || k.IsKeyDown(Key.D) || k.IsKeyDown(Key.Z) || k.IsKeyDown(Key.X))
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
            if (k.IsKeyDown(Key.Z))
                transFZ += 0.01f;
            if (k.IsKeyDown(Key.X))
                transFZ -= 0.01f;
            rotation *= Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), transFX);
            rotation *= Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), transFY);
            rotation *= Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), transFZ);
        }
    }
}