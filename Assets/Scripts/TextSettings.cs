using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//テキストがぼやけるのを防止する汎用スクリプト
public class TextSettings : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().font.material.mainTexture.filterMode = FilterMode.Point;
    }
}
