using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringParser : MonoBehaviour {

	public static string GetPercentString(int maxLenght, float f)
	{
		string s = f.ToString ();
		if (s.Length > maxLenght)
			s = s.Substring (0, maxLenght);
		if (s [s.Length - 1] == '.')
			s = s.Substring (0,s.Length-1);
		return s;
	}

	public static string GetPercentString(float f)
	{
		return GetPercentString (3, f);
	}

    public static string GetTimeString(float t, bool isNeedMilli = false)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(t);  
        if(timeSpan.Hours>0)
            return string.Format("{0:0}h {1:D2}m {2:D2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        if (!isNeedMilli)
            return string.Format("{0:D2}m {1:D2}s", timeSpan.Minutes, timeSpan.Seconds);
        else
            return string.Format("{0:D2}m {1:D2}s {2:D2}ms", timeSpan.Minutes, timeSpan.Seconds,timeSpan.Milliseconds/10);
    }
    
    public static void GetTime(float t, out int hours, out int minutes, out int seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(t);
        hours = 0;
        if (timeSpan.Hours > 0)
            hours = timeSpan.Hours;

        minutes = timeSpan.Minutes;
        seconds = timeSpan.Seconds;       
    }

    public static string GetMoneyStringFormat(long money)
    {
        string s = string.Format("{0:## ### ### ### ### ### ### ### ### ### ###}", money);
        if (money == 0)
        {
            s = "0";
        }
        return s.Trim();
    } 
    
    public static string GetFloatFormat(float value)
    {
        string s = string.Format("{0:f1}", value);
        return s.Trim();
    }

    public static string GetOdometerStringFormat(long curValue)
    {
        string s = curValue.ToString("D6");
        if (curValue == 0)
        {
            s = "000000";
        }
        return s.Trim();
    }

    public static string GetDate(DateTime newDate)
    {
        return newDate.Date.ToShortDateString();
    }
}
