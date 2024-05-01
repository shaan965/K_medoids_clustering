using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChurnPredictor
{
    //TODO; FIX THIS FUNCTION FOR EXTRA CREDIT, TEST WITH "Check Accuracy" BUTTON
    public Datapoint[] AssignPredictedChurn(Datapoint[] verificationDatapoints, Dictionary<Datapoint, List<Datapoint>> clustersByCenters)
    {
        foreach(Datapoint d in verificationDatapoints)
        {
            if (d.level < 100 || d.maxScore <= 2950 || d.ghostsEaten >= 13000 || d.pelletsEaten >= 1000000 || d.totalScore > 385000 || d.fruitEaten > 5000)
            {
                d.SetPredictedChurn(true);
            } else if (d.level > 1300 ||d.maxScore > 2950 || d.ghostsEaten < 13000 || d.pelletsEaten < 1000000 || d.totalScore < 385000 ||d.totalScore < 385000)
            {
                d.SetPredictedChurn(false);
            }
        }
        return verificationDatapoints;
    }
}