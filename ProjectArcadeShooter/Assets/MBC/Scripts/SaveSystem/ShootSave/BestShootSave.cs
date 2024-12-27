using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class BestShootSave
{
    public static void SaveShoot(ReflectBulletFunctions[] rbfbullet, Enemy[] enemies, NormalBulletFunction[] normalBullet, int waveData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        int i = 0;
        string path = "";
        while (i < 3)
        {
            path = Application.persistentDataPath + "/BestShootsData/shoot" + i + ".bss";
            if (!File.Exists(path))
            {
                break;
            }
            i++;
        }


        FileStream stream = new FileStream(path, FileMode.Create);
//        formatter.Serialize(stream, data);
        stream.Close();
    }


}
