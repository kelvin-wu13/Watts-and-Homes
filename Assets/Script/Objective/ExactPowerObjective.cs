using UnityEngine;
using System.Collections.Generic;

public class ExactPowerObjective : Objective
{
    public List<Target> targetsToCheck = new List<Target>();

    public override void CheckObjective()
    {
        if (targetsToCheck == null || targetsToCheck.Count == 0)
        {
            IsComplete = true;
            return;
        }

        foreach (Target target in targetsToCheck)
        {
            if (target == null)
            {
                IsComplete = false;
                return;
            }

            float powerNeeded = target.GetPowerRequirement();
            float powerGot = target.PowerReceived;

            if (!Mathf.Approximately(powerGot, powerNeeded))
            {
                IsComplete = false;
                return;
            }
        }
        IsComplete = true;
    }
}
