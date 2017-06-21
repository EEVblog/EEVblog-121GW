if (outwards)
{
	//Horozontal scaling.
	if (value.Right > value.Left)
	{
		//Standard rectangle
		x = (int) Math.Floor (value.Left);
		r = (int) Math.Ceiling (value.Right);
	}
	else
	{
		//Inverted rectangle
		x = (int) Math.Ceiling (value.Left);
		r = (int) Math.Floor (value.Right);
		
	}
	
	//Vertical scaling
	if (value.Bottom > value.Top)
	{
		//Standard rectangle
		y = (int) Math.Floor (value.Top);
		b = (int) Math.Ceiling (value.Bottom);
	}
	else
	{
		//Inverted rectangle
		y = (int) Math.Ceiling (value.Top);
		b = (int) Math.Floor (value.Bottom);
	}
}
else
{
	x = (int) Math.Floor (value.Left); 
	y = (int) Math.Floor (value.Top);
}
