using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

//[CustomEditor(typeof(QTEManager))]
//public class LevelScriptEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();
//        QTEManager myTarget = (QTEManager)target;

//        void UpdateVids()
//        {
//            myTarget.m_clipData.Clear();
//            DirectoryInfo dir = new DirectoryInfo("/Users/oliverpowell/Documents/GitHub/QTE/docs/Clips");
//            FileInfo[] info = dir.GetFiles("*.mp4");
//            foreach (FileInfo f in info)
//            {
//                QTEManager.ClipData cd = new QTEManager.ClipData();
//                cd.m_url = "https://ahmunnaeetchoo.github.io/QTE/Clips/" + f.Name;

//                myTarget.m_clipData.Add(cd);
//            }
//        }

//        if (GUILayout.Button("Update Vids"))
//        {
//            UpdateVids();
//        }
//    }
//}
