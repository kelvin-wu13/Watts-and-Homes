using UnityEngine;
using System.Collections.Generic;

public class TargetStateObjective : Objective
{
    public List<Target> targetsToCheck = new List<Target>();

    public override void CheckObjective()
    {
        if (targetsToCheck == null || targetsToCheck.Count == 0)
        {
            IsComplete =true;
            return;
        }
        foreach (Target target in targetsToCheck)
        {
            if (target == null || !target.IsPowered)
            {
                IsComplete = false;
                return;
            }
        }

        IsComplete = true;
    }
}
