using System.Collections.Generic;
using System.IO;
using GameBase.Audio;
using UnityEditor;
using UnityEngine;
using AudioType = GameBase.Audio.AudioType;

namespace GameBase.Editor
{
    [CustomEditor(typeof(AudioCue))]
    [CanEditMultipleObjects]
    public class AudioCueEditor : UnityEditor.Editor
    {
        private AudioCue audioCue;
        private List<Clip> items;
        private SerializedObject targetObject = null;
        private SerializedProperty listProperty = null;
        private string fileSearchPattern = "";
        private bool foldoutAdvanced = false;

        void OnEnable()
        {
            audioCue = (AudioCue)target;
            items = audioCue.Clips;
            targetObject = new SerializedObject(target);
            listProperty = targetObject.FindProperty("Clips");

            if (audioCue != null && audioCue.name.Length > 0)
            {
                fileSearchPattern = (audioCue.name + "*.wav").ToLower();
            }
        }

        override public void OnInspectorGUI()
        {
            audioCue = (AudioCue)target;
            items = audioCue.Clips;
            targetObject = new SerializedObject(target);
            listProperty = targetObject.FindProperty("Clips");

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Play") && audioCue)
            {
                /*if (Application.isEditor)
                {
                    if (AudioSystem.IsValid())
                    {
                        GameObject.DestroyImmediate(AudioSystem.Instance().gameObject);
                    }
                }*/

                foreach (Object targ in targets)
                    AudioSystem.Instance().Play(targ as AudioCue);
            }

            if (GUILayout.Button("Stop"))
            {
                if (AudioSystem.IsValid())
                {
                    //GameObject.DestroyImmediate(AudioSystem.Instance().gameObject);
                    var sources = AudioSystem.Instance().GetComponentsInChildren<AudioSource>();
                    foreach (var source in sources)
                    {
                        DestroyImmediate(source.gameObject);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add cue to Audio System"))
            {
                string[] strResults = AssetDatabase.FindAssets("AudioSystem");
                AudioSystem audioSystem = null;
                if (strResults.Length > 0)
                {
                    audioSystem =
                        AssetDatabase.LoadAssetAtPath<AudioSystem>(AssetDatabase.GUIDToAssetPath(strResults[0]));
                }

                if (audioSystem == null)
                {
                    Debug.LogWarning("Couldn't find AudioSystem");
                }
                else
                {
                    foreach (Object targ in targets)
                    {
                        AudioCue cue = targ as AudioCue;
                        if (cue == null)
                        {
                            Debug.LogWarning("Cue is null");
                        }
                        else if (cue.gameObject.scene.IsValid())
                        {
                            Debug.LogWarning("Can't add cues from prefab editor");
                            EditorUtility.DisplayDialog("Couldn't add cue",
                                "You can't add cues from the prefab editor.\n\nIn 2019+ you can just do it from regular inspector. In 2018.3+ drag cues manually into the list in SystemAudio prefab. (Blame unity for breaking things)",
                                "Ok");
                        }
                        else
                        {
                            Undo.RecordObject(audioSystem, "Adding cue to audio system");
                            if (audioSystem.EditorAddCue(cue))
                            {
                                Debug.Log("Added cue");
                                EditorUtility.SetDirty(audioSystem);
                            }
                            else
                            {
                                Debug.Log("Cue already existed");
                            }
                        }
                    }
                }
            }

            SerializedProperty prop = targetObject.GetIterator();
            if (prop.Next(true))
            {
                // Skip built in properites					
                for (int i = 0; i < 9 && prop.Next(false); ++i)
                {
                }

                while (prop.Next(false))
                {
                    if (prop.editable && prop.name != "Clips")
                    {
                        EditorGUILayout.PropertyField(prop);
                    }
                }
            }

            if (targets.Length <= 1)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Sound Clips:", EditorStyles.boldLabel);
                EditorUtils.UpdateListInspector<Clip>(ref items, null,
                    new EditorUtils.CreateListItemGUIDelegate(BuildSpawnItemInspector), null);
                DropAreaGUI();
            }

            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();

            if (targets.Length <= 1)
            {
                foldoutAdvanced = EditorGUILayout.Foldout(foldoutAdvanced, "Advanced");
                if (foldoutAdvanced)
                {
                    EditorGUILayout.Space();
                    AutoImportCuesGUI();
                }
            }

            if (GUI.changed)
            {
                targetObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);

                targetObject = new SerializedObject(target);
                listProperty = targetObject.FindProperty("Clips");
            }
        }


        private void DropAreaGUI()
        {
            var evt = Event.current;
            var dropArea = GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drop sounds here to add");

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                {
                    if (dropArea.Contains(evt.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            foreach (var draggedObject in DragAndDrop.objectReferences)
                            {
                                AudioClip clip = draggedObject as AudioClip;
                                if (!clip)
                                    continue;
                                items.Add(new Clip() { AudioClip = clip });
                                GUI.changed = true;
                            }
                        }

                        Event.current.Use();
                        EditorUtility.SetDirty(target);
                    }
                }
                    break;
            }
        }


        void AutoImportCuesGUI()
        {
            fileSearchPattern = EditorGUILayout.TextField("File Search Pattern", fileSearchPattern);

            if (GUILayout.Button("Load Matching Files"))
            {
                // Clear all existing items first
                items.Clear();

                string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(audioCue));

                DirectoryInfo directory = new DirectoryInfo(path);

                FileInfo[] info = directory.GetFiles(fileSearchPattern);

                foreach (FileInfo f in info)
                {
                    string assetPath = MakeRelative(f.FullName, Application.dataPath);

                    //Debug.Log ();

                    AudioClip clip = AssetDatabase.LoadAssetAtPath(assetPath, typeof(AudioClip)) as AudioClip;

                    if (!clip)
                    {
                        Debug.Log("asset didn't load");

                        continue;
                    }
                    else
                        Debug.Log(clip.name);

                    // search for duplicate name
                    bool duplicateItem = false;

                    foreach (Clip item in items)
                    {
                        if (item.AudioClip.name == clip.name)
                        {
                            duplicateItem = true;
                        }
                    }

                    if (!duplicateItem)
                    {
                        items.Add(new Clip() { AudioClip = clip });
                    }
                }
            }
        }

        // Delegate
        void BuildSpawnItemInspector(int i)
        {
            EditorGUILayout.BeginVertical();
            items[i].AudioClip =
                EditorGUILayout.ObjectField("", (Object)items[i].AudioClip, typeof(AudioClip),
                    false) as AudioClip; // false is "allowSceneObjects"

            if (i >= listProperty.arraySize)
            {
                targetObject = new SerializedObject(target);
                listProperty = targetObject.FindProperty("Clips");
            }

            if (i < listProperty.arraySize)
            {
                SerializedProperty listItem = listProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(listItem, new GUIContent("Data"), true);
            }

            EditorGUILayout.EndVertical();
        }

        public static string MakeRelative(string filePath, string referencePath)
        {
            try
            {
                var fileUri = new System.Uri(filePath);
                var referenceUri = new System.Uri(referencePath);
                if (referenceUri.IsAbsoluteUri)
                    return referenceUri.MakeRelativeUri(fileUri).ToString();
            }
            catch (System.Exception e)
            {
                if (e != null)
                {
                }
            }

            return filePath;
        }

        [MenuItem("Assets/Create Audio Cue from Clips #%&c", true)]
        static bool ContextCreateAudioCueValidate(MenuCommand command)
        {
            if (Selection.objects.Length < 0)
                return false;
            return System.Array.Find(Selection.objects, item => item is AudioClip);

            //return (command.context as AudioClip) != null;
        }

        [MenuItem("Assets/Create Audio Cue from Clips #%&c", false, 32)]
        static void ContextCreateAudioCue(MenuCommand command)
        {
            // Create object with audio cue component

            // Add all selected cues to object
            Object[] clipObjs = System.Array.FindAll(Selection.objects, item => item is AudioClip);
            AudioClip[] clips = System.Array.ConvertAll(clipObjs, item => item as AudioClip);
            if (clips.Length <= 0)
                return;


            // Sort sprites by name and insert
            using (PowerTools.Anim.NaturalComparer comparer = new PowerTools.Anim.NaturalComparer())
            {
                System.Array.Sort(clips, (a, b) => comparer.Compare(a.name, b.name));
            }

            string path = AssetDatabase.GetAssetPath(clips[0]);
            path = Path.GetDirectoryName(path) + "/Sound" + Path.GetFileNameWithoutExtension(path) + ".prefab";

            // Create the audio cue
            GameObject go = new GameObject();
            AudioCue cue = go.AddComponent<AudioCue>();

            // Add the clips
            foreach (AudioClip clip in clips)
            {
                cue.Clips.Add(new Clip() { AudioClip = clip, Weight = 100 });
            }

            // Set mask as "sound"
            cue.AudioType = AudioType.Sound;

            // Create the prefab
            Object p = PrefabUtility.SaveAsPrefabAsset(go, path);

            GameObject.DestroyImmediate(go);

            // Select the cue
            Selection.activeObject = p;
            //EditorGUIUtility.PingObject(p);
        }
    }
}