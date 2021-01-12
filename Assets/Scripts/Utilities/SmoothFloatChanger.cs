using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFloatChanger
{
    public float CurValue
    {
        get { return curValue; }
    }

    private float curValue = 0;
    private float newValue = 0;

    public void Init(float newValue)
    {
        //выставляем оба, чтобы первое значение выставлялось сразу, а не набиралось
        this.curValue = newValue;
        this.newValue = newValue;
    }

    public void SetNewValue(float newValue)
    {
        this.newValue = newValue;
    }


    public bool UpdateValue()
    {
        if (newValue != curValue)
        {
            float sign = (newValue - curValue) / 15;
            curValue = curValue + sign;
            if (Mathf.Abs(curValue - newValue) < sign*2)
                curValue = newValue;

            return true;
        }

        return false;
    }

    public bool UpdateValue(float speed)
    {
        if (newValue != curValue)
        {
            float sign = Mathf.Sign(newValue - curValue);
            float increment = Mathf.Abs(newValue - curValue) * Time.deltaTime * speed;
            curValue = curValue + (increment * sign);
          
            return true;
        }

        return false;
    }

}