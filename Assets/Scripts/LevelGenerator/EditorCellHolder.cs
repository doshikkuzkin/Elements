using System.Linq;
using Data;
using Extensions;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace LevelGenerator
{
	public class EditorCellHolder : MonoBehaviour
	{
		[SerializeField] private Button _cellButton;
		[SerializeField] private EditorBlockViewData[] _blockViewData;
		[SerializeField] private GameSettingsConfig _gameSettingsConfig;
		
		private CellModel _cellModel;

		private void Awake()
		{
			_cellButton.onClick.AddListener(OnCellButtonClicked);
		}

		public void SetCellModel(CellModel cellModel)
		{
			_cellModel = cellModel;
			
			UpdateBlockType(_cellModel.BlockType);
		}

		private void OnCellButtonClicked()
		{
			var blockType = _gameSettingsConfig.GetNextBlockType(_cellModel.BlockType);
			
			UpdateBlockType(blockType);
		}

		private void UpdateBlockType(int blockType)
		{
			_cellModel.SetBlockType(blockType);
			
			var typeSprite = _blockViewData.FirstOrDefault(viewData => viewData.BlockType == _cellModel.BlockType)?.BlockSprite;
			_cellButton.image.sprite = typeSprite;
		}
	}
}