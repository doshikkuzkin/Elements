using System;
using Newtonsoft.Json;

namespace DefaultNamespace
{
	[Serializable]
	public class CellModel
	{
		[JsonProperty] public BlockType BlockType;

		public CellModel()
		{
			BlockType = BlockType.None;
		}
		
		public CellModel(BlockType blockType)
		{
			BlockType = blockType;
		}
		
		public void SetBlockType(BlockType blockType)
		{
			BlockType = blockType;
		}
	}
}