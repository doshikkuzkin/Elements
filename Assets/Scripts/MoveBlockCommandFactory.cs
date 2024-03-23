using UnityEngine;

namespace DefaultNamespace
{
	public class MoveBlockCommandFactory : IMoveBlockCommandFactory
	{
		private readonly IGridViewModel _gridViewModel;
		private readonly IAnimationsProcessor _animationsProcessor;

		public MoveBlockCommandFactory(IGridViewModel gridViewModel, IAnimationsProcessor animationsProcessor)
		{
			_gridViewModel = gridViewModel;
			_animationsProcessor = animationsProcessor;
		}


		public MoveBlockCommand Create(Vector2Int cellToMove, Vector2Int direction)
		{
			return new MoveBlockCommand(cellToMove, direction, _gridViewModel, _animationsProcessor);
		}
	}
}