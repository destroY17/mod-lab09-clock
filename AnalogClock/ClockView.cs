using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace AnalogClock
{
    public partial class ClockView : Form
    {
        private const int CLOCK_WORKING_RADIUS = 200;
        private const int CLOCK_BLOCK = 20;
        private const int CLOCK_RADIUS = CLOCK_WORKING_RADIUS + CLOCK_BLOCK;
        private const int MINI_CLOCK_RADIUS = 100;

        private const int SECOND_HAND_LENGTH = (int)(CLOCK_WORKING_RADIUS * 0.8);
        private const int MINUTE_HAND_LENGTH = (int)(CLOCK_WORKING_RADIUS * 0.6);
        private const int HOUR_HAND_LENGTH = (int)(CLOCK_WORKING_RADIUS * 0.4);

        private const int HOUR_DIVISION_LENGTH = 30;
        private const int MINUTE_DIVISION_LENGTH = 15;

        private const int HANDS_OFFSET = -30;

        private readonly Font bigFont_ = new Font("Comic Sans MS", CLOCK_WORKING_RADIUS / 7f, FontStyle.Bold);
        private readonly Font miniFont_ = new Font("Comic Sans MS", CLOCK_WORKING_RADIUS / 11f, FontStyle.Bold);

        private readonly Pen dialPen_ = new Pen(Color.FromArgb(222, 184, 135), CLOCK_BLOCK);
        private readonly Pen miniDialPen_ = new Pen(Color.FromArgb(222, 184, 135), 5);

        private readonly Pen secondHandPen_ = new Pen(Color.Red, 2);
        private readonly Pen minuteHandPen = new Pen(Color.Black, 3);
        private readonly Pen hourHandPen_ = new Pen(Color.Black, 10);

        private readonly Pen minuteDivisionPen_ = new Pen(Color.Black, 2);
        private readonly Pen hourDivisionPen_ = new Pen(Color.Black, 3);

        public ClockView()
        {
            InitializeComponent();

            DoubleBuffered = true;
            timer.Enabled = true;
        }

        private void ClockView_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            GraphicsState gs;

            g.TranslateTransform(ClientSize.Width / 2, ClientSize.Height / 2);

            g.DrawEllipse(dialPen_, -CLOCK_RADIUS, -CLOCK_RADIUS, CLOCK_RADIUS * 2, CLOCK_RADIUS * 2);

            g.DrawEllipse(miniDialPen_, -MINI_CLOCK_RADIUS, -MINI_CLOCK_RADIUS,
                MINI_CLOCK_RADIUS * 2, MINI_CLOCK_RADIUS * 2);
            g.FillEllipse(new SolidBrush(miniDialPen_.Color), -MINI_CLOCK_RADIUS, -MINI_CLOCK_RADIUS,
                MINI_CLOCK_RADIUS * 2, MINI_CLOCK_RADIUS * 2);

            DrawMinutesDivisions(g);
            DrawHourDivisions(g);

            var current_time = DateTime.Now;

            //draw second hand
            gs = g.Save();
            g.RotateTransform(6 * current_time.Second + 180);
            g.DrawLine(secondHandPen_, 0, HANDS_OFFSET, 0, SECOND_HAND_LENGTH);
            g.Restore(gs);

            //draw minute hand
            gs = g.Save();
            g.RotateTransform(6 * current_time.Minute + current_time.Second / 10f + 180);
            g.DrawLine(minuteHandPen, 0, HANDS_OFFSET, 0, MINUTE_HAND_LENGTH);
            g.Restore(gs);

            //draw hour hand
            gs = g.Save();
            g.RotateTransform(30 * (current_time.Hour % 12) + current_time.Minute / 2f + current_time.Second / 120f + 180);
            g.DrawLine(hourHandPen_, 0, HANDS_OFFSET, 0, HOUR_HAND_LENGTH);
            g.Restore(gs);
        }

        private void DrawHourDivisions(Graphics g)
        {
            var divisions_count = 12;

            for (int hour = 1; hour <= divisions_count; hour++)
            {
                float angle = 30 * hour - 90;
                float radians = (float)(angle * Math.PI / 180);

                var startPoint = new Point
                {
                    X = (int)(CLOCK_WORKING_RADIUS * Math.Cos(radians)),
                    Y = (int)(CLOCK_WORKING_RADIUS * Math.Sin(radians))
                };

                var endPoint = new Point
                {
                    X = (int)((CLOCK_WORKING_RADIUS - HOUR_DIVISION_LENGTH) * Math.Cos(radians)),
                    Y = (int)((CLOCK_WORKING_RADIUS - HOUR_DIVISION_LENGTH) * Math.Sin(radians))
                };

                g.DrawLine(hourDivisionPen_, startPoint, endPoint);

                if (hour % 3 != 0)
                {
                    g.DrawString(hour.ToString(), miniFont_, Brushes.Black, startPoint.X * 0.75f, startPoint.Y * 0.75f, new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    });
                }
                else
                {
                    g.DrawString(hour.ToString(), bigFont_, Brushes.Black, startPoint.X * 0.75f, startPoint.Y * 0.75f, new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    });
                }
            }
        }

        private void DrawMinutesDivisions(Graphics g)
        {
            var divisions_count = 60;

            for (int minute = 0; minute < divisions_count; minute++)
            {
                double angle = 6 * minute;
                double radians = angle * Math.PI / 180;

                var startPoint = new Point
                {
                    X = (int)(CLOCK_WORKING_RADIUS * Math.Sin(radians)),
                    Y = (int)(CLOCK_WORKING_RADIUS * Math.Cos(radians))
                };

                var endPoint = new Point
                {
                    X = (int)((CLOCK_WORKING_RADIUS - MINUTE_DIVISION_LENGTH) * Math.Sin(radians)),
                    Y = (int)((CLOCK_WORKING_RADIUS - MINUTE_DIVISION_LENGTH) * Math.Cos(radians))
                };

                g.DrawLine(minuteDivisionPen_, startPoint, endPoint);
            }
        }

        private void timer_Tick(object sender, EventArgs e) => Invalidate();
    }
}
