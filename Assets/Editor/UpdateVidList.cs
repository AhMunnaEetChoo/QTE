using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(QTEManager))]
public class LevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        QTEManager myTarget = (QTEManager)target;

        void UpdateVids()
        {
            myTarget.m_gameData.clipData.Clear();
            DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/../docs/Clips");
            FileInfo[] info = dir.GetFiles("*.mp4");
            foreach (FileInfo f in info)
            {
                QTEManager.ClipData cd = new QTEManager.ClipData();
                cd.url = "https://ahmunnaeetchoo.github.io/QTE/Clips/" + f.Name;

                QTEManager.QTE defaultQTE = new QTEManager.QTE();
                cd.qtes.Add(defaultQTE);
                myTarget.m_gameData.clipData.Add(cd);
            }
        }

        if (GUILayout.Button("Update Vids"))
        {
            UpdateVids();
        }
        if(GUILayout.Button("Export to JSON"))
        {
            string jsonString = JsonUtility.ToJson(myTarget.m_gameData, true);
            System.IO.File.WriteAllText(Application.dataPath + "/GameData.json", jsonString);
            Debug.Log(jsonString);
        }
        if(GUILayout.Button("Import from JSON"))
        {
            string jsonString = System.IO.File.ReadAllText(Application.dataPath + "/GameData.json");
            myTarget.m_gameData = JsonUtility.FromJson<QTEManager.GameData>(jsonString);
        }
    }
}
