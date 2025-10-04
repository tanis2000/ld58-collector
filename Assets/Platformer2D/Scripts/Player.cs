using System;
using GameBase.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using Collision = GameBase.Physics.Collision;

namespace App.Platformer2D.Scripts
{
    public class Player: Actor
    {
        private Vector2 speed;

        private bool onGround;

        private Collision onCollideH;

        private Collision onCollideV;
        private InputAction inputMove;
        
        public Vector2 Speed
        {
            get => speed;
            set => speed = value;
        }

        private void OnCollideH(CollisionData data)
        {
            speed.x = 0;
        }

        private void OnCollideV(CollisionData data)
        {
            speed.y = 0;
        }

        private void Awake()
        {
            onCollideH = OnCollideH;
            onCollideV = OnCollideV;
            inputMove = InputSystem.actions.FindAction("Move");
        }

        private void Update()
        {
            var moveX = 0f;
            var move = inputMove.ReadValue<Vector2>();
            if (move.x > 0)
            {
                moveX = 1;
            } else if (move.x < 0)
            {
                moveX = -1;
            }

            var multH = 1.0f;
            var maxH = 90f;
            speed.x = Calc.Approach(speed.x, maxH * moveX, 400f * multH * Time.deltaTime);
            
            if (!onGround)
            {
                var mult = 1.0f;
                var max = -160f;
                speed.y = Calc.Approach(speed.y, max, 900f * mult * Time.deltaTime);
            }
            if (speed.y >= 0)
            {
                Platform first = CollideFirst<Solid>(Position + Vector2.down);
                if (first == null)
                {
                    first = CollideFirstOutside<JumpThru>(Position + Vector2.down);
                }
                if (first != null)
                {
                    onGround = true;
                }
                else
                {
                    onGround = false;
                }
            }
            MoveH(Speed.x * Time.deltaTime, onCollideH);
            MoveV(Speed.y * Time.deltaTime, onCollideV);
        }
    }
}