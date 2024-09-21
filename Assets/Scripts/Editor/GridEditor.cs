using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SetUpBoard))]
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target object (TwoDArrayExample script)
        SetUpBoard example = (SetUpBoard)target;

        // Ensure the grid is initialized
        if (example.grid == null || example.grid.GetLength(0) != example.rows || example.grid.GetLength(1) != example.cols)
        {
            example.grid = new int[example.rows, example.cols];
        }

        // Draw rows and columns fields for user input
        example.rows = EditorGUILayout.IntField("Rows", example.rows);
        example.cols = EditorGUILayout.IntField("Columns", example.cols);

        // Iterate over the grid and display each element in the Inspector
        for (int row = 0; row < example.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(); // Start a new row
            for (int col = 0; col < example.cols; col++)
            {
                example.grid[row, col] = EditorGUILayout.IntField(example.grid[row, col], GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal(); // End the row
        }

        // Apply changes to the target object
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

