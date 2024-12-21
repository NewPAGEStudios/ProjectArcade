using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class saveTxtData
{
    public static void CreateData(string datas)
    {
        string path = Application.persistentDataPath + "/Statistics.txt";


        File.WriteAllText(path, string.Empty);

        File.WriteAllText(path, datas);
    }


}
