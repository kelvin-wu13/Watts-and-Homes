using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    [Header("Connection Configuration")]
    public MonoBehaviour parentItem;

    [Header("Connection Status")]
    public bool isConnected = false;

    [Header("Cable Management")]
    public GameObject connectedCable;
    public List<GameObject> connectedCables = new List<GameObject>();

    [Header("Connection Limits")]
    public int maxConnections = 1;
    public bool allowMultipleConnections = false;

    private void Awake()
    {
        ConnectionPointManager.RegisterConnectionPoint(this);

        if (parentItem == null)
        {
            parentItem = GetComponentInParent<PowerCell>();
        }
        if (parentItem == null)
        {
            parentItem = GetComponentInParent<Target>();
        }
        if (parentItem == null)
        {
            parentItem = GetComponentInParent<SolarFrame>();
        }
        if (parentItem == null)
        {
            parentItem = GetComponentInParent<CombinerBox>();
        }
        if (parentItem == null)
        {
            parentItem = GetComponentInParent<DividerBox>();
        }

        if (parentItem is CombinerBox || parentItem is Target)
        {
            allowMultipleConnections = true;
            maxConnections = 10;
        }
        else
        {
            allowMultipleConnections = false;
            maxConnections = 1;
        }

        if (connectedCables == null)
        {
            connectedCables = new List<GameObject>();
        }

        if (connectedCable != null && !connectedCables.Contains(connectedCable))
        {
            connectedCables.Add(connectedCable);
        }

        UpdateConnectionStatus();
    }

    private void Start()
    {
        UpdateConnectionStatus();
    }

    public bool ConnectCable(GameObject cable)
    {
        if (cable == null)
        {
            return false;
        }

        if (connectedCables.Contains(cable))
        {
            return true;
        }

        if (!CanAcceptMoreConnections())
        {
            return false;
        }

        connectedCables.Add(cable);

        if (connectedCables.Count == 1)
        {
            connectedCable = cable;
        }

        UpdateConnectionStatus();
        return true;
    }

    public bool DisconnectCable(GameObject cable)
    {
        if (cable == null || !connectedCables.Contains(cable))
        {
            return false;
        }

        connectedCables.Remove(cable);

        if (connectedCables.Count > 0)
        {
            connectedCable = connectedCables[0];
        }
        else
        {
            connectedCable = null;
        }

        UpdateConnectionStatus();
        return true;
    }

    public void DisconnectAllCables()
    {
        connectedCables.Clear();
        connectedCable = null;
        UpdateConnectionStatus();
    }

    private void UpdateConnectionStatus()
    {
        isConnected = connectedCables.Count > 0;
    }

    public int GetConnectionCount()
    {
        return connectedCables.Count;
    }
    public bool CanAcceptMoreConnections()
    {
        return connectedCables.Count < maxConnections;
    }

    public List<GameObject> GetConnectedCables()
    {
        return new List<GameObject>(connectedCables);
    }

    public bool IsCableConnected(GameObject cable)
    {
        return cable != null && connectedCables.Contains(cable);
    }

    public void SetConnectionLimits(bool allowMultiple, int maxConnections)
    {
        this.allowMultipleConnections = allowMultiple;
        this.maxConnections = maxConnections;

        if (!allowMultiple && connectedCables.Count > 1)
        {
            GameObject firstCable = connectedCables[0];
            for (int i = connectedCables.Count - 1; i > 0; i--)
            {
                connectedCables.RemoveAt(i);
            }
            connectedCable = firstCable;
            UpdateConnectionStatus();
        }
    }

    private void OnDestroy()
    {
        ConnectionPointManager.UnregisterConnectionPoint(this);
        DisconnectAllCables();
    }
}