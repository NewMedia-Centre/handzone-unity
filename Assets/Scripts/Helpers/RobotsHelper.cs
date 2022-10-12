using System.Text.RegularExpressions;
using Rhino.Geometry;
using Robots;
using UnityEngine;
using Plane = Rhino.Geometry.Plane;

public class RobotsHelper
{
    public static Target CreateTarget(string data)
    {
        // Input example: 
        // Target (Cartesian (301.86,-50.89,596.67), 
        // Joint, "Shoulder, Elbow, Wrist", 
        // Frame (0,0,0), 
        // Tool (DefaultTool), 
        // Speed (DefaultSpeed), 
        // Zone (DefaultZone), 
        // Contains commands)
        string pattern = @"(\w*) \((\w*) \((-?\d*.\d*),(-?\d*.\d*),(-?\d*.\d*)\), ";
        Regex regex = new Regex(pattern);
        Debug.Log(regex.Matches(data));
        
        var planeA = Plane.WorldYZ;
        return new CartesianTarget(planeA, RobotConfigurations.Wrist, Motions.Joint);
    }

    public static string GetFirstWord(string text)
    {
        int index = text.IndexOf(' ');

        if (index > -1) { // Check if there is more than one word.
            return text.Substring(0, index).Trim(); // Extract first word.
        }
        return text; // Text is the first word itself.
    }

    public static string[] GetParameters(string text)
    {
        string[] parameters = text.Split(", ");

        return parameters;
    }
}
