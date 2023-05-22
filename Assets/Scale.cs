using System;
using UnityEngine;
using UnityEngine.UI;

public class Scale : MonoBehaviour
{
    // Start is called before the first frame update

    public Text scaleText;
    // public float ratio = 1.0f;

    private void Awake()
    {
        Mouse.Scroll += OnScale;
    }

    float MinScale
    {
        get
        {
            return Math.Max(Screen.width / GetComponent<RectTransform>().rect.width,
                Screen.height / GetComponent<RectTransform>().rect.height);
        }
    }

    Vector2 CalculPivotOffset(Vector2 pivotChanged)
    {
        Vector2 offset = pivotChanged * new Vector2(GetComponent<RectTransform>().rect.width,
            GetComponent<RectTransform>().rect.height) * new Vector2(GetComponent<RectTransform>().localScale.x, GetComponent<RectTransform>().localScale.y);
        return offset;
    }

    private void SetScalePivot(ref Vector2 center, Vector2 position)
    {
        var width = GetComponent<RectTransform>().rect.width * GetComponent<RectTransform>().localScale.x;
        var height = GetComponent<RectTransform>().rect.height * GetComponent<RectTransform>().localScale.y;

        var origin = new Vector2(GetComponent<RectTransform>().localPosition.x, GetComponent<RectTransform>().localPosition.y) - CalculPivotOffset(GetComponent<RectTransform>().pivot);

        // position -= origin;

        center.x = Math.Abs( position.x - origin.x ) / width;
        center.y = Math.Abs( position.y - origin.y ) / height;
    }
    
    void OnScale(float deltaX, float deltaY, Vector2 position)
    {

        Vector2 center = Vector2.zero;

        SetScalePivot(ref center, position);

        // Debug.Log($"{gameObject.name} Center:({center.x},{center.y})");
        // return;
        
        var scaleValue = Math.Abs(deltaY) / deltaY * 0.01f;

        if (GetComponent<RectTransform>().localScale.x + scaleValue <= MinScale)
        {
            return;
        }
        
        Vector2 pivotChanged = center - GetComponent<RectTransform>().pivot;

        var offset = CalculPivotOffset(pivotChanged);
        
        GetComponent<RectTransform>().pivot = center;
       
        GetComponent<RectTransform>().localScale += new Vector3(scaleValue, scaleValue, 0);
        
        GetComponent<Move>().ScaleMove(offset.x, offset.y);

        // if (Math.Abs(GetComponent<RectTransform>().localScale.x - 1.0f) <= 0.01f)
        // {
        //     center.x = 0.0f;
        //     center.y = 0.0f;
        //     
        //     pivotChanged = center - GetComponent<RectTransform>().pivot;
        //     GetComponent<RectTransform>().pivot = center;
        //   
        //     offset = CalculPivotOffset(pivotChanged);
        //     GetComponent<Move>().ScaleMove(offset.x, offset.y);
        //     
        // }
        
    }

    
    // Update is called once per frame
    void Update()
    {
        if (scaleText != null)
        {
            scaleText.text =
                "scale:" + GetComponent<RectTransform>().localScale.x;
        }
    }
}
