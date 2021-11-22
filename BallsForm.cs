using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Numerics;
using System.Collections.Generic;

namespace GravityBalls
{
	public class BallsForm : Form
	{
		private Timer timer;
		private List<Ball> balls;
		private Ball radiusBall;
		private Random random;
		private int numberBalls = 33;

		private int maxRadius = 12;
		private int minRadius = 6;

		private int maxVel = 1;
		private int minVel = -1;

		private bool IsMousePressed;
		private void UpdWorld()
        {
			World.height = ClientSize.Height;
			World.width = ClientSize.Width;
		}

        protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdWorld();
			radiusBall.radius = World.width * 0.3f;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			DoubleBuffered = true;
			BackColor = Color.Black;

			timer = new Timer { Interval = 10 };
			timer.Tick += TimerOnTick;
			timer.Start();

			UpdWorld();

			random = new Random();

			balls = new List<Ball>();

			for (int i = 0; i < numberBalls; i++)
			{
				var x = GetRandomNumber(ClientSize.Width);
				var y = GetRandomNumber(ClientSize.Height);
				var r = GetRandomNumber(minRadius,maxRadius);
				var color = GetColor();
				var vel = new Vector2(
					GetRandomNumber(minVel, maxVel),
					GetRandomNumber(minVel, maxVel));
				balls.Add(new Ball(x, y, r,vel,color)); 
			}
			radiusBall = new Ball(Mouse.x, Mouse.y, World.width * 0.3f, new Vector2(0, 0), new SolidBrush(Color.FromArgb(
											15,
											255,
											255,
											255)));
		}

		private float GetRandomNumber(int min, int max)
		{
			return (float)random.NextDouble() * (max - min) + min;
		}
		private float GetRandomNumber(int max)
		{
			return (float)random.NextDouble() * max;
		}
		public SolidBrush GetColor()
        {
			return new SolidBrush(Color.FromArgb(
											255,
											random.Next(0, 255),
											random.Next(0, 255),
											random.Next(0, 255)));

		}
		private void TimerOnTick(object sender, EventArgs eventArgs)
		{
			Ball.SimulateTimeframe(timer.Interval / 1500d);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;

            //рисует радиус притяжения/отдаления
            DrawRadiusBall(g);

			if (IsMousePressed)
				AddBall();

            for (int i = 0; i < balls.Count; i++)
            {
                var ball = balls[i];
                ball.Upd();
                DrawBall(g, ball);
                //CheckCollisions(i, ball);
            }
        }

        private void DrawRadiusBall(Graphics g)
        {
            radiusBall.position.X = Mouse.x;
            radiusBall.position.Y = Mouse.y;
            DrawBall(g, radiusBall);
        }

        //https://ru.wikipedia.org/wiki/%D0%A3%D0%B4%D0%B0%D1%80#%D0%90%D0%B1%D1%81%D0%BE%D0%BB%D1%8E%D1%82%D0%BD%D0%BE_%D1%83%D0%BF%D1%80%D1%83%D0%B3%D0%B8%D0%B9_%D1%83%D0%B4%D0%B0%D1%80_%D0%B2_%D0%BF%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%B5
        private void CheckCollisions(int i, Ball ball)
        {
            for (int j = i + 1; j < balls.Count; j++)
            {
                var ball2 = balls[j];
                var m = ball.radius + ball2.radius;
                var r1 = ball.radius;
                var r2 = ball2.radius;

                var v1 = ball.velocity;
                var v2 = ball2.velocity;
                var dist = (ball.position - ball2.position);

                if (dist.Length() <= r1 + r2)
                {
                    ball.velocity += (2 * r2 * v2) + v1 * (r1 - r2);
                    ball.velocity /= m;

                    ball2.velocity += (2 * r1 * v1) + v2 * (r2 - r1);
                    ball2.velocity /= m;

                    ball.position += Vector2.Normalize(dist) * 1.02f;
                    ball2.position -= Vector2.Normalize(dist) * 1.02f;
                }
            }
        }

        private static void DrawBall(Graphics graphics, Ball ball)
        {
            graphics.FillEllipse(ball.color,
                                ball.position.X - ball.radius,
                                ball.position.Y - ball.radius,
                                2 * ball.radius,
                                2 * ball.radius);
        }

        protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Text = string.Format("Cursor ({0}, {1})", e.X, e.Y);
			Mouse.x = e.X;
			Mouse.y = e.Y;
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.Left)
				Ball.Direction *= -1;
			if (e.Button == MouseButtons.Right)
				AddBall();
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.Right)
				IsMousePressed = true;
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.Right)
				IsMousePressed = false;
		}
		private void AddBall()
        {
			balls.Add(new Ball(Mouse.x, Mouse.y, GetRandomNumber(minRadius,maxRadius), new Vector2(0, 0), GetColor()));
        }
    }
}
