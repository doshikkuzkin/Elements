using System;
using Newtonsoft.Json;

namespace Data
{
	[Serializable]
	public class GridModel : ICloneable
	{
		[JsonProperty] public ColumnModel[] Grid;

		public GridModel()
		{
		}

		public GridModel(ColumnModel[] grid)
		{
			Grid = grid;
		}

		public GridModel(int rows, int columns)
		{
			Grid = CreateGrid(columns, rows);
		}

		private int ColumnsCount => Grid?.Length ?? 0;
		private int RowsCount => ColumnsCount > 0 ? Grid[0]?.Cells?.Length ?? 0 : 0;

		public object Clone()
		{
			var newGrid = CreateGrid(ColumnsCount, RowsCount);
			CopyGrid(Grid, newGrid);

			return new GridModel(newGrid);
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
			for (var x = 0; x < source.Length; x++)
			{
				for (var y = 0; y < source[x].Cells.Length; y++)
				{
					if (x < destination.Length && y < destination[x].Cells.Length)
					{
						destination[x].Cells[y] = new CellModel(source[x].Cells[y].BlockType, x, y);
					}
				}
			}
		}

		private ColumnModel[] CreateGrid(int columns, int rows)
		{
			var grid = new ColumnModel[columns];

			for (var x = 0; x < columns; x++)
			{
				grid[x] = new ColumnModel {Cells = new CellModel[rows]};

				for (var y = 0; y < rows; y++)
				{
					grid[x].Cells[y] = new CellModel(BlockTypeData.EmptyBlockType, x, y);
				}
			}

			return grid;
		}
	}
}