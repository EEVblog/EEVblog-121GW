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
        static List<ILayer> mLayerCache;
        private Touch mTouch;

        delegate void CacheImage(ILayer image);
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
            {
                return _State;
            }
            set
            {
                _State = value;
                ChangeColors();
            }
        }

        private void MTouch_Pressed(object sender, TouchActionEventArgs args)
        {
            State = eControlInputState.ePressed;
        }
        private void MTouch_Tap(object sender, Touch.TouchTapEventArgs args)
        {
            OnClicked(EventArgs.Empty);
        }
        private void MTouch_Hover(object sender, TouchActionEventArgs args)
        {
            State = eControlInputState.eHover;
        }
        private void MTouch_Release(object sender, TouchActionEventArgs args)
        {
            State = eControlInputState.eNone;
        }

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
        protected virtual void OnClicked(EventArgs e)
        {
            Clicked?.Invoke(this, e);
        }

        private Layers mOther;
        public SKBitmap mLayer;
        public SKCanvas mCanvas;
        private Layers mBargraph;
        private List<Layers> mSegments;
        private List<Layers> mSubSegments;
        private int mDecimalPosition;
        private SKRect mDrawRectangle;

        protected virtual void LayerChange(object o, EventArgs e)
        {

        }
        private void    SetLargeSegments(string pInput)
        {
            if (pInput.EndsWith("."))
                pInput += "0";

            if (pInput.Length > mSegments.Count)
                pInput.Substring(0, mSegments.Count);

            SetSegments(pInput.PadLeft(mSegments.Count, ' '), ref mSegments);
        }
        private void    SetSmallSegments(string pInput)
        {
            if (pInput.Length > mSubSegments.Count)
                throw (new Exception("Small segment value too many decimal places."));

            SetSegments(pInput.PadLeft(mSubSegments.Count, ' '), ref mSubSegments);
        }
        public float    LargeSegments
        {
            set
            {
                SetLargeSegments(value.ToString());
            }
        }
        public string   LargeSegmentsWord
        {
            set
            {
                SetLargeSegments(value);
            }
        }
        public float    SmallSegments
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
        public int      Bargraph
        {
            set
            {
                if (value <= mBargraph.mLayers.Count)
                    SetBargraph(value);
                else
                    throw (new Exception("Bargraph value too high."));
            }
        }
        private void    SetBargraph(int pInput)
        {
            foreach (ILayer Layer in mBargraph.mLayers)
                Layer.Off();

            for (int i = 0; i < mBargraph.mLayers.Count; i++)
                mBargraph.mLayers[i].Set(pInput >= i);
        }
        private void    SetOther(string Label, bool State)
        {
            foreach (var other in mOther.mLayers)
                if (other.Name == Label)
                    other.Set(State);
        }

        public Packet121GW MainMode
        {
            set
            {
                switch (value.Mode)
                {
                    case Packet121GW.eMode.Low_Z:
                        SetOther("LowZ", true);
                        SetOther("SegV", true);
                        break;
                    case Packet121GW.eMode.DCV:
                        SetOther("SegV", true);
                        break;
                    case Packet121GW.eMode.ACV:
                        SetOther("SegV", true);
                        break;
                    case Packet121GW.eMode.DCmV:
                        SetOther("SegV", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet121GW.eMode.ACmV:
                        SetOther("AC", true);
                        SetOther("SegV", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet121GW.eMode.Temp:
                        SetOther("SegTempC", true);
                        break;
                    case Packet121GW.eMode.Hz:
                        SetOther("SegHz", true);
                        break;
                    case Packet121GW.eMode.mS:
                        SetOther("Subms", true);
                        break;
                    case Packet121GW.eMode.Duty:
                        SetOther("Sub%", true);
                        break;
                    case Packet121GW.eMode.Resistor:
                        SetOther("SegR", true);
                        break;
                    case Packet121GW.eMode.Continuity:
                        SetOther("Beep", true);
                        SetOther("SegR", true);
                        break;
                    case Packet121GW.eMode.Diode:
                        SetOther("Diode", true);
                        SetOther("SegV", true);
                        break;
                    case Packet121GW.eMode.Capacitor:
                        SetOther("SegCapF", true);
                        break;
                    case Packet121GW.eMode.ACuVA:
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        SetOther("Segu", true);
                        break;
                    case Packet121GW.eMode.ACmVA:
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet121GW.eMode.ACVA:
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        break;
                    case Packet121GW.eMode.ACuA:
                        SetOther("SegA", true);
                        SetOther("Segu", true);
                        break;
                    case Packet121GW.eMode.DCuA:
                        SetOther("SegA", true);
                        SetOther("Segu", true);
                        break;
                    case Packet121GW.eMode.ACmA:
                        SetOther("SegA", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet121GW.eMode.DCmA:
                        SetOther("SegA", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet121GW.eMode.ACA:
                        SetOther("SegA", true);
                        break;
                    case Packet121GW.eMode.DCA:
                        SetOther("SegA", true);
                        break;
                    case Packet121GW.eMode.DCuVA:
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        SetOther("Segu", true);
                        break;
                    case Packet121GW.eMode.DCmVA:
                        SetOther("SegV", true);
                        SetOther("SegA", true);
                        SetOther("SegmV", true);
                        break;
                    case Packet121GW.eMode.DCVA:
                        SetOther("SegV", true);
                        SetOther("SegA", true);
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
                    if (Sign == Packet121GW.eSign.eNegative)
                        SetOther("Seg-", true);
                    else
                        SetOther("Seg-", false);

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
                        case 'm':
                            SetOther("SegmV", true);
                            break;
                        case 'M':
                            SetOther("SegM", true);
                            break;
                        case 'k':
                            SetOther("Segk", true);
                            break;
                        case 'u':
                            SetOther("Segu", true);
                            break;
                        case 'n':
                            SetOther("Segn", true);
                            break;
                    }

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
                        SetOther("SubV", true);
                        break;
                    case Packet121GW.eMode.DCV:
                        SetOther("SubDC", true);
                        SetOther("SubV", true);
                        break;
                    case Packet121GW.eMode.ACV:
                        SetOther("SubAC", true);
                        SetOther("SubV", true);
                        break;
                    case Packet121GW.eMode.DCmV:
                        SetOther("SubDC", true);
                        SetOther("SubV", true);
                        SetOther("Subm", true);
                        break;
                    case Packet121GW.eMode.ACmV:
                        SetOther("AC", true);
                        SetOther("SegV", true);
                        SetOther("Subm", true);
                        break;
                    case Packet121GW.eMode.Temp:
                        break;
                    case Packet121GW.eMode.Hz:
                        SetOther("SubHz", true);
                        break;
                    case Packet121GW.eMode.mS:
                        SetOther("Subms", true);
                        break;
                    case Packet121GW.eMode.Duty:
                        SetOther("Sub%", true);
                        break;
                    case Packet121GW.eMode.Resistor:
                        SetOther("SubR", true);
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
                        SetOther("SubAC", true);
                        SetOther("SubV", true);
                        SetOther("SubA", true);
                        break;
                    case Packet121GW.eMode.ACuA:
                        break;
                    case Packet121GW.eMode.DCuA:
                        break;
                    case Packet121GW.eMode.ACmA:
                        SetOther("SubAC", true);
                        SetOther("SubA", true);
                        SetOther("Subm", true);
                        break;
                    case Packet121GW.eMode.DCmA:
                        SetOther("SubDC", true);
                        SetOther("SubA", true);
                        SetOther("SubmV", true);
                        break;
                    case Packet121GW.eMode.ACA:
                        SetOther("SubAC", true);
                        SetOther("SubA", true);
                        break;
                    case Packet121GW.eMode.DCA:
                        SetOther("SubDC", true);
                        SetOther("SubA", true);
                        break;
                    case Packet121GW.eMode.DCuVA:
                        break;
                    case Packet121GW.eMode.DCmVA:
                        break;
                    case Packet121GW.eMode.DCVA:
                        SetOther("SubDC", true);
                        SetOther("SubV", true);
                        SetOther("SubA", true);
                        break;
                    case Packet121GW.eMode._Battery:
                        SetOther("SubDC", true);
                        SetOther("SubV", true);
                        break;
                    case Packet121GW.eMode._BURDEN_VOLTAGE:
                        SetOther("SubV", true);
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
                        SetOther("SubdB", true);
                        break;
                    case Packet121GW.eMode._Interval:
                        SetOther("Subm", true);
                        SetOther("SubS", true);
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
                    if (Sign == Packet121GW.eSign.eNegative)
                        SetOther("Sub-", true);
                    else
                        SetOther("Sub-", false);

                    //Calculate the position of the decimal point
                    mDecimalPosition = (int)Range / 10 + 1;

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
                    SetOther("BarTick0_0", true);
                    if (_0_150)
                    {
                        SetOther("BarTick1_2", true);
                        SetOther("BarTick2_4", true);
                        SetOther("BarTick3_6", true);
                        SetOther("BarTick4_8", true);
                        SetOther("BarTick5_1", true);
                        SetOther("BarTick5_0", true);
                    }
                    else
                    {
                        SetOther("BarTick1_1", true);
                        SetOther("BarTick2_2", true);
                        SetOther("BarTick3_3", true);
                        SetOther("BarTick4_4", true);
                        SetOther("BarTick5_5", true);
                    }

                    switch (_1000_500)
                    {
                        case 0://5
                            SetOther("Bar500_5_0",  true);
                            break;
                        case 1://50
                            SetOther("Bar500_5_0",  true);
                            SetOther("Bar500_0_1",  true);
                            break;
                        case 2://500
                            SetOther("Bar500_5_0",  true);
                            SetOther("Bar500_0_1",  true);
                            SetOther("Bar500_0_2",  true);
                            break;
                        case 3://1000
                            SetOther("Bar1000_1_0", true);
                            SetOther("Bar1000_0_1", true);
                            SetOther("Bar1000_0_2", true);
                            SetOther("Bar1000_0_3", true);
                            break;
                    }

                    if (sign == Packet121GW.eSign.eNegative)
                        SetOther("BarTick -", true);
                    else
                        SetOther("Bar+", true);

                    Bargraph = barval + 1;
                }
            }
        }
        public Packet121GW IconStatus
        {
            set
            {
                SetOther("1 kHz", value.Status1KHz);

                if (value.Status1ms)
                {
                    SetOther("Subms", true);
                    SetOther("Sub1", true);
                }

                switch (value.StatusAC_DC)
                {
                    case Packet121GW.eAD_DC.eDC:
                        SetOther("DC", true);
                        break;
                    case Packet121GW.eAD_DC.eAC:
                        SetOther("AC", true);
                        break;
                    case Packet121GW.eAD_DC.eACDC:
                        SetOther("DC+AC", true);
                        break;
                    case Packet121GW.eAD_DC.eNone:
                        break;
                }

                SetOther("auto",    value.StatusAuto);
                SetOther("apo",     value.StatusAPO);
                SetOther("Battery", value.StatusBAT);
                SetOther("Arrow",   value.StatusArrow);
                SetOther("REL",     value.StatusRel);
                SetOther("SubdB",   value.StatusdBm);

                //NOTE UNKONWN MIN/MAX bits config
                SetOther("TEST",    value.StatusTest);
                SetOther("MEM",     value.StatusMem > 0);

                switch (value.StatusAHold)
                {
                    case 0:
                        break;
                    case 1:
                        SetOther("HOLD", true);
                        SetOther("A-", true);
                        break;
                    case 2:
                        SetOther("HOLD", true);
                        break;
                }
                switch (value.StatusMinMax)
                {
                    case 0:
                        break;
                    case 1:
                        SetOther("MAX", true);
                        break;
                    case 2:
                        SetOther("MIN", true);
                        break;
                    case 3:
                        SetOther("AVG", true);
                        break;
                    case 4:
                        SetOther("AVG", true);
                        SetOther("MIN", true);
                        SetOther("MAX", true);
                        break;
                }
            }
        }
        public void Update(Packet121GW pInput)
        {
            SetOther("BT", true);
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

        Layers segments = new Layers("mSegments");
        Layers subsegments = new Layers("mSubsegments");

        CacheImage CacheFunction;
        void Cacher(ILayer image)
        {
            mLayerCache.Add(image);
        }
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
            mTouch          = new rMultiplatform.Touch();
            mTouch.Tap      += MTouch_Tap;
            mTouch.Pressed  += MTouch_Pressed;
            mTouch.Hover    += MTouch_Hover;
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
        private void Invalidate()
        {
            InvalidateSurface();
        }

        float LayerAspect = 1, LayerX = 0, LayerY = 0;
        public (float aspect, float width, float height) GetResultSize(double Width = 0)
        {
            return (LayerAspect, LayerX, LayerY);
        }

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
        bool RemakeCanvas = true;
        static private void SetSegment(char pInput, bool dp, Layers pSegment)
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
                if (cur == '.')
                    continue;

                char nxt = (j + 1 < pInput.Length) ? pInput[j + 1] : ' ';
                var dp = (nxt == '.');

                SetSegment(cur, dp, pSegments[i]);
                i++;
            }
        }

        private SKRect rendrect = new SKRect();
        public override void PaintSurface ( SKCanvas canvas, SKSize dimension )
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

                //Shift canvas as required
                rendrect.Right = mDrawRectangle.Right;
                rendrect.Bottom = mDrawRectangle.Bottom;
                var offset_x = (float)Width - rendrect.Width;
                if (offset_x > 0)
                    rendrect.Offset(offset_x / 2, 0);
                else
                {
                    rendrect.Right = canvas.DeviceClipBounds.Width;
                    rendrect.Bottom = canvas.DeviceClipBounds.Height;
                }

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
            canvas.Clear();
            canvas.DrawBitmap(mLayer, rendrect);
        }
        public MultimeterScreen()
        {
            //New layer images
            mSegments       = new List<Layers>();
            mSubSegments    = new List<Layers>();
            mBargraph       = new Layers("mBargraph");
            mOther          = new Layers("mOther");

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
                foreach (var layer in mLayerCache)
                    ProcessImage(layer);

            //Sort Images alphabetically within layered images
            //Sort segments and subsegments into seperate digits
            subsegments.Sort();
            mBargraph.Sort();
            segments.Sort();
            mOther.Sort();

            mOther.On();
            foreach (var item in mOther.mLayers)
                item.Off();

            PressColor = Globals.FocusColor;
            HoverColor = Globals.HighlightColor;
            IdleColor = Globals.TextColor;

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

            (LayerX, LayerY) = mBargraph.GetResultSize();
            LayerAspect = LayerY / LayerX;

            //
            rendrect.Bottom = 0;
            rendrect.Right = 0;
            rendrect.Left = 0;
            rendrect.Top = 0;
        }
    }
}