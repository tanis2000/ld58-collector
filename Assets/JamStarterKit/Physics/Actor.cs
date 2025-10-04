using UnityEngine;

namespace GameBase.Physics
{
    public class Actor: Entity
    {
	    private Vector2 movementCounter;
	    
	    private bool ignoreJumpThrus;

	    public Vector2 ExactPosition => Position + movementCounter;

	    public Vector2 PositionRemainder => movementCounter;
	    public bool IgnoreJumpThrus
	    {
		    get => ignoreJumpThrus;
		    set => ignoreJumpThrus = value;
	    }

	    protected virtual void OnSquish(CollisionData data)
        {
            //if (!TrySquishWiggle(data))
            {
                RemoveSelf();
            }
        }
        
        public virtual bool IsRiding(Solid solid)
        {
            return CollideCheck(solid, Position + Vector2.down);
        }

        public bool OnGround(int downCheck = 1)
        {
            if (!CollideCheck<Solid>(Position + Vector2.down * downCheck))
            {
                if (!IgnoreJumpThrus)
                {
                    return CollideCheckOutside<JumpThru>(Position + Vector2.down * downCheck);
                }
                return false;
            }
            return true;
        }

        public bool OnGround(Vector2 at, int downCheck = 1)
        {
            Vector2 was = Position;
            Position = at;
            bool result = OnGround(downCheck);
            Position = was;
            return result;
        }
        
        public bool MoveH(float moveH, Collision onCollide = null, Solid pusher = null)
	{
		movementCounter.x += moveH;
		int move = (int)Mathf.Round(movementCounter.x);
		if (move != 0)
		{
			movementCounter.x -= move;
			return MoveHExact(move, onCollide, pusher);
		}
		return false;
	}

	public bool MoveV(float moveV, Collision onCollide = null, Solid pusher = null)
	{
		movementCounter.y += moveV;
		int move = (int)Mathf.Round(movementCounter.y);
		if (move != 0)
		{
			movementCounter.y -= move;
			return MoveVExact(move, onCollide, pusher);
		}
		return false;
	}

	public bool MoveHExact(int moveH, Collision onCollide = null, Solid pusher = null)
	{
		Vector2 target = Position + Vector2.right * moveH;
		int sign = Mathf.RoundToInt(Mathf.Sign(moveH));
		int moved = 0;
		while (moveH != 0)
		{
			Solid hit = CollideFirst<Solid>(Position + Vector2.right * sign);
			if (hit != null)
			{
				movementCounter.x = 0f;
				onCollide?.Invoke(new CollisionData
				{
					Direction = Vector2.right * sign,
					Moved = Vector2.right * moved,
					TargetPosition = target,
					Hit = hit,
					Pusher = pusher
				});
				return true;
			}
			moved += sign;
			moveH -= sign;
			base.X += sign;
		}
		return false;
	}

	public bool MoveVExact(int moveV, Collision onCollide = null, Solid pusher = null)
	{
		Vector2 target = Position + Vector2.down * moveV;
		int sign = Mathf.RoundToInt(Mathf.Sign(moveV));
		int moved = 0;
		while (moveV != 0)
		{
			Platform hit = CollideFirst<Solid>(Position - Vector2.down * sign);
			if (hit != null)
			{
				movementCounter.y = 0f;
				onCollide?.Invoke(new CollisionData
				{
					Direction = Vector2.down * sign,
					Moved = Vector2.down * moved,
					TargetPosition = target,
					Hit = hit,
					Pusher = pusher
				});
				return true;
			}
			if (moveV > 0 && !IgnoreJumpThrus)
			{
				hit = CollideFirstOutside<JumpThru>(Position + Vector2.down * sign);
				if (hit != null)
				{
					movementCounter.y = 0f;
					onCollide?.Invoke(new CollisionData
					{
						Direction = Vector2.down * sign,
						Moved = Vector2.down * moved,
						TargetPosition = target,
						Hit = hit,
						Pusher = pusher
					});
					return true;
				}
			}
			moved += sign;
			moveV -= sign;
			base.Y += sign;
		}
		return false;
	}

	public void MoveTowardsX(float targetX, float maxAmount, Collision onCollide = null)
	{
		float moveTo = Calc.Approach(ExactPosition.x, targetX, maxAmount);
		MoveToX(moveTo, onCollide);
	}

	public void MoveTowardsY(float targetY, float maxAmount, Collision onCollide = null)
	{
		float moveTo = Calc.Approach(ExactPosition.y, targetY, maxAmount);
		MoveToY(moveTo, onCollide);
	}

	public void MoveToX(float toX, Collision onCollide = null)
	{
		MoveH(toX - ExactPosition.x, onCollide);
	}

	public void MoveToY(float toY, Collision onCollide = null)
	{
		MoveV(toY - ExactPosition.y, onCollide);
	}

	public void NaiveMove(Vector2 amount)
	{
		movementCounter += amount;
		int moveX = Mathf.RoundToInt(movementCounter.x);
		int moveY = Mathf.RoundToInt(movementCounter.y);
		Position += new Vector2(moveX, moveY);
		movementCounter -= new Vector2(moveX, moveY);
	}
    }
}