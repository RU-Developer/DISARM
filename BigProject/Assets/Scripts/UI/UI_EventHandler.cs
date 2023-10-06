using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * UI Event를 추가할 때 사용하는 이벤트 핸들러입니다.
 */
public class UI_EventHandler : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    Action<PointerEventData> OnClickHandler = null;
    Action<PointerEventData> OnDragHandler = null;

    /**
     * 마우스 클릭시 이벤트 전파
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);
    }

    /**
     * 마우스 클릭 수동 호출
     */ 
    public void OnPointerClick()
    {
        if (OnClickHandler != null)
            OnClickHandler.Invoke(null);
    }

    /**
     * 마우스 드래그시 이벤트 전파
     */
    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

    /**
     * 마우스 클릭 이벤트 핸들러 등록
     */
    public void AddOnClickHandler(Action<PointerEventData> handler)
    {
        OnClickHandler -= handler;
        OnClickHandler += handler;
    }

    /**
     * 마우스 드래그 이벤트 핸들러 등록
     */
    public void AddOnDragHandler(Action<PointerEventData> handler)
    {
        OnDragHandler -= handler;
        OnDragHandler += handler;
    }
}
