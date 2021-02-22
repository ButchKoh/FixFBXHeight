using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using Unity.EditorCoroutines.Editor;

public class FixFBXHeight : EditorWindow
{

    [MenuItem("Tools/Fix FBX Height")]
    static void Open()
    {
        GetWindow<FixFBXHeight>();
    }
    private GameObject source;
    private bool l2w = false, quit = false;
    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mesh");
        source = (GameObject)EditorGUILayout.ObjectField(source, typeof(GameObject), true);

        if (GUILayout.Button("start"))
        {
            quit = false;
            this.StartCoroutine(Fix(source));
        }
        if (GUILayout.Button("quit"))
        {
            quit = true;
            EditorUtility.ClearProgressBar();
        }
        l2w = GUILayout.Toggle(l2w, "Local to Absolute");
        EditorGUILayout.Space(30);
        EditorGUILayout.HelpBox("This tool is an extension of FBX Exporter. "
            +"The tool corrects the position of the specified mesh in order to the vertex with the minimum Y coordinate in the mesh will meet Y = 0. "
            +"If [Local to Absolute] is clicked, all the vertices will be converted from local transtorm to world coordinate in advance. "
            +"Both original mesh and converted mesh will be output in Assets folder. "
            + "If you want to abort while the process, press [quit].", MessageType.None);
    }

    private IEnumerator
        Fix(GameObject go)
    {
        //ファイルを読み込み
        //PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
        GameObject newGo = go;
        System.DateTime dt = System.DateTime.Now;
        Mesh mf = go.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] test = mf.vertices;

        //頂点に加える変更を取得
        Transform tr = go.GetComponent<Transform>();
        Vector3 rot = tr.rotation.eulerAngles;
        Vector3 scl = tr.localScale;
        Vector3 pos = tr.position;

        //変形前に元のファイルを避難
        go.transform.position = new Vector3(0, 0, 0);
        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        string originalPath = Path.Combine(Application.dataPath.Replace("/", "\\"), dt.ToString().Replace(" ", "_").Replace(":", "_").Replace("/", "_") + "_originalFBXModel.fbx");
        ModelExporter.ExportObject(originalPath, go);

        if (l2w)
        {
            //transformを各頂点に適応
            for (int i = 0; i < test.Length; i++)
            {
                test[i].x = test[i].x * scl.x + pos.x;
                test[i].y = test[i].y * scl.y + pos.y;
                test[i].z = test[i].z * scl.z + pos.z;
                test[i] = Quaternion.Euler(rot) * test[i];
                EditorUtility.DisplayProgressBar("Fixing Transform Values", "processing:" + (((float)i * 100) / test.Length) + "[%]", ((float)i / test.Length));
                if (quit)
                {
                    EditorUtility.ClearProgressBar();
                    yield break;
                }
                yield return i;
            }
        }
        //高さ揃える処理

        float height = float.MaxValue;
        for (int i = 0; i < test.Length; i++)
        {
            if (test[i].y < height) height = test[i].y;
        }

        for (int i = 0; i < test.Length; i++)
        {
            test[i].y -= height;
            EditorUtility.DisplayProgressBar("Fixing Height", "processing:" + (((float)i * 100) / test.Length) + "[%]", ((float)i / test.Length));
            if (quit)
            {
                EditorUtility.ClearProgressBar();
                yield break;
            }
            yield return i;
        }

        mf.vertices = test;
        mf.RecalculateBounds();
        go.transform.rotation = new Quaternion(0, 0, 0, 0);
        go.transform.position = new Vector3(0, 0, 0);
        go.transform.localScale = new Vector3(1, 1, 1);
        string filePath = Path.Combine(Application.dataPath.Replace("/", "\\"), dt.ToString().Replace(" ", "-").Replace(":", "-").Replace("/", "-") + "_FBXModel.fbx");
        try
        {
            //変形後のメッシュを書き出し

            ModelExporter.ExportObject(filePath, go);
            //Debug.Log("New .fbx model has output : " + filePath);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            //消し忘れると出続けるので
            EditorUtility.ClearProgressBar();
        }
    }
}
