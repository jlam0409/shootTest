using UnityEngine;
using System.Collections;

public enum EaseType {
	None,	/**< normal at start, normal at end */
	In,	/**< acceleration at start, normal at end */
	Out,	/**< normal at start, deceleration at end */
	InOut,	/**< acceleration at start, deceleration at end */
	Spring,	/**< spring motion at end*/
	InElastic,
	OutElastic,
	InOutElastic
}

public class Ease{
	/**
		\fn	float	Ease (time : float, easeType : EaseType)
		\brief	time calculation with input ease type
		\param[in]	time	input time [ 0f - 1.0f ] 
		\param[in]	easeType	EaseType
		\return	time calculated
	*/
	public static float GetTime (float time , EaseType easeType) {
		float returnTime= time;
		switch ( easeType ){
			case EaseType.In:
				returnTime = EaseIn (time);
				break;
			
			case EaseType.Out:
				returnTime = EaseOut (time);
				break;
			
			case EaseType.InOut:			
				returnTime = EaseInOut(time);
				break;
			
			case EaseType.Spring:
				returnTime = EaseSpring (time);
				break;
			case EaseType.InElastic:
				returnTime = EaseInElastic ( 0f, 1f, time);
				break;
			
			case EaseType.OutElastic:
				returnTime = EaseOutElastic (0f,1f, time);
				break;
			case EaseType.InOutElastic:
				returnTime = EaseInOutElastic (0f,1f, time);
				break;
			case EaseType.None:
			default:
				returnTime = time;
				break;
		}
		return returnTime;
	}
	
	private static float EaseIn(float time) {
		return Mathf.Lerp (0.0f, 1.0f, 1.0f - Mathf.Cos (time * Mathf.PI * 0.5f) );
	}
	private static float EaseOut (float time) {
		return Mathf.Lerp(0.0f, 1.0f, Mathf.Sin(time * Mathf.PI * 0.5f));
	}
	
	private static float EaseInOut (float time) {
		return Mathf.SmoothStep(0.0f, 1.0f, time);
	}
	
	private static float EaseSpring (float value) {
		float start = 0.0f;
		float end = 1.0f ;
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}
	
	private static float EaseInElastic(float start, float end, float value) {
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0f;
		float a = 0f;
		
		if (value == 0)
			return start;
		
		if ((value /= d) == 1)
			return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)) {
			a = end;
			s = p / 4;
		} else {
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		return -(a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	}
		
	
	private static float EaseOutElastic(float start, float end, float value) {
		end -= start;
		
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0)
			return start;
		
		if ((value /= d) == 1)
			return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)) {
			a = end;
			s = p / 4;
		} else {
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}		

	
	private static float EaseInOutElastic(float start, float end, float value) {
		end -= start;
			
		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;
		
		if (value == 0)
			return start;
		
		if ((value /= d/2) == 2)
			return start + end;
		
		if (a == 0f || a < Mathf.Abs(end)) {
			a = end;
			s = p / 4;
		} else {
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}
		
		if (value < 1) {
			return -0.5f * (a * Mathf.Pow(2, 10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		} else {
			return a * Mathf.Pow(2, -10 * (value-=1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
		}
	}
}
