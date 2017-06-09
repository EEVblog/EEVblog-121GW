using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Reflection;
using System.Resources;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

namespace App_112GW
{
	public class SevenSegment
	{
		private enum		SevenSegmentLetters
		{
			ea = 95,
			eb = 124,
			ec = 88,
			ed = 94,
			ee = 123,
			ef = 113,
			eg = 111,
			eh = 116,
			ei = 4,
			ej = 14,
			ek = 122,
			el = 6,
			em = 20,
			en = 84,
			eo = 92,
			ep = 115,
			eq = 103,
			er = 80,
			es = 109,
			et = 120,
			eu = 28,
			ev = 98,
			ew = 42,
			ex = 100,
			ey = 110,
			ez = 91,
			e0 = 0x3F,
			e1 = 0x6,
			e2 = 0x5B,
			e3 = 0x4F,
			e4 = 0x66,
			e5 = 0x6D,
			e6 = 0x7d,
			e7 = 0x7,
			e8 = 0x7F,
			e9 = 0x6F,
			eA = 0x77,
			eB = 0x7F,
			eC = 0x39,
			eD = 0x3F,
			eE = 0x79,
			eF = 0x71,
			eG = 0x3D,
			eH = 0x76,
			eI = 0x06,
			eJ = 0x1E,
			eK = 0x57,
			eL = 0x38,
			eM = 0x76,
			eN = 0x76,
			eO = 0x3F,
			eP = 0x73,
			eQ = 0x3F,
			eR = 0x77,
			eS = 0x6D,
			eT = 0x31,
			eU = 0x3E,
			eV = 0x3E,
			eW = 0x7E,
			eX = 0x76,
			eY = 0x66,
			eZ = 0x5B,
			eDot = 0x80
		};
		private static char	ToUpper		(char pCHR)
		{
			const char a = 'a';
			const char A = 'A';
			if (('a' <= pCHR) && (pCHR <= 'z'))
				return (char)((pCHR - a) + A);

			return pCHR;
		}
		private static int	ToSegment	(char pCHR)
		{
			switch (pCHR)
			{

				case '0': return (int)SevenSegmentLetters.e0;
				case '1': return (int)SevenSegmentLetters.e1;
				case '2': return (int)SevenSegmentLetters.e2;
				case '3': return (int)SevenSegmentLetters.e3;
				case '4': return (int)SevenSegmentLetters.e4;
				case '5': return (int)SevenSegmentLetters.e5;
				case '6': return (int)SevenSegmentLetters.e6;
				case '7': return (int)SevenSegmentLetters.e7;
				case '8': return (int)SevenSegmentLetters.e8;
				case '9': return (int)SevenSegmentLetters.e9;
				case 'a': return (int)SevenSegmentLetters.ea;
				case 'b': return (int)SevenSegmentLetters.eb;
				case 'c': return (int)SevenSegmentLetters.ec;
				case 'd': return (int)SevenSegmentLetters.ed;
				case 'e': return (int)SevenSegmentLetters.ee;
				case 'f': return (int)SevenSegmentLetters.ef;
				case 'g': return (int)SevenSegmentLetters.eg;
				case 'h': return (int)SevenSegmentLetters.eh;
				case 'i': return (int)SevenSegmentLetters.ei;
				case 'j': return (int)SevenSegmentLetters.ej;
				case 'k': return (int)SevenSegmentLetters.ek;
				case 'l': return (int)SevenSegmentLetters.el;
				case 'm': return (int)SevenSegmentLetters.em;
				case 'n': return (int)SevenSegmentLetters.en;
				case 'o': return (int)SevenSegmentLetters.eo;
				case 'p': return (int)SevenSegmentLetters.ep;
				case 'q': return (int)SevenSegmentLetters.eq;
				case 'r': return (int)SevenSegmentLetters.er;
				case 's': return (int)SevenSegmentLetters.es;
				case 't': return (int)SevenSegmentLetters.et;
				case 'u': return (int)SevenSegmentLetters.eu;
				case 'v': return (int)SevenSegmentLetters.ev;
				case 'w': return (int)SevenSegmentLetters.ew;
				case 'x': return (int)SevenSegmentLetters.ex;
				case 'y': return (int)SevenSegmentLetters.ey;
				case 'z': return (int)SevenSegmentLetters.ez;
				case 'A': return (int)SevenSegmentLetters.eA;
				case 'B': return (int)SevenSegmentLetters.eB;
				case 'C': return (int)SevenSegmentLetters.eC;
				case 'D': return (int)SevenSegmentLetters.eD;
				case 'E': return (int)SevenSegmentLetters.eE;
				case 'F': return (int)SevenSegmentLetters.eF;
				case 'G': return (int)SevenSegmentLetters.eG;
				case 'H': return (int)SevenSegmentLetters.eH;
				case 'I': return (int)SevenSegmentLetters.eI;
				case 'J': return (int)SevenSegmentLetters.eJ;
				case 'K': return (int)SevenSegmentLetters.eK;
				case 'L': return (int)SevenSegmentLetters.eL;
				case 'M': return (int)SevenSegmentLetters.eM;
				case 'N': return (int)SevenSegmentLetters.eN;
				case 'O': return (int)SevenSegmentLetters.eO;
				case 'P': return (int)SevenSegmentLetters.eP;
				case 'Q': return (int)SevenSegmentLetters.eQ;
				case 'R': return (int)SevenSegmentLetters.eR;
				case 'S': return (int)SevenSegmentLetters.eS;
				case 'T': return (int)SevenSegmentLetters.eT;
				case 'U': return (int)SevenSegmentLetters.eU;
				case 'V': return (int)SevenSegmentLetters.eV;
				case 'W': return (int)SevenSegmentLetters.eW;
				case 'X': return (int)SevenSegmentLetters.eX;
				case 'Y': return (int)SevenSegmentLetters.eY;
				case 'Z': return (int)SevenSegmentLetters.eZ;
				case '.': return (int)SevenSegmentLetters.eDot;
			}
			return 0;
		}

		SevenSegment	(){}
		static public void SetSegment	(char pInput, ref Layers pImages)
		{
            //Make len a member 
            int len		= pImages.mLayers.Count;
			int pValue	= ToSegment(pInput);
			pValue		&= 0xff;

			for (int i = 0; i < len; i++)
			{
				if ((pValue & 1) == 1)
					pImages.mLayers[i].On();
				else
				{
					if (pInput != '.')
						pImages.mLayers[i].Off();
				}
				pValue >>= 1;
			}
		}
        static public void Blank(Layers pImages)
        {
            SetSegment(' ', ref pImages);
        }
        static public void Blank(ref Layers pImages)
		{
			SetSegment(' ', ref pImages);
		}
	}

    public class MultimeterScreen : 
#if __ANDROID__
        SKGLView
#elif __IOS__
        SKGLView
#else
        SKCanvasView
#endif
    {
        delegate void CacheImage(ILayer image);

        private float   _MaxHeight = 300;
        public float    MaxHeight
        {
            get
            {
                return _MaxHeight;
            }
            set
            {
                _MaxHeight = value;
            }
        }

        private TapGestureRecognizer    mTouch;
        public event EventHandler       Clicked;
        protected virtual   void        OnClicked   (EventArgs e)
        {
            EventHandler handler = Clicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private             void        TapCallback (object sender, EventArgs args)
        {
            OnClicked(EventArgs.Empty);
        }

        SKBitmap                        mLayer;
        SKCanvas                        mCanvas;
        static List<ILayer>             mLayerCache;
        List<Layers>	                mSegments;
		List<Layers>	                mSubSegments;
        Layers                          mBargraph;
        Layers                          mOther;

        protected virtual void          LayerChange(object o, EventArgs e)
        {
        }
        private void                    SetLargeSegments(string pInput)
        {
            if (pInput.Length > mSegments.Count)
                throw (new Exception("Large segment value too many decimal places."));

            SetSegments(pInput.PadLeft(mSegments.Count, ' '), ref mSegments);
        }
        private void                    SetSmallSegments(string pInput)
        {
            if (pInput.Length > mSubSegments.Count)
                throw (new Exception("Small segment value too many decimal places."));

            SetSegments(pInput.PadLeft(mSubSegments.Count, ' '), ref mSubSegments);
        }
        public float                    LargeSegments
        {
            set
            {
                SetLargeSegments(value.ToString());
            }
        }
        public string                   LargeSegmentsWord
        {
            set
            {
                SetLargeSegments(value);
            }
        }
        public float                    SmallSegments
        {
            set
            {
                SetSmallSegments(value.ToString());
            }
        }
        public string                   SmallSegmentsWord
        {
            set
            {
                SetSmallSegments(value);
            }
        }
        public int                      Bargraph
        {
            set
            {
                if (value <= mBargraph.mLayers.Count)
                    SetBargraph(value);
                else
                    throw (new Exception("Bargraph value too high."));
            }
        }
        private void                    SetBargraph(int pInput)
        {
            foreach (ILayer Layer in mBargraph.mLayers)
                Layer.Off();

            for (int i = 0; i < mBargraph.mLayers.Count; i++)
                mBargraph.mLayers[i].Set(pInput >= i);
        }

        Layers segments        = new Layers("mSegments");
        Layers subsegments     = new Layers("mSubsegments");

        CacheImage              CacheFunction;
        void                    Cacher(ILayer image)
        {
            mLayerCache.Add(image);
        }

        bool                    ProcessImage    (string filename, Polycurve Image)
        {
            if (CacheFunction != null)
                CacheFunction((new PathLayer(Image, filename) as ILayer));

            if (filename.Contains("seg"))
                segments.AddLayer(Image, filename);
            else if (filename.Contains("sub"))
                subsegments.AddLayer(Image, filename);
            else if (filename.Contains("bar"))
                mBargraph.AddLayer(Image, filename);
            else
                mOther.AddLayer(Image, filename);

            return true;
        }
        bool                    ProcessImage    (string filename, SKSvg Image)
        {
            if (CacheFunction != null)
                CacheFunction((new SVGLayer(Image, filename) as ILayer));

            if (filename.Contains("seg"))
                segments.AddLayer(Image, filename);
            else if (filename.Contains("sub"))
                subsegments.AddLayer(Image, filename);
            else if (filename.Contains("bar"))
                mBargraph.AddLayer(Image, filename);
            else
                mOther.AddLayer(Image, filename);

            return true;
        }
        bool                    ProcessImage    (string filename, SKImage Image)
        {
            if (CacheFunction != null)
                CacheFunction((new ImageLayer(Image, filename) as ILayer));


            if (filename.Contains("seg"))
                segments.AddLayer(Image, filename);
            else if (filename.Contains("sub"))
                subsegments.AddLayer(Image, filename);
            else if (filename.Contains("bar"))
                mBargraph.AddLayer(Image, filename);
            else
                mOther.AddLayer(Image, filename);

            return true;
        }
        bool                    ProcessImage    (ILayer Image)
        {
            var filename = Image.Name;

            if (filename.Contains("seg"))
                segments.AddLayer(Image);
            else if (filename.Contains("sub"))
                subsegments.AddLayer(Image);
            else if (filename.Contains("bar"))
                mBargraph.AddLayer(Image);
            else
                mOther.AddLayer(Image);

            return true;
        }

        public MultimeterScreen()
        {
            //Default size options
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.StartAndExpand;

            //New layer images
            mSegments = new List<Layers>();
            mSubSegments = new List<Layers>();
            mBargraph = new Layers("mBargraph");
            mOther = new Layers("mOther");

            //Setup the image cache if it doesn't exist
            CacheFunction = null;
            if (mLayerCache == null)
            {
                mLayerCache = new List<ILayer>();
                CacheFunction = Cacher;

                //Sort images into appropreate layered images
                var Loader = new PathLoader(ProcessImage);
            }
            else
            {
                //Load from cache
                foreach (var layer in mLayerCache)
                    ProcessImage(layer);
            }

            //Sort Images alphabetically within layered images
            //Sort segments and subsegments into seperate digits
            subsegments.Sort();
            mBargraph.Sort();
            segments.Sort();
            mOther.Sort();

            //Setup the different segments
            {
                Layers returned;
                int i;
                i = 1;
			    while (segments.Group("seg" + (i++).ToString(), out returned))
				    mSegments.Add(returned);

			    //Setup the different subsegments
			    i = 1;
			    while (subsegments.Group("sub" + (i++).ToString(), out returned))
				    mSubSegments.Add(returned);
            }

			//Move decimal point to the end
			foreach (var temp in mSegments)
            {
                temp.ToBottom("dp");
                temp.OnChanged += LayerChange;
            }
            foreach (var temp in mSubSegments)
            {
				temp.ToBottom("dp");
                temp.OnChanged += LayerChange;
            }

            //
            mOther.OnChanged += LayerChange;
            mBargraph.OnChanged += LayerChange;

            //Add the gesture recognizer 
            mTouch = new TapGestureRecognizer();
            mTouch.Tapped += TapCallback;
            GestureRecognizers.Add(mTouch);
        }
        private void            Invalidate()
        {
            InvalidateSurface();
        }

        public (float aspect, float width, float height)       
                                GetResultSize   (double Width = 0)
        {
            (float x, float y) = mBargraph.GetResultSize();
            return ((y / x), x, y);
        }

        private float           ConvertWidthToPixel(float value)
        {
            return (CanvasSize.Width * value / (float)Width);
        }
        private float           ConvertHeightToPixel(float value)
        {
            return (CanvasSize.Height * value / (float)Height);
        }

        //Only maintains aspect ratio
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            //Get image dimensions
            (float aspect, float x, float y) = GetResultSize();
            var NewHeight = (float)width * aspect;

            //Make sure the height
            if (NewHeight > MaxHeight)
                NewHeight = MaxHeight;

            //Setup the height request
            HeightRequest = NewHeight;
        }

        SKRect                  mDrawRectangle;
        private void            Rescale()
        {
            (float aspect, float x, float y) = GetResultSize();

            var width = CanvasSize.Width;
            var height = CanvasSize.Height;

            //Scale Image by height to match request
            var scale = (float)height / y;
            var imageHeight = (float)height;
            var imageWidth = x * scale;

            //
            if (imageWidth > width)
            {
                imageWidth = (float)width;
                imageHeight = aspect * imageWidth;
            }

            //
            mDrawRectangle = new SKRect(0, 0, imageWidth, imageHeight);
        }

        bool                    NeedClear = true;
        private void            Render (SKCanvas pSurface)
        {
            if (NeedClear)
            {
                if (CanvasSize.Width == 0 || CanvasSize.Height == 0)
                    return;

                NeedClear = false;

                //Cancel render if canvas doesn't exist
                if (mDrawRectangle.Width == 0 || mDrawRectangle.Height == 0)
                    Rescale();

                //Setup a clear bitmap
                mLayer = new SKBitmap((int)mDrawRectangle.Width, (int)mDrawRectangle.Height);
                mLayer.Erase(SKColors.Transparent);
                
                //Setup a clear canvas
                mCanvas = new SKCanvas(mLayer);
                mCanvas.Clear(App_112GW.Globals.BackgroundColor.ToSKColor());

                //Clear draw surface
                pSurface.Clear(App_112GW.Globals.BackgroundColor.ToSKColor());
            }

            //Create the draw rectnagle from the draw size
            var rendrect = new SKRect();
            rendrect.Top = 0;
            rendrect.Left = 0;
            rendrect.Right = mDrawRectangle.Right;
            rendrect.Bottom = mDrawRectangle.Bottom;

            //Add render on change
            for (int i = 0; i < mSegments.Count; i++)
                mSegments[i].Render(ref mCanvas, mDrawRectangle);
            for (int i = 0; i < mSegments.Count; i++)
                mSubSegments[i].Render(ref mCanvas, mDrawRectangle);
            mBargraph.Render (ref mCanvas, mDrawRectangle);
            mOther.Render    (ref mCanvas, mDrawRectangle);

            //Shift canvas as required
            var offset_x = (float) Width - rendrect.Width;
            if (offset_x > 0)
                rendrect.Offset(offset_x / 2, 0);
            else
            {
                rendrect.Right  = pSurface.DeviceClipBounds.Width;
                rendrect.Bottom = pSurface.DeviceClipBounds.Height;
            }

            //Draw bitmap
            pSurface.DrawBitmap(mLayer, rendrect);
        }
		static private void     SetSegment(char pInput, Layers pSegment)
		{
			SevenSegment.SetSegment(pInput, ref pSegment);
		}
		private void            SetSegments(string pInput, ref List<Layers> pSegments)
		{
            foreach (Layers Segment in pSegments)
                SevenSegment.Blank(Segment);

			for(int i = 0; i < pInput.Length; i++)
			{
				if (i >= pSegments.Count)
					return;

				char cur = pInput[i];
				SetSegment(cur, pSegments[i]);
			}
		}

#if __ANDROID__
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#elif __IOS__
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#else
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
#endif
        {
            Render(e.Surface.Canvas);
        }
    }
}