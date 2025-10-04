using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace GameBase.Editor
{
    public static class EditorUtils
    {
        public delegate void RefreshTargetDelegate();

        public delegate void CreateListItemGUIDelegate(int index);

        public delegate T CreateNewListItemDelegate<T>(T before, T after);

        public static void UpdateListInspector<T>(ref List<T> list, RefreshTargetDelegate refreshTargetDelegate,
            CreateListItemGUIDelegate createItemGUIDelegate, CreateNewListItemDelegate<T> createNewItemDelegate)
            where T : new()
        {
            GUILayout.Space(5);

            int addAt = -2;
            int removeAt = -1;
            int upAt = -1;
            int downAt = -1;

            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                EditorGUILayout.BeginHorizontal();

                //EditorGUILayout.BeginVertical(GUILayout.MaxWidth(25));
                EditorGUILayout.LabelField(i.ToString() + "-", EditorStyles.boldLabel, GUILayout.MaxWidth(17));

                //EditorGUILayout.EndVertical();

                createItemGUIDelegate(i);


                if (GUILayout.Button(new GUIContent("\u25B2"), EditorStyles.miniButtonLeft, GUILayout.MaxWidth(20)))
                {
                    upAt = i;
                }

                if (GUILayout.Button(new GUIContent("\u25BC"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(20)))
                {
                    downAt = i;
                }

                if (GUILayout.Button(new GUIContent("+"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(20)))
                {
                    addAt = i;
                }

                if (GUILayout.Button(new GUIContent("X"), EditorStyles.miniButtonRight, GUILayout.MaxWidth(20)))
                {
                    removeAt = i;
                }

                EditorGUILayout.EndHorizontal();


                //EditorGUILayout.BeginVertical(GUILayout.MaxWidth(40));

                //EditorGUILayout.EndVertical();
            }

            if (count == 0)
            {
                if (GUILayout.Button(new GUIContent("+"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(20)))
                {
                    addAt = -1;
                }
            }

            if (addAt >= -1)
            {
                T prev = default(T);
                T next = default(T);
                if (addAt == -1)
                {
                    if (count > 0)
                    {
                        prev = list[count - 1];
                    }
                }
                else
                {
                    prev = list[addAt];
                }

                if (addAt + 1 >= count)
                {
                    if (count > 0)
                    {
                        next = list[0];
                    }
                }
                else
                {
                    next = list[addAt + 1];
                }

                if (createNewItemDelegate != null)
                    list.Insert(addAt + 1, createNewItemDelegate(prev, next));
                else
                    list.Insert(addAt + 1, new T());


                if (refreshTargetDelegate != null)
                    refreshTargetDelegate();
            }

            else if (removeAt >= 0)
            {
                list.RemoveAt(removeAt);

                if (refreshTargetDelegate != null)
                    refreshTargetDelegate();
            }

            else if (upAt > 0)
            {
                T temp = list[upAt - 1];
                list[upAt - 1] = list[upAt];
                list[upAt] = temp;

                if (refreshTargetDelegate != null)
                    refreshTargetDelegate();
            }
            else if (downAt >= 0 && downAt < list.Count - 1)
            {
                T temp = list[downAt];
                list[downAt] = list[downAt + 1];
                list[downAt + 1] = temp;

                if (refreshTargetDelegate != null)
                    refreshTargetDelegate();
            }
        }


        // Trying using sprite data to generate collider. This needs the sprite to be the currently visible one
        // Also won't handle saving/loading or even room changes. Probably better as a flag for "use sprite collision" and it'll update on sprite change
        // Or, drag sprites in with ID to choose which to use for collider based on ID/Name
        public static void UpdateClickableCollider(GameObject clickable)
        {
            SpriteRenderer renderer = clickable.GetComponentInChildren<SpriteRenderer>();
            PolygonCollider2D polygonCollider = clickable.GetComponentInChildren<PolygonCollider2D>();
            if (renderer == null || polygonCollider == null)
                return;

            Sprite sprite = renderer.sprite;
            if (sprite == null)
                return;

            polygonCollider.pathCount = sprite.GetPhysicsShapeCount();

            List<Vector2> path = new List<Vector2>();
            for (int i = 0; i < polygonCollider.pathCount; i++)
            {
                path.Clear();
                sprite.GetPhysicsShape(i, path);
                polygonCollider.SetPath(i, path.ToArray());
            }

            // offset by powersprite offset
            PowerTools.PowerSprite powerSprite = renderer.GetComponent<PowerTools.PowerSprite>();
            if (powerSprite != null)
                polygonCollider.offset = powerSprite.Offset;
        }

        public static string FindCurrentPath()
        {
            var path = string.Empty;
            var selectedObj = Selection.activeObject;
            // if we have an asset selected, we can get its path
            if (selectedObj != null)
            {
                path = AssetDatabase.GetAssetPath(selectedObj);
            }
            else
            {
                var selectedAssetGuid = Selection.assetGUIDs[0];
                path = AssetDatabase.GUIDToAssetPath(selectedAssetGuid);
            }

            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(selectedObj)), "");
            }

            return path;
        }

        public static string GetPrefabPath(GameObject gameObject)
        {
#if UNITY_2018_3_OR_NEWER

            for (int i = 0; i <= 1; ++i) // loop so we can try without the root, then with it.
            {
                // Get root gameobject
                GameObject prefabObject = gameObject;
                if (i == 1)
                    prefabObject = gameObject == null ? null : gameObject.transform.root.gameObject;

                // If staged, return staged path			
                UnityEditor.SceneManagement.PrefabStage prefabStage =
                    UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(prefabObject);
                if (prefabStage != null)
                {
#if UNITY_2020_1_OR_NEWER
                    return prefabStage.assetPath;
#else
						return prefabStage.prefabAssetPath;
#endif
                }

                // Otherwise, find the prefab if exists
                if (PrefabUtility.GetPrefabInstanceStatus(prefabObject) == PrefabInstanceStatus.Connected ||
                    PrefabUtility.GetPrefabInstanceStatus(prefabObject) == PrefabInstanceStatus.Disconnected)
                    prefabObject = PrefabUtility.GetCorrespondingObjectFromSource(prefabObject);

                if (prefabObject != null)
                    return AssetDatabase.GetAssetPath(prefabObject);
            }

#else
			// Get root gameobject
			Object prefabObject = gameObject == null ? null : gameObject.transform.root.gameObject;

			// The nice old easy pre-2018.3 way
			if ( PrefabUtility.GetPrefabType(prefabObject) == PrefabType.PrefabInstance )
				prefabObject = PrefabUtility.GetPrefabParent(prefabObject);
			if ( prefabObject != null )
				return AssetDatabase.GetAssetPath(prefabObject);

#endif

            return null;
        }

        public static bool CreateSpriteAtlas(string path, string spriteFolder, bool pixel, bool isGui,
            bool refreshAssetDB = true)
        {
            SpriteAtlas atlas = new SpriteAtlas();

            if (File.Exists(path))
            {
                Debug.Log($"Atlas already exists at {path}. Skipping.");
                return false;
            }

            AssetDatabase.CreateAsset(atlas, path);

            // Set packing settings if its a gui one
            SpriteAtlasPackingSettings packingSettings = atlas.GetPackingSettings();
            packingSettings.enableTightPacking = !isGui;
            packingSettings.enableRotation = !isGui;
            atlas.SetPackingSettings(packingSettings);

            // Set filter and compression for pixel art
            if (pixel)
            {
                SpriteAtlasTextureSettings texSettings = atlas.GetTextureSettings();
                texSettings.filterMode = FilterMode.Point;
                atlas.SetTextureSettings(texSettings);
                TextureImporterPlatformSettings platSettings = atlas.GetPlatformSettings("DefaultTexturePlatform");
                platSettings.textureCompression = TextureImporterCompression.Uncompressed;
                atlas.SetPlatformSettings(platSettings);
            }

            // Add the sprite folder
            Object folderObject = AssetDatabase.LoadAssetAtPath(spriteFolder, typeof(DefaultAsset));
            atlas.Add(new Object[] { folderObject });

            EditorUtility.SetDirty(atlas);
            AssetDatabase.SaveAssets();
            if (refreshAssetDB)
                AssetDatabase.Refresh();

            return true;
        }
    }
}