using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KClusterer
{
    const int K = 6;//TODO; set K to the optimal value that you've found via experimentation
    const int MAX_ATTEMPTS = 10000;//Maximum number of clustering attempts, you may want to use this
    const float threshold = 0.02f;//Threshold for cluster similarity, you may want to use this and alter it if so

    //TODO; fix this function
    public Dictionary<Datapoint, List<Datapoint>> Cluster(Datapoint[] datapoints)
    {

        Dictionary<Datapoint, List<Datapoint>> clustersByCenters = new Dictionary<Datapoint, List<Datapoint>>();

        //Select K random centers to start
        List<Datapoint> centers = new List<Datapoint>();

        while (centers.Count < K)
        {
            // Generate a random index less than the size of the array.
            int randomIndex = Random.Range(0, datapoints.Length);
            Datapoint randomCenter = datapoints[randomIndex];

            if (!centers.Contains(randomCenter))
            {
                centers.Add(randomCenter);
            }
        }

        //Instantiate clusters by these centers
        foreach (Datapoint center in centers)
        {
            clustersByCenters.Add(center, new List<Datapoint>());
        }

        //Map each datapoint to the closest center
        foreach (Datapoint pnt in datapoints)
        {
            Datapoint closestCenter = null;
            float minDistance = float.PositiveInfinity;

            foreach (Datapoint center in centers)
            {
                float thisDistance = Distance(pnt, center);
                if (thisDistance < minDistance)
                {
                    closestCenter = center;
                    minDistance = thisDistance;
                }
            }

            clustersByCenters[closestCenter].Add(pnt);
        }

        //K means/ k medoids

        // oldcentroids = []
        List<Datapoint> oldCentroids = new List<Datapoint>();

        int attempts = 0;
        // while not centroids==oldcentroids:
        while ((DifferenceBetweenCenters(oldCentroids.ToArray(), centers.ToArray()) > threshold) && (attempts < MAX_ATTEMPTS)){


            // oldcentroids = centroids
            oldCentroids = new List<Datapoint>();
            foreach (Datapoint d in centers)
            {
                oldCentroids.Add(d);
            }
            
            //Instantiate clusters by these centers
             List<Datapoint> newCenters = new List<Datapoint>();
             foreach (Datapoint center in centers)
            {
                newCenters.Add(GetMedian(clustersByCenters[center].ToArray()));
            }

            //Map each datapoint to its closest center
            foreach (Datapoint pnt in datapoints)
            {
                Datapoint closestCenter = null;
                float minDistance = float.PositiveInfinity;

                foreach (Datapoint center in centers)
                {
                    float thisDistance = Distance(pnt, center);
                    if (thisDistance < minDistance)
                    {
                        closestCenter = center;
                        minDistance = thisDistance;
                    }
                }

                clustersByCenters[closestCenter].Add(pnt);
            }
            return clustersByCenters;
        }
        return clustersByCenters;
    }

    //Calculate the difference between sets of centers
    private float DifferenceBetweenCenters(Datapoint[] centers1, Datapoint[] centers2)
    {
        List<Datapoint> mappedPoints = new List<Datapoint>();
        float totalDistance = 0;
        foreach(Datapoint c1 in centers1)
        {
            Datapoint minPoint = null;
            float minDistance = float.PositiveInfinity;

            foreach(Datapoint c2 in centers2)
            {
                if (!mappedPoints.Contains(c2))
                {
                    float thisDistance = Distance(c1, c2);

                    if (thisDistance < minDistance)
                    {
                        minDistance = thisDistance;
                        minPoint = c2;
                    }
                }
            }
            mappedPoints.Add(minPoint);
            totalDistance += minDistance;
        }
        
        return totalDistance;
    }

    //Calculate and returns the geometric median of the given datapoints
    public Datapoint GetMedian(Datapoint[] datapoints)
    {
        Datapoint medianPnt = null;
        float totalDistance = float.PositiveInfinity;

        for(int i = 0; i<datapoints.Length; i++)
        {
            float thisDistance = 0;
            for(int j=0; j<datapoints.Length; j++)
            {
                if (i != j)
                {
                    thisDistance += Distance(datapoints[i], datapoints[j]);
                }
            }

            if (thisDistance < totalDistance)
            {
                totalDistance = thisDistance;
                medianPnt = datapoints[i];
            }
        }

        return medianPnt;
    }

    //Calculate and returns the average of the given datapoints
    public Datapoint GetAverage(Datapoint[] datapoints)
    {
        Datapoint sumDatapoint = new Datapoint("", 0, 0, 0, 0, 0, 0, 0, 0);
        int churnedVal = 0;

        foreach(Datapoint d in datapoints)
        {
            sumDatapoint = new Datapoint("", sumDatapoint.HoursPlayed + d.HoursPlayed, sumDatapoint.Level + d.Level, sumDatapoint.PelletsEaten + d.PelletsEaten, sumDatapoint.FruitEaten + d.FruitEaten, sumDatapoint.GhostsEaten + d.GhostsEaten, sumDatapoint.AvgScore + d.AvgScore, sumDatapoint.MaxScore + d.MaxScore, sumDatapoint.TotalScore + d.TotalScore);

            if (d.Churned)
            {
                churnedVal += 1;
            }
            else
            {
                churnedVal -= 1;
            }
        }
        //Calculate averages
        int hoursPlayed = (int)(((float)sumDatapoint.HoursPlayed) / ((float)datapoints.Length));
        int level = (int)(((float)sumDatapoint.Level) / ((float)datapoints.Length));
        int pelletsEaten = (int)(((float)sumDatapoint.PelletsEaten) / ((float)datapoints.Length));
        int fruitEaten = (int)(((float)sumDatapoint.FruitEaten) / ((float)datapoints.Length));
        int ghostsEaten = (int)(((float)sumDatapoint.GhostsEaten) / ((float)datapoints.Length));
        float avgScore = (((float)sumDatapoint.AvgScore) / ((float)datapoints.Length));
        int maxScore = (int)(((float)sumDatapoint.MaxScore) / ((float)datapoints.Length));
        int totalScore = (int)(((float)sumDatapoint.TotalScore) / ((float)datapoints.Length));

        bool churned = false;
        if (churnedVal > 0)
        {
            churned = true;
        }

        return new Datapoint("", hoursPlayed, level, pelletsEaten, fruitEaten, ghostsEaten, avgScore, maxScore, totalScore, churned);
    }

    //TODO; alter this distance function
    //WARNING: DO NOT USE CHURNED AS PART OF THIS FUNCTION
    
    public static float Distance(Datapoint a, Datapoint b)
    {
    
    // Hardcoded maximum values for each feature Muhammad Data
    float maxHoursPlayed = 4823f;
    float maxLevel = 2384f;
    float maxPelletsEaten = 1190540f;
    float maxFruitEaten = 7823f;
    float maxGhostsEaten = 74848f;
    float maxAvgScore = 2122f;
    float maxMaxScore = 11203f;
    float maxTotalScore = 1033295f;

    float minHoursPlayed = 1564f;
    float minLevel = 1f;
    float minPelletsEaten = 21210f;
    float minFruitEaten = 737f;
    float minGhostsEaten = 938f;
    float minAvgScore = 413f;
    float minMaxScore = 1003f;
    float minTotalScore = 95227f;


    //Example data
    // float maxHoursPlayed = 4992f;
    // float maxLevel = 352f;
    // float maxPelletsEaten = 1231048f;
    // float maxFruitEaten = 2895f;
    // float maxGhostsEaten = 88858f;
    // float maxAvgScore = 2440f;
    // float maxMaxScore = 6911f;
    // float maxTotalScore = 979347f;

    // float minHoursPlayed = 1564f;
    // float minLevel = 1f;
    // float minPelletsEaten = 17120f;
    // float minFruitEaten = 276f;
    // float minGhostsEaten = 1382f;
    // float minAvgScore = 171f;
    // float minMaxScore = 1123f;
    // float minTotalScore = 140368f;
    
    // Normalize features
    float normHoursPlayed = Mathf.Abs(a.HoursPlayed - b.HoursPlayed) / (float)Mathf.Abs(maxHoursPlayed - minHoursPlayed);
    float normLevel = Mathf.Abs(a.Level - b.Level) / (float)Mathf.Abs(maxLevel - minLevel);
    float normPelletsEaten = Mathf.Abs(a.PelletsEaten - b.PelletsEaten) / (float)Mathf.Abs(maxPelletsEaten - minPelletsEaten);
    float normFruitEaten = Mathf.Abs(a.FruitEaten - b.FruitEaten) / (float)Mathf.Abs(maxFruitEaten - minFruitEaten);
    float normGhostsEaten = Mathf.Abs(a.GhostsEaten - b.GhostsEaten) / (float)Mathf.Abs(maxGhostsEaten - minGhostsEaten);
    float normAvgScore = Mathf.Abs(a.AvgScore - b.AvgScore) / (float)Mathf.Abs(maxAvgScore - minAvgScore);
    float normMaxScore = Mathf.Abs(a.MaxScore - b.MaxScore) / (float)Mathf.Abs(maxMaxScore - minMaxScore);
    float normTotalScore = Mathf.Abs(a.TotalScore - b.TotalScore) / (float)Mathf.Abs(maxTotalScore - minTotalScore);

    // Calculate Euclidean distance
    float dist = Mathf.Sqrt(normHoursPlayed * normHoursPlayed + normLevel * normLevel +
                             normPelletsEaten * normPelletsEaten + normFruitEaten * normFruitEaten +
                             normGhostsEaten * normGhostsEaten + normAvgScore * normAvgScore +
                             normMaxScore * normMaxScore + normTotalScore * normTotalScore);
    return dist;

 
    }

}

