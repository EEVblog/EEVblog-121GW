using System;
using System.Collections.Generic;
using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;

namespace rMultiplatform
{
	public class MultimeterScreen : GeneralRenderedView
	{
		static List<ILayer> mLayerCache = null;
		private Touch mTouch;

		delegate void CacheImage(ILayer image);

		public enum eControlInputState { eNone, ePressed, eHover }
		private eControlInputState _State;
		public eControlInputState State
		{
			get
			{
				return _State;
			}
			set
			{
				_State = value;
				ChangeColors();
			}
		}

		private void MTouch_Tap	 (object sender, Touch.TouchTapEventArgs args)   =>  OnClicked(EventArgs.Empty);
		private void MTouch_Pressed (object sender, TouchActionEventArgs args)	  =>  State = eControlInputState.ePressed;
		private void MTouch_Hover   (object sender, TouchActionEventArgs args)	  =>  State = eControlInputState.eHover;
		private void MTouch_Release (object sender, TouchActionEventArgs args)	  =>  State = eControlInputState.eNone;

		private SKColor _IdleColor;
		public Color IdleColor
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
		private SKColor _PressColor;
		public Color PressColor
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
		private SKColor _HoverColor;
		public Color HoverColor
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
		private SKColor _BackgroundColor;
		public new Color BackgroundColor
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
		private void ChangeColors()
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
		private void ChangePrimaryColor(SKColor pInput)
		{
			for (int i = 0; i < mSegments.Count; i++)
				mSegments[i].DrawColor = pInput;
			for (int i = 0; i < mSegments.Count; i++)
				mSubSegments[i].DrawColor = pInput;
			mBargraph.DrawColor = pInput;
			mOther.DrawColor = pInput;
		}
		private void ChangeBackgroundColor(SKColor pInput)
		{
			for (int i = 0; i < mSegments.Count; i++)
				mSegments[i].BackgroundColor = pInput;
			for (int i = 0; i < mSegments.Count; i++)
				mSubSegments[i].BackgroundColor = pInput;
			mBargraph.BackgroundColor = pInput;
			mOther.BackgroundColor = pInput;
		}

		public event EventHandler Clicked;
		protected virtual void  OnClicked(EventArgs e) => Clicked?.Invoke(this, e);

		private Layers		  mOther;
		public  SKBitmap		mLayer;
		public  SKCanvas		mCanvas;
		private Layers		  mBargraph;
		private List<Layers>	mSegments;
		private List<Layers>	mSubSegments;
		private int			 mDecimalPosition;
		private SKRect		  mDrawRectangle;

		private void	SetLargeSegments(string pInput)
		{
			if (pInput.EndsWith("."))
				pInput += "0";

			if (pInput.Length > mSegments.Count)
				pInput.Substring(0, mSegments.Count);

			SetSegments(pInput.PadLeft(mSegments.Count, ' '), ref mSegments);
		}
		private void	SetSmallSegments(string pInput)
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
		public float	SmallSegments
		{
			set
			{
				SetSmallSegments(value.ToString());
			}
		}
		public string   SmallSegmentsWord
		{
			set
			{
				SetSmallSegments(value);
			}
		}
		public int	  Bargraph
		{
			set
			{
				if (value <= mBargraph.mLayers.Count)
					SetBargraph(value);
				else
					throw (new Exception("Bargraph value too high."));
			}
		}
		private void	SetBargraph(int pInput)
		{
			foreach (ILayer Layer in mBargraph.mLayers)
				Layer.Off();

			for (int i = 0; i < mBargraph.mLayers.Count; i++)
				mBargraph.mLayers[i].Set(pInput >= i);
		}

		ILayer 
			LowZ, 
			SegV, 
			SegmV, 
			AC, 
			DC, 
			SegTempC, 
			SegHz, 
			SubPercent, 
			SegCapF, 
			Diode, 
			Subms, 
			SegR, 
			Segu, 
			SegA, 
			Beep, 
			SegM, 
			Segk, 
			Segn, 
			SubV, 
			SubDC, 
			SubAC,
			Subm, 
			SubHz, 
			SubR, 
			SubA, 
			SubS, 
			SubdB, 
			Seg_Minus, 
			Sub_Minus, 
			BarTick_Minus, 
			BarTick_Plus, 
			BarTick1_2, 
			BarTick2_4, 
			BarTick3_6, 
			BarTick4_8, 
			BarTick5_0, 
			BarTick5_1, 
			BarTick0_0, 
			BarTick1_1, 
			BarTick2_2, 
			BarTick3_3, 
			BarTick4_4, 
			BarTick5_5,
			Bar500_5_0, 
			Bar500_0_1, 
			Bar500_0_2, 
			Bar1000_1_0,
			Bar1000_0_1,
			Bar1000_0_2,
			Bar1000_0_3,
			One_kHz,
			Sub1, 
			auto, 
			apo, 
			Battery, 
			REL, 
			DC_Plus_AC, 
			TEST, 
			MEM, 
			HOLD, 
			A_Minus, 
			MAX, 
			MIN, 
			AVG, 
			BT;

		public ILayer   GetOther(string Label)
		{
			foreach (var other in mOther.mLayers)
				if (other.Name.Contains(Label))
					return other;
			throw new Exception("Cannot find layer.");
		}
		private void	SetLayer(ILayer Layer, bool State)
		{
			Layer.Set(State);
		}

		public void	 SetupOtherCache()
		{
			LowZ			= GetOther("LowZ"	   );
			SegV			= GetOther("SegV"	   );
			SegmV		   = GetOther("SegmV"	  );
			AC			  = GetOther("AC"		 );
			DC			  = GetOther("DC"		 );
			SegTempC		= GetOther("SegTempC"   );
			SegHz		   = GetOther("SegHz"	  );
			SubPercent	  = GetOther("Sub%"	   );
			SegCapF		 = GetOther("SegCapF"	);
			Diode		   = GetOther("Diode"	  );
			Subms		   = GetOther("Subms"	  );
			SegR			= GetOther("SegR"	   );
			Segu			= GetOther("Segu"	   );
			SegA			= GetOther("SegA"	   );
			Beep			= GetOther("Beep"	   );
			SegM			= GetOther("SegM"	   );
			Segk			= GetOther("Segk"	   );
			Segn			= GetOther("Segn"	   );
			SubV			= GetOther("SubV"	   );
			SubDC		   = GetOther("SubDC"	  );
			SubAC		   = GetOther("SubAC"	  );
			Subm			= GetOther("Subm"	   );
			SubS			= GetOther("SubS"	   );
			SubdB		   = GetOther("SubdB"	  );
			Sub_Minus	   = GetOther("Sub-"	   );
			BarTick_Minus   = GetOther("Bar+"	   );
			BarTick_Plus	= GetOther("BarTick-"   );
			BarTick1_2	  = GetOther("BarTick1_2" );
			BarTick2_4	  = GetOther("BarTick2_4" );
			BarTick3_6	  = GetOther("BarTick3_6" );
			BarTick4_8	  = GetOther("BarTick4_8" );
			BarTick5_0	  = GetOther("BarTick5_0" );
			BarTick5_1	  = GetOther("BarTick5_1" );
			BarTick0_0	  = GetOther("BarTick0_0" );
			BarTick1_1	  = GetOther("BarTick1_1" );
			BarTick2_2	  = GetOther("BarTick2_2" );
			BarTick3_3	  = GetOther("BarTick3_3" );
			BarTick4_4	  = GetOther("BarTick4_4" );
			BarTick5_5	  = GetOther("BarTick5_5" );
			Bar500_5_0	  = GetOther("Bar500_5_0" );
			Bar500_0_1	  = GetOther("Bar500_0_1" );
			Bar500_0_2	  = GetOther("Bar500_0_2" );
			Bar1000_1_0	 = GetOther("Bar1000_1_0");
			Bar1000_0_1	 = GetOther("Bar1000_0_1");
			Bar1000_0_2	 = GetOther("Bar1000_0_2");
			Bar1000_0_3	 = GetOther("Bar1000_0_3");
			One_kHz		 = GetOther("1 kHz");
			Sub1			= GetOther("Sub1");
			auto			= GetOther("auto");
			apo			 = GetOther("apo");
			Battery		 = GetOther("Battery");
			REL			 = GetOther("REL");
			DC_Plus_AC	  = GetOther("DC+AC");
			TEST			= GetOther("TEST");
			MEM			 = GetOther("MEM");
			HOLD			= GetOther("HOLD");
			A_Minus		 = GetOther("A-");
			MAX			 = GetOther("MAX");
			MIN			 = GetOther("MIN");
			AVG			 = GetOther("AVG");
			BT			  = GetOther("bt");
			Seg_Minus	   = GetOther("Seg-");
			SubA			= GetOther("SubA");
			SubR			= GetOther("SubR");
			SubHz		   = GetOther("SubHz");
		}

		public Packet121GW MainMode
		{
			set
			{
				switch (value.Mode)
				{
					case Packet121GW.eMode.Low_Z:
						SetLayer(LowZ, true);
						SetLayer(SegV, true);
						break;
					case Packet121GW.eMode.DCV:
						SetLayer(SegV, true);
						break;
					case Packet121GW.eMode.ACV:
						SetLayer(SegV, true);
						break;
					case Packet121GW.eMode.DCmV:
						SetLayer(SegV, true);
						SetLayer(SegmV, true);
						break;
					case Packet121GW.eMode.ACmV:
						SetLayer(AC, true);
						SetLayer(SegV, true);
						SetLayer(SegmV, true);
						break;
					case Packet121GW.eMode.Temp:
						SetLayer(SegTempC, true);
						break;
					case Packet121GW.eMode.Hz:
						SetLayer(SegHz, true);
						break;
					case Packet121GW.eMode.mS:
						SetLayer(Subms, true);
						break;
					case Packet121GW.eMode.Duty:
						SetLayer(SubPercent, true);
						break;
					case Packet121GW.eMode.Resistor:
						SetLayer(SegR, true);
						break;
					case Packet121GW.eMode.Continuity:
						SetLayer(Beep, true);
						SetLayer(SegR, true);
						break;
					case Packet121GW.eMode.Diode:
						SetLayer(Diode, true);
						SetLayer(SegV, true);
						break;
					case Packet121GW.eMode.Capacitor:
						SetLayer(SegCapF, true);
						break;
					case Packet121GW.eMode.ACuVA:
						SetLayer(SegV, true);
						SetLayer(SegA, true);
						SetLayer(Segu, true);
						break;
					case Packet121GW.eMode.ACmVA:
						SetLayer(SegV, true);
						SetLayer(SegA, true);
						SetLayer(SegmV, true);
						break;
					case Packet121GW.eMode.ACVA:
						SetLayer(SegV, true);
						SetLayer(SegA, true);
						break;
					case Packet121GW.eMode.ACuA:
						SetLayer(SegA, true);
						SetLayer(Segu, true);
						break;
					case Packet121GW.eMode.DCuA:
						SetLayer(SegA, true);
						SetLayer(Segu, true);
						break;
					case Packet121GW.eMode.ACmA:
						SetLayer(SegA, true);
						SetLayer(SegmV, true);
						break;
					case Packet121GW.eMode.DCmA:
						SetLayer(SegA, true);
						SetLayer(SegmV, true);
						break;
					case Packet121GW.eMode.ACA:
						SetLayer(SegA, true);
						break;
					case Packet121GW.eMode.DCA:
						SetLayer(SegA, true);
						break;
					case Packet121GW.eMode.DCuVA:
						SetLayer(SegV, true);
						SetLayer(SegA, true);
						SetLayer(Segu, true);
						break;
					case Packet121GW.eMode.DCmVA:
						SetLayer(SegV, true);
						SetLayer(SegA, true);
						SetLayer(SegmV, true);
						break;
					case Packet121GW.eMode.DCVA:
						SetLayer(SegV, true);
						SetLayer(SegA, true);
						break;
					default:
						Debug.WriteLine("Other mode recieved" + value.ToString());
						break;
				}
			}
		}
		public Packet121GW MainRangeValue
		{
			set
			{
				var OFL = value.MainOverload;
				var Sign = value.MainSign;
				var Range = value.MainRangeValue;

				//Overload
				if (OFL)
					LargeSegmentsWord = "OFL";
				else
				{
					//Negative sign for segments
					SetLayer(Seg_Minus, (Sign == Packet121GW.eSign.eNegative));

					//Calculate the position of the decimal point
					mDecimalPosition = Range;

					//Align the string to the right of the display
					var DisplayString = value.MainIntValue.ToString().PadLeft(5, ' ');

					//Cannot insert a decimal point outside the range of the string
					if (mDecimalPosition < 5)
						DisplayString = DisplayString.Insert(mDecimalPosition, ".");

					//Combine decimal points and charaters so that a decimal point 
					// doesn't occupy a full character
					bool beforepoint = true;
					string outstring = "";
					for (int i = 0; i < DisplayString.Length; ++i)
					{
						var c = DisplayString[i];
						if (c == '.')
							beforepoint = false;

						if (beforepoint)
							outstring += c;
						else
						{
							if (c == ' ')
								outstring += '0';
							else
								outstring += c;
						}
					}

					//Setup the SI units outputs
					var units = value.MainRangeUnits;
					switch (units)
					{
						case 'm':   SetLayer(SegmV, true);  break;
						case 'M':   SetLayer(SegM,  true);  break;
						case 'k':   SetLayer(Segk,  true);  break;
						case 'u':   SetLayer(Segu,  true);  break;
						case 'n':   SetLayer(Segn,  true);  break;
					};

					//Output the value to the emulated LCD
					outstring = outstring.PadLeft(5, ' ').Replace(" .", "0.");
					LargeSegmentsWord = outstring;
				}
			}
		}
		private Packet121GW.eMode _SubMode;
		public Packet121GW SubMode
		{
			set
			{
				var mode = value.SubMode;
				_SubMode = mode;
				switch (mode)
				{
					case Packet121GW.eMode.Low_Z:
						SetLayer(SubV,  true);
						break;
					case Packet121GW.eMode.DCV:
						SetLayer(SubDC, true);
						SetLayer(SubV,  true);
						break;
					case Packet121GW.eMode.ACV:
						SetLayer(SubAC, true);
						SetLayer(SubV,  true);
						break;
					case Packet121GW.eMode.DCmV:
						SetLayer(SubDC, true);
						SetLayer(SubV,  true);
						SetLayer(Subm,  true);
						break;
					case Packet121GW.eMode.ACmV:
						SetLayer(AC,	true);
						SetLayer(SegV,  true);
						SetLayer(Subm,  true);
						break;
					case Packet121GW.eMode.Temp:
						break;
					case Packet121GW.eMode.Hz:
						SetLayer(SubHz, true);
						break;
					case Packet121GW.eMode.mS:
						SetLayer(Subms, true);
						break;
					case Packet121GW.eMode.Duty:
						SetLayer(SubPercent, true);
						break;
					case Packet121GW.eMode.Resistor:
						SetLayer(SubR,  true);
						break;
					case Packet121GW.eMode.Continuity:
						break;
					case Packet121GW.eMode.Diode:
						break;
					case Packet121GW.eMode.Capacitor:
						break;
					case Packet121GW.eMode.ACuVA:
						break;
					case Packet121GW.eMode.ACmVA:
						break;
					case Packet121GW.eMode.ACVA:
						SetLayer(SubAC, true);
						SetLayer(SubV, true);
						SetLayer(SubA, true);
						break;
					case Packet121GW.eMode.ACuA:
						break;
					case Packet121GW.eMode.DCuA:
						break;
					case Packet121GW.eMode.ACmA:
						SetLayer(SubAC, true);
						SetLayer(SubA, true);
						SetLayer(Subm, true);
						break;
					case Packet121GW.eMode.DCmA:
						SetLayer(SubDC, true);
						SetLayer(SubA, true);
						SetLayer(Subm, true);
						break;
					case Packet121GW.eMode.ACA:
						SetLayer(SubAC, true);
						SetLayer(SubA, true);
						break;
					case Packet121GW.eMode.DCA:
						SetLayer(SubDC, true);
						SetLayer(SubA, true);
						break;
					case Packet121GW.eMode.DCuVA:
						break;
					case Packet121GW.eMode.DCmVA:
						break;
					case Packet121GW.eMode.DCVA:
						SetLayer(SubDC, true);
						SetLayer(SubV, true);
						SetLayer(SubA, true);
						break;
					case Packet121GW.eMode._Battery:
						SetLayer(SubDC, true);
						SetLayer(SubV, true);
						break;
					case Packet121GW.eMode._BURDEN_VOLTAGE:
						SetLayer(SubV, true);
						break;
					case Packet121GW.eMode._YEAR:
						break;
					case Packet121GW.eMode._DATE:
						break;
					case Packet121GW.eMode._TIME:
						break;
					case Packet121GW.eMode._LCD:
						break;
					case Packet121GW.eMode._TempC:
						break;
					case Packet121GW.eMode._TempF:
						break;
					case Packet121GW.eMode._dBm:
						SetLayer(SubdB, true);
						break;
					case Packet121GW.eMode._Interval:
						SetLayer(Subm, true);
						SetLayer(SubS, true);
						break;
					default:
						Debug.WriteLine("Other mode recieved" + value.ToString());
						break;
				}
			}
		}
		public Packet121GW SubRangeValue
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
					SetLayer(Sub_Minus, (Sign == Packet121GW.eSign.eNegative));

					//Calculate the position of the decimal point
					mDecimalPosition = Range / 10 + 1;

					var DisplayString = value.SubValue.ToString();

					//Cannot insert a decimal point outside the range of the string
					if (mDecimalPosition + 1 < DisplayString.Length)
					{
						if (mDecimalPosition < 5)
							DisplayString = DisplayString.Insert(mDecimalPosition + 1, ".");

						//Combine decimal points and charaters so that a decimal point 
						// doesn't occupy a full character
						bool beforepoint = true;
						string outstring = "";
						for (int i = 0; i < DisplayString.Length; ++i)
						{
							var c = DisplayString[i];
							if (c == '.')
								beforepoint = false;

							if (beforepoint)
								outstring += c;
							else
							{
								if (c == ' ')
									outstring += '0';
								else
									outstring += c;
							}
						}
						switch (_SubMode)
						{
							case Packet121GW.eMode.Temp:
							case Packet121GW.eMode._TempC:
								DisplayString += "c";
								break;
							case Packet121GW.eMode._TempF:
								DisplayString += "f";
								break;
						}
						SmallSegmentsWord = DisplayString;
					}
				}
			}
		}
		public Packet121GW BarStatus
		{
			set
			{
				var On = value.BarOn;
				var _0_150 = value.Bar0_150;
				var _1000_500 = value.Bar1000_500;
				var sign = value.BarSign;
				var barval = value.BarValue;
				if (On)
				{
					//Setup bargraph ranges
					SetLayer(BarTick0_0, true);
					if (_0_150)
					{
						SetLayer(BarTick1_2, true);
						SetLayer(BarTick2_4, true);
						SetLayer(BarTick3_6, true);
						SetLayer(BarTick4_8, true);
						SetLayer(BarTick5_1, true);
						SetLayer(BarTick5_0, true);
					}
					else
					{
						SetLayer(BarTick1_1, true);
						SetLayer(BarTick2_2, true);
						SetLayer(BarTick3_3, true);
						SetLayer(BarTick4_4, true);
						SetLayer(BarTick5_5, true);
					}

					switch (_1000_500)
					{
						case Packet121GW.eBarRange.e5://5
							SetLayer(Bar500_5_0,  true);
							break;
						case Packet121GW.eBarRange.e50://50
							SetLayer(Bar500_5_0,  true);
							SetLayer(Bar500_0_1,  true);
							break;
						case Packet121GW.eBarRange.e500://500
							SetLayer(Bar500_5_0,  true);
							SetLayer(Bar500_0_1,  true);
							SetLayer(Bar500_0_2,  true);
							break;
						case Packet121GW.eBarRange.e1000://1000
							SetLayer(Bar1000_1_0, true);
							SetLayer(Bar1000_0_1, true);
							SetLayer(Bar1000_0_2, true);
							SetLayer(Bar1000_0_3, true);
							break;
					}

					if (sign == Packet121GW.eSign.eNegative)
						SetLayer(BarTick_Minus, true);
					else
						SetLayer(BarTick_Plus, true);

					Bargraph = barval + 1;
				}
			}
		}
		public Packet121GW IconStatus
		{
			set
			{
				SetLayer(One_kHz, value.Status1KHz);

				if (value.Status1ms)
				{
					SetLayer(Subms, true);
					SetLayer(Sub1, true);
				}

				switch (value.StatusAC_DC)
				{
					case Packet121GW.eAD_DC.eDC:
						SetLayer(DC, true);
						break;
					case Packet121GW.eAD_DC.eAC:
						SetLayer(AC, true);
						break;
					case Packet121GW.eAD_DC.eACDC:
						SetLayer(DC_Plus_AC, true);
						break;
					case Packet121GW.eAD_DC.eNone:
						break;
				}

				SetLayer(auto,	value.StatusAuto);
				SetLayer(apo,	 value.StatusAPO);
				SetLayer(Battery, value.StatusBAT);
				SetLayer(REL,	 value.StatusRel);
				SetLayer(SubdB,   value.StatusdBm);

				//NOTE UNKONWN MIN/MAX bits config
				SetLayer(TEST,	value.StatusTest);
				SetLayer(MEM,	 value.StatusMem > 0);

				switch (value.StatusAHold)
				{
					case 0:
						break;
					case 1:
						SetLayer(HOLD, true);
						SetLayer(A_Minus, true);
						break;
					case 2:
						SetLayer(HOLD, true);
						break;
				}
				switch (value.StatusMinMax)
				{
					case 0:
						break;
					case 1:
						SetLayer(MAX, true);
						break;
					case 2:
						SetLayer(MIN, true);
						break;
					case 3:
						SetLayer(AVG, true);
						break;
					case 4:
						SetLayer(AVG, true);
						SetLayer(MIN, true);
						SetLayer(MAX, true);
						break;
				}
			}
		}
		public void Update(Packet121GW pInput)
		{
			SetLayer(BT, true);
			foreach (var other in mOther.mLayers)
				other.Off();

			//Main range bits
			MainMode = pInput;
			MainRangeValue = pInput;

			//Sub range bits
			SubMode = pInput;
			SubRangeValue = pInput;

			//Bar graph bits
			BarStatus = pInput;

			//Update icons
			IconStatus = pInput;
		}

		Layers segments	 = new Layers("mSegments");
		Layers subsegments  = new Layers("mSubsegments");
		CacheImage CacheFunction = (image) => { mLayerCache.Add(image); };
		bool ProcessImage(string filename, Polycurve Image)
		{
			CacheFunction?.Invoke((new PathLayer(Image, filename) as ILayer));

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
		bool ProcessImage(string filename, SKSvg Image)
		{
			CacheFunction?.Invoke((new SVGLayer(Image, filename) as ILayer));

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
		bool ProcessImage(string filename, SKImage Image)
		{
			CacheFunction?.Invoke((new ImageLayer(Image, filename) as ILayer));

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
		bool ProcessImage(ILayer Image)
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

		private void SetupTouch()
		{
			//Add the gesture recognizer 
			mTouch		  = new rMultiplatform.Touch();
			mTouch.Tap	  += MTouch_Tap;
			mTouch.Pressed  += MTouch_Pressed;
			mTouch.Hover	+= MTouch_Hover;
			mTouch.Released += MTouch_Release;
			Effects.Add(mTouch);
		}

		private void Redraw()
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
		private void Invalidate() => InvalidateSurface();

		float LayerAspect = 1, LayerX = 0, LayerY = 0;
		public (float aspect, float width, float height) GetResultSize(double Width = 0) => (LayerAspect, LayerX, LayerY);

		//Only maintains aspect ratio
		protected override void OnSizeAllocated(double width, double height)
		{
			if (width > 0)
			{
				//Get image dimensions
				var NewHeight = (float)width * LayerAspect;

				//Setup the height request
				HeightRequest = NewHeight;
				RemakeCanvas = true;
				base.OnSizeAllocated(width, height);
			}
		}
		protected override void InvalidateMeasure()
		{
			RemakeCanvas = true;
			base.InvalidateMeasure();
		}
		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			RemakeCanvas = true;
			return base.OnMeasure(widthConstraint, heightConstraint);
		}
		private void Rescale()
		{
			var width = CanvasSize.Width;
			var height = CanvasSize.Height;

			//Scale Image by height to match request
			var scale = width / LayerX;
			var imageWidth = LayerX * scale;
			float imageHeight;
			if (imageWidth > width)
			{
				imageWidth = width;
				imageHeight = LayerAspect * imageWidth;
			}
			else
				imageHeight = height;

			mDrawRectangle = new SKRect(0, 0, imageWidth, imageHeight);
		}
		private static void SetSegment(char pInput, bool dp, Layers pSegment)
		{
			SevenSegment.SetSegment(pInput, dp, ref pSegment);
		}
		private void SetSegments(string pInput, ref List<Layers> pSegments)
		{
			foreach (Layers Segment in pSegments)
				SevenSegment.Blank(Segment);

			int i = 0;
			for (int j = 0; j < pInput.Length; j++)
			{
				char cur = pInput[j];
				if (cur == '.') continue;

				char nxt = (j + 1 < pInput.Length) ? pInput[j + 1] : ' ';
				var dp = (nxt == '.');

				SetSegment(cur, dp, pSegments[i]);
				i++;
			}
		}
		bool RemakeCanvas = true;
		public override void PaintSurface ( SKCanvas canvas, SKSize dimension, SKSize viewsize)
		{
			if ( RemakeCanvas )
			{
				//Handles glitch in android.
				var canvas_aspect = dimension.Height / dimension.Width;
				if (canvas_aspect >= (LayerAspect / 2))
					RemakeCanvas = false;
				else return;

				Rescale();
				Redraw();
				if (dimension.Width == 0 || dimension.Height == 0)
					return;

				//Setup a clear bitmap
				mLayer = new SKBitmap((int)mDrawRectangle.Width, (int)mDrawRectangle.Height);
				mLayer.Erase(SKColors.Transparent);

				//Setup a clear canvas
				mCanvas = new SKCanvas(mLayer);
				mCanvas.Clear(Globals.BackgroundColor.ToSKColor());
			}

			//Add render on change
			for (int i = 0; i < mSegments.Count; i++)
				mSegments[i].Render(ref mCanvas, mDrawRectangle);
			for (int i = 0; i < mSegments.Count; i++)
				mSubSegments[i].Render(ref mCanvas, mDrawRectangle);
			mBargraph.Render(ref mCanvas, mDrawRectangle);
			mOther.Render(ref mCanvas, mDrawRectangle);

			//Draw bitmap
			canvas.Clear(Globals.BackgroundColor.ToSKColor());
			canvas.DrawBitmap(mLayer, new SKRect(0, 0, dimension.Width, dimension.Height));
		}
		public MultimeterScreen()
		{
			//New layer images
			mSegments	   = new List<Layers>();
			mSubSegments	= new List<Layers>();
			mBargraph	   = new Layers("mBargraph");
			mOther		  = new Layers("mOther");

			//Setup the image cache if it doesn't exist
			CacheFunction = null;
			if (mLayerCache == null)
			{
				mLayerCache = new List<ILayer>();
				CacheFunction = (image) => { mLayerCache.Add(image); };

				//Loads on construction.
				var Loader = new PathLoader(ProcessImage);
			}
			else
				foreach (var layer in mLayerCache)
					ProcessImage(layer);

			//Sort Images alphabetically within layered images
			//Sort segments and subsegments into seperate digits
			subsegments.Sort();
			mBargraph.Sort();
			segments.Sort();
			mOther.Sort();
			SetupOtherCache();

			mOther.On();
			foreach (var item in mOther.mLayers)
				item.Off();

			PressColor  = Globals.FocusColor;
			HoverColor  = Globals.HighlightColor;
			IdleColor   = Globals.TextColor;

			//Setup the different segments
			Layers returned;
			int i = 1;
			while (segments.Group("seg" + (i++).ToString(), out returned))
				mSegments.Add(returned);

			//Setup the different subsegments
			i = 1;
			while (subsegments.Group("sub" + (i++).ToString(), out returned))
				mSubSegments.Add(returned);

			//Move decimal point to the end
			foreach (var temp in mSegments)
			{
				temp.ToBottom("dp");
				temp.OnChanged += (e, o) => { };
			}
			foreach (var temp in mSubSegments)
			{
				temp.ToBottom("dp");
				temp.OnChanged += (e, o) => { };
			}

			//
			mOther.OnChanged	+= (e, o) => { };
			mBargraph.OnChanged += (e, o) => { };

			//Add the gesture recognizer 
			SetupTouch();
			ChangeColors();

			(LayerX, LayerY) = mBargraph.GetResultSize();
			LayerAspect = LayerY / LayerX;
		}
	}
}