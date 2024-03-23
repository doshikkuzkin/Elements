using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
	public interface IGridViewModel
	{
		Transform GridParent { get; }
		GridModel GridModel { get; }
		float CellSize { get; }

		BlockView[][] BlockViews { get; }
		
		void InitGrid(GridModel gridModel, GridView gridView);
		void InitBlocks(BlockView[][] blockViews);

		void ApplyScaleFactor();
		void ResetScaleFactor();
		Vector3 GetCellPosition(int x, int y);
		bool TryGetBlockView(Vector2Int cellPosition, out BlockView blockView);
		bool IsValidCellPosition(Vector2Int cellPosition);
		bool IsEmptyCell(Vector2Int cellPosition);
		void SwapCells(Vector2Int firstCellPosition, Vector2Int secondCellPosition);
		void SwapCellsViews(Vector2Int firstCellPosition, Vector2Int secondCellPosition);
		void DestroyCellsViews(IEnumerable<Vector2Int> cellsToDestroy);
		bool AreAllBlocksDestroyed();
	}
}