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

    private float distributionRatio1 = 0.5f;


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

        UpdateUIFields(0.5f);
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
        if (totalPowerInput <= 0)
        {
            UpdateUIFields(0.5f);
            return;
        }

        float.TryParse(newValue, out float activeValue);
        activeValue = Mathf.Clamp(activeValue, 0, totalPowerInput);

        if (activeInput == valueInput1)
        {
            distributionRatio1 = activeValue / totalPowerInput;
        }
        else
        {
            distributionRatio1 = (totalPowerInput - activeValue) / totalPowerInput;
        }

        UpdateUIFields(distributionRatio1);
    }

    public void ForceUpdateDistribution()
    {
        UpdateTotalInputPower();

        float powerForOutput1 = totalPowerInput * distributionRatio1;
        float powerForOutput2 = totalPowerInput * (1.0f - distributionRatio1);

        isUpdatingUI = true;
        if (valueInput1 != null) valueInput1.text = powerForOutput1.ToString("F1");
        if (valueInput2 != null) valueInput2.text = powerForOutput2.ToString("F1");
        isUpdatingUI = false;


        DistributePowerToOutput(outputPoint1, powerForOutput1);
        DistributePowerToOutput(outputPoint2, powerForOutput2);
    }
    private void UpdateUIFields(float ratio1)
    {
        distributionRatio1 = Mathf.Clamp01(ratio1);
        float otherRatio = 1.0f - distributionRatio1;

        float value1 = totalPowerInput * distributionRatio1;
        float value2 = totalPowerInput * otherRatio;

        isUpdatingUI = true;
        if (valueInput1 != null) valueInput1.text = value1.ToString("F1");
        if (valueInput2 != null) valueInput2.text = value2.ToString("F1");
        isUpdatingUI = false;
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
