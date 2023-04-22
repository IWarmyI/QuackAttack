/* Camera Regions by Stephan Zuidam
www.szuidam.weebly.com
*/

using System.Collections.Generic;
using UnityEngine;

/*  RegionHandler holds all the regions and is the class between the camera and the different regions. This class allows for adding new regions,
removing regions and retrieving the current active region. This class also makes sure the regions are validated. A validated region has 
point 0 to the left and above point 1. 

A region considered valid is as follows:
region = new Region(new Vector2(0,0), new Vector2(10, -10))

RegionHandler has a custom inspector menu (RegionHandlerEditor.cs) which makes creating, editing and customizing regions more accessible. 
Regions can be customized with an outline and fill color. New regions can be added by pressing the Add Region button. To validate all regions 
press the Validate Regions button. Invalid regions are colored red in the sceneview during editing. When validating regions RegionHandler changes around
the points of the regions so that they are according to the above mentioned valid standard. This is required for calculating the boundaries the camera can move to.
*/

public class RegionHandler : MonoBehaviour {

    public Color regionOutlineColor = new Color(1, 1, 1, 1);
    public Color regionFillColor = new Color(1, 1, 1, 0.2f);

    [SerializeField]
    private int currentArea = 0;
    [SerializeField]
    private List<Region> regionList = new List<Region>();
    public List<Region> Regions { get { return regionList; } }

    public void AddRegion(Vector2 _position) {
        _position = new Vector2(Mathf.Round(_position.x), Mathf.Round(_position.y));
        regionList.Add(new Region(_position + new Vector2(-1, 1), _position + new Vector2(1, -1)));
    }

    public void RemoveRegion(int _index) {
        regionList.RemoveAt(_index);
    }

    public void Validate() {
        bool allRegionsAreValid = true;

        for (int i = 0; i < regionList.Count; i++) {
            Region r = regionList[i];
            Vector2 m_p0 = r.p0;
            Vector2 m_p1 = r.p1;

            r.p0 = new Vector2(Mathf.Min(m_p0.x, m_p1.x), Mathf.Max(m_p0.y, m_p1.y));
            r.p1 = new Vector2(Mathf.Max(m_p0.x, m_p1.x), Mathf.Min(m_p0.y, m_p1.y));

            allRegionsAreValid = allRegionsAreValid == true ? IsRegionValidated(i) : false;
        }

        if (allRegionsAreValid) {
            Debug.Log("All regions have been validated!");
        }
        else {
            Debug.Log("Something happened and not all regions could be validated");
        }
    }

    public bool IsRegionValidated(int _index) {
        Vector2 m_p0 = regionList[_index].p0;
        Vector2 m_p1 = regionList[_index].p1;

        return (m_p0.x < m_p1.x && m_p0.y > m_p1.y);
    }

    public void SetActiveRegion(Vector3 _positionOfTarget) {
        foreach (Region r in regionList) {
            if (r.Contains(_positionOfTarget)) {
                currentArea = regionList.IndexOf(r);
                break;
            }
        }
    }

    public Region ActiveRegion
    {
        get { return Regions[currentArea]; }
    }
}
