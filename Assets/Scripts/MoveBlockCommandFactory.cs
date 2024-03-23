using UnityEngine;

namespace DefaultNamespace
{
	public class MoveBlockCommandFactory : IMoveBlockCommandFactory
	{
		private readonly IGridViewModel _gridViewModel;

		public MoveBlockCommandFactory(IGridViewModel gridViewModel)
		{
			_gridViewModel = gridViewModel;
		}


		public MoveBlockCommand Create(Vector2Int cellToMove, Vector2Int direction)
		{
			return new MoveBlockCommand(cellToMove, direction, _gridViewModel);
		}
	}
}