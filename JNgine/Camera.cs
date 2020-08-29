using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNgine
{
    public class Camera : GameComponent
    {

        private Vector3 cameraPosition;
        private Vector3 cameraRotation;
        
        private Vector3 cameraLookAt;
        private Vector3 mouseRotationBuffer;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        protected float fieldOfView = MathHelper.PiOver4;


        public Color Ambient { get; set; }
        public Color FogColor { get; set; }
        public float FogStart { get; set; }
        public float FogEnd { get; set; }
        public float ViewDistance { get; set; }

        public float MouseRotationScaleX { get; set; }
        public float MouseRotationScaleY { get; set; }

        public float MoveSpeed { get; set; }

        public float FieldOfView
		{
			get { return MathHelper.ToDegrees(fieldOfView); }
            set { 
                fieldOfView = MathHelper.ToRadians(value);
                UpdateProjectionMatrix();
            }
		}
        public bool MouseLock { get; set; }
        private bool mouseDown;
        private Vector2 mouseDownPos;
        public Vector3 Position {
            get { return cameraPosition; }
            set {
                cameraPosition = value;
                UpdateLookAt();
            }
        }
        public Vector3 Rotation {
            get { return cameraRotation; }
            set {
                cameraRotation = value;
                UpdateLookAt();
            }
        }

        protected virtual void UpdateProjectionMatrix()
        {

            float cappedFOV = (float)Math.Max(fieldOfView, 0.01f);
            cappedFOV = (float)Math.Min(cappedFOV, Math.PI);
            Projection = Matrix.CreatePerspectiveFieldOfView(cappedFOV, Game.GraphicsDevice.Viewport.AspectRatio, 0.05f, 1000.0f);
        }

        public Matrix Projection {
            get;
            protected set;
        }

        public Matrix View {
            get {
                return Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
            }
        }

        public Camera(Game game, Vector3 position, Vector3 rotation, float speed) : base(game) {
            MoveSpeed = speed;

            ViewDistance = 1000.0f;
            MouseRotationScaleX = 1;
            MouseRotationScaleY = 1;

            Ambient = new Color(0.0f, 0.0f, 0.0f);
            FogColor = new Color(1.0f, 1.0f, 1.0f);
            FogStart = 10;
            FogEnd = 20;

            UpdateProjectionMatrix();

            MoveTo(position, rotation);
            previousMouseState = Mouse.GetState();
            MouseLock = true;
            mouseDown = false;
            mouseDownPos = new Vector2(0, 0);
        }

        private void MoveTo(Vector3 pos, Vector3 rot) {
            Position = pos;
            Rotation = rot;
        }

        private void UpdateLookAt() {
            Matrix rotationMatrix = Matrix.CreateRotationX(cameraRotation.X) * Matrix.CreateRotationY(cameraRotation.Y);// * Matrix.CreateRotationZ(cameraRotation.Z);

            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);

            cameraLookAt = cameraPosition + lookAtOffset;
        }

        private void Movement(GameTime gameTime) {

            KeyboardState keyboard = Keyboard.GetState();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboard.IsKeyDown(Keys.LeftControl)) {
                dt /= 4;
            }
            if (keyboard.IsKeyDown(Keys.LeftShift)) {
                dt *= 4;
            }

            Vector3 moveVector = Vector3.Zero;

            if (keyboard.IsKeyDown(Keys.W))
                Position += new Vector3(-View.Forward.X, 0, View.Forward.Z) * dt * MoveSpeed;
            if (keyboard.IsKeyDown(Keys.S))
                Position -= new Vector3(-View.Forward.X, 0, View.Forward.Z) * dt * MoveSpeed;
            if (keyboard.IsKeyDown(Keys.A))
                Position += new Vector3(View.Forward.Z, 0, View.Forward.X) * dt * MoveSpeed;
            if (keyboard.IsKeyDown(Keys.D))
                Position -= new Vector3(View.Forward.Z, 0, View.Forward.X) * dt * MoveSpeed;
            if (keyboard.IsKeyDown(Keys.Q))
                Position -= new Vector3(0, 1, 0) * dt * MoveSpeed;
            if (keyboard.IsKeyDown(Keys.E))
                Position += new Vector3(0, 1, 0) * dt * MoveSpeed;

            if (moveVector != Vector3.Zero) {
             //   moveVector.Normalize(); // normalize vec to stop diagonal speed boost
                //moveVector *= dt * cameraSpeed;
              //  Position += (moveVector);
            }
        }


        private void MouseControls(GameTime gameTime)
        {

            currentMouseState = Mouse.GetState();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float deltaX;
            float deltaY;

            if (currentMouseState.RightButton == ButtonState.Pressed) {
                if (mouseDown == false)
				{
                    mouseDownPos = new Vector2(currentMouseState.X, currentMouseState.Y);
                    mouseDown = true;
				}

                deltaX = currentMouseState.X - mouseDownPos.X;
                deltaY = currentMouseState.Y - mouseDownPos.Y;


                Mouse.SetPosition((int)mouseDownPos.X, (int)mouseDownPos.Y);
                
                mouseRotationBuffer.X -= 0.005f * deltaX * MouseRotationScaleX;
                mouseRotationBuffer.Y -= 0.005f * deltaY * MouseRotationScaleY;

                if (mouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                {
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                }

                if (mouseRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                {
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(75.0f));
                }

                Rotation = new Vector3(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(mouseRotationBuffer.X), 0);

            } else
			{
                mouseDown = false;
            }

            FieldOfView -= (currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue)*0.01f;
            previousMouseState = currentMouseState;
            deltaX = 0;
            deltaY = 0;

            
        }

        public override void Update(GameTime gameTime) {
            Movement(gameTime);

            if (MouseLock)
                MouseControls(gameTime);
           


            base.Update(gameTime);
        }
    }
}
