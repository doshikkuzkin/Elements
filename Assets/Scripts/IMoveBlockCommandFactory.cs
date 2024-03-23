using UnityEngine;

namespace DefaultNamespace
{
	public interface IMoveBlockCommandFactory
	{
		MoveBlockCommand Create(Vector2Int cellToMove, Vector2Int direction);
	}
}