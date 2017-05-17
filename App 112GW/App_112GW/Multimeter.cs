using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
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
		static public void SetSegment	(char pInput, ref ImageLayers pImages)
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
		static public void Blank(ImageLayers pImages)
		{
			SetSegment(' ', ref pImages);
		}
	}

    public class Multimeter: View
    {
		public enum MultimeterMode
		{
			eMultiMeterMode1 = 0,
			eMultiMeterMode2 = 1
		}


		public static readonly BindableProperty PrimaryValueProperty = BindableProperty.Create(propertyName: "PrimaryValue", returnType: typeof(double), declaringType: typeof(double), defaultValue: 0.0);
		public static readonly BindableProperty SecondaryValueProperty = BindableProperty.Create(propertyName: "SecondaryValue", returnType: typeof(double), declaringType: typeof(double), defaultValue: 0.0);
		public static readonly BindableProperty BarValueProperty = BindableProperty.Create(propertyName: "BarValue", returnType: typeof(int), declaringType: typeof(int), defaultValue: 0);
		public static readonly BindableProperty ModeProperty = BindableProperty.Create(propertyName: "Mode", returnType: typeof(MultimeterMode), declaringType: typeof(MultimeterMode), defaultValue: (MultimeterMode)0);
		public static readonly BindableProperty SelectedProperty = BindableProperty.Create(propertyName: "Selected", returnType: typeof(bool), declaringType: typeof(bool), defaultValue: false);

		public double			PrimaryValue
		{
			get
			{ return (double)GetValue(PrimaryValueProperty); }
			set
			{ SetValue(PrimaryValueProperty, value); }
		}
		public double			SecondaryValue
		{
			get
			{ return (double)GetValue(SecondaryValueProperty); }
			set
			{ SetValue(SecondaryValueProperty, value); }
		}
		public int				BarValue
		{
			get
			{ return (int)GetValue(BarValueProperty); }
			set
			{ SetValue(BarValueProperty, value); }
		}

		public MultimeterMode	Mode
		{
			get
			{ return (MultimeterMode)GetValue(ModeProperty); }
			set
			{ SetValue(ModeProperty, value); }
		}
		public bool				Selected
		{
			get
			{ return (bool)GetValue(SelectedProperty); }
			set
			{ SetValue(SelectedProperty, value); }
		}


		private enum eFocusState
		{
			eFocused,
			eClicked,
			eNormal
		};
	
		eFocusState			mFocusState;
		List<ImageLayers>	mSegments;
		List<ImageLayers>	mSubSegments;
		ImageLayers			mBargraph;
		ImageLayers			mOther;

		SKPaint				mClickedPaint;

		public Multimeter(string pLayerPath)
		{
			ImageLayers segments = new ImageLayers("mSegments");
			ImageLayers subsegments = new ImageLayers("mSubsegments");

			mFocusState = eFocusState.eNormal;

			mClickedPaint = new SKPaint();
			mClickedPaint.Color = new SKColor(0,0,0,200);

			//New layer images
			mSegments		= new List<ImageLayers>();
			mSubSegments	= new List<ImageLayers>();
			mBargraph		= new ImageLayers("mBargraph");
			mOther			= new ImageLayers("mOther");

			//Sort images into appropreate layered images
			string[] files = System.IO.Directory.GetFiles(pLayerPath);
			foreach (string filename in files)
			{
				if		(filename.Contains("seg"))
					segments.AddLayer(filename);

				else if (filename.Contains("sub"))
					subsegments.AddLayer(filename);

				else if (filename.Contains("bar"))
					mBargraph.AddLayer(filename);

				else
					mOther.AddLayer(filename);
			}

			//Sort Images alphabetically within layered images
			subsegments.Sort();
			mBargraph.Sort();
			segments.Sort();
			mOther.Sort();

			//Sort segments and subsegments into seperate digits
			ImageLayers returned;
			int i;

			//Setup the different segments
			i = 1;
			while (segments.Group("seg" + (i++).ToString(), out returned))
				mSegments.Add(returned);

			//Setup the different subsegments
			i = 1;
			while (subsegments.Group("sub" + (i++).ToString(), out returned))
				mSubSegments.Add(returned);

			//Move decimal point to the end
			foreach (ImageLayers temp in mSegments)
				temp.ToBottom("dp");

			foreach (ImageLayers temp in mSubSegments)
				temp.ToBottom("dp");
		}
		public void Render(ref SKCanvas pSurface, SKRect pRectangle)
		{
			//Add render on change

			for (int i = 0; i < mSegments.Count; i++)
				mSegments[i].Render(ref pSurface, ref pRectangle);

			for (int i = 0; i < mSegments.Count; i++)
				mSubSegments[i].Render(ref pSurface, ref pRectangle);
			
			mBargraph.Render(ref pSurface, ref pRectangle);
			mOther.Render(ref pSurface, ref pRectangle);

			switch (mFocusState)
			{
				case eFocusState.eFocused:
					break;
				case eFocusState.eClicked:
					pSurface.DrawRect(pRectangle, mClickedPaint);
					break;
				case eFocusState.eNormal:
					break;
			}
		}

		static private void SetSegment(char pInput, ImageLayers pSegment)
		{
			SevenSegment.SetSegment(pInput, ref pSegment);
		}
		private void SetSegments(string pInput, ref List<ImageLayers> pSegments)
		{
			for (int i = 0; i < pSegments.Count; i++)
				SevenSegment.Blank(pSegments[i]);

			for(int i = 0; i < pInput.Length; i++)
			{
				if (i >= pSegments.Count)
					return;

				char cur = pInput[i];
				SetSegment(cur, pSegments[i]);
			}
		}

		public void SetLargeSegments(string pInput)
		{
			SetSegments(pInput, ref mSegments);
		}
		public void SetSmallSegments(string pInput)
		{
			SetSegments(pInput, ref mSubSegments);
		}
		public void SetBargraph(int pInput)
		{
			for (int i = 0; i < mBargraph.mLayers.Count; i++)
				mBargraph.mLayers[i].Set(pInput >= i);
		}

		public void Select()
		{
			mFocusState = eFocusState.eFocused;
		}
		public void Deselect()
		{
			mFocusState = eFocusState.eNormal;
		}
		public void Clicked()
		{
			mFocusState = eFocusState.eClicked;
		}

		bool mDimUninit = true;
		Size mDimensions; 
		public Size Dimensions()
		{
			if (mDimUninit)
				if (mSegments.Count > 0)
				{
					mDimensions = new Size(mSegments[0].Width, mSegments[0].Height);
					mDimUninit = false;
				}
			return mDimensions;
		}
	}

	public class MultimeterGLRenderer : SKGLViewRenderer
	{

	}
	public class MultimeterCanvasRenderer : SKCanvasViewRenderer
	{

	}
}
