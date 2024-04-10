using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GreasableObject : MonoBehaviour
{

    [SerializeField] private Renderer grease;

    private bool isGreased = false;
    private bool hasGrease = false;
    private int maxCount = 15;
    private int count = 0;
    private Color color;

    private void Start()
    {
        color = grease.material.color;
        color.a = 0f;
        grease.material.color = color;
    }

    public void AddGrease()
    {
        if (isGreased) return;

        count++;
        hasGrease = true;
        color.a = (float)count / (float)maxCount;
        grease.material.color = color;
        if (count >= maxCount - 5) isGreased = true;
    }

    public bool IsGreased()
    {
        return isGreased;
    }

    public bool HasGrease() => hasGrease;

}
