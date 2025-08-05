using TMPro;
using UnityEngine;

public class DividerBox : MonoBehaviour, IPowerDataProvider
{
    public ItemData itemData;

    public ConnectionPoint inputPoint;
    public ConnectionPoint outputPoint1;
    public ConnectionPoint outputPoint2;

    public TMP_InputField valueInput1;
    public TMP_InputField valueInput2;

    private float totalPowerInput = 0f;
    private bool isUpdatingUI = false;


    void Start()
    {
        EnsureInputFieldsAreChildren();

        if (valueInput1 != null && valueInput2 != null)
        {
            valueInput1.onEndEdit.AddListener((value) => OnValueChanged(valueInput1, valueInput2, value));
            valueInput2.onEndEdit.AddListener((value) => OnValueChanged(valueInput2, valueInput1, value));
        }

        if (inputPoint != null) inputPoint.SetConnectionLimits(false, 1);
        if (outputPoint1 != null) outputPoint1.SetConnectionLimits(false, 1);
        if (outputPoint2 != null) outputPoint2.SetConnectionLimits(false, 1);

        ForceUpdateDistribution();
    }
    public string GetPowerDisplayText()
    {
        return $"{totalPowerInput:F1} W";
    }

    private void EnsureInputFieldsAreChildren()
    {
        if (valueInput1 != null && !IsChildOfThis(valueInput1.transform))
        {
            valueInput1.transform.SetParent(this.transform, true);
        }

        if (valueInput2 != null && !IsChildOfThis(valueInput2.transform))
        {
            valueInput2.transform.SetParent(this.transform, true);
        }
    }
    private bool IsChildOfThis(Transform child)
    {
        Transform parent = child.parent;
        while (parent != null)
        {
            if (parent == this.transform)
                return true;
            parent = parent.parent;
        }
        return false;
    }

    private Vector3 lastPosition;

    void Update()
    {
        if (transform.position != lastPosition)
        {
            lastPosition = transform.position;
        }
    }
   

    private void OnValueChanged(TMP_InputField activeInput, TMP_InputField otherInput, string newValue)
    {
        if (isUpdatingUI) return;

        UpdateTotalInputPower();

        float.TryParse(newValue, out float activeValue);
        activeValue = Mathf.Clamp(activeValue, 0, totalPowerInput);

        float otherValue = totalPowerInput - activeValue;

        isUpdatingUI = true;
        activeInput.text = activeValue.ToString("F1");
        otherInput.text = otherValue.ToString("F1");
        isUpdatingUI = false;

        float finalPower1 = float.Parse(valueInput1.text);
        float finalPower2 = float.Parse(valueInput2.text);

        DistributePowerToOutput(outputPoint1, finalPower1);
        DistributePowerToOutput(outputPoint2, finalPower2);
    }

    public void ForceUpdateDistribution()
    {
        UpdateTotalInputPower();

        float halfPower = totalPowerInput / 2f;

        isUpdatingUI = true;
        if (valueInput1 != null) valueInput1.text = halfPower.ToString("F1");
        if (valueInput2 != null) valueInput2.text = halfPower.ToString("F1");
        isUpdatingUI = false;

        DistributePowerToOutput(outputPoint1, halfPower);
        DistributePowerToOutput(outputPoint2, halfPower);
    }
    private void UpdateTotalInputPower()
    {
        totalPowerInput = 0f;
        if (inputPoint != null && inputPoint.isConnected)
        {
            if (inputPoint.connectedCables.Count == 0) return;
            GameObject cable = inputPoint.connectedCables[0];
            if (cable == null) return;
            CableInteractable cableInfo = cable.GetComponentInChildren<CableInteractable>();
            if (cableInfo != null)
            {
                ConnectionPoint sourcePoint = cableInfo.GetOtherPoint(inputPoint);
                if (sourcePoint != null)
                {
                    totalPowerInput = GetPowerFromSource(sourcePoint);
                }
            }
        }
    }
    private float GetPowerFromSource(ConnectionPoint inputPoint)
    {
        MonoBehaviour parent = inputPoint.parentItem;

        if (parent is PowerCell cell)
        {
            return cell.itemData.powerGeneration;
        }
        if (parent is SolarFrame frame)
        {
            return frame.GetNetPowerOutput();
        }
        return 0;
    }
    private void DistributePowerToOutput(ConnectionPoint outputPoint, float power)
    {
        if (outputPoint != null && outputPoint.isConnected)
        {
            if (outputPoint.connectedCables.Count == 0) return;
            GameObject cable = outputPoint.connectedCables[0];
            if (cable == null) return;

            CableInteractable cableInfo = cable.GetComponentInChildren<CableInteractable>();
            if (cableInfo != null)
            {
                ConnectionPoint destinationPoint = cableInfo.GetOtherPoint(outputPoint);
                if (destinationPoint == null || destinationPoint.parentItem == null) return;

                if (destinationPoint.parentItem is Target target)
                {
                    target.UpdatePower(power);
                }
                else if (destinationPoint.parentItem is CombinerBox combiner)
                {
                    combiner.ForceUpdateDistribution();
                }
                else if (destinationPoint.parentItem is DividerBox divider)
                {
                    divider.ForceUpdateDistribution();
                }
            }
        }
    }
}
