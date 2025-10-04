using System;
using UnityEngine;

namespace GameBase.Physics
{
    public abstract class Collider: MonoBehaviour
    {
        public Vector2 Position;

        [SerializeField]
        private Entity entity;

        public Entity Entity
        {
	        get => entity;
	        set => entity = value;
        }

        public abstract float Width { get; set; }

        public abstract float Height { get; set; }

        public abstract float Top { get; set; }

        public abstract float Bottom { get; set; }

        public abstract float Left { get; set; }

        public abstract float Right { get; set; }

        public float CenterX
	{
		get
		{
			return Left + Width / 2f;
		}
		set
		{
			Left = value - Width / 2f;
		}
	}

	public float CenterY
	{
		get
		{
			return Top + Height / 2f;
		}
		set
		{
			Top = value - Height / 2f;
		}
	}

	public Vector2 TopLeft
	{
		get => new(Left, Top);
		set
		{
			Left = value.x;
			Top = value.y;
		}
	}

	public Vector2 TopCenter
	{
		get => new(CenterX, Top);
		set
		{
			CenterX = value.x;
			Top = value.y;
		}
	}

	public Vector2 TopRight
	{
		get
		{
			return new Vector2(Right, Top);
		}
		set
		{
			Right = value.x;
			Top = value.y;
		}
	}

	public Vector2 CenterLeft
	{
		get
		{
			return new Vector2(Left, CenterY);
		}
		set
		{
			Left = value.x;
			CenterY = value.y;
		}
	}

	public Vector2 Center
	{
		get
		{
			return new Vector2(CenterX, CenterY);
		}
		set
		{
			CenterX = value.x;
			CenterY = value.y;
		}
	}

	public Vector2 Size => new Vector2(Width, Height);

	public Vector2 HalfSize => Size * 0.5f;

	public Vector2 CenterRight
	{
		get
		{
			return new Vector2(Right, CenterY);
		}
		set
		{
			Right = value.x;
			CenterY = value.y;
		}
	}

	public Vector2 BottomLeft
	{
		get
		{
			return new Vector2(Left, Bottom);
		}
		set
		{
			Left = value.x;
			Bottom = value.y;
		}
	}

	public Vector2 BottomCenter
	{
		get
		{
			return new Vector2(CenterX, Bottom);
		}
		set
		{
			CenterX = value.x;
			Bottom = value.y;
		}
	}

	public Vector2 BottomRight
	{
		get
		{
			return new Vector2(Right, Bottom);
		}
		set
		{
			Right = value.x;
			Bottom = value.y;
		}
	}

	public Vector2 AbsolutePosition
	{
		get
		{
			if (Entity != null)
			{
				return Entity.Position + Position;
			}
			return Position;
		}
	}

	public float AbsoluteX
	{
		get
		{
			if (Entity)
			{
				return Entity.Position.x + Position.x;
			}
			return Position.x;
		}
	}

	public float AbsoluteY
	{
		get
		{
			if (Entity != null)
			{
				return Entity.Position.y + Position.y;
			}
			return Position.y;
		}
	}

	public float AbsoluteTop
	{
		get
		{
			if (Entity != null)
			{
				return Top + Entity.Position.y;
			}
			return Top;
		}
	}

	public float AbsoluteBottom
	{
		get
		{
			if (Entity != null)
			{
				return Bottom + Entity.Position.y;
			}
			return Bottom;
		}
	}

	public float AbsoluteLeft
	{
		get
		{
			if (Entity != null)
			{
				return Left + Entity.Position.x;
			}
			return Left;
		}
	}

	public float AbsoluteRight
	{
		get
		{
			if (Entity != null)
			{
				return Right + Entity.Position.x;
			}
			return Right;
		}
	}

	public Rect Bounds => new Rect((int)AbsoluteLeft, (int)AbsoluteTop, (int)Width, (int)Height);

	internal virtual void Added(Entity entity)
	{
		Entity = entity;
	}

	internal virtual void Removed()
	{
		Entity = null;
	}

	public bool Collide(Entity entity)
	{
		return Collide(entity.Collider);
	}

	public bool Collide(Collider collider)
	{
		if (collider is Hitbox)
		{
			return Collide(collider as Hitbox);
		}
		// if (collider is Grid)
		// {
		// 	return Collide(collider as Grid);
		// }
		if (collider is ColliderList)
		{
			return Collide(collider as ColliderList);
		}
		if (collider is Circle)
		{
			return Collide(collider as Circle);
		}
		Debug.LogWarning("Collisions against the collider type are not implemented!");
		return false;
	}

	public abstract bool Collide(Vector2 point);

	public abstract bool Collide(Rect rect);

	public abstract bool Collide(Vector2 from, Vector2 to);

	public abstract bool Collide(Hitbox hitbox);

	// public abstract bool Collide(Grid grid);

	public abstract bool Collide(Circle circle);

	public abstract bool Collide(ColliderList list);

	public abstract Collider Clone();

	public abstract void Render(Camera camera, Color color);

	public void CenterOrigin()
	{
		Position.x = (0f - Width) / 2f;
		Position.y = (0f - Height) / 2f;
	}

	public void Render(Camera camera)
	{
		Render(camera, Color.red);
	}
	
    }
}