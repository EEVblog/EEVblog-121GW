using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Reflection;
using System.Resources;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

using App_112GW;
using System.Diagnostics;

namespace rMultiplatform
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

        private rMultiplatform.Touch mTouch;

        public enum eControlInputState
        {
            eNone,
            ePressed,
            eHover
        }
        private eControlInputState _State;
        public eControlInputState State
        {
            get
            {   return _State;  }
            set
            {
                _State = value;
                InvalidateSurface();
            }
        }

        private void MTouch_Press           (object sender, rMultiplatform.TouchActionEventArgs args)
        {
            State = eControlInputState.ePressed;
            ChangeColors();
        }
        private void MTouch_Hover           (object sender, rMultiplatform.TouchActionEventArgs args)
        {
            State = eControlInputState.eHover;
            ChangeColors();
        }
        private void MTouch_Release         (object sender, rMultiplatform.TouchActionEventArgs args)
        {
            if (State == eControlInputState.ePressed)
                OnClicked(EventArgs.Empty);
            State = eControlInputState.eNone;
            ChangeColors();
        }

        private SKColor     _IdleColor;
        public Color         IdleColor
        {
            set
            {
                _IdleColor = value.ToSKColor();
                ChangeColors();
            }
            get
            {
                return _IdleColor.ToFormsColor();
            }
        }
        private SKColor     _PressColor;
        public Color         PressColor
        {
            set
            {
                _PressColor = value.ToSKColor();
                ChangeColors();
            }
            get
            {
                return _PressColor.ToFormsColor();
            }
        }
        private SKColor     _HoverColor;
        public Color         HoverColor
        {
            set
            {
                _HoverColor = value.ToSKColor();
                ChangeColors();
            }
            get
            {
                return _HoverColor.ToFormsColor();
            }
        }
        private SKColor     _BackgroundColor;
        public new Color     BackgroundColor
        {
            set
            {
                _BackgroundColor = value.ToSKColor();
                ChangeColors();
            }
            get
            {
                return _BackgroundColor.ToFormsColor();
            }
        }
        private void ChangeColors           ()
        {
            switch (State)
            {
                case eControlInputState.eNone:
                    ChangePrimaryColor(_IdleColor);
                    break;
                case eControlInputState.ePressed:
                    ChangePrimaryColor(_PressColor);
                    break;
                case eControlInputState.eHover:
                    ChangePrimaryColor(_HoverColor);
                    break;
            }
            ChangeBackgroundColor(_BackgroundColor);
            Redraw();
        }
        private void ChangePrimaryColor     (SKColor pInput)
        {
            for (int i = 0; i < mSegments.Count; i++)
                mSegments[i].DrawColor = pInput;
            for (int i = 0; i < mSegments.Count; i++)
                mSubSegments[i].DrawColor = pInput;
            mBargraph.DrawColor = pInput;
            mOther.DrawColor = pInput;
        }
        private void ChangeBackgroundColor  (SKColor pInput)
        {
            for (int i = 0; i < mSegments.Count; i++)
                mSegments[i].BackgroundColor = pInput;
            for (int i = 0; i < mSegments.Count; i++)
                mSubSegments[i].BackgroundColor = pInput;
            mBargraph.BackgroundColor = pInput;
            mOther.BackgroundColor = pInput;
        }

        public event EventHandler       Clicked;
        protected virtual   void        OnClicked   (EventArgs e)
        {
            EventHandler handler = Clicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        SKBitmap                        mLayer;
        SKCanvas                        mCanvas;
        static List<ILayer>             mLayerCache;
        List<Layers>	                mSegments;
		List<Layers>	                mSubSegments;
        Layers                          mBargraph;
        Layers                          mOther;

        int                             mDecimalPosition;

        protected virtual void LayerChange(object o, EventArgs e)
        {

        }
        private void SetLargeSegments(string pInput)
        {
            if (pInput.Length > mSegments.Count)
                pInput.Substring(0, mSegments.Count);

            SetSegments(pInput.PadLeft(mSegments.Count, ' '), ref mSegments);
        }
        private void SetSmallSegments(string pInput)
        {
            if (pInput.Length > mSubSegments.Count)
                throw (new Exception("Small segment value too many decimal places."));

            SetSegments(pInput.PadLeft(mSubSegments.Count, ' '), ref mSubSegments);
        }
        public float LargeSegments
        {
            set
            {
                SetLargeSegments(value.ToString());
            }
        }
        public string LargeSegmentsWord
        {
            set
            {
                SetLargeSegments(value);
            }
        }
        public float SmallSegments
        {
            set
            {
                SetSmallSegments(value.ToString());
            }
        }
        public string SmallSegmentsWord
        {
            set
            {
                SetSmallSegments(value);
            }
        }
        public int Bargraph
        {
            set
            {
                if (value <= mBargraph.mLayers.Count)
                    SetBargraph(value);
                else
                    throw (new Exception("Bargraph value too high."));
            }
        }
        private void SetBargraph(int pInput)
        {
            foreach (ILayer Layer in mBargraph.mLayers)
                Layer.Off();

            for (int i = 0; i < mBargraph.mLayers.Count; i++)
                mBargraph.mLayers[i].Set(pInput >= i);
        }
        private void SetOther(string Label, bool State)
        {
            foreach (var other in mOther.mLayers)
                if (other.Name.ToLower() == Label.ToLower())
                    other.Set(State);
        }

        public Packet112GW MainMode
        {
            set
            {
                switch (value.Mode)
                {   
                    case Packet112GW.eMode.Low_Z:
                        SetOther("LowZ", true);
                        SetOther("SegV", true);
                        break;
                    case Packet112GW.eMode.DCV:
                        SetOther("DC", true);
                        SetOther("SegV", true);
                        break;
                    case Packet112GW.eMode.ACV:
                        SetOther("AC", true);
                        SetOther("SegV", true);
                        break;
                    case Packet112GW.eMode.DCmV:
                        SetOther("DC", true);
                        SetOther("SegV", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet112GW.eMode.ACmV:
                        SetOther("AC", true);
                        SetOther("SegV", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet112GW.eMode.Temp:
                        SetOther("SegTempC", true);
                        break;
                    case Packet112GW.eMode.Hz:
                        SetOther("SegHz", true);
                        break;
                    case Packet112GW.eMode.mS:
                        SetOther("Subms", true);
                        break;
                    case Packet112GW.eMode.Duty:
                        SetOther("Sub%", true);
                        break;
                    case Packet112GW.eMode.Resistor:
                        SetOther("SegR", true);
                        break;
                    case Packet112GW.eMode.Continuity:
                        SetOther("Beep", true);
                        SetOther("SegR", true);
                        break;
                    case Packet112GW.eMode.Diode:
                        SetOther("Diode", true);
                        SetOther("SegV", true);
                        break;
                    case Packet112GW.eMode.Capacitor:
                        SetOther("SegCapF", true);
                        break;
                    case Packet112GW.eMode.ACuVA:
                        SetOther("AC", true);
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        SetOther("Segu", true);
                        break;
                    case Packet112GW.eMode.ACmVA:
                        SetOther("AC", true);
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet112GW.eMode.ACVA:
                        SetOther("AC", true);
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        break;
                    case Packet112GW.eMode.ACuA:
                        SetOther("AC", true);
                        SetOther("SegA", true);
                        SetOther("Segu", true);
                        break;
                    case Packet112GW.eMode.DCuA:
                        SetOther("DC", true);
                        SetOther("SegA", true);
                        SetOther("Segu", true);
                        break;
                    case Packet112GW.eMode.ACmA:
                        SetOther("AC", true);
                        SetOther("SegA", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet112GW.eMode.DCmA:
                        SetOther("DC", true);
                        SetOther("SegA", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet112GW.eMode.ACA:
                        SetOther("AC", true);
                        SetOther("SegA", true);
                        break;
                    case Packet112GW.eMode.DCA:
                        SetOther("DC", true);
                        SetOther("SegA", true);
                        break;
                    case Packet112GW.eMode.DCuVA:
                        SetOther("DC", true);
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        SetOther("Segu", true);
                        break;
                    case Packet112GW.eMode.DCmVA:
                        SetOther("DC", true);
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet112GW.eMode.DCVA:
                        SetOther("DC", true);
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        break;
                    default:
                        Debug.WriteLine("Other mode recieved" + value.ToString());
                        break;
                }
            }
        }
        public Packet112GW MainRangeValue
        {
            set
            {
                var OFL = value.MainOverload;
                var Sign = value.MainSign;
                var Range = value.MainRangeValue;

                //Overload
                if ( OFL )
                    LargeSegmentsWord = "OFL";
                else
                {
                    //Negative sign for segments
                    if ( Sign == Packet112GW.eSign.eNegative )
                        SetOther("Seg-", true);
                    else
                        SetOther("Seg-", false);

                    //Calculate the position of the decimal point
                    mDecimalPosition = (int)Math.Log10(Range) + 1;

                    var DisplayString = value.MainValue.ToString();

                    DisplayString = DisplayString.Insert(mDecimalPosition, ".");
                    LargeSegmentsWord = DisplayString;
                }
            }
        }

        private Packet112GW.eMode _SubMode;
        public Packet112GW SubMode
        {
            set
            {
                var mode = value.SubMode;
                _SubMode = mode;
                switch (mode)
                {
                    case Packet112GW.eMode.Low_Z:
                        SetOther("SubV", true);
                        break;
                    case Packet112GW.eMode.DCV:
                        SetOther("SubDC", true);
                        SetOther("SubV", true);
                        break;
                    case Packet112GW.eMode.ACV:
                        SetOther("SubAC", true);
                        SetOther("SubV", true);
                        break;
                    case Packet112GW.eMode.DCmV:
                        SetOther("SubDC", true);
                        SetOther("SubV", true);
                        SetOther("Subm", true);
                        break;
                    case Packet112GW.eMode.ACmV:
                        SetOther("AC", true);
                        SetOther("SegV", true);
                        SetOther("Subm", true);
                        break;
                    case Packet112GW.eMode.Temp:
                        break;
                    case Packet112GW.eMode.Hz:
                        SetOther("SubHz", true);
                        break;
                    case Packet112GW.eMode.mS:
                        SetOther("Subms", true);
                        break;
                    case Packet112GW.eMode.Duty:
                        SetOther("Sub%", true);
                        break;
                    case Packet112GW.eMode.Resistor:
                        SetOther("SubR", true);
                        break;
                    case Packet112GW.eMode.Continuity:
                        break;
                    case Packet112GW.eMode.Diode:
                        break;
                    case Packet112GW.eMode.Capacitor:
                        break;
                    case Packet112GW.eMode.ACuVA:
                        break;
                    case Packet112GW.eMode.ACmVA:
                        break;
                    case Packet112GW.eMode.ACVA:
                        SetOther("SubAC", true);
                        SetOther("SubV", true);
                        SetOther("SubA", true);
                        break;
                    case Packet112GW.eMode.ACuA:
                        break;
                    case Packet112GW.eMode.DCuA:
                        break;
                    case Packet112GW.eMode.ACmA:
                        SetOther("SubAC", true);
                        SetOther("SubA", true);
                        SetOther("Subm", true);
                        break;
                    case Packet112GW.eMode.DCmA:
                        SetOther("SubDC", true);
                        SetOther("SubA", true);
                        SetOther("SubmV", true);
                        break;
                    case Packet112GW.eMode.ACA:
                        SetOther("SubAC", true);
                        SetOther("SubA", true);
                        break;
                    case Packet112GW.eMode.DCA:
                        SetOther("SubDC", true);
                        SetOther("SubA", true);
                        break;
                    case Packet112GW.eMode.DCuVA:
                        break;
                    case Packet112GW.eMode.DCmVA:
                        break;
                    case Packet112GW.eMode.DCVA:
                        SetOther("SubDC", true);
                        SetOther("SubV", true);
                        SetOther("SubA", true);
                        break;
                    case Packet112GW.eMode._Battery:
                        SetOther("SubDC", true);
                        SetOther("SubV", true);
                        break;
                    case Packet112GW.eMode._BURDEN_VOLTAGE:
                        SetOther("SubV", true);
                        break;
                    case Packet112GW.eMode._YEAR:
                        break;
                    case Packet112GW.eMode._DATE:
                        break;
                    case Packet112GW.eMode._TIME:
                        break;
                    case Packet112GW.eMode._LCD:
                        break;
                    case Packet112GW.eMode._TempC:
                        break;
                    case Packet112GW.eMode._TempF:
                        break;
                    case Packet112GW.eMode._dBm:
                        SetOther("SubdB", true);
                        break;
                    case Packet112GW.eMode._Interval:
                        SetOther("Subm", true);
                        SetOther("SubS", true);
                        break;
                    default:
                        Debug.WriteLine("Other mode recieved" + value.ToString());
                        break;
                }
            }
        }
        public Packet112GW SubRangeValue
        {
            set
            {
                var OFL = value.SubOverload;
                var Sign = value.SubSign;
                var Range = value.SubPoint;

                //Overload
                if (OFL)
                    SmallSegmentsWord = "OFL";
                else
                {
                    //Negative sign for segments
                    if (Sign == Packet112GW.eSign.eNegative)
                        SetOther("Sub-", true);
                    else
                        SetOther("Sub-", false);

                    //Calculate the position of the decimal point
                    mDecimalPosition = (int)Range / 10 + 1;

                    var DisplayString = value.SubValue.ToString();
                    DisplayString.Insert(mDecimalPosition, ".");

                    switch (_SubMode)
                    {
                        case Packet112GW.eMode.Temp:
                        case Packet112GW.eMode._TempC:
                            DisplayString += "c";
                            break;
                        case Packet112GW.eMode._TempF:
                            DisplayString += "f";
                            break;
                    }

                    SmallSegmentsWord = DisplayString;
                }
            }
        }

        public Packet112GW BarStatus
        {
            set
            {
                var use = value.BarOn;
                var _0_150 = value.Bar0_150;
                var _1000_500 = value.Bar1000_500;
                var sign = value.BarSign;
                var barval = value.BarValue;

                if (use)
                {
                    //Setup bargraph ranges
                    SetOther("BarTick0_0",  true);
                    if (_1000_500 > 0)
                    {
                        SetOther("BarTick1_2", false);
                        SetOther("BarTick2_4", false);
                        SetOther("BarTick3_6", false);
                        SetOther("BarTick4_8", false);
                        SetOther("BarTick5_1", false);
                        SetOther("BarTick5_0", false);
                    }
                    else
                    {
                        SetOther("BarTick1_1", false);
                        SetOther("BarTick2_2", false);
                        SetOther("BarTick3_3", false);
                        SetOther("BarTick4_4", false);
                        SetOther("BarTick5_5", false);
                    }

                    if (sign == Packet112GW.eSign.eNegative)
                        SetOther("BarTick -", false);
                    else
                        SetOther("Bar+", false);

                    Bargraph = barval + 1;
                }
            }
        }
        public Packet112GW IconStatus
        {
            set
            {
                SetOther("1 kHz",   value.Status1KHz);
                SetOther("Subms",   value.Status1ms);
                SetOther("Sub1",    value.Status1ms);
                SetOther("DC+AC",   value.StatusAC_DC > 0);
                SetOther("auto",    value.StatusAuto);
                SetOther("apo",     value.StatusAPO);
                SetOther("Battery", value.StatusBAT);
                SetOther("BT",      value.StatusBT);
                SetOther("Arrow",   value.StatusArrow);
                SetOther("REL",     value.StatusRel);
                SetOther("SubdB",   value.StatusdBm);

                //NOTE UNKONWN MIN/MAX bits config
                SetOther("TEST",    value.StatusTest);
                SetOther("MEM",     value.StatusMem > 0);
                SetOther("HOLD",    value.StatusAHold > 0);
                SetOther("AC",      value.StatusAC);
                SetOther("DC",      value.StatusDC);
            }
        }

        public void Update (Packet112GW pInput)
        {
            SetOther("BT", true);
            foreach (var other in mOther.mLayers)
                other.Off();

            //Main range bits
            MainMode        = pInput;
            MainRangeValue  = pInput;

            //Sub range bits
            SubMode         = pInput;
            SubRangeValue   = pInput;

            //Bar graph bits
            BarStatus       = pInput;

            //Update icons
            IconStatus      = pInput;
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

        private void            SetupTouch()
        {
            //Add the gesture recognizer 
            mTouch = new rMultiplatform.Touch();
            mTouch.Pressed += MTouch_Press;
            mTouch.Hover += MTouch_Hover;
            mTouch.Released += MTouch_Release;
            Effects.Add(mTouch);
        }
        private void            Redraw()
        {
            //Add render on change
            for (int i = 0; i < mSegments.Count; i++)
                mSegments[i].Redraw();
            for (int i = 0; i < mSegments.Count; i++)
                mSubSegments[i].Redraw();
            mBargraph.Redraw();
            mOther.Redraw();
            InvalidateSurface();
        }
        private void            Invalidate()
        {
            InvalidateSurface();
        }
        public (float aspect, float width, float height)       
                                GetResultSize(double Width = 0)
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
                mCanvas.Clear(Globals.BackgroundColor.ToSKColor());

                //Clear draw surface
                pSurface.Clear(Globals.BackgroundColor.ToSKColor());
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
        public                  MultimeterScreen()
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

            mOther.On();
            foreach (var item in mOther.mLayers)
                item.Off();


            BackgroundColor = Globals.BackgroundColor;
            PressColor = Globals.FocusColor;
            HoverColor = Globals.HighlightColor;
            IdleColor = Globals.TextColor;

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
            SetupTouch();
            ChangeColors();
        }
    }
}