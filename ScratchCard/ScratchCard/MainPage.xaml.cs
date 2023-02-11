using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace ScratchCard
{
    public partial class MainPage : ContentPage
    { 
            Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
            List<SKPath> completedPaths = new List<SKPath>();
            string height;
            string width;

            //color is used to draw rectangle
            SKPaint color = new SKPaint
            {
                Color = SKColors.DarkOrange
            };

            SKRect rect = new SKRect();


            // paint is used to create skpath
            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
                BlendMode = SKBlendMode.Clear,
                StrokeWidth = 100,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round
            };

            public MainPage()
        {
            InitializeComponent();
        }

            //this method draws the rectangle and the paths..
            private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
            {
                SKCanvas canvas = e.Surface.Canvas;
                canvas.Clear();
                height = canvasView.Height.ToString();
                width = canvasView.Width.ToString();
                float w = float.Parse(width);
                float h = float.Parse(height);

                canvas.DrawRect(0, 0, 2000, 2400, color);

                var area = rect.Width * rect.Height;
                foreach (SKPath path in completedPaths)
                {
                    canvas.DrawPath(path, paint);
                    rect.Size = new SKSize(100, 100);
                    path.GetBounds(out rect);
                }

                foreach (SKPath path in inProgressPaths.Values)
                {
                    canvas.DrawPath(path, paint);
                    rect.Size = new SKSize(100, 100);
                    path.GetBounds(out rect);                   
                }


            }

            //this method when we touch or drag the finger on the screen while scratching..
            private void TouchEffect_TouchAction(object sender, ScratchCard.TouchActionEventArgs args)
            {
                switch (args.Type)
                {
                    case ScratchCard.TouchActionType.Pressed:
                        if (!inProgressPaths.ContainsKey(args.Id))
                        {
                            SKPath path = new SKPath();
                            path.MoveTo(ConvertToPixel(args.Location));
                            inProgressPaths.Add(args.Id, path);
                            canvasView.InvalidateSurface();
                        }
                        break;

                    case TouchActionType.Moved:
                        if (inProgressPaths.ContainsKey(args.Id))
                        {
                            SKPath path = inProgressPaths[args.Id];
                            path.LineTo(ConvertToPixel(args.Location));
                            canvasView.InvalidateSurface();
                        }
                        break;

                    case TouchActionType.Released:
                        if (inProgressPaths.ContainsKey(args.Id))
                        {
                            completedPaths.Add(inProgressPaths[args.Id]);
                            inProgressPaths.Remove(args.Id);

                            canvasView.InvalidateSurface();
                        }
                        break;

                    case TouchActionType.Cancelled:
                        if (inProgressPaths.ContainsKey(args.Id))
                        {
                            inProgressPaths.Remove(args.Id);
                            canvasView.InvalidateSurface();
                        }
                        break;
                }
            }

            //this method provide the runtime coordinates of the screen while drawing the paths.
            SKPoint ConvertToPixel(Point pt)
            {
                return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
            }

            
        }
}

