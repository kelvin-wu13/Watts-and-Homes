using System.Collections.Generic;
using UnityEngine;

public class ConnectionPointManager : MonoBehaviour
{
    private static ConnectionPointManager instance;
    private static List<ConnectionPoint> allConnectionPoints = new List<ConnectionPoint>();
    private static ConnectionPoint[] cachedArray;
    private static bool arrayNeedsUpdate = true;

    public static ConnectionPointManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ConnectionPointManager");
                instance = go.AddComponent<ConnectionPointManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public static void RegisterConnectionPoint(ConnectionPoint point)
    {
        if (!allConnectionPoints.Contains(point))
        {
            allConnectionPoints.Add(point);
            arrayNeedsUpdate = true;
        }
    }

    public static void UnregisterConnectionPoint(ConnectionPoint point)
    {
        if (allConnectionPoints.Remove(point))
        {
            arrayNeedsUpdate = true;
        }
    }

    public static ConnectionPoint[] GetAllConnectionPoints()
    {
        if (arrayNeedsUpdate || cachedArray == null)
        {
            cachedArray = allConnectionPoints.ToArray();
            arrayNeedsUpdate = false;
        }
        return cachedArray;
    }

    public static int GetConnectionPointCount()
    {
        return allConnectionPoints.Count;
    }

    public static void CleanupNullReferences()
    {
        for (int i = allConnectionPoints.Count - 1; i >= 0; i--)
        {
            if (allConnectionPoints[i] == null)
            {
                allConnectionPoints.RemoveAt(i);
                arrayNeedsUpdate = true;
            }
        }
    }
}