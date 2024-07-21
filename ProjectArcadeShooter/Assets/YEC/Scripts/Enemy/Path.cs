using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
public class Path : MonoBehaviour
{

    public List<Vector3> waypoints = new List<Vector3>();
    [SerializeField]
    private bool alwaysDrawPath;
    [SerializeField]
    private bool drawAsLoop;
    [SerializeField]
    private bool drawNumbers;
    public Color debugColour = Color.white;

    public void OnDrawGizmos()
    {
        if (alwaysDrawPath)
        {
            DrawPath();
        }
    }
    public void DrawPath()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 30;
            labelStyle.normal.textColor = debugColour;
            if (drawNumbers)
                Handles.Label(waypoints[i], i.ToString(), labelStyle);
            if (i >= 1)
            {
                Gizmos.color = debugColour;
                Gizmos.DrawLine(waypoints[i - 1], waypoints[i]);

                if (drawAsLoop)
                    Gizmos.DrawLine(waypoints[waypoints.Count - 1], waypoints[0]);

            }
        }
    }
    public void OnDrawGizmosSelected()
    {
        if (alwaysDrawPath)
            return;
        else
            DrawPath();
    }

    public static object Combine(string dataPath, string v)
    {
        throw new NotImplementedException();
    }
}