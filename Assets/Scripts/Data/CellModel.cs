using System;
using UnityEngine;

namespace Data
{
	[Serializable]
	public class CellModel
	{
		public int BlockType;

		public CellModel(int blockType, int column, int row)
		{
			BlockType = blockType;
			Column = column;
			Row = row;
		}

		public Vector2Int Position => new(Column, Row);
		public int Column { get; private set; }
		public int Row { get; private set; }
		public bool IsConnected { get; private set; }

		public void SetBlockType(int blockType)
		{
			BlockType = blockType;
		}

		public void SetPosition(Vector2Int position)
		{
			SetColumn(position.x);
			SetRow(position.y);
		}

		public void SetColumn(int column)
		{
			Column = column;
		}

		public void SetRow(int row)
		{
			Row = row;
		}

		public void SetIsConnected(bool isConnected)
		{
			IsConnected = isConnected;
		}
	}
}