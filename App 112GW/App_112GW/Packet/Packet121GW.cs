using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace rMultiplatform
{
	public class Range121GW
	{
		public string mUnits;
		public string mLabel;
		public int[] mValues;
		public string mNotation;

		public Range121GW(string pUnits, string pLabel, int[] pValues, string pNotation)
		{
			mNotation = pNotation;
			mUnits = pUnits;
			mLabel = pLabel;
			mValues = pValues;
		}
	}
	public class Packet121GW
	{
        string pNibbles;

		public enum eMode
		{
			Low_Z = 0,
			DCV = 1,
			ACV = 2,
			DCmV = 3,
			ACmV = 4,
			Temp = 5,
			Hz = 6,
			mS = 7,
			Duty = 8,
			Resistor = 9,
			Continuity = 10,
			Diode = 11,
			Capacitor = 12,
			ACuVA = 13,
			ACmVA = 14,
			ACVA = 15,
			ACuA = 16,
			DCuA = 17,
			ACmA = 18,
			DCmA = 19,
			ACA = 20,
			DCA = 21,
			DCuVA = 22,
			DCmVA = 23,
			DCVA = 24,
			_TempC = 100,
			_TempF = 105,
			_Battery = 110,
			_APO_On = 120,
			_APO_Off = 125,
			_YEAR = 130,
			_DATE = 135,
			_TIME = 140,
			_BURDEN_VOLTAGE = 150,
			_LCD = 160,
			_dBm= 180,
			_Interval = 190
		}
		public enum eSign
		{
			ePositive = 0,
			eNegative = 1
		}
		public enum eAD_DC
		{
			eNone   = 0,
			eDC,
			eAC,
			eACDC
        }
        public enum eBarRange
        {
            e5 = 0,
            e50 = 1,
            e500 = 2,
            e1000 = 3
        }

		Range121GW[] RangeLookup =
		{
			new Range121GW("V",		"Voltage Low Z (V)",	new int[]{4}			," "    ),    //0
			new Range121GW("V",		"Voltage DC (V)",		new int[]{1,2,3,4}		,"    "    ),    //1
			new Range121GW("V",		"Voltage AC (V)",		new int[]{1,2,3,4}		,"    "    ),    //2
			new Range121GW("mV",	"Voltage DC (V)",		new int[]{2,3}			,"mm"    ),    //3 
			new Range121GW("mV",	"Voltage AC (V)",		new int[]{2,3}			,"mm"    ),    //4
			new Range121GW("°C",	"Temp (°C)",			new int[]{4}			," "    ),    //5
			new Range121GW("KHz",   "Frequency (Hz)",		new int[]{2,3,1,2,3}	,"  kkk"),  //6
			new Range121GW("ms",	"Period (s)",			new int[]{1,2,3}		,"   "    ),    //7
			new Range121GW("%",		"Duty (%)",				new int[]{4}			," "    ),    //8
			new Range121GW("KΩ",	"Resistance (Ω)",		new int[]{2,3,1,2,3,1,2},"  kkkMM"),//9
			new Range121GW("KΩ",	"Continuity (Ω)",		new int[]{3}			," "    ),    //10
			new Range121GW("V",		"Diode (V)",			new int[]{1,2}			,"  "    ),    //11
			new Range121GW("ms",	"Capacitance (F)",		new int[]{3,4,2,3,4,5}  ,"nnuuuu"), //12
			new Range121GW("uVA",   "Power AC (VA)",		new int[]{4,5,2,3}		,"    "    ),    //13
			new Range121GW("mVA",   "Power AC (VA)",		new int[]{4,5,2,3}		,"mm  "    ),    //14
			new Range121GW("mVA",   "Power AC (VA)",		new int[]{4,5,2,3}		,"mm  "    ),    //15
			new Range121GW("uA",	"Current AC (A)",		new int[]{2,3}			,"  "    ),    //16
			new Range121GW("uA",	"Current DC (A)",		new int[]{2,3}			,"  "    ),    //17
			new Range121GW("mA",	"Current AC (A)",		new int[]{3,1,2}		,"mmm"    ),    //18
			new Range121GW("mA",	"Current DC (A)",		new int[]{1,2}			,"mm"    ),    //19
			new Range121GW("A",		"Current AC (A)",		new int[]{3,1,2}		,"m  "    ),    //20
			new Range121GW("A",		"Current DC (A)",		new int[]{3,1,2}		,"m  "    ),    //21
			new Range121GW("uVA",   "Power DC (VA)",		new int[]{3,4,4,5}		,"    "    ),    //22
			new Range121GW("mVA",   "Power DC (VA)",		new int[]{4,5,2,3}		,"mm  "    ),    //23
			new Range121GW("VA",	"Power DC (VA)",		new int[]{4,5,2,3}		,"mm  "    )    //24
		};

		public int BoolToInt(bool value) => (value) ? 1 : 0;
		public int DecimalNibbleToValue(int start, int length)
		{
			var result = 0;

			for (int c = 0; c < length; c++)
				result = result * 10 + (int)HexAToHex(pNibbles[start + c]);

			return result;
        }
        public char HexAToHex(char value)
        {
            char ten = ((char)(byte)10);
            if      (value >= '0' && value <= '9') value = (char)(value - '0');
            else if (value >= 'a' && value <= 'f') value = (char)(value + (ten - 'a'));
            else if (value >= 'A' && value <= 'F') value = (char)(value + (ten - 'A'));
            return value;
        }
        public int NibbleToValue(int start, int length)
		{
			int result = 0;
			for (int c = 0; c < length; c++)
            {
                result <<= 4;
				result |= HexAToHex(pNibbles[c + start]);
            }
			return result;
		}
        //Start decimal coded nibbles protocol
        //Don't know why this is a seperate protocol...
        public int			Year			=>				 DecimalNibbleToValue(	0,	2	) + 2000;
		public int			Month			=>				 DecimalNibbleToValue(	2,	2	);
		public int			Serial			=>				 DecimalNibbleToValue(	4,	5	);

		//Start hex coded bytes protocol
		public eMode		Mode			=>	(eMode)				NibbleToValue(	9,	2	);

		public bool			MainOverload	=>	(					NibbleToValue(	11,	1	) & 0x8)	> 0;
		public eSign		MainSign		=>	(eSign)BoolToInt((	NibbleToValue(	11,	1	) & 0x4)	> 0);

		public int			MainRangeValue	=>  MainRange.mValues  [NibbleToValue(	12,	1	)];
		public char			MainRangeUnits	=>  MainRange.mNotation[NibbleToValue(	12,	1	)];

		public int			MainIntValue	=>                      NibbleToValue(	13,	4	);
		public eMode		SubMode			=>	(eMode)				NibbleToValue(	17,	2	);

		public bool			SubOverload		=>	(					NibbleToValue(	19,	1	) & 0x8)	!= 0;
		public eSign		SubSign			=>	((eSign)BoolToInt((	NibbleToValue(	19,	1	) & 0x4)	> 0));
		public bool			SubK			=>	((					NibbleToValue(	19,	1	) & 0x2)	> 0);
		public bool			SubHz			=>	((					NibbleToValue(	19,	1	) & 0x1)	> 0);

		public int			SubPoint		=>	(					NibbleToValue(	20,	1	) & 0xF);

		public int			SubValue		=>                      NibbleToValue(	21,	4	);
		public bool			BarOn			=>	((					NibbleToValue(	25,	1	) & 0x1)	== 0);
		public bool			Bar0_150		=>	((					NibbleToValue(	26,	1	) & 0x8)	!= 0);
		public eSign		BarSign			=>	(eSign)BoolToInt((	NibbleToValue(	26,	1	) & 0x4)	> 0);
		public eBarRange    Bar1000_500		=>	(eBarRange)(		NibbleToValue(	26,	1	) & 3);
		public int			BarValue		=>						NibbleToValue(	27,	2	) & 0x1F;
        public byte         Status1         =>  (byte)NibbleToValue(29, 2);
        public byte         Status2         =>  (byte)NibbleToValue(31, 2);
        public byte         Status3         =>  (byte)NibbleToValue(33, 2);
        public bool			Status1KHz		=>	(					NibbleToValue(	29,	1	) & 0x4)	!= 0;
		public bool			Status1ms		=>	(					NibbleToValue(	29,	1	) & 0x2)	!= 0;
		public eAD_DC		StatusAC_DC		=>	(eAD_DC)((			NibbleToValue(	29,	2	) >> 3)	    & 3);
		public bool			StatusAuto		=>	(					NibbleToValue(	30,	1	) & 0x4)	!= 0;
		public bool			StatusAPO		=>	(					NibbleToValue(	30,	1	) & 0x2)	!= 0;
		public bool			StatusBAT		=>	(					NibbleToValue(	30,	1	) & 0x1)	!= 0;
		public bool			StatusBT		=>	(					NibbleToValue(	31,	1	) & 0x4)	!= 0;
		public bool			StatusArrow		=>	(					NibbleToValue(	31,	1	) & 0x2)	!= 0;
		public bool			StatusRel		=>	(					NibbleToValue(	31,	1	) & 0x1)	!= 0;
		public bool			StatusdBm		=>	(					NibbleToValue(	32,	1	) & 0x8)	!= 0;
		public int			StatusMinMax	=>	(					NibbleToValue(	32,	1	) & 0x7);
		public bool			StatusTest		=>	((					NibbleToValue(	33,	1	) & 0x4)	!= 0);
		public int			StatusMem		=>	(					NibbleToValue(	33,	1	) & 0x3)	>> 4;
		public int			StatusAHold		=>	(					NibbleToValue(	34,	1	) >> 2)	    & 3;
		public bool			StatusAC		=>	(					NibbleToValue(	34,	1	) & 0x2)	!= 0;
		public bool			StatusDC		=>  (					NibbleToValue(	34,	1	) & 0x1)	!= 0;


		public Packet121GW()
		{
			pNibbles =  "";
		}

		public Range121GW MainRange
        {
            get
            {
                var index = (int)Mode;
                return RangeLookup[index];
            }
        }

		public float		MainValue		=>	(float)MainIntValue * (float)MainRangeMultiple / (float)Math.Pow(10.0, ( 5 - (float)MainRangeValue));
		public string		MainRangeLabel	=>  MainRange.mLabel;

		public double MainRangeMultiple
		{
			get
			{
				switch (MainRangeUnits)
				{
					case 'n':
						return 1.0 / 1000000000.0;
					case 'u':
						return 1.0 / 1000000.0;
					case 'm':
						return 1.0 / 1000.0;
					case 'K':
					case 'k':
						return 1000.0;
					case 'M':
						return 1000000.0;
				}
				return 1;
			}
		}


		//Note the above properties should not be read until this subroutine
		// completes
		public void ProcessPacket(byte[] pInput)
		{
            pNibbles = Encoding.UTF8.GetString(pInput);
            Debug.WriteLine(pNibbles);
		}

		private static byte[] KEYCODE_RANGE			= { (byte)0xF4, 0x30, 0x31, 0x30, 0x31 };
		private static byte[] KEYCODE_HOLD			= { (byte)0xF4, 0x30, 0x32, 0x30, 0x32 };
		private static byte[] KEYCODE_REL			= { (byte)0xF4, 0x30, 0x33, 0x30, 0x33 };
		private static byte[] KEYCODE_PEAK			= { (byte)0xF4, 0x30, 0x34, 0x30, 0x34 };
		private static byte[] KEYCODE_MODE			= { (byte)0xF4, 0x30, 0x35, 0x30, 0x35 };
		private static byte[] KEYCODE_MINMAX		= { (byte)0xF4, 0x30, 0x36, 0x30, 0x36 };
		private static byte[] KEYCODE_MEM			= { (byte)0xF4, 0x30, 0x37, 0x30, 0x37 };
		private static byte[] KEYCODE_SETUP			= { (byte)0xF4, 0x30, 0x38, 0x30, 0x38 };
		private static byte[] KEYCODE_LONG_RANGE	= { (byte)0xF4, 0x38, 0x31, 0x38, 0x31 };
		private static byte[] KEYCODE_LONG_HOLD		= { (byte)0xF4, 0x38, 0x32, 0x38, 0x32 };
		private static byte[] KEYCODE_LONG_REL		= { (byte)0xF4, 0x38, 0x33, 0x38, 0x33 };
		private static byte[] KEYCODE_LONG_PEAK		= { (byte)0xF4, 0x38, 0x34, 0x38, 0x34 };
		private static byte[] KEYCODE_LONG_MODE		= { (byte)0xF4, 0x38, 0x35, 0x38, 0x35 };
		private static byte[] KEYCODE_LONG_MINMAX	= { (byte)0xF4, 0x38, 0x36, 0x38, 0x36 };
		private static byte[] KEYCODE_LONG_MEM		= { (byte)0xF4, 0x38, 0x37, 0x38, 0x37 };
		private static byte[] KEYCODE_LONG_SETUP	= { (byte)0xF4, 0x38, 0x38, 0x38, 0x38 };
		
		public enum Keycode
		{
			RANGE,
			HOLD,
			REL,
			PEAK,
			MODE,
			MINMAX,
			MEM,
			SETUP,
			LONG_RANGE,
			LONG_HOLD,
			LONG_REL,
			LONG_PEAK,
			LONG_MINMAX,
			LONG_MEM,
			LONG_SETUP
		}
		static public byte[] GetKeycode(Keycode Input)
		{
			switch (Input)
			{
				case Keycode.RANGE:
					return KEYCODE_RANGE;
				case Keycode.HOLD:
					return KEYCODE_HOLD;
				case Keycode.REL:
					return KEYCODE_REL;
				case Keycode.PEAK:
					return KEYCODE_PEAK;
				case Keycode.MODE:
					return KEYCODE_MODE;
				case Keycode.MINMAX:
					return KEYCODE_MINMAX;
				case Keycode.MEM:
					return KEYCODE_MEM;
				case Keycode.SETUP:
					return KEYCODE_SETUP;
				case Keycode.LONG_RANGE:
					return KEYCODE_LONG_RANGE;
				case Keycode.LONG_HOLD:
					return KEYCODE_LONG_HOLD;
				case Keycode.LONG_REL:
					return KEYCODE_LONG_REL;
				case Keycode.LONG_PEAK:
					return KEYCODE_LONG_PEAK;
				case Keycode.LONG_MINMAX:
					return KEYCODE_LONG_MINMAX;
				case Keycode.LONG_MEM:
					return KEYCODE_LONG_MEM;
				case Keycode.LONG_SETUP:
					return KEYCODE_LONG_SETUP;
			}
			return null;
		}
	}
}
