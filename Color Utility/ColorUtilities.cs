using System;
using UnityEngine;
using System.Text;

public class ColorUtilities
{

	public enum ColorStrategy
	{
		None,
		Complementary,
		Triad,
		Analagous,
		Inverted,
		Brightness,
		Saturation,
		Hue
	}
		
	public const float BLACK_UPPER_BOUND = 0.15f;
	public const float BLACK_LOWER_BOUND = 0.075f;

	static float defaultEqualityThreshold = 0.05f;
	
	static float defaultDifferenceThreshold = 0.55f;

	static float _colorInterval = float.MinValue;

	const float colorDivisor = 12f;

	static int[] triadIntervals = new int[]{0,-4,4};
	static int[] complementaryIntervals = new int[]{6,5,7};
	static int[] analogousIntervals = new int[]{0,-1,1};

	public static float colorInterval{

		get{

			if(_colorInterval == float.MinValue){

				_colorInterval = 1.0f / colorDivisor;

			}

			return _colorInterval;

		}

	}

	#region Basic Color Queries

	/// <summary>
	/// Is this color approximately black?
	/// </summary>
	/// <returns><c>true</c> if the color is approximately black.</returns>
	/// <param name="c">The color</param>
	public static bool IsBlack(Color c){

		if(c.r == c.g && c.g == c.b){

			float cf = c.r;

			if(cf <= BLACK_UPPER_BOUND){
				return true;
			}

		}


		return false;

	}

	/// <summary>
	/// Is this color approximately greyscale?
	/// </summary>
	/// <returns><c>true</c> if the color is approximately greyscale.</returns>
	/// <param name="c">C.</param>
	public static bool IsGreyscale(Color c){

		//You should really just use HSB for this

		if(IsBlack(c)){
			return false;
		}

		return(Mathf.Approximately(c.r,c.g) && Mathf.Approximately(c.r,c.b));

	}

	/// <summary>
	/// Returns the 'difference' between two colors. It's kind of arbitrary. I use it for sorting. Difference is based on proximity. E.g: 0.7 to 1.0 has a proximity of 0.3, likewise for 1.3 to 1.0
	/// </summary>
	/// <returns>The difference.</returns>
	/// <param name="ca">Color a</param>
	/// <param name="cb">Color b</param>
	public static float DifferenceBetween(Color ca, Color cb){

		HSBColor a = new HSBColor(ca);
		HSBColor b = new HSBColor(cb);

		float h = Mathf.Abs(a.h - b.h);

		if(h > 0.5f){
			h = 1.0f - (1f-h);
		}

		float s = Mathf.Abs(a.s - b.s);

		if(s > 0.5f){
			s = 1.0f - (1f-s);
		}

		float k = Mathf.Abs(a.b - b.b);

		if(k > 0.5f){
			k = 1.0f - (1f-k);
		}

		return h + s + k;

	}

	/// <summary>
	/// Returns the raw 'difference' between two colors. Unlike the other function, this does not use proximity.
	/// </summary>
	/// <returns>The difference.</returns>
	/// <param name="ca">Color a.</param>
	/// <param name="cb">Color b.</param>
	public static float GetRawDifferenceBetween(Color ca, Color cb){

		float diff = 0.0f;

		float r = Mathf.Abs(ca.r - cb.r);
		float g = Mathf.Abs(ca.g - cb.g);
		float b = Mathf.Abs(ca.b - cb.b);

		diff = r + g + b;

		return diff / 3f;

	}

	/// <summary>
	/// Is this a _strong_ color? In that... is there a big difference between any R/G/B? No idea why I needed this.
	/// </summary>
	/// <returns><c>true</c> if the color is 'strong'.</returns>
	/// <param name="c">The color</param>
	public static bool IsStrong(Color c){
		return IsStrong(c,defaultDifferenceThreshold);
	}

	/// <summary>
	/// Is this a _strong_ color? In that... is there a big difference between any R/G/B? No idea why I needed this.
	/// </summary>
	/// <returns><c>true</c> if the color is 'strong'.</returns>
	/// <param name="c">The color</param>
	/// <param name="differenceThreshold">How different should R/G or B be before returning true?</param>
	public static bool IsStrong(Color c, float differenceThreshold){

		float rg = Mathf.Abs(c.r - c.g);
		float gb = Mathf.Abs(c.g - c.b);
		float rb = Mathf.Abs(c.r - c.b);

		if(rg >= differenceThreshold ||
			gb >= differenceThreshold ||
			rb >= differenceThreshold){

			return true;

		}

		return false;

	}

	/// <summary>
	/// Is this color approximately greyscale within the given threshold?
	/// </summary>
	/// <returns><c>true</c> if it is approximately greyscale.</returns>
	/// <param name="c">The color</param>
	/// <param name="threshold">The threshold.</param>
	public static bool IsApproximatelyGreyscale(Color c, float threshold){

		float r = c.r;
		float g = c.g;
		float b = c.b;

		if(Mathf.Abs (r-g) <= threshold && Mathf.Abs(g-b) <= threshold){

			return true;

		}

		return false;

	}

	/// <summary>
	/// Is this color approximately greyscale within the default threshold?
	/// </summary>
	/// <returns><c>true</c> if it is approximately greyscale.</returns>
	/// <param name="c">The color</param>
	public static bool IsApproximatelyGreyscale(Color c){
		return IsApproximatelyGreyscale(c,defaultEqualityThreshold);
	}

	/// <summary>
	/// Are these colors approximately equal?
	/// </summary>
	/// <returns><c>true</c> if the colors are approximately equal.</returns>
	/// <param name="a">Color a.</param>
	/// <param name="b">Color b.</param>
	public static bool ApproximatelyEqual(Color a, Color b){

		return ApproximatelyEqual (a,b,defaultEqualityThreshold);

	}

	/// <summary>
	/// Are these colors approximately equal given an equality threshold?
	/// </summary>
	/// <returns><c>true</c> if the colors are approximately equal.</returns>
	/// <param name="a">Color a.</param>
	/// <param name="b">Color b.</param>
	/// <param name="threshold">The equality threshold.</param>
	public static bool ApproximatelyEqual(Color a, Color b, float threshold){

		float r_d = Mathf.Abs(a.r - b.r);

		if(r_d > threshold){
			return false;
		}

		float g_d = Mathf.Abs(a.g - b.g);

		if(g_d > threshold){
			return false;
		}

		float b_d = Mathf.Abs(a.b - b.b);

		if(b_d > threshold){
			return false;
		}

		return true;

	}

	#endregion

	#region Basic Color Modifiers

	/// <summary>
	/// Returns a random greyscale color - within tolerance bounds of .25 to .75
	/// </summary>
	/// <returns>The random greyscale color.</returns>
	public static Color GetRandomGreyscaleColor(){

		float r = UnityEngine.Random.Range(.25f,.75f);

		r = (float)Math.Round((double)r,2);

		return new Color(r,
			r,
			r);

	}

	/// <summary>
	/// Returns a random color.
	/// </summary>
	/// <returns>The random color.</returns>
	public static Color GetRandomColor(){

		return new Color(GetRandomFloat(),
			GetRandomFloat(),
			GetRandomFloat());

	}

	/// <summary>
	/// Inverts a color
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="c">The ... provided color.</param>
	public static Color FlipColor(Color c){

		c.r = 1.0f - c.r;
		c.g = 1.0f - c.g;
		c.b = 1.0f - c.b;

		return c;

	}

	/// <summary>
	/// Varies a color by random 'intensity' (e.g: red + (red * intensity))
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="c">Base color</param>
	/// <param name="intensity">Intensity - will be randomised as +/- intensity.</param>
	/// <param name="random">If true, each channel will have random intensity calculated for it.</param>
	public static Color VaryColor(Color c, float intensity, bool random){

		if(!random){

			float r = UnityEngine.Random.Range(-intensity,intensity);

			return new Color(
				c.r + (c.r * r),
				c.g + (c.g * r),
				c.b + (c.b * r)
			);

		}

		return new Color(VaryFloat (c.r,intensity),
			VaryFloat (c.g,intensity),
			VaryFloat (c.b,intensity));

	}

	/// <summary>
	/// Increments the hue.
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="c">The base color</param>
	/// <param name="hueChange">Hue change.</param>
	public static Color IncrementHue(Color c, float hueChange){

		HSBColor color = new HSBColor (c);

		color.h = WrapFloat(color.h + hueChange);

		color.h = Mathf.Clamp (color.h, 0f, 1f);

		return color.ToColor ();

	}

	/// <summary>
	/// Darkens the color.
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="c">The base color.</param>
	/// <param name="darkenIntensity">Darken intensity.</param>
	public static Color DarkenColor(Color c, float darkenIntensity){

		HSBColor color = new HSBColor (c);

		color.b -= darkenIntensity;

		color.b = Mathf.Clamp (color.b, 0f, 1f);

		return color.ToColor ();

	}

	/// <summary>
	/// Brightens the color.
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="c">Base color.</param>
	/// <param name="brightenIntensity">Brighten intensity.</param>
	public static Color BrightenColor(Color c, float brightenIntensity){

		HSBColor color = new HSBColor (c);

		color.b += brightenIntensity;

		color.b = Mathf.Clamp (color.b, 0f, 1f);

		return color.ToColor ();

	}

	/// <summary>
	/// Darkens the color by (intensity multiplied by darkness).
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="c">Base color</param>
	/// <param name="darkenIntensity">Darken intensity.</param>
	public static Color DarkenColorNormalized(Color c, float darkenIntensity){

		HSBColor color = new HSBColor (c);

		color.b -= (darkenIntensity * color.b);

		color.b = Mathf.Clamp (color.b, 0f, 1f);

		return color.ToColor ();

	}

	/// <summary>
	/// Brightens the color by (intensity multiplied by brightness).
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="c">Base color</param>
	/// <param name="brightenIntensity">Brighten intensity.</param>
	public static Color BrightenColorNormalized(Color c, float brightenIntensity){

		HSBColor color = new HSBColor (c);

		color.b += (brightenIntensity * color.b);

		color.b = Mathf.Clamp (color.b, 0f, 1f);

		return color.ToColor ();

	}

	/// <summary>
	/// Saturates the color by (intensity multiplied by saturation).
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="c">Base color</param>
	/// <param name="saturation">Saturation.</param>
	public static Color SaturateColorNormalized(Color c, float saturation){

		HSBColor color = new HSBColor (c);

		color.s += (saturation * color.s);

		color.s = Mathf.Clamp (color.s, 0f, 1f);

		return color.ToColor ();

	}

	/// <summary>
	/// Saturates the color.
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="c">Base color.</param>
	/// <param name="saturation">Saturation.</param>
	public static Color SaturateColor(Color c, float saturation){

		HSBColor color = new HSBColor (c);

		color.s += saturation;

		color.s = Mathf.Clamp (color.s, 0f, 1f);

		return color.ToColor ();

	}

	#endregion

	#region The meat - the color palette functions

	/// <summary>
	/// Get a color - derived from basecolor, modified by ....
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="baseColor">Base color.</param>
	/// <param name="offsetIntervals">The number of intervals (1/12th) to offset the hue of this color</param>
	/// <param name="desaturate">The amount to desaturate the color</param>
	/// <param name="darken">The amount to darken the color</param>
	static Color GetColor(HSBColor baseColor, int offsetIntervals, float desaturate, float darken){

		float offset = (float)offsetIntervals * colorInterval;

		baseColor.h = WrapFloat(baseColor.h + offset);

		baseColor.b = Mathf.Clamp(baseColor.b - darken,0f,1f);
		baseColor.s = Mathf.Clamp(baseColor.s - desaturate,0f,1f);

		return baseColor.ToColor();

	}

	/// <summary>
	/// Returns a set of analogous colors for the given base color
	/// </summary>
	/// <returns>The analogous colors.</returns>
	/// <param name="c">The base color</param>
	/// <param name="desaturate">The amount to desaturate the color</param>
	/// <param name="darken">The amount to darken the color</param>
	public static Color[] GetAnalogousColors(Color c, float desaturate, float darken){

		Color[] colors = new Color[analogousIntervals.Length];

		HSBColor hsb = HSBColor.FromColor(c);

		for(int i = 0; i < colors.Length; i++){

			colors[i] = GetColor(hsb,analogousIntervals[i],desaturate,darken);

		}

		return colors;

	}

	/// <summary>
	/// Returns a set of complementary colors for the given base color
	/// </summary>
	/// <returns>The complementary colors.</returns>
	/// <param name="c">The base color</param>
	/// <param name="desaturate">The amount to desaturate the color</param>
	/// <param name="darken">The amount to darken the color</param>
	public static Color[] GetComplementaryColors(Color c, float desaturate, float darken){

		Color[] colors = new Color[complementaryIntervals.Length];
		
		HSBColor hsb = HSBColor.FromColor(c);
		
		for(int i = 0; i < colors.Length; i++){

			colors[i] = GetColor(hsb,complementaryIntervals[i],desaturate,darken);
			
		}
		
		return colors;

	}

	/// <summary>
	/// Returns a set of triad colors for the given base color
	/// </summary>
	/// <returns>The triad colors.</returns>
	/// <param name="c">The base color</param>
	/// <param name="desaturate">The amount to desaturate the color</param>
	/// <param name="darken">The amount to darken the color</param>
	public static Color[] GetTriadColors(Color c, float desaturate, float darken){

		Color[] colors = new Color[triadIntervals.Length];
		
		HSBColor hsb = HSBColor.FromColor(c);
		
		for(int i = 0; i < colors.Length; i++){

			colors[i] = GetColor(hsb,triadIntervals[i],desaturate,darken);
			
		}
		
		return colors;

	}

	#endregion

	#region Color Strategies

	public static bool CanFetchColorForStrategy(ColorStrategy c){

		switch (c) {
		case ColorStrategy.Brightness:
		case ColorStrategy.Hue:
		case ColorStrategy.Saturation:
			{
				return false;
			}
		}

		return true;

	}

	public static Color[] GetColorsForStrategy(ColorStrategy strategy, Color baseColor){

		switch (strategy) {

		case ColorStrategy.Analagous:
			{
				return GetAnalogousColors (baseColor, 0f, 0f);
			}
		case ColorStrategy.Complementary:
			{
				return GetComplementaryColors (baseColor,0f,0f);
			}
		case ColorStrategy.Inverted:
			{
				return new Color[]{FlipColor (baseColor)};
			}
		case ColorStrategy.Triad:
			{
				return GetTriadColors (baseColor, 0f, 0f);
			}

		}

		return new Color[]{baseColor};

	}


	#endregion

	#region Randomization Functions

	static float VaryFloat(float f, float intensity){
		return VaryFloat(f,intensity,0.0f,1.0f);
	}

	static float VaryFloat(float f, float intensity, float min, float max){

		float r = UnityEngine.Random.Range(-intensity,intensity);

		f += r;

		if(f < min){
			f = min;
		}

		if(f > max){
			f = max;
		}

		return (float)Math.Round((double)f,2);

	}

	static float GetRandomFloat(){

		#if RESTRICT_DECIMALS
		return (float)Math.Round((double)UnityEngine.Random.value,2);
		#else
		return UnityEngine.Random.value;
		#endif

	}

	#endregion

	#region Helper Functions

	static float WrapFloat(float f){

		if(f < 0f){
			f = 1f + f;
		}

		if(f > 1f){
			f = f - 1f;
		}

		return f;

	}

	public static string ColorToHex(Color32 color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}

	public static Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r,g,b, 255);
	}

	#endregion

	#region To and From String functions - You won't need this - I use it for saving / loading

	public static string ColorToRawString(Color c){

		StringBuilder sb = new StringBuilder();

		int ri = (int)(c.r * 100.0f);
		int gi = (int)(c.g * 100.0f);
		int bi = (int)(c.b * 100.0f);

		sb.Append(ri);
		sb.Append(',');
		sb.Append(gi);
		sb.Append(',');
		sb.Append(bi);

		return sb.ToString();
	}


	public static string ColorToString(Color c){

		StringBuilder sb = new StringBuilder();

		c.r = (float)Math.Round((double)c.r,2);
		c.g = (float)Math.Round((double)c.g,2);
		c.b = (float)Math.Round((double)c.b,2);

		int ri = (int)(c.r * 100.0f);
		int gi = (int)(c.g * 100.0f);
		int bi = (int)(c.b * 100.0f);

		sb.Append(ri);
		sb.Append(',');
		sb.Append(gi);
		sb.Append(',');
		sb.Append(bi);

		return sb.ToString();
	}

	public static Color StringToColor(string s){

		string[] cSpl = s.Split(',');

		if(cSpl.Length != 3){

			return Color.black;
		}

		int ri,gi,bi;

		if(!int.TryParse(cSpl[0], out ri)){

			return Color.black;
		}
		if(!int.TryParse(cSpl[1], out gi)){

			return Color.black;

		}
		if(!int.TryParse(cSpl[2], out bi)){

			return Color.black;

		}

		float r = (float)ri / 100.0f;
		float g = (float)gi / 100.0f;
		float b = (float)bi / 100.0f;

		return new Color(r,g,b);

	}

	#endregion

}


