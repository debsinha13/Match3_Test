using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SetUpBoard : MonoBehaviour
{
    public int rows = 8;
    public int cols = 9;
    public int[,] grid;

    private void Start()
    {
        grid = new int[rows, cols];
    }
}
