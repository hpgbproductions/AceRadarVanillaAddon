using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class RaceBlipManager : MonoBehaviour
{
    [SerializeField] private AceRadarBackend Backend;

    private Type LevelCourseScriptType;
    private Component LevelCourseScript;
    private PropertyInfo CurrentRingInfo;

    private Type RingScriptType;
    private PropertyInfo RingNumberInfo;

    private Component[] AllRings;
    private int[] AllRingsNumbers;

    private Component CurrentRing;
    private int CurrentRingNumber;

    private void Start()
    {
        Debug.Log("Found AceRadarBackend in GameObject " + Backend.gameObject.name);

        Component[] allComponents = FindObjectsOfType<Component>();
        foreach (Component c in allComponents)
        {
            Type cType = c.GetType();
            if (cType.Name == "LevelCourseScript")
            {
                LevelCourseScript = c;
                LevelCourseScriptType = cType;
                Debug.Log("Found LevelCourseScript in GameObject " + LevelCourseScript.gameObject.name);
                break;
            }
        }
    }
}
