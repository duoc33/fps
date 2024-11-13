using System;
using UnityEngine;

[Serializable]
public class OnGUIAgent
{
    public bool isActivate = true;

    public int startX;
    public int startY;
    public int height = 50;
    public int width = 100;
    public int maxRows = 10;
    
    private int i = 0;
    private int j = 0;

    public void Begin(int startX, int startY, int height, int width, int maxRows)
    {
        this.startX = startX;
        this.startY = startY;
        this.height = height;
        this.width = width;
        this.maxRows = maxRows;
        Begin();
    }

    public void Begin()
    {
        i = 0;
        j = 0;
        PlaceHideThis();

    }
    
    public void PlaceElement(string display, Action action)
    {
        if(!isActivate) return;
        
        if (GUI.Button(new Rect(startX + width * i, startY + height * j++, width, height), display))
        {
            action?.Invoke();
        }
            
        if (j > maxRows)
        {
            AnotherColumn();
        }
    }

    private void PlaceHideThis()
    {
        if(!isActivate) return;
        if (GUI.Button(new Rect(startX + width * i, startY + height * j++, width, height), "HideThis"))
        {
            isActivate = false;
        }
        
    }

    private void AnotherColumn()
    {
        i++;
        j = 0;
    }
        
        
}