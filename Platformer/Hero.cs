using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace Platformer
{
    public class Hero : Entity
    {
        public const float WalkSpeed = 100.0f;
        public const float JumpForce = 250.0f;
        public const float GravityForce = 400.0f;

        private bool faceRight = false;
        private float verticalSpeed;
        private bool isGrounded;
        private bool isUpPressed;
        private bool firstFrame = true;
        private float animationTime;

        public Hero() : base("characters")
        {
            sprite.TextureRect = new IntRect(0, 0, 24, 24);
            sprite.Origin = new Vector2f(12, 12);
        }

        public override FloatRect Bounds 
        {
            get
            {
                var bounds = base.Bounds;
                bounds.Left += 3;
                bounds.Width -= 6;
                bounds.Top += 3;
                bounds.Height -= 3;
                return bounds;
            }
        }

        private void Animation(float deltaTime) //Uses deltaTime with a timer that executes the frame switches
        {
            animationTime += deltaTime;

            if (animationTime > 1/10f)
            {
                if (firstFrame)
                {
                    sprite.TextureRect = new IntRect(24, 0, 24, 24);
                }else if (!firstFrame)
                {
                    sprite.TextureRect = new IntRect(0, 0, 24, 24);
                }

                animationTime = 0;
                firstFrame = !firstFrame;
            }
        }

        public override void Update(Scene scene, float deltaTime)   //For now, just movement and movement control, mainly jump control
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                scene.TryMove(this, new Vector2f(-WalkSpeed * deltaTime, 0));
                faceRight = false;
                Animation(deltaTime);
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
            {
                scene.TryMove(this, new Vector2f(WalkSpeed * deltaTime, 0));
                faceRight = true;
                Animation(deltaTime);
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                if (isGrounded && !isUpPressed)
                {
                    verticalSpeed = -JumpForce;
                    isUpPressed = true;
                }
                
            }else
            {
                isUpPressed = false;
            }

            verticalSpeed += GravityForce * deltaTime;
            if (verticalSpeed > 500.0f)
            {
                verticalSpeed = 500.0f;
            }

            isGrounded = false;
            
            Vector2f velocity = new Vector2f(0, verticalSpeed * deltaTime);
            if (scene.TryMove(this, velocity))
            {
                if (verticalSpeed > 0.0f)
                {
                    isGrounded = true;
                    verticalSpeed = 0.0f;
                }else
                {
                    verticalSpeed = -0.5f * verticalSpeed;
                }
               
            }

            if (Position.X > 800 || Position.Y > 600 || Position.X < 0 || Position.Y < 0)
            {
                scene.Reload();
            }
        }

        public override void Render(RenderTarget target)
        {
            sprite.Scale = new Vector2f(faceRight ? -1 : 1, 1);
            base.Render(target);
        }
    }
}
