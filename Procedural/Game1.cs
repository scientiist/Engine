using NLua;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        private void ApplyLightSource(MeshEntity entity, BasicEffect effect, ILightSource source)
		{
            Vector3 vtfo = (entity.Position - source.Position);

            float dot = Vector3.Dot(vtfo, source.Direction);

		}


        public void ApplyLightSources(MeshEntity entity, BasicEffect effect)
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

        public void Draw(GraphicsDevice g, Camera camera, BasicEffect effect)
        {

            effect.View = camera.View;
            effect.Projection = camera.ProjectionMatrix;

            effect.CurrentTechnique.Passes[0].Apply();
            var vertices = new[] { new VertexPositionColor(Position, Color), new VertexPositionColor(Position+Direction, Color) };
            g.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D defaultTexture;

        #region GameComponents
        BasicEffect effect;
        Camera camera;
        FrameCounter frametracker;
        #endregion

        #region GameObjects
        Floor floor;
        MeshEntity teapot;
        MeshEntity cube;

        DirectionalLight light;
        #endregion

        bool mouseLocked;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
            mouseLocked = true;
        }

        protected override void Initialize()
        {

            camera = new Camera(this, new Vector3(10, 1, 5), Vector3.Zero, 15f);
            Components.Add(camera);
            frametracker = new FrameCounter(this);
            Components.Add(frametracker);
            floor = new Floor(GraphicsDevice, 40, 40);
            effect = new BasicEffect(GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {

            defaultTexture = Content.Load<Texture2D>("default");

            light = new DirectionalLight();
            light.Direction = new Vector3(0, -0.3f, 0.5f);
            light.Color = Color.Red;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            TextRenderer.Initialize(Content);
            ShapeRenderer.Initialize(GraphicsDevice);
            teapot = new MeshEntity(Content.Load<Model>("Teapot"));
            teapot.Size = new Vector3(25, 25, 25);
            cube = new MeshEntity(Content.Load<Model>("cube"));

            cube.Position = new Vector3(0, 1, -5);
            cube.Size = new Vector3(2, 4, 2);
            cube.Rotation = new Vector3(45, 0, 45);
            cube.Texture = defaultTexture;
        }

        protected override void UnloadContent(){}

        private void TeapotUpdate(float dt)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.NumPad1))
                teapot.Position.X += dt * 5;
            if (keyboard.IsKeyDown(Keys.NumPad2))
                teapot.Position.X -= dt * 5;
            if (keyboard.IsKeyDown(Keys.NumPad3))
                teapot.Position.Y += dt * 5;
            if (keyboard.IsKeyDown(Keys.NumPad4))
                teapot.Position.Y -= dt * 5;
            if (keyboard.IsKeyDown(Keys.NumPad6))
                teapot.Position.Z += dt * 5;
            if (keyboard.IsKeyDown(Keys.NumPad7))
                teapot.Position.Z -= dt * 5;
            if (keyboard.IsKeyDown(Keys.Left))
                cube.Rotation.Z += dt;
            if (keyboard.IsKeyDown(Keys.Right))
                cube.Rotation.Z -= dt;

            if (keyboard.IsKeyDown(Keys.Up))
                cube.Position.Z += dt * 2;
            if (keyboard.IsKeyDown(Keys.Down))
                cube.Position.Z -= dt * 2;
        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            TeapotUpdate(dt);

            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboard.IsKeyDown(Keys.OemTilde))
            {
                if (mouseLocked == false) {
                    camera.MouseLock = !camera.MouseLock;
                    mouseLocked = true;
                }
            } else {
                mouseLocked = false;
            }
            base.Update(gameTime);
        }

        private void DrawGrid()
        {
            if (effect != null)
            {
                effect.View = camera.View;
                effect.Projection = camera.ProjectionMatrix;

                int camX = (int)Math.Floor(camera.Position.X/16)*16;
                float camY = (0);
                int camZ = (int)Math.Floor(camera.Position.Z/16)*16;

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

            

            ShapeRenderer.Rect(spriteBatch, new Color(0,0,0,0.5f), new Vector2(0, 0), new Vector2(200, 50));
            TextRenderer.Print(spriteBatch, String.Format("fps: {0} ", Math.Floor(frametracker.GetAverageFramerate())), new Vector2(2, 0), Color.White);
            TextRenderer.Print(spriteBatch, "campos"+ FormatVector(camera.Position), new Vector2(2, 12), Color.White);
            TextRenderer.Print(spriteBatch, "camlookat"+ FormatVector(camera.View.Forward), new Vector2(2, 24), Color.White);
            

            spriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            light.Draw(GraphicsDevice, camera, effect);
            teapot.Draw(GraphicsDevice, camera);
            cube.Draw(GraphicsDevice, camera);
            floor.Draw(camera, effect);

            DrawGrid();
            DrawDebugOverlay();

            base.Draw(gameTime);
        }
    }
}
