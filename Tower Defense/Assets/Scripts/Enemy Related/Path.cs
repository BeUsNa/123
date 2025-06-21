using System.Collections.Generic;
using UnityEngine;

namespace Enemy_Related
{
    public class Path : MonoBehaviour
    {
        public List<Transform> GetPath()
        {
            List<Transform> waypoints = new List<Transform>();
            foreach (Transform point in transform)
            {
                waypoints.Add(point);
            }
            return waypoints;
        }
    }
}
