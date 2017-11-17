using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
	interface ICappedRange
	{
		float Minimum
		{
			get;
			set;
		}
		float Maximum
		{
			get;
			set;
		}
		float Distance
		{
			get;
		}
		void SetBoundary(Range Input);

		void Reset();

		void Set			(Range Input);
		void Set			(float ValA, float ValB);

		bool InRange		(float Value);
		void AddToMaximum   (float Value);
		void AddToMinimum   (float Value);

		void ShiftRange	 (float Value);
		void ShiftFit	   (float Value);
		void ExpandFit	  (float Value);
		void Pan			(float Amount);
		void Zoom		   (float Amount, float About);

		Range GetRange();
		Range Combine(List<Range> mRanges);
	}

	public class CappedRange: ICappedRange
	{
		private enum Current
		{
			Visible,
			Boundary
		};

		public bool IsCapped => Select == Current.Visible;

		private Range   Visible;
		private Range   Boundary;
		private Current Select;
		private float  Dist(float A, float B)
		{
			if (A > B)
				return A - B;
			return B - A;
		}

		public float Minimum
		{
			get
			{
				switch (Select)
				{
					case Current.Visible:
						return Visible.Minimum;
					case Current.Boundary:
						return Boundary.Minimum;
					default:
						throw new Exception("Not possible.");
				}
			}
			set
			{
				switch (Select)
				{
					case Current.Visible:
						if (value >= Boundary.Minimum)
							Visible.Minimum = value;
						break;
					case Current.Boundary:
						Boundary.Minimum = value;
						if (value > Visible.Minimum)
							Visible.Minimum = value;
						break;
					default:
						throw new Exception("Not possible.");
				}
			}
		}
		public float Maximum
		{
			get
			{
				switch (Select)
				{
					case Current.Visible:
						return Visible.Maximum;
					case Current.Boundary:
						return Boundary.Maximum;
					default:
						throw new Exception("Not possible.");
				}
			}
			set
			{
				switch (Select)
				{
					case Current.Visible:
						if (value <= Boundary.Maximum)
							Visible.Maximum = value;
						break;
					case Current.Boundary:
						Boundary.Maximum = value;
						if (value < Visible.Maximum)
							Visible.Maximum = value;
						break;
					default:
						throw new Exception("Not possible.");
				}
			}
		}
		public float Distance
		{
			get
			{
				switch (Select)
				{
					case Current.Visible:
						return Visible.Distance;
					case Current.Boundary:
						return Boundary.Distance;
					default:
						throw new Exception("Not possible.");
				}
			}
		}
		public void SetBoundary(Range Input)
		{
			Boundary.Minimum = Input.Minimum;
			Boundary.Maximum = Input.Maximum;

			if (Select == Current.Boundary)
			{
				Visible.Minimum = Input.Minimum;
				Visible.Maximum = Input.Maximum;
			}
		}
		public void SetBoundary(float A, float B)
		{
			SetBoundary(new Range(A, B));
		}

		public void Reset()
		{
			Boundary.Rescale();
			Visible.Rescale();

			Select = Current.Boundary;
		}

		public void Set(Range Input)
		{
			Select = Current.Boundary;

			Boundary.Minimum = Input.Minimum;
			Boundary.Maximum = Input.Maximum;

			Visible.Minimum = Input.Minimum;
			Visible.Maximum = Input.Maximum;
		}
		public void Set(float ValA, float ValB)
		{
			if (ValA > ValB)	Set(new Range(ValB, ValA));
			else				Set(new Range(ValA, ValB));
		}

		public bool InRange(float Value)
		{
			return (Minimum <= Value) && (Value <= Maximum);
		}

		public void AddToMaximum(float Value)
		{
			Maximum += Value;
			if (Maximum < Minimum) Minimum = Minimum;
		}
		public void AddToMinimum(float Value)
		{
			Minimum += Value;
			if (Minimum > Maximum) Maximum = Minimum;
		}

		public void ShiftRange(float Value)
		{
			if (Value == 0.0)
				return;

			Minimum = Minimum + Value;
			Maximum = Maximum + Value;
		}
		public void ShiftFit(float Value)
		{
			float diff = 0.0f;
			if	  (Value > Maximum)
				diff = Value - Maximum;
			else if (Value < Minimum)
				diff = Minimum - Value;
			ShiftRange(diff);
		}
		public void ExpandFit(float Value)
		{
			if	  (Value > Maximum) Maximum = Value;
			else if (Value < Minimum) Minimum = Value;
		}

		public void Pan(float Amount)
		{
			if (Select != Current.Visible)  return;

			var min = Visible.Minimum + Amount;
			var max = Visible.Maximum + Amount;

			if ( Boundary.InRange(min) && Boundary.InRange(max) )
			{
				Visible.Minimum = min;
				Visible.Maximum = max;
			}
		}
		public void Zoom(float Amount, float About)
		{
			if (Amount == 1.0)
				return;

			if (InRange(About))
			{
				var l = Dist(About, Minimum) / Amount;
				var h = Dist(About, Maximum) / Amount;
				var lower = About - l;
				var upper = About + h;
				int c = 0;

				//Clip upper and lower bounds
				//If zoomed out to full range re-enable normal mode.
				if (lower <= Boundary.Minimum)
				{
					++c;
					lower = Boundary.Minimum;
				}
				if (upper >= Boundary.Maximum)
				{
					++c;
					upper = Boundary.Maximum;
				}
				if (c == 2)
					Select = Current.Boundary;
				else
				{
					Visible.Minimum = lower;
					Visible.Maximum = upper;
					Select = Current.Visible;
				}
			}
		}

		public Range GetRange()
		{
			switch (Select)
			{
				case Current.Visible:
					return Visible;
				case Current.Boundary:
					return Boundary;
				default:
					throw new Exception("Not possible.");
			}
		}

		public Range Combine(List<Range> mRanges)
		{
			var output = new Range(Boundary.Minimum, Boundary.Maximum);
			foreach(var item in mRanges)
			{
				if (output.Distance == 0)
				{
					output.Minimum = item.Minimum;
					output.Maximum = item.Maximum;
				}
				else
				{
					var min = item.Minimum;
					var max = item.Maximum;
					output.Minimum = Math.Min(min, output.Minimum);
					output.Maximum = Math.Max(max, output.Maximum);
				}
			}
			return output;
		}

		public CappedRange(float A, float B)
		{
			Boundary = new Range(A, B);
			Visible = new Range(A, B);
			Set(A, B);
			Boundary.Rescale();
			Visible.Rescale();
		}
	}
}