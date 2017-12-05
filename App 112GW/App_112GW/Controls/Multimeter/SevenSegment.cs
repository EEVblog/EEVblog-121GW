using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
	public class SevenSegment
	{
		private enum SevenSegmentLetters
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
		private static char ToUpper(char pCHR)
		{
			const char a = 'a';
			const char A = 'A';
			if (('a' <= pCHR) && (pCHR <= 'z'))
				return (char)((pCHR - a) + A);

			return pCHR;
		}
		private static int ToSegment(char pCHR)
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

		SevenSegment() { }
		public static void SetSegment(char pInput, bool dp, ref Layers pImages)
		{
			//Make len a member 
			int len = pImages.mLayers.Count;
			int pValue = ToSegment(pInput);

			pValue &= 0xff;
			pValue |= ((dp) ? 0x80 : 0);

			for (int i = 0; i < len; i++)
			{
				if ((pValue & 1) == 1)
					pImages.mLayers[i].On();
				else
					pImages.mLayers[i].Off();

				pValue >>= 1;
			}
		}
		public static void Blank(Layers pImages)
		{
			SetSegment(' ', false, ref pImages);
		}
		public static void Blank(ref Layers pImages)
		{
			SetSegment(' ', false, ref pImages);
		}
	}
}
