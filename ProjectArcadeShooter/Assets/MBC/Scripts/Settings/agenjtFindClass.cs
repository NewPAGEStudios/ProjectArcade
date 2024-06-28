using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class agenjtFindClass
{
    public static int GetAgentTypeIDbyName(string name)
    {
        int count = NavMesh.GetSettingsCount();
        string[] agentNames = new string[count + 2];
        for(int i = 0; i < count; i++)
        {
            int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            string nameD = NavMesh.GetSettingsNameFromID(id);
            if(nameD == name)
            {
                return id;
            }
        }
        return -1;
    }
}