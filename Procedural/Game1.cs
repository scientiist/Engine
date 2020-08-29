using NLua;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using JNgine;
using JNgine.Geometry;
using JNgine.Primitives;

namespace Procedural
{
    public interface ILightSource {
        Vector3 Position { get; set; }
        Vector3 Direction { get; set; }
        Color Color { get; set; }
        float Intensity { get; set; }
    }

    public class Lighting
	{
        public Color Ambient { get; set; }

        public float FogStart { get; set; }
        public float FogEnd { get; set; }
        public Color FogColor { get; set; }

        public Color SkyColor { get; set; }

        private List<ILightSource> lightSources;

        public ILightSource Source0 { get; set; }
        public ILightSource Source1 { get; set; }
        public ILightSource Source2 { get; set; }
        public Lighting()
		{

		}

        private void ApplyLightSource(Mesh entity, BasicEffect effect, ILightSource source)
		{
            Vector3 vtfo = (entity.Position - source.Position);

            float dot = Vector3.Dot(vtfo, source.Direction);

		}


        public void ApplyLightSources(Mesh entity, BasicEffect effect)
		{
            ApplyLightSource(entity, effect, Source0);
            ApplyLightSource(entity, effect, Source1);
            ApplyLightSource(entity, effect, Source2);
        }

	}

	public class PointLight : ILightSource
	{
		Vector3 ILightSource.Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		Vector3 ILightSource.Direction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		Color ILightSource.Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		float ILightSource.Intensity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}

	public class DirectionalLight: ILightSource
    {
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public Color Color { get; set; }
        public float Intensity { get; set; }

        public DirectionalLight()
        {
            Direction = Vector3.Up;
            Position = new Vector3(-4, 0, -4);
        }

        public void Draw(GraphicsDevice g, Camera camera)
        {

            Renderer.Line3D(camera, Position, Position + Direction, Color);
            //effect.View = camera.View;
           // effect.Projection = camera.Projection;

            //effect.CurrentTechnique.Passes[0].Apply();
           // var vertices = new[] { new VertexPositionColor(Position, Color), new VertexPositionColor(Position+Direction, Color) };
           // g.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }



    public interface IGameEnvironment
	{
        Color SkyColor { get; set; }

	}

    public class Game1 : Game
    {

        bool wireframeMode = false;
        public Color SkyColor { get; set; }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D defaultTexture;

        KeyboardState previousKB;

        #region GameComponents
        BasicEffect effect;
        public Camera Camera { get; private set; }
        public FrameCounter FrameTracker { get; private set; }
        public CommandBar Console { get; private set; }
        #endregion

        #region GameObjects
        Floor floor;
        Mesh teapot;
        Mesh cube;
        DirectionalLight light;
        Cube geomCube;
        Cylinder geomCylinder;
        Sphere geomSphere;
        Torus geomTorus;

        #endregion

        public Game1()
        {
            SkyColor = Color.CornflowerBlue;
            
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
 
            Window.AllowUserResizing = true;
            Window.Title = "JNgine Testbench";
        }

        protected override void Initialize()
        {
            
            Console = new CommandBar(this);
            Camera = new Camera(this, new Vector3(10, 1, 5), Vector3.Zero, 15f);
            FrameTracker = new FrameCounter(this);

            Components.Add(Camera);
            Components.Add(Console);
            Components.Add(FrameTracker);

            Window.TextInput += Console.OnTextInput;

            floor = new Floor(GraphicsDevice, 10, 10);
            floor.Color = new Color(1f, 1f, 1f);
            effect = new BasicEffect(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Renderer.Initialize(this);

            defaultTexture = Content.Load<Texture2D>("default");

            light = new DirectionalLight();
            light.Direction = new Vector3(0, -0.3f, 0.5f);
            light.Color = Color.Red;

            geomCube = new Cube(GraphicsDevice);
            geomCube.Position = new Vector3(2, 1, 0);
            geomCube.Color = new Color(1.0f, 0.0f, 0.0f);

            geomCylinder = new Cylinder(GraphicsDevice, 5);
            geomCylinder.Position = new Vector3(4, 1, 0);
            geomCylinder.Color = new Color(1.0f, 1.0f, 0.0f);

            geomSphere = new Sphere(GraphicsDevice, 3);
            geomSphere.Position = new Vector3(6, 1, 0);
            geomSphere.Color = new Color(0.0f, 1.0f, 0.0f);

            geomTorus = new Torus(GraphicsDevice);
            geomTorus.Position = new Vector3(8, 1, 0);
            geomTorus.Color = new Color(0.0f, 1.0f, 1.0f);

            teapot = new Mesh(Content.Load<Model>("Teapot")) {
                Size = new Vector3(0.5f, 0.5f, 0.5f),
                Position = new Vector3(10, 0, 0),
                Texture = defaultTexture
            };
            

            cube = new Mesh(Content.Load<Model>("cube")) {
                Position = new Vector3(0, 1, -5),
                Size = new Vector3(2, 4, 2),
                Rotation = new Vector3(45, 0, 45),
                Texture = defaultTexture
            };

            
        }

        protected override void UnloadContent(){}

        private void TeapotUpdate(float dt){}

        protected override void Update(GameTime gameTime)
        {

            Camera.Enabled = !Console.Open;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TeapotUpdate(dt);

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.F1) && !previousKB.IsKeyDown(Keys.F1))
                wireframeMode = !wireframeMode;



            previousKB = keyboard;

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        private void DrawGrid()
        {
            if (effect != null)
            {
                effect.View = Camera.View;
                effect.Projection = Camera.Projection;

                int camX = (int)Math.Floor(Camera.Position.X/16)*16;
                float camY = (0);
                int camZ = (int)Math.Floor(Camera.Position.Z/16)*16;

                for (int x = 0; x < 128; x++)
                {
                    Vector3 start = new Vector3(camX+x-64,camY,camZ-64);
                    Vector3 end = new Vector3(camX+x-64,camY,camZ+64);
                    effect.CurrentTechnique.Passes[0].Apply();
                    var vertices = new[] { new VertexPositionColor(start, Color.Black), new VertexPositionColor(end, Color.Black) };
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
                }
                for (int z = 0; z < 128; z++)
                {
                    Vector3 start = new Vector3(camX-64,camY,camZ+z-64);
                    Vector3 end = new Vector3(camX+64,camY,camZ+z-64);
                    effect.CurrentTechnique.Passes[0].Apply();
                    var vertices = new[] { new VertexPositionColor(start, Color.White), new VertexPositionColor(end, Color.White) };
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
                }
            }
        }

        private static string FormatVector(Vector3 vec)
        {
            int deci = 2;
            float x = (float)Math.Round(vec.X, deci);
            float y = (float)Math.Round(vec.Y, deci);
            float z = (float)Math.Round(vec.Z, deci);

            return "{"+x.ToString()+", "+y.ToString()+", "+z.ToString()+"}";
        }


        private void DrawDebugOverlay()
        {
            spriteBatch.Begin();

            spriteBatch.Rect(new Color(0, 0, 0, 0.5f), new Vector2(0, 0), new Vector2(200, 50));
            spriteBatch.Print(Color.White, new Vector2(2, 0), String.Format("fps: {0} ", Math.Floor(FrameTracker.GetAverageFramerate())));
            spriteBatch.Print(Color.White, new Vector2(2, 12), "campos" + FormatVector(Camera.Position));
            spriteBatch.Print(Color.White, new Vector2(2, 24), "camlookat" + FormatVector(Camera.View.Forward));

            spriteBatch.End();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(SkyColor);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            if (wireframeMode)
			{
                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.FillMode = FillMode.WireFrame;
                GraphicsDevice.RasterizerState = rasterizerState;
            } else
			{
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			}
            

            light.Draw(GraphicsDevice, Camera);
            teapot.Draw(GraphicsDevice, Camera);
            //cube.Draw(GraphicsDevice, Camera);
            geomCube.Draw(GraphicsDevice, Camera);
            geomCylinder.Draw(GraphicsDevice, Camera);
            geomSphere.Draw(GraphicsDevice, Camera);
            geomTorus.Draw(GraphicsDevice, Camera);
            floor.Draw(GraphicsDevice, Camera);

            DrawGrid();
            DrawDebugOverlay();

            Console.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
