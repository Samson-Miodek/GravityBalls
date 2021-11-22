using System;
using System.Drawing;
using System.Numerics;
using System.Threading;

namespace GravityBalls
{
	public class Ball
	{
		public Vector2 position;
		public Vector2 velocity;
		public Vector2 G;
		public SolidBrush color;
		public float radius;

		public static int Direction = -1;
		public static float dt;
		public Ball(float x, float y, float radius,Vector2 velocity, SolidBrush color)
        {
            this.velocity = velocity;
            this.G = new Vector2(0, 9.8f * 30 * (radius));
            this.color = color;
            this.position = new Vector2(x, y);
            this.radius = radius;
        }


        public static void SimulateTimeframe(double deltaTime)
		{
			dt = (float)deltaTime;
		}

        public void Upd()
        {
            CheckWalls();
            SimulateForceRepulsionFromCursor();
            SimulateForceResistance(0.001f);
            SimulateForceGravity();
            UpdPosition();
        }

        private void SimulateForceRepulsionFromCursor()
        {
            var dist = new Vector2(Mouse.x - position.X, Mouse.y - position.Y);
            var vec = Vector2.Normalize(dist);

            if (dist.Length() < World.width * 0.3 && dist.Length() > 30)
            {
                velocity += Direction * vec * 1000000 * dt / dist.Length();
                SimulateForceResistance(0.01f);
            }
        }

        private void UpdPosition()
        {
            position += velocity * dt;
        }

        private void SimulateForceResistance(float k)
        {
            velocity -= velocity * k;
        }

        private void SimulateForceGravity()
        {
            velocity += G * dt;
        }

        private void CheckWalls()
        {
            var rightBorder = World.width - radius;
            var leftBorder = radius;
            var bottomBorder = World.height * 0.98f - radius * 2;
            var upperBorder = radius;

            if (position.X > rightBorder)
            {
                position.X = rightBorder;
                velocity.X *= -1;
            }
            if (position.X < leftBorder)
            {
                position.X = leftBorder;
                velocity.X *= -1;
            }
            if (position.Y > bottomBorder)
            {
                position.Y = bottomBorder;
                velocity.Y *= -0.7f;
                velocity.X -= velocity.X * 0.01f;
            }
            if (position.Y < upperBorder)
            {
                position.Y = upperBorder;
                velocity.Y *= -1;
            }
        }
    }
}
