using GameBase.Utils;
using UnityEngine;
using Color = UnityEngine.Color;

namespace GameBase.Physics
{
    public class Circle: Collider
    {
        public float Radius;

        public override float Width
        {
            get
            {
                return Radius * 2f;
            }
            set
            {
                Radius = value / 2f;
            }
        }

        public override float Height
        {
            get
            {
                return Radius * 2f;
            }
            set
            {
                Radius = value / 2f;
            }
        }

        public override float Left
        {
            get
            {
                return Position.x - Radius;
            }
            set
            {
                Position.x = value + Radius;
            }
        }

        public override float Top
        {
            get
            {
                return Position.y - Radius;
            }
            set
            {
                Position.y = value + Radius;
            }
        }

        public override float Right
        {
            get
            {
                return Position.x + Radius;
            }
            set
            {
                Position.x = value - Radius;
            }
        }

        public override float Bottom
        {
            get
            {
                return Position.y + Radius;
            }
            set
            {
                Position.y = value - Radius;
            }
        }

        public Circle(float radius, float x = 0f, float y = 0f)
        {
            Radius = radius;
            Position.x = x;
            Position.y = y;
        }

        public override Collider Clone()
        {
            return new Circle(Radius, Position.x, Position.y);
        }

        public override void Render(Camera camera, Color color)
        {
            DebugExtensions.DrawCircle(base.AbsolutePosition, Radius, 4, color);
        }

        public override bool Collide(Vector2 point)
        {
            return Physics.Collide.CircleToPoint(base.AbsolutePosition, Radius, point);
        }

        public override bool Collide(Rect rect)
        {
            return Physics.Collide.RectToCircle(rect, base.AbsolutePosition, Radius);
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            return Physics.Collide.CircleToLine(base.AbsolutePosition, Radius, from, to);
        }

        public override bool Collide(Circle circle)
        {
            return Vector2.Distance(base.AbsolutePosition, circle.AbsolutePosition) < (Radius + circle.Radius);
        }

        public override bool Collide(Hitbox hitbox)
        {
            return hitbox.Collide(this);
        }

        // public override bool Collide(Grid grid)
        // {
        //     return grid.Collide(this);
        // }

        public override bool Collide(ColliderList list)
        {
            return list.Collide(this);
        }
    }
}