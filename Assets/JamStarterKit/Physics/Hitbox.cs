using System;
using System.Drawing;
using GameBase.Utils;
using UnityEngine;
using Color = UnityEngine.Color;

namespace GameBase.Physics
{
    public class Hitbox: Collider
    {
	    [SerializeField]
        private float width;

	    [SerializeField]
	private float height;

	public override float Width
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
		}
	}

	public override float Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	public override float Left
	{
		get
		{
			return Position.x;
		}
		set
		{
			Position.x = value;
		}
	}

	public override float Bottom
	{
		get
		{
			return Position.y;
		}
		set
		{
			Position.y = value;
		}
	}

	public override float Right
	{
		get
		{
			return Position.x + Width;
		}
		set
		{
			Position.x = value - Width;
		}
	}

	public override float Top
	{
		get
		{
			return Position.y + Height;
		}
		set
		{
			Position.y = value - Height;
		}
	}

	public Hitbox(float width, float height, float x = 0f, float y = 0f)
	{
		this.width = width;
		this.height = height;
		Position.x = x;
		Position.y = y;
	}

	public bool Intersects(Hitbox hitbox)
	{
		if (base.AbsoluteLeft < hitbox.AbsoluteRight && base.AbsoluteRight > hitbox.AbsoluteLeft && base.AbsoluteBottom < hitbox.AbsoluteTop)
		{
			return base.AbsoluteTop > hitbox.AbsoluteBottom;
		}
		return false;
	}

	public bool Intersects(float x, float y, float width, float height)
	{
		if (base.AbsoluteRight > x && base.AbsoluteBottom > y && base.AbsoluteLeft < x + width)
		{
			return base.AbsoluteTop < y + height;
		}
		return false;
	}

	public override Collider Clone()
	{
		return new Hitbox(width, height, Position.x, Position.y);
	}

	public override void Render(Camera camera, Color color)
	{
		DebugExtensions.DrawRectangle(new Vector2(base.AbsoluteLeft, base.AbsoluteBottom), new Vector2(Width, Height), color);
	}

	public void SetFromRectangle(Rectangle rect)
	{
		Position = new Vector2(rect.X, rect.Y);
		Width = rect.Width;
		Height = rect.Height;
	}

	public void Set(float x, float y, float w, float h)
	{
		Position = new Vector2(x, y);
		Width = w;
		Height = h;
	}

	public void GetTopEdge(out Vector2 from, out Vector2 to)
	{
		from.x = base.AbsoluteLeft;
		to.x = base.AbsoluteRight;
		from.y = (to.y = base.AbsoluteTop);
	}

	public void GetBottomEdge(out Vector2 from, out Vector2 to)
	{
		from.x = base.AbsoluteLeft;
		to.x = base.AbsoluteRight;
		from.y = (to.y = base.AbsoluteBottom);
	}

	public void GetLeftEdge(out Vector2 from, out Vector2 to)
	{
		from.y = base.AbsoluteTop;
		to.y = base.AbsoluteBottom;
		from.x = (to.x = base.AbsoluteLeft);
	}

	public void GetRightEdge(out Vector2 from, out Vector2 to)
	{
		from.y = base.AbsoluteTop;
		to.y = base.AbsoluteBottom;
		from.x = (to.x = base.AbsoluteRight);
	}

	public override bool Collide(Vector2 point)
	{
		return Physics.Collide.RectToPoint(base.AbsoluteLeft, base.AbsoluteTop, Width, Height, point);
	}

	public override bool Collide(Rect rect)
	{
		if (base.AbsoluteRight > (float)rect.left && base.AbsoluteBottom > (float)rect.top && base.AbsoluteLeft < (float)rect.right)
		{
			return base.AbsoluteTop < (float)rect.bottom;
		}
		return false;
	}

	public override bool Collide(Vector2 from, Vector2 to)
	{
		return Physics.Collide.RectToLine(base.AbsoluteLeft, base.AbsoluteTop, Width, Height, from, to);
	}

	public override bool Collide(Hitbox hitbox)
	{
		return Intersects(hitbox);
	}

	// public override bool Collide(Grid grid)
	// {
	// 	return grid.Collide(base.Bounds);
	// }

	public override bool Collide(Circle circle)
	{
		return Physics.Collide.RectToCircle(base.AbsoluteLeft, base.AbsoluteTop, Width, Height, circle.AbsolutePosition, circle.Radius);
	}

	public override bool Collide(ColliderList list)
	{
		return list.Collide(this);
	}
	
    }
}