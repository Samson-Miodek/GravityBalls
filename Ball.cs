using System;
using System.Drawing;
using System.Numerics;
using System.Threading;

namespace GravityBalls
{
	public class Ball
	{
		public Vector2 pos;
		public Vector2 vel;
		public Vector2 G;
		public SolidBrush color;
		public float radius;

		public static int K = -1;
		public static float dt;
		public Ball(float x, float y, float radius,Vector2 vel, SolidBrush color)
        {
            this.vel = vel;
            this.G = new Vector2(0, 9.8f * 30 * (radius));
            this.color = color;
            this.pos = new Vector2(x, y);
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
            var dist = new Vector2(Mouse.x - pos.X, Mouse.y - pos.Y);
            var vec = Vector2.Normalize(dist);

            if (dist.Length() < World.width * 0.3 && dist.Length() > 30)
            {
                vel += K * vec * 1000000 * dt / dist.Length();
                SimulateForceResistance(0.01f);
            }
        }

        private void UpdPosition()
        {
            pos += vel * dt;
        }

        private void SimulateForceResistance(float k)
        {
            vel -= vel * k;
        }

        private void SimulateForceGravity()
        {
            vel += G * dt;
        }

        private void CheckWalls()
        {
            var rightBorder = World.width - radius;
            var leftBorder = radius;
            var bottomBorder = World.height * 0.98f - radius * 2;
            var upperBorder = radius;

            if (pos.X > rightBorder)
            {
                pos.X = rightBorder;
                vel.X *= -1;
            }
            if (pos.X < leftBorder)
            {
                pos.X = leftBorder;
                vel.X *= -1;
            }
            if (pos.Y > bottomBorder)
            {
                pos.Y = bottomBorder;
                vel.Y *= -0.7f;
                vel.X -= vel.X * 0.01f;
            }
            if (pos.Y < upperBorder)
            {
                pos.Y = upperBorder;
                vel.Y *= -1;
            }
        }
    }
}