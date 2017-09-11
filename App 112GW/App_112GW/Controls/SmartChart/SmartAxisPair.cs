using SkiaSharp;

namespace rMultiplatform
{
    abstract class ASmartAxisPair : ASmartElement
    {
        public ASmartData Parent;
        public ASmartAxis Horizontal, Vertical;
        public abstract SKMatrix Transform { get; }

        public void Reset()
        {
            Horizontal.Range.Reset();
            Vertical.Range.Reset();
        }
        public void Zoom(SKPoint Amount, SKPoint About)
        {
            Horizontal.Range.Zoom(Amount.X, About.X);
            Vertical.Range.Zoom(Amount.Y, About.Y);
        }
        public void Pan(SKPoint Amount)
        {
            Horizontal.Range.Pan(Amount.X);
            Vertical.Range.Pan(Amount.Y);
        }
        public void Set(SKRect Boundary)
        {
            Horizontal.Range.Set(Boundary.Left, Boundary.Right);
            Vertical.Range.Set(Boundary.Top, Boundary.Bottom);
        }

        public abstract void Draw(SKCanvas Canvas, SKSize dimension);
    }

    class SmartAxisPair : ASmartAxisPair
    {
        SmartAxisPair(ASmartAxis pHorizontal, ASmartAxis pVertical)
        {
            Horizontal = pHorizontal;
            Vertical = pVertical;
        }

        //Takes a SKPath and transforms it based on the transformations present in the axis
        public override SKMatrix Transform
        {
            get
            {
                var matrix = SKMatrix.MakeIdentity();
                matrix.ScaleX   = Horizontal.Scaling;
                matrix.TransX   = Horizontal.Translation;
                matrix.ScaleY   = Vertical.Scaling;
                matrix.TransY   = Vertical.Translation;
                return matrix;
            }
        }

        public override void Draw(SKCanvas Canvas, SKSize dimension)
        {
            Horizontal.Draw(Canvas, dimension);
            Vertical.Draw(Canvas, dimension);
        }
    }
}