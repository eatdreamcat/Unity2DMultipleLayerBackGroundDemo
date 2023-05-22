using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    
    public float ratio = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        Mouse.Move += OnMove;
      
    }

    float MaxHorizonalBorder
    {
        get
        {
            return (GetComponent<RectTransform>().rect.width * GetComponent<RectTransform>().localScale.x - Screen.width) / 2f * ratio;
        }
    }
    
    float MaxVerticalBorder
    {
        get
        {
            return (GetComponent<RectTransform>().rect.height * GetComponent<RectTransform>().localScale.y - Screen.height) / 2f * ratio;
        }
    }
    
    public void ScaleMove(float deltaX, float deltaY)
    {
        
        
        GetComponent<RectTransform>().localPosition = new Vector3(
            GetComponent<RectTransform>().localPosition.x + deltaX, 
            GetComponent<RectTransform>().localPosition.y + deltaY,
            GetComponent<RectTransform>().localPosition.z);
        
        Vector3 posTuneing = Vector3.zero;
        if (xTransformed >= MaxHorizonalBorder)
        {
            posTuneing.x = -(float)(xTransformed - MaxHorizonalBorder);
            
        } else if (xTransformed <= -MaxHorizonalBorder)
        {
            posTuneing.x = (float)(Math.Abs(xTransformed) - MaxHorizonalBorder);
        }
      
        
        if (yTransformed >= MaxVerticalBorder)
        {
            posTuneing.y = -(float)(yTransformed - MaxVerticalBorder);
        }
        else if (yTransformed <= -MaxVerticalBorder)
        {
            posTuneing.y = (float)(Math.Abs(yTransformed) - MaxVerticalBorder);
        }

        GetComponent<RectTransform>().localPosition += posTuneing;
    }

    double xTransformed
    {
        get
        {
            return GetComponent<RectTransform>().localPosition.x
                   + (0.5 - GetComponent<RectTransform>().pivot.x) * GetComponent<RectTransform>().rect.width * GetComponent<RectTransform>().localScale.x;
        }
    }

    double yTransformed
    {
        get
        {
            return GetComponent<RectTransform>().localPosition.y
                   + (0.5 - GetComponent<RectTransform>().pivot.y) * GetComponent<RectTransform>().rect.height * GetComponent<RectTransform>().localScale.y;
        }
    }
    public void OnMove(float deltaX, float deltaY)
    {
        if (Math.Abs(xTransformed + deltaX) >= MaxHorizonalBorder)
        {
            deltaX = 0;
        }
      
        
        if (Math.Abs(yTransformed + deltaY) >= MaxVerticalBorder)
           
        {
            deltaY = 0;
        }
      
      

        if (deltaX != 0 || deltaY != 0)
        {
            GetComponent<RectTransform>().localPosition = new Vector3(
                GetComponent<RectTransform>().localPosition.x + deltaX * ratio, 
                GetComponent<RectTransform>().localPosition.y + deltaY * ratio,
                GetComponent<RectTransform>().localPosition.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
