using Observers;
using Processors;
using Providers;
using UnityEngine;
using Views.ViewModels;

namespace Commands
{
	public class MoveBlockCommandFactory : IMoveBlockCommandFactory
	{
		private readonly IAnimationsProcessor _animationsProcessor;
		private readonly IGridViewModel _gridViewModel;
		private readonly ILevelStateProvider _levelStateProvider;
		private readonly ISaveRestoreDataObserver _saveRestoreDataObserver;

		public MoveBlockCommandFactory(
			IGridViewModel gridViewModel,
			IAnimationsProcessor animationsProcessor,
			ISaveRestoreDataObserver saveRestoreDataObserver,
			ILevelStateProvider levelStateProvider)
		{
			_gridViewModel = gridViewModel;
			_animationsProcessor = animationsProcessor;
			_saveRestoreDataObserver = saveRestoreDataObserver;
			_levelStateProvider = levelStateProvider;
		}

		public MoveBlockCommand Create(Vector2Int cellToMove, Vector2Int direction)
		{
			return new MoveBlockCommand(cellToMove, direction, _gridViewModel, _animationsProcessor,
				_saveRestoreDataObserver, _levelStateProvider);
		}
	}
}