using UnityEngine;

namespace DefaultNamespace
{
	public class MoveBlockCommandFactory : IMoveBlockCommandFactory
	{
		private readonly IGridViewModel _gridViewModel;
		private readonly IAnimationsProcessor _animationsProcessor;
		private readonly ISaveRestoreDataObserver _saveRestoreDataObserver;

		public MoveBlockCommandFactory(IGridViewModel gridViewModel, IAnimationsProcessor animationsProcessor, ISaveRestoreDataObserver saveRestoreDataObserver)
		{
			_gridViewModel = gridViewModel;
			_animationsProcessor = animationsProcessor;
			_saveRestoreDataObserver = saveRestoreDataObserver;
		}


		public MoveBlockCommand Create(Vector2Int cellToMove, Vector2Int direction)
		{
			return new MoveBlockCommand(cellToMove, direction, _gridViewModel, _animationsProcessor, _saveRestoreDataObserver);
		}
	}
}