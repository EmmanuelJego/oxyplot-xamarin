using System;
using System.Linq;
using Android.Views;

namespace OxyPlot.Xamarin.Android
{
    internal class PanPinchGestureListener : GestureDetector.SimpleOnGestureListener
    {
        private ScreenPoint _previousLeftPoint, _previousRightPoint;

        private double _screenScale;
        public event EventHandler<PanEventArgs> OnPan;
        public event EventHandler<PinchEventArgs> OnPinch;

        public PanPinchGestureListener(double screenScale = 1)
        {
            _screenScale = screenScale;
        }

        public override bool OnDown(MotionEvent e)
        {
            _previousLeftPoint = new ScreenPoint(0, 0);
            _previousRightPoint = new ScreenPoint(0, 0);
            return base.OnDown(e);
        }

        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            if (e2.PointerCount == 1)
            {
                // Pan
                if (Math.Abs(distanceX) > Math.Abs(distanceY))
                {
                    OnPan?.Invoke(this, new PanEventArgs(-distanceX));
                }
            }
            else if (e2.PointerCount == 2)
            {
                // Pinch
                var points = e2.GetTouchPoints(_screenScale);
                var leftPoint = points.First(point => point.X == points.Min(point => point.X));
                var rightPoint = points.First(point => point.X == points.Max(point => point.X));

                var d = leftPoint - rightPoint;
                var pd = _previousLeftPoint - _previousRightPoint;

                var scale = pd.Length > 0 ? d.Length / pd.Length : 1;

                _previousLeftPoint = leftPoint;
                _previousRightPoint = rightPoint;

                OnPinch?.Invoke(this, new PinchEventArgs(scale));
            }

            return base.OnScroll(e1, e2, distanceX, distanceY);
        }
    }

    public class PanEventArgs : EventArgs
    {
        public double DeltaX { get; protected set; }

        public PanEventArgs(double deltaX)
        {
            DeltaX = deltaX;
        }
    }

    public class PinchEventArgs : EventArgs
    {
        public double DeltaScale { get; protected set; }

        public PinchEventArgs(double deltaScale)
        {
            DeltaScale = deltaScale;
        }
    }
}