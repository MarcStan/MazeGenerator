using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Engine.Datastructures.Quadtree;
using Engine.Input;
using Engine.Rendering;

namespace Engine.Physics.Collision
{
	public class CollisionEngine : IComponent
	{
		#region Fields

		private readonly List<ICollidable> _collidables;
		private readonly Quadtree<ICollidable> _tree;

		#endregion

		#region Constructors

		public CollisionEngine(BoundingBox bbox)
		{
			_tree = new Quadtree<ICollidable>(bbox);
			_collidables = new List<ICollidable>();
		}

		#endregion

		#region Methods

		public void Render(IRenderContext renderContext, GameTime dt)
		{
		}

		public void Update(KeyboardManager keyboard, MouseManager mouse, GameTime dt)
		{
			PerformCollisionChecks();
		}

		public void Add(ICollidable collidable)
		{
			if (collidable == null)
			{
				throw new ArgumentNullException(nameof(collidable));
			}
			// also keep them in a list for easier collision checks
			// static collidables don't move, so they can't "run into" other collidables
			if (!collidable.IsStatic)
			{
				_collidables.Add(collidable);
			}
			else
			{
				_tree.Add(collidable);
			}
		}

		public void Remove(ICollidable collidable)
		{
			if (collidable == null)
			{
				throw new ArgumentNullException(nameof(collidable));
			}
			if (collidable.IsStatic)
			{
				_tree.Remove(collidable);
			}
			else
			{
				_collidables.Remove(collidable);
			}
		}

		private void PerformCollisionChecks()
		{
			for (int i = 0; i < _collidables.Count; i++)
			{
				var current = _collidables[i];
				// TODO: currently we only compare against static collidables using the quadtree, if dynamic collision checks are required we will need something better than a list here
				var collisionTargets = _tree.GetIntersectingElements(current.BoundingBox);
				foreach (var ct in collisionTargets)
				{
					if (ct.Collides(current))
					{
						// we have a collision
						current.CollisionResponse(ct);
						ct.CollisionResponse(current);
					}
				}
			}
		}

		#endregion
	}
}