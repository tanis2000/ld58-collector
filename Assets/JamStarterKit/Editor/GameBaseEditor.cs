using System.Collections.Generic;
using GameBase.Audio;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameBase.Editor
{
    public partial class GameBaseEditor : EditorWindow
    {
        public static GameBaseEditor Instance()
        {
            return instance;
        }

        public static bool IsOpen()
        {
            return GameBaseEditor.instance != null;
        }

        private enum GuiTabs { Main,Tools,Count }

        private const string PathAudio = "Assets/Audio";
        private const string PathAudioSystem = "Assets/Systems";
        private static GameBaseEditor instance;
        private AudioSystem audioSystem;
        private string audioSystemPath = PathAudioSystem;
        private GuiTabs selectedTab = GuiTabs.Main;

        [MenuItem("Window/GameBase")]
        private static void ShowWindow()
        {
            var window = GetWindow<GameBaseEditor>();
            window.titleContent = new GUIContent("Game Base");
            window.Show();
        }

        public static GameBaseEditor OpenGameBaseEditor()
        {
            if (IsOpen())
                return Instance();

            GameBaseEditor editor = GetWindow<GameBaseEditor>();
            return editor;
        }

        public void RequestAssetRefresh()
        {
            AssetDatabase.Refresh();
        }

        private void OnEnable()
        {
            instance = this;
            CreateMainGuiLists();
        }

        private void Update()
        {
            UpdateCheckInitialSetup();
        }

        private void OnGUI()
        {
            string[] tabstrings = new string[] { "Main", "Tools" };
            GuiTabs prevTab = selectedTab;
            selectedTab = Tabs( tabstrings, selectedTab );
            bool tabChanged = prevTab != selectedTab;

            
            if ( selectedTab == GuiTabs.Main )
            {
                OnGUIMain();
            }
        }

        public void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            PostProcessAudioCueLists();
        }

        private void PostProcessAudioCueLists()
        {
            if (audioSystem.AutoAddCues)
            {
                if (RefreshObjectList(audioSystem.AudioCues, PathAudio))
                    EditorUtility.SetDirty(audioSystem);
            }
        }

        // Updates list with objects of specified type in the path, returns true if it changed.
        bool RefreshObjectList<T>(List<T> list, string path) where T : MonoBehaviour
        {
            string[] assets = AssetDatabase.FindAssets("t:prefab", new[] { path });

            // Create list of all items, with check to see they're the same type (check's prefab)
            List<T> newList = new List<T>();
            for (int i = 0; i < assets.Length; ++i)
            {
                T prefab = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assets[i]));
                if (prefab != null)
                    newList.Add(prefab);
            }

            bool changed = list.Count != newList.Count;
            if (changed == false)
            {
                // same number of assets- check if any have changed. Ignore order changes.
                for (int i = 0; i < list.Count; ++i)
                {
                    if (newList.Contains(list[i]) == false)
                    {
                        changed = true;
                        break;
                    }
                }
            }

            if (changed)
            {
                //  Something's changed, use the new list
                list.Clear();
                list.AddRange(newList);
            }

            return changed;
        }

        private void UpdateCheckInitialSetup()
        {
            if ( audioSystem == null)
            {
                string systemPath = audioSystemPath+"/AudioSystem.prefab";
                GameObject obj = AssetDatabase.LoadAssetAtPath(systemPath, typeof(GameObject)) as GameObject;
                if ( obj != null )
                {
                    audioSystem = obj.GetComponent<AudioSystem>();
                }
            }
        }
        
        //
        // Creates tab style layout
        //
        GuiTabs Tabs(string[] options, GuiTabs selected)
        {
            const float darkGray = 0.6f;
            const float lightGray = 0.9f;
            const float startSpace = 5;
	 
            GUILayout.Space(startSpace);
            Color storeColor = GUI.backgroundColor;
            Color highlightCol = new Color(lightGray, lightGray, lightGray);
            Color bgCol = new Color(darkGray, darkGray, darkGray);
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding.bottom = 8;
            buttonStyle.margin.left = 0;
            buttonStyle.margin.right = 0;
	 
            GUILayout.BeginHorizontal();
            {   //Create a row of buttons
                for (GuiTabs i = 0; i < GuiTabs.Count; ++i)
                {
                    GUI.backgroundColor = i == selected ? highlightCol : bgCol;
                    if (GUILayout.Button(options[(int)i], buttonStyle))
                    {
                        selected = i; //Tab click
                    }
                }
            } GUILayout.EndHorizontal();
            //Restore color
            GUI.backgroundColor = storeColor;	 
            return selected;
        }

        // Get/Set objects as "favorites", they just get highlighted for now
        public static bool IsHighlighted(Object obj)
        {
            if ( obj == null )
                return false;
            const string fav = "HL";
            return System.Array.Exists(AssetDatabase.GetLabels(obj), item=>string.Equals(item,fav));
        }	
        public static void ToggleHighlight(Object obj)
        {
            if ( obj == null )
                return;
            List<string> labels = new List<string>(AssetDatabase.GetLabels(obj));
            if ( IsHighlighted(obj) )
                labels.Remove("HL");
            else
                labels.Add("HL");
            AssetDatabase.SetLabels(obj,labels.ToArray());		

            // Refresh lists
            if ( instance )
                instance.CreateMainGuiLists();
        }

    }
}