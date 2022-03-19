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

			for (var i = 0; i < numberBalls; i++)
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

            for (var i = 0; i < balls.Count; i++)
            {
                var ball = balls[i];
                ball.Upd();
                DrawBall(g, ball);
                CheckCollisions(i, ball);
            }
        }

        private void DrawRadiusBall(Graphics g)
        {
            radiusBall.position.X = Mouse.x;
            radiusBall.position.Y = Mouse.y;
            DrawBall(g, radiusBall);
        }

        private void CheckCollisions(int i, Ball ball)
        {
            for (var j = i+1; j < balls.Count; j++)
            {
                var ball2 = balls[j];	
                
                var distance = ball.position-ball2.position;
                var length = distance.Length();

                if(length<(ball.radius+ball2.radius))
                {
	                var f = 150*ball.radius * ball2.radius / (length * length);
	                ball.velocity += distance*f/ball.radius;
	                ball2.velocity -= distance*f/ball2.radius;
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
