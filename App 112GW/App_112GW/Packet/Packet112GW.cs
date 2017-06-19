using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public class Range112GW
    {
        public string mUnits;
        public string mLabel;
        public double[] mValues;

        public Range112GW(string pUnits, string pLabel, double[] pValues)
        {
            mUnits = pUnits;
            mLabel = pLabel;
            mValues = pValues;
        }
    }
    public class Packet112GW
    {
        private byte ToByte(string pInput)
        {
            return Convert.ToByte(pInput, 16);
        }
        List<byte> pData;
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
            _BURDEN_BOLTAGE = 150,
            _LCD = 160,
            _dBm= 180,
            _Interval = 190
        }
        Range112GW[] RangeLookup =
        {
            new Range112GW("V",     "Low_Z",        new double[]{600.0}),
            new Range112GW("V",     "DCV",          new double[]{5.0000,50.000,500.00,600.0}),
            new Range112GW("V",     "ACV",          new double[]{5.0000,50.000,500.00,600.0}),
            new Range112GW("mV",    "DCmV",         new double[]{50.000,500.00}),
            new Range112GW("mV",    "ACmV",         new double[]{50.000,500.00}),
            new Range112GW("°C",    "Temp",         new double[]{1350}),
            new Range112GW("KHz",   "Hz",           new double[]{99.999,999.99,9999.9,99999,999999}),
            new Range112GW("ms",    "mS",           new double[]{9.9999,99.999,999.99}),
            new Range112GW("%",     "Duty",         new double[]{99.9}),
            new Range112GW("KΩ",    "Resistor",     new double[]{0.05,0.5,5.00,50.000,500.00,5000,50000}),
            new Range112GW("KΩ",    "Continuity",   new double[]{500}),
            new Range112GW("ms",    "Diode",        new double[]{3.0000,10.000}),
            new Range112GW("ms",    "Capacitor",    new double[]{0.010,0.100,1.000,10.00,100.0,1000}),
            new Range112GW("uVA",   "ACuVA",        new double[]{250.00,2500.0,2500.0,25000}),
            new Range112GW("mVA",   "ACmVA",        new double[]{25.000,250.00,250.00,2500.0}),
            new Range112GW("mVA",   "ACVA",         new double[]{2500.0,25000,50.000,500.00}),
            new Range112GW("uA",    "ACuA",         new double[]{50.000,500.00}),
            new Range112GW("uA",    "DCuA",         new double[]{50.000,500.00}),
            new Range112GW("mA",    "ACmA",         new double[]{5.0000,50.000}),
            new Range112GW("mA",    "DCmA",         new double[]{5.0000,50.000}),
            new Range112GW("A",     "ACA",          new double[]{500.00,5.0000,10.000}),
            new Range112GW("A",     "DCA",          new double[]{500.00,5.0000,10.000}),
            new Range112GW("uVA",   "DCuVA",        new double[]{250.00,2500.0,2500.0,25000}),
            new Range112GW("mVA",   "DCmVA",        new double[]{25.000,250.00,250.00,2500.0}),
            new Range112GW("VA",    "DCVA",         new double[]{2500.0,25000,50.000,500.00})
        };
        public eMode Mode
        {
            get
            {
                return (eMode)pData[0];
            }
        }
        public Range112GW Range
        {
            get
            {
                var md = (int)Mode;
                return RangeLookup[md];
            }
        }
        public double RangeValue
        {
            get
            {
                var rg = Range;
                return rg.mValues[(int)pData[1] & 0xF];
            }
        }
        public int MainValue
        {
            get
            {
                var msb = (int)pData[2];
                var lsb = (int)pData[3];
                return (msb << 8) | lsb;
            }
        }
        public bool MainOverload
        {
            get
            {
                return (pData[1] & 0x80) > 0;
            }
        }
        public enum eSign
        {
            ePositive = 0,
            eNegative = 1
        }
        public eSign MainSign
        {
            get
            {
                var val = pData[1] & 0x40;
                var bol = val > 0;
                var intt = bol ? 1 : 0;
                return (eSign)intt;
            }
        }
        public eMode SubMode
        {
            get
            {
                return (eMode) pData[4];
            }
        }
        public bool SubOverload
        {
            get
            {
                return (pData[6] & 0x80) != 0;
            }
        }
        public eSign SubSign
        {
            get
            {
                var val = pData[6] & 0x40;
                var bol = val > 0;
                var intt = bol ? 1 : 0;
                return (eSign)intt;
            }
        }
        public bool SubK
        {
            get
            {
                return (pData[6] & 0x20) > 0;
            }
        }
        public bool SubHz
        {
            get
            {
                return (pData[6] & 0x10) > 0;
            }
        }
        public int SubPoint
        {
            get
            {
                return (pData[6] & 0xF);
            }
        }
        public int SubValue
        {
            get
            {
                var msb = (int)pData[6];
                var lsb = (int)pData[7];
                return (msb << 8) | lsb;
            }
        }
        public bool BarOn
        {
            get
            {
                return (pData[8] & 0x10) != 0;
            }
        }
        public bool Bar0_150
        {
            get
            {
                return (pData[8] & 0x08) != 0;
            }
        }
        public eSign BarSign
        {
            get
            {
                var val = pData[8] & 0x04;
                var bol = val > 0;
                var intt = bol ? 1 : 0;
                return (eSign)intt;
            }
        }
        public int  Bar1000_500
        {
            get
            {
                return pData[8] & 3;
            }
        }
        public int  BarValue
        {
            get
            {
                var value = (pData[9] & 0x1F) + 1;
                return value;
            }
        }
        public bool Status1KHz
        {
            get
            {
                return (pData[10] & 0x40) != 0;
            }
        }
        public bool Status1ms
        {
            get
            {
                return (pData[10] & 0x20) != 0;
            }
        }
        public int  StatusAC_DC
        {
            get
            {
                return (pData[10] & 0x18) >> 3;
            }
        }
        public bool StatusAuto
        {
            get
            {
                return (pData[10] & 0x04) != 0;
            }
        }
        public bool StatusAPO
        {
            get
            {
                return (pData[10] & 0x02) != 0;
            }
        }
        public bool StatusBAT
        {
            get
            {
                return (pData[10] & 0x01) != 0;
            }
        }
        public bool StatusBT
        {
            get
            {
                return (pData[11] & 0x40) != 0;
            }
        }
        public bool StatusArrow
        {
            get
            {
                return (pData[11] & 0x20) != 0;
            }
        }
        public bool StatusRel
        {
            get
            {
                return (pData[11] & 0x10) != 0;
            }
        }
        public bool StatusdBm
        {
            get
            {
                return (pData[11] & 0x08) != 0;
            }
        }
        public int  StatusMinMax
        {
            get
            {
                return pData[11] & 0x7;
            }
        }
        public bool StatusTest
        {
            get
            {
                return (pData[12] & 0x40) != 0;
            }
        }
        public int  StatusMem
        {
            get
            {
                return (pData[12] & 0x30) >> 4;
            }
        }
        public int  StatusAHold
        {
            get
            {
                return (pData[12] & 0x0B) >> 4;
            }
        }
        public bool StatusAC
        {
            get
            {
                return (pData[12] & 0x02) != 0;
            }
        }
        public bool StatusDC
        {
            get
            {
                return (pData[12] & 0x01) != 0;
            }
        }
        public Packet112GW()
        {
            pData = new List<byte>();
        }

        //Note the above properties should not be read until this subroutine
        // completes
        public void ProcessPacket(byte[] pInput)
        {
            var str = Encoding.UTF8.GetString(pInput);
            pData.Clear();
            for (int i = 0; i < str.Length - 1; i += 2)
                pData.Add(ToByte(str.Substring(i, 2)));
        }
    }
}
