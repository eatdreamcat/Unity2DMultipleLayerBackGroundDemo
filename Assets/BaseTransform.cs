using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BaseTransform : MonoBehaviour
{
    public float ratio = 1.0f;
    
    public Text scaleText;
    
    [Serializable]
    protected struct ConstraintChildNode
    {
        public float moveRatio;
        public GameObject go;
    }
    [SerializeField]
    protected List<ConstraintChildNode> m_ConstraintList = new List<ConstraintChildNode>();

    
    /**
     * 计算水平边界
     */
    static float MaxHorizonalBorder(GameObject go, float ratio)
    {
        return (go.GetComponent<RectTransform>().rect.width * go.GetComponent<RectTransform>().localScale.x - Screen.width) / 2f * ratio;
    }
    
    /**
     * 计算竖直边界
     */
    static float MaxVerticalBorder(GameObject go, float ratio)
    {
        return ( go.GetComponent<RectTransform>().rect.height *  go.GetComponent<RectTransform>().localScale.y - Screen.height) / 2f * ratio;
    }
    
    /**
     * 计算当前变换x
     */
    static float xTransformed(GameObject go)
    {
        return go.GetComponent<RectTransform>().localPosition.x
               + (0.5f - go.GetComponent<RectTransform>().pivot.x) * go.GetComponent<RectTransform>().rect.width * go.GetComponent<RectTransform>().localScale.x;
    }

    /**
     * 计算当前变换y
     */
    static float yTransformed(GameObject go)
    {
        return go.GetComponent<RectTransform>().localPosition.y
               + (0.5f - go.GetComponent<RectTransform>().pivot.y) * go.GetComponent<RectTransform>().rect.height * go.GetComponent<RectTransform>().localScale.y;
    }
    
    
    
    private void Awake()
    {
        Mouse.Move += OnMove;
        Mouse.Scroll += OnScale;
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

    /**
     * 更新子节点位置
     */
    private void UpdateConstraintList(float deltaX, float deltaY)
    {
        /// 更新约束子节点
        for (int i = 0; i < m_ConstraintList.Count; i++)
        {
            var go = m_ConstraintList[i].go;
            var ratio = m_ConstraintList[i].moveRatio;
            
            go.GetComponent<RectTransform>().localPosition = new Vector3(
                go.GetComponent<RectTransform>().localPosition.x + deltaX * ratio, 
                go.GetComponent<RectTransform>().localPosition.y + deltaY * ratio,
                go.GetComponent<RectTransform>().localPosition.z);
        }
    }
    
    /**
     * 场景拖拽
     */
    public void OnMove(float deltaX, float deltaY)
    {
        if (Math.Abs(xTransformed(gameObject) + deltaX) >= MaxHorizonalBorder(gameObject, ratio))
        {
            deltaX = 0;
        }
      
        
        if (Math.Abs(yTransformed(gameObject) + deltaY) >= MaxVerticalBorder(gameObject, ratio))
           
        {
            deltaY = 0;
        }
      
      

        if (deltaX != 0 || deltaY != 0)
        {
            GetComponent<RectTransform>().localPosition = new Vector3(
                GetComponent<RectTransform>().localPosition.x + deltaX * ratio, 
                GetComponent<RectTransform>().localPosition.y + deltaY * ratio,
                GetComponent<RectTransform>().localPosition.z);
            
            UpdateConstraintList(deltaX * ratio, deltaY * ratio);
        }
    }
    
    /**
     * 获取最小缩放值
     */
    float MinScale
    {
        get
        {
            return Math.Max(Screen.width / GetComponent<RectTransform>().rect.width,
                Screen.height / GetComponent<RectTransform>().rect.height);
        }
    }

    /**
     * 计算改变pivot之后，需要调整的位移，确保保持相对位置不变
     */
    static Vector2 CalculPivotOffset(Vector2 pivotChanged, GameObject go)
    {
        Vector2 offset = pivotChanged * new Vector2(go.GetComponent<RectTransform>().rect.width,
            go.GetComponent<RectTransform>().rect.height) * new Vector2(go.GetComponent<RectTransform>().localScale.x, go.GetComponent<RectTransform>().localScale.y);
        return offset;
    }

    /**
     * 根据点击位置，获取每个图片对应的缩放pivot
     */
    static void SetScalePivot(ref Vector2 center, Vector2 position, GameObject go)
    {
        var width = go.GetComponent<RectTransform>().rect.width *  go.GetComponent<RectTransform>().localScale.x;
        var height =  go.GetComponent<RectTransform>().rect.height *  go.GetComponent<RectTransform>().localScale.y;

        var origin = new Vector2( go.GetComponent<RectTransform>().localPosition.x, go.GetComponent<RectTransform>().localPosition.y) - CalculPivotOffset(go.GetComponent<RectTransform>().pivot, go);

        // position -= origin;

        center.x = Math.Abs( position.x - origin.x ) / width;
        center.y = Math.Abs( position.y - origin.y ) / height;
    }
    
    /**
     * 执行缩放
     */
    void OnScale(float deltaX, float deltaY, Vector2 position)
    {

        Vector2 center = Vector2.zero;

        // 获取对应的中心位置
        SetScalePivot(ref center, position, gameObject);

        // Debug.Log($"{gameObject.name} Center:({center.x},{center.y})");
        // return;
        
        var scaleValue = Math.Abs(deltaY) / deltaY * 0.01f;

        if (GetComponent<RectTransform>().localScale.x + scaleValue <= MinScale)
        {
            return;
        }
        
        // 设置中心点
        UpdatePivot(gameObject, center,  out var offset);
        
        // 缩放
        GetComponent<RectTransform>().localScale += new Vector3(scaleValue, scaleValue, 0);

        // 因为重设了中心点，调整位置避免黑边，父节点肯定是早于子节点有黑边， 因为父节点移动的快，这里的父节点是城堡
        ScaleMove(gameObject, offset.x, offset.y, ratio);

        
        /// 更新约束子节点
        for (int i = 0; i < m_ConstraintList.Count; i++)
        {
            var go = m_ConstraintList[i].go;
            var ratio = m_ConstraintList[i].moveRatio;
            
            // 获取对应的中心位置
            SetScalePivot(ref center, position, go);
            
            if ( go.GetComponent<RectTransform>().localScale.x + scaleValue <= MinScale)
            {
                continue;
            }
            
            // 设置中心点
            UpdatePivot(go, center,  out var childOffset);
            
            go.GetComponent<RectTransform>().localScale += new Vector3(scaleValue, scaleValue, 0);
            
            ScaleMove( go,  childOffset.x,  childOffset.y, ratio);
            
        }
      
    }

    // 设置中心点后，计算返回需要调整的位移，保证位置不动
    public static void UpdatePivot(GameObject go, Vector2 center, out Vector2 offset)
    {
        Vector2 pivotChanged = center - go.GetComponent<RectTransform>().pivot;

        offset = CalculPivotOffset(pivotChanged, go);
        
        go.GetComponent<RectTransform>().pivot = center;
       
       
    }
    
    
    public static void ScaleMove(GameObject go, float deltaX, float deltaY, float ratio)
    {
        
        go.GetComponent<RectTransform>().localPosition = new Vector3(
            go.GetComponent<RectTransform>().localPosition.x + deltaX, 
            go.GetComponent<RectTransform>().localPosition.y + deltaY,
            go.GetComponent<RectTransform>().localPosition.z);
        
        Vector3 tuneing = Vector3.zero;
        if (xTransformed(go) >= MaxHorizonalBorder(go, ratio))
        {
            tuneing.x = -(float)(xTransformed(go) - MaxHorizonalBorder(go, ratio));
            
        } else if (xTransformed(go) <= -MaxHorizonalBorder(go, ratio))
        {
            tuneing.x = (float)(Math.Abs(xTransformed(go)) - MaxHorizonalBorder(go, ratio));
        }
      
        
        if (yTransformed(go) >= MaxVerticalBorder(go, ratio))
        {
            tuneing.y = -(float)(yTransformed(go) - MaxVerticalBorder(go, ratio));
        }
        else if (yTransformed(go) <= -MaxVerticalBorder(go, ratio))
        {
            tuneing.y = (float)(Math.Abs(yTransformed(go)) - MaxVerticalBorder(go, ratio));
        }

        if (tuneing.magnitude > 0)
        {
            go.GetComponent<RectTransform>().localPosition += tuneing;
        }

    }

    
    
}
