using NUnit.Framework.Interfaces;
using UnityEngine;

public class PowerCell : MonoBehaviour
{
    public ItemData itemData;

    private void OnDestroy()
    {
        ConnectionPoint cp = GetComponent<ConnectionPoint>();
        if (cp != null && cp.connectedCable != null)
        {
            Destroy(cp.connectedCable);
        }
    }
}
