using Commands;
using UnityEngine;
using Views;
using Zenject;

namespace Processors
{
	public class GridMovementProcessor : IGridMovementProcessor
	{
		[Inject] private ICommandsProcessor _commandsProcessor;

		private Vector2 _firstPressPosition;
		[Inject] private IMoveBlockCommandFactory _moveBlockCommandFactory;

		public void ProcessUserInput()
		{
			if (Input.GetMouseButtonDown(0))
			{
				_firstPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

				return;
			}

			if (Input.GetMouseButtonUp(0))
			{
				var secondPressPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				var currentSwipe = new Vector2(secondPressPosition.x - _firstPressPosition.x,
					secondPressPosition.y - _firstPressPosition.y);
				currentSwipe.Normalize();

				if (TryGetSwipeDirection(currentSwipe, out var direction))
				{
					TryMoveBlockUnderSwipe(direction);
				}
			}
		}

		private bool TryGetSwipeDirection(Vector2 swipe, out Vector2Int direction)
		{
			direction = Vector2Int.zero;

			if (swipe is {y: > 0, x: > -0.5f and < 0.5f})
			{
				direction = Vector2Int.up;
				return true;
			}

			if (swipe is {y: < 0, x: > -0.5f and < 0.5f})
			{
				direction = Vector2Int.down;
				return true;
			}

			if (swipe is {x: < 0, y: > -0.5f and < 0.5f})
			{
				direction = Vector2Int.left;
				return true;
			}

			if (swipe is {x: > 0, y: > -0.5f and < 0.5f})
			{
				direction = Vector2Int.right;
				return true;
			}

			return false;
		}

		private void TryMoveBlockUnderSwipe(Vector2Int moveDirection)
		{
			var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(_firstPressPosition), Vector2.zero);

			var blockUnderSwipe = hit.collider?.GetComponent<BlockView>();
			
			if(blockUnderSwipe != null && blockUnderSwipe.IsAllowedToMove)
			{
				_commandsProcessor.AddCommand(_moveBlockCommandFactory.Create(blockUnderSwipe.CellModel.Position, moveDirection));
			}
		}
	}
}