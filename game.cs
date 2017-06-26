using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using OpenTK.Input;
using System.Collections.Generic;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

class Game
{
    // member variables
    public Surface screen;                  // background surface for printing etc.
    Mesh teapot, floor, city, scraper;                     // a mesh to draw using OpenGL
    const float PI = 3.1415926535f;         // PI
    float a = 0;                            // teapot rotation angle
    Stopwatch timer;                        // timer for measuring frame duration
    Shader shader, skyshader;                          // shader to use for rendering
    Shader postproc;                        // shader to use for post processing
    Texture wood, skytex, windows, asphalt, skyscraper;                           // texture to use for rendering
    RenderTarget target;                    // intermediate render target
    ScreenQuad quad;                        // screen filling quad for post processing
    bool useRenderTarget = true;

    SceneGraph sceneGraph;                  // create new scenegraph
    public static SceneObject camera, world, tp, tp0, tp1, tp2, town, skypot, road, skycrap;      // Used sceneobjects
    List<SceneObject> teapots;              // list of all teapots in scene
    Light light0, light1, light2, light3;

    Matrix4 cameraMatrix;                   // moving the camera                    
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
        city = new Mesh("../../assets/city.obj");
        scraper = new Mesh("../../assets/scraper.obj");
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
        windows = new Texture("../../assets/city.jpg");
        asphalt = new Texture("../../assets/asphalt.jpg");
        skyscraper = new Texture("../../assets/skyscraper.jpg");
        // create the render target
        target = new RenderTarget(screen.width, screen.height);
        quad = new ScreenQuad();
        // create scenegraph and main object
        sceneGraph = new SceneGraph();
        teapots = new List<SceneObject>();
        // initialize matrices
        cameraMatrix = Matrix4.Identity;
        worldMatrix = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
        rotation = Matrix4.Identity;
        // ambient light preparation
        int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
        GL.UseProgram(shader.programID);
        GL.Uniform3(ambientID, 0.25f, 0.25f, 0.25f);
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
        //transform *= Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), -a*2f);
        transform *= Matrix4.CreateTranslation(-400, -300, -1100);
        transform *= rotation; // rotation before movement makes the player move in the direction the camera is facing.
        transform *= Matrix4.CreateTranslation(transLX, transLY, transLZ);
        toWorld = transform;
        transform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 100000);
        world.mainTransform = transform;
        UpdateScene();

        // update rotation
        a += 0.001f * frameDuration;
        if (a > 1000 * PI) a -= 1000 * PI;

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
        town = new SceneObject(city, skyshader, 1, windows, Matrix4.CreateScale(1) * Matrix4.CreateTranslation(0, -500, 0), toWorld, world);
        road = new SceneObject(floor, skyshader, 0, asphalt, Matrix4.CreateTranslation(-3, 0, -8) * Matrix4.CreateScale(135, 0, 100) , toWorld, town);
        skycrap = new SceneObject(teapot, shader, 1, skyscraper, Matrix4.CreateRotationY(-PI/4) * Matrix4.CreateTranslation(26, 3.1f, 12.5f) * Matrix4.CreateScale(30), toWorld, world);

        tp0 = new SceneObject(teapot, shader, 0, wood, Matrix4.Identity, toWorld, tp);
        tp1 = new SceneObject(teapot, shader, 0, wood, Matrix4.Identity, toWorld, tp);
        tp2 = new SceneObject(teapot, shader, 0, wood, Matrix4.Identity, toWorld, tp);
        teapots.Add(tp0); teapots.Add(tp1); teapots.Add(tp2);

        for (int i = 1; i < 100; i++)
        {
            teapots.Add(new SceneObject(teapot, shader, 1, wood, Matrix4.CreateTranslation(-5, i * 2, 5) * Matrix4.CreateRotationY(PI / 8), toWorld, teapots[3 * i - 3]));
            teapots.Add(new SceneObject(teapot, shader, 1, wood, Matrix4.CreateTranslation(20, i * 2, 5) * Matrix4.CreateRotationY(PI / 8), toWorld, teapots[3 * i - 2]));
            teapots.Add(new SceneObject(teapot, shader, 1, wood, Matrix4.CreateTranslation(-20, i * 2, 5) * Matrix4.CreateRotationY(PI / 8), toWorld, teapots[3 * i - 1]));
        }

        skypot = new SceneObject(teapot, skyshader, 0, skytex, Matrix4.CreateScale(10), toWorld, world);

        // sorry for the code
        light0 = new Light(0, new Vector3(100, 100, 0), new Vector3(2.0f, 2.0f, 2.0f), shader, Matrix4.Identity, toWorld, world);
        light1 = new Light(1, new Vector3(-100, 100, 0), new Vector3(2.0f, 2.0f, 2.0f), shader, Matrix4.Identity, toWorld, world);
        light2 = new Light(2, new Vector3(0, 100, 100), new Vector3(2.0f, 2.0f, 2.0f), shader, Matrix4.Identity, toWorld, world);
        light3 = new Light(3, new Vector3(0, 100, -100), new Vector3(2.0f, 2.0f, 2.0f), shader, Matrix4.Identity, toWorld, world);
    }

    public void UpdateScene()
    {
        skypot.mainTransform = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0.01f * a) * Matrix4.CreateTranslation(0, -3, 0) * Matrix4.CreateScale(1000);
        tp.mainTransform = Matrix4.CreateScale(2f) * Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 5 * a);
        town.mainTransform = Matrix4.CreateTranslation(450, -100, 600);
    }

    public void HandleInput()
    {
        KeyboardState k = Keyboard.GetState();
        if (k.IsKeyDown(Key.Up))
            transLY -= 1f;
        if (k.IsKeyDown(Key.Down))
            transLY += 1f;
        if (k.IsKeyDown(Key.Left))
            transLX += 1f;
        if (k.IsKeyDown(Key.Right))
            transLX -= 1f;
        if (k.IsKeyDown(Key.Plus))
            transLZ += 1f;
        if (k.IsKeyDown(Key.Minus))
            transLZ -= 1f;
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