using UnityEngine;
using System.Collections.Generic;

public class ItemCountObjective : Objective
{
    public List<ItemType> typesToCount;

    public int maxAllowedCount;

    public override void CheckObjective()
    {
        int currentItemCount = 0;

        if (typesToCount.Contains(ItemType.Generator))
        {
            currentItemCount += FindObjectsByType<PowerCell>(FindObjectsSortMode.None).Length;
        }
        if (typesToCount.Contains(ItemType.Conductor))
        {
            currentItemCount += FindObjectsByType<CombinerBox>(FindObjectsSortMode.None).Length;
            currentItemCount += FindObjectsByType<DividerBox>(FindObjectsSortMode.None).Length;
        }

        IsComplete = currentItemCount <= maxAllowedCount;
    }
}