using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Procedural
{
    public class Camera : GameComponent
    {

        private Vector3 cameraPosition;
        private Vector3 cameraRotation;
        private float cameraSpeed;
        private Vector3 cameraLookAt;
        private Vector3 mouseRotationBuffer;
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private float fieldOfView = MathHelper.PiOver4;

        public bool MouseLock { get; set; }

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

        public Matrix ProjectionMatrix {
            get;
            protected set;
        }

        public Matrix View {
            get {
                return Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
            }
        }

        public Camera(Game game, Vector3 position, Vector3 rotation, float speed) : base(game) {
            cameraSpeed = speed;

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, Game.GraphicsDevice.Viewport.AspectRatio, 0.05f, 1000.0f);

            MoveTo(position, rotation);
            previousMouseState = Mouse.GetState();
            MouseLock = true;
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
                Position += new Vector3(-View.Forward.X, 0, View.Forward.Z) * dt * cameraSpeed;
            if (keyboard.IsKeyDown(Keys.S))
                Position -= new Vector3(-View.Forward.X, 0, View.Forward.Z) * dt * cameraSpeed;
            if (keyboard.IsKeyDown(Keys.A))
                Position += new Vector3(View.Forward.Z, 0, View.Forward.X) * dt * cameraSpeed;
            if (keyboard.IsKeyDown(Keys.D))
                Position -= new Vector3(View.Forward.Z, 0, View.Forward.X) * dt * cameraSpeed;
            if (keyboard.IsKeyDown(Keys.Q))
                Position -= new Vector3(0, 1, 0) * dt * cameraSpeed;
            if (keyboard.IsKeyDown(Keys.E))
                Position += new Vector3(0, 1, 0) * dt * cameraSpeed;

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

            if (currentMouseState != previousMouseState)
            {

                fieldOfView += (currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue) * dt * 0.01f;

                ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.Clamp(fieldOfView, 0.00001f, MathHelper.Pi - 0.00001f), Game.GraphicsDevice.Viewport.AspectRatio, 0.05f, 1000.0f);

                deltaX = currentMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
                deltaY = currentMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

                mouseRotationBuffer.X -= 0.015f * deltaX * dt;
                mouseRotationBuffer.Y -= 0.015f * deltaY * dt;

                if (mouseRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                {
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(-75.0f));
                }

                if (mouseRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                {
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - MathHelper.ToRadians(75.0f));
                }

                Rotation = new Vector3(-MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(mouseRotationBuffer.X), 0);

            }

            deltaX = 0;
            deltaY = 0;
            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);


            previousMouseState = currentMouseState;
        }

        public override void Update(GameTime gameTime) {
            Movement(gameTime);

            if (MouseLock)
                MouseControls(gameTime);
           


            base.Update(gameTime);
        }
    }
}
