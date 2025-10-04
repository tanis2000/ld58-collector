using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace GameBase.Physics
{
    public static class Collide
    {
        public static bool Check(Entity a, Entity b)
	{
		if (a.Collider == null || b.Collider == null)
		{
			return false;
		}
		if (a != b && b.Collidable)
		{
			return a.Collider.Collide(b);
		}
		return false;
	}

	public static bool Check(Entity a, Entity b, Vector2 at)
	{
		Vector2 old = a.Position;
		a.Position = at;
		bool result = Check(a, b);
		a.Position = old;
		return result;
	}

	public static bool Check(Entity a, IEnumerable<Entity> b)
	{
		foreach (Entity e in b)
		{
			if (Check(a, e))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Check(Entity a, IEnumerable<Entity> b, Vector2 at)
	{
		Vector2 old = a.Position;
		a.Position = at;
		bool result = Check(a, b);
		a.Position = old;
		return result;
	}

	public static Entity First(Entity a, IEnumerable<Entity> b)
	{
		foreach (Entity e in b)
		{
			if (Check(a, e))
			{
				return e;
			}
		}
		return null;
	}

	public static Entity First(Entity a, IEnumerable<Entity> b, Vector2 at)
	{
		Vector2 old = a.Position;
		a.Position = at;
		Entity result = First(a, b);
		a.Position = old;
		return result;
	}

	public static List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into)
	{
		foreach (Entity e in b)
		{
			if (Check(a, e))
			{
				into.Add(e);
			}
		}
		return into;
	}

	public static List<Entity> All(Entity a, IEnumerable<Entity> b, List<Entity> into, Vector2 at)
	{
		Vector2 old = a.Position;
		a.Position = at;
		List<Entity> result = All(a, b, into);
		a.Position = old;
		return result;
	}

	public static List<Entity> All(Entity a, IEnumerable<Entity> b)
	{
		return All(a, b, new List<Entity>());
	}

	public static List<Entity> All(Entity a, IEnumerable<Entity> b, Vector2 at)
	{
		return All(a, b, new List<Entity>(), at);
	}

	public static bool CheckPoint(Entity a, Vector2 point)
	{
		if (a.Collider == null)
		{
			return false;
		}
		return a.Collider.Collide(point);
	}

	public static bool CheckPoint(Entity a, Vector2 point, Vector2 at)
	{
		Vector2 old = a.Position;
		a.Position = at;
		bool result = CheckPoint(a, point);
		a.Position = old;
		return result;
	}

	public static bool CheckLine(Entity a, Vector2 from, Vector2 to)
	{
		if (a.Collider == null)
		{
			return false;
		}
		return a.Collider.Collide(from, to);
	}

	public static bool CheckLine(Entity a, Vector2 from, Vector2 to, Vector2 at)
	{
		Vector2 old = a.Position;
		a.Position = at;
		bool result = CheckLine(a, from, to);
		a.Position = old;
		return result;
	}

	public static bool CheckRect(Entity a, Rect rect)
	{
		if (a.Collider == null)
		{
			return false;
		}
		return a.Collider.Collide(rect);
	}

	public static bool CheckRect(Entity a, Rect rect, Vector2 at)
	{
		Vector2 old = a.Position;
		a.Position = at;
		bool result = CheckRect(a, rect);
		a.Position = old;
		return result;
	}

	public static bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
	{
		Vector2 b3 = a2 - a1;
		Vector2 d = b2 - b1;
		float bDotDPerp = b3.x * d.y - b3.y * d.x;
		if (bDotDPerp == 0f)
		{
			return false;
		}
		Vector2 c = b1 - a1;
		float t = (c.x * d.y - c.y * d.x) / bDotDPerp;
		if (t < 0f || t > 1f)
		{
			return false;
		}
		float u = (c.x * b3.y - c.y * b3.x) / bDotDPerp;
		if (u < 0f || u > 1f)
		{
			return false;
		}
		return true;
	}

	public static bool LineCheck(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
	{
		intersection = Vector2.zero;
		Vector2 b3 = a2 - a1;
		Vector2 d = b2 - b1;
		float bDotDPerp = b3.x * d.y - b3.y * d.x;
		if (bDotDPerp == 0f)
		{
			return false;
		}
		Vector2 c = b1 - a1;
		float t = (c.x * d.y - c.y * d.x) / bDotDPerp;
		if (t < 0f || t > 1f)
		{
			return false;
		}
		float u = (c.x * b3.y - c.y * b3.x) / bDotDPerp;
		if (u < 0f || u > 1f)
		{
			return false;
		}
		intersection = a1 + t * b3;
		return true;
	}

	public static bool CircleToLine(Vector2 cPosiition, float cRadius, Vector2 lineFrom, Vector2 lineTo)
	{
		return Vector2.Distance(cPosiition, Calc.ClosestPointOnLine(lineFrom, lineTo, cPosiition)) < cRadius;
	}

	public static bool CircleToPoint(Vector2 cPosition, float cRadius, Vector2 point)
	{
		return Vector2.Distance(cPosition, point) < cRadius;
	}

	public static bool CircleToRect(Vector2 cPosition, float cRadius, float rX, float rY, float rW, float rH)
	{
		return RectToCircle(rX, rY, rW, rH, cPosition, cRadius);
	}

	public static bool CircleToRect(Vector2 cPosition, float cRadius, Rect rect)
	{
		return RectToCircle(rect, cPosition, cRadius);
	}

	public static bool RectToCircle(float rX, float rY, float rW, float rH, Vector2 cPosition, float cRadius)
	{
		if (RectToPoint(rX, rY, rW, rH, cPosition))
		{
			return true;
		}
		PointSectors sector = GetSector(rX, rY, rW, rH, cPosition);
		if ((sector & PointSectors.Top) != 0)
		{
			Vector2 edgeFrom = new Vector2(rX, rY);
			Vector2 edgeTo = new Vector2(rX + rW, rY);
			if (CircleToLine(cPosition, cRadius, edgeFrom, edgeTo))
			{
				return true;
			}
		}
		if ((sector & PointSectors.Bottom) != 0)
		{
			Vector2 edgeFrom = new Vector2(rX, rY + rH);
			Vector2 edgeTo = new Vector2(rX + rW, rY + rH);
			if (CircleToLine(cPosition, cRadius, edgeFrom, edgeTo))
			{
				return true;
			}
		}
		if ((sector & PointSectors.Left) != 0)
		{
			Vector2 edgeFrom = new Vector2(rX, rY);
			Vector2 edgeTo = new Vector2(rX, rY + rH);
			if (CircleToLine(cPosition, cRadius, edgeFrom, edgeTo))
			{
				return true;
			}
		}
		if ((sector & PointSectors.Right) != 0)
		{
			Vector2 edgeFrom = new Vector2(rX + rW, rY);
			Vector2 edgeTo = new Vector2(rX + rW, rY + rH);
			if (CircleToLine(cPosition, cRadius, edgeFrom, edgeTo))
			{
				return true;
			}
		}
		return false;
	}

	public static bool RectToCircle(Rect rect, Vector2 cPosition, float cRadius)
	{
		return RectToCircle(rect.x, rect.y, rect.width, rect.height, cPosition, cRadius);
	}

	public static bool RectToLine(float rX, float rY, float rW, float rH, Vector2 lineFrom, Vector2 lineTo)
	{
		PointSectors fromSector = GetSector(rX, rY, rW, rH, lineFrom);
		PointSectors toSector = GetSector(rX, rY, rW, rH, lineTo);
		if (fromSector == PointSectors.Center || toSector == PointSectors.Center)
		{
			return true;
		}
		if ((fromSector & toSector) != 0)
		{
			return false;
		}
		PointSectors both = fromSector | toSector;
		if ((both & PointSectors.Top) != 0)
		{
			Vector2 a = new Vector2(rX, rY);
			Vector2 edgeTo = new Vector2(rX + rW, rY);
			if (LineCheck(a, edgeTo, lineFrom, lineTo))
			{
				return true;
			}
		}
		if ((both & PointSectors.Bottom) != 0)
		{
			Vector2 a2 = new Vector2(rX, rY + rH);
			Vector2 edgeTo = new Vector2(rX + rW, rY + rH);
			if (LineCheck(a2, edgeTo, lineFrom, lineTo))
			{
				return true;
			}
		}
		if ((both & PointSectors.Left) != 0)
		{
			Vector2 a3 = new Vector2(rX, rY);
			Vector2 edgeTo = new Vector2(rX, rY + rH);
			if (LineCheck(a3, edgeTo, lineFrom, lineTo))
			{
				return true;
			}
		}
		if ((both & PointSectors.Right) != 0)
		{
			Vector2 a4 = new Vector2(rX + rW, rY);
			Vector2 edgeTo = new Vector2(rX + rW, rY + rH);
			if (LineCheck(a4, edgeTo, lineFrom, lineTo))
			{
				return true;
			}
		}
		return false;
	}

	public static bool RectToLine(Rectangle rect, Vector2 lineFrom, Vector2 lineTo)
	{
		return RectToLine(rect.X, rect.Y, rect.Width, rect.Height, lineFrom, lineTo);
	}

	public static bool RectToPoint(float rX, float rY, float rW, float rH, Vector2 point)
	{
		if (point.x >= rX && point.y >= rY && point.x < rX + rW)
		{
			return point.y < rY + rH;
		}
		return false;
	}

	public static bool RectToPoint(Rectangle rect, Vector2 point)
	{
		return RectToPoint(rect.X, rect.Y, rect.Width, rect.Height, point);
	}

	public static PointSectors GetSector(Rectangle rect, Vector2 point)
	{
		PointSectors sector = PointSectors.Center;
		if (point.x < (float)rect.Left)
		{
			sector |= PointSectors.Left;
		}
		else if (point.x >= (float)rect.Right)
		{
			sector |= PointSectors.Right;
		}
		if (point.y < (float)rect.Top)
		{
			sector |= PointSectors.Top;
		}
		else if (point.y >= (float)rect.Bottom)
		{
			sector |= PointSectors.Bottom;
		}
		return sector;
	}

	public static PointSectors GetSector(float rX, float rY, float rW, float rH, Vector2 point)
	{
		PointSectors sector = PointSectors.Center;
		if (point.x < rX)
		{
			sector |= PointSectors.Left;
		}
		else if (point.x >= rX + rW)
		{
			sector |= PointSectors.Right;
		}
		if (point.y < rY)
		{
			sector |= PointSectors.Top;
		}
		else if (point.y >= rY + rH)
		{
			sector |= PointSectors.Bottom;
		}
		return sector;
	}
    }
}