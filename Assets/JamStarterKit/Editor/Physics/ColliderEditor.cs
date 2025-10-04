using System;
using System.Collections;
using System.Collections.Generic;
using GameBase.Physics;
using UnityEditor;
using UnityEngine;
using Collider = GameBase.Physics.Collider;

namespace GameBase.Editor.Physics
{
    [CustomEditor(typeof(Hitbox))]
    public class ColliderEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var collider = (Collider)target;
            
            Handles.color = Color.yellow;
            GUI.color = Color.yellow;
            
            // {
            //     Vector3 position =  new Vector3( collider.Left, collider.Bottom, 0);
            //     position = Handles.FreeMoveHandle( position+Vector3.one,1.0f,new Vector3(0,1,0),Handles.DotHandleCap)-Vector3.one;
            //     //Handles.Label(position + new Vector3(5,0,0), "Bounds", textStyle );
            //     //Handles.color = Color.yellow.WithAlpha(0.5f);
            //     //position.x = Mathf.Min(position.x,collider.AbsoluteRight);
            //     // position.y = Mathf.Min(position.y,collider.AbsoluteTop);
            //     collider.Left = Mathf.RoundToInt(position.x);
            //     collider.Bottom = Mathf.RoundToInt(position.y);
            // }
            // {
            //     Vector3 position =  new Vector3( roomBounds.xMax, roomBounds.yMax, 0);
            //     position = Handles.FreeMoveHandle( position-Vector3.one, Quaternion.identity,1.0f,new Vector3(0,1,0),Handles.DotHandleCap)+Vector3.one;
            //
            //     position.x = Mathf.Max(position.x,roomBounds.xMin);
            //     position.y = Mathf.Max(position.y,roomBounds.yMin);
            //     roomBounds.max = new Vector2Int((int)position.x, (int)position.y);
            // }

            Handles.DrawLine( new Vector2(collider.AbsoluteLeft, collider.AbsoluteBottom), new Vector2(collider.AbsoluteLeft,collider.AbsoluteTop) );
            Handles.DrawLine( new Vector2(collider.AbsoluteLeft, collider.AbsoluteBottom), new Vector2(collider.AbsoluteRight,collider.AbsoluteBottom) );
            Handles.DrawLine( new Vector2(collider.AbsoluteRight, collider.AbsoluteTop), new Vector2(collider.AbsoluteLeft,collider.AbsoluteTop) );
            Handles.DrawLine( new Vector2(collider.AbsoluteRight, collider.AbsoluteTop), new Vector2(collider.AbsoluteRight,collider.AbsoluteBottom) );
        }
    }
}
