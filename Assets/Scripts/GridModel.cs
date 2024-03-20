using System;
using Newtonsoft.Json;

namespace DefaultNamespace
{
	[Serializable]
	public class GridModel : ICloneable
	{
		[JsonProperty]
		public ColumnModel[] Grid;
		
		private int ColumnsCount => Grid?.Length ?? 0;
		private int RowsCount => ColumnsCount > 0 ? Grid[0]?.Cells?.Length ?? 0 : 0;
		
		public GridModel(ColumnModel[] grid)
		{
			Grid = grid;
		}
		
		public GridModel(int rows, int columns)
		{
			Grid = CreateGrid(columns, rows);
		}
		
		public object Clone()
		{
			var newGrid = CreateGrid(ColumnsCount, RowsCount);
			CopyGrid(Grid, newGrid);

			return new GridModel(newGrid);
		}

		public void SetCellValue(int columnIndex, int rowIndex, BlockType blockType)
		{
			if (columnIndex < 0 || columnIndex >= ColumnsCount || rowIndex < 0 || rowIndex >= RowsCount)
			{
				throw new ArgumentOutOfRangeException();
			}
			
			Grid[columnIndex].Cells[rowIndex] ??= new CellModel();
			Grid[columnIndex].Cells[rowIndex].SetBlockType(blockType);
		}
		
		public void SetRows(int rows)
		{
			UpdateGrid(ColumnsCount, rows);
		}
		
		public void SetColumns(int columns)
		{
			UpdateGrid(columns, RowsCount);
		}
		
		private void UpdateGrid(int columns, int rows)
		{
			var updatedGrid = CreateGrid(columns, rows);
			CopyGrid(Grid, updatedGrid);

			Grid = updatedGrid;
		}
		
		private void CopyGrid(ColumnModel[] source, ColumnModel[] destination)
		{
			for (var i = 0; i < source.Length; i++)
			{
				for (var j = 0; j < source[i].Cells.Length; j++)
				{
					if (i < destination.Length && j < destination[i].Cells.Length)
					{
						destination[i].Cells[j] = new CellModel(source[i].Cells[j].BlockType);
					}
				}
			}
		}

		private ColumnModel[] CreateGrid(int columns, int rows)
		{
			var grid = new ColumnModel[columns];
			
			for (var i = 0; i < columns; i++)
			{
				grid[i] = new ColumnModel {Cells = new CellModel[rows]};

				for (int j = 0; j < rows; j++)
				{
					grid[i].Cells[j] = new CellModel();
				}
			}

			return grid;
		}
	}
}