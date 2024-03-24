namespace DefaultNamespace.Tools
{
	public static class GameSettingsConfigExtensions
	{
		public static int GetNextBlockType(this GameSettingsConfig gameSettingsConfig, int currentBlockTypeIndex)
		{
			var blocksCount = gameSettingsConfig.BlockTypesData.Length;
			var nextBlockIndex = currentBlockTypeIndex + 1;

			return nextBlockIndex >= blocksCount ? BlockTypeData.EmptyBlockType : nextBlockIndex;
		}
	}
}