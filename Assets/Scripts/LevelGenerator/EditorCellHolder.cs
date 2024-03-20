using System.Linq;
using DefaultNamespace.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
	public class EditorCellHolder : MonoBehaviour
	{
		[SerializeField] private Button _cellButton;
		[SerializeField] private EditorBlockViewData[] _blockViewData;
		
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
			var blockType = _cellModel.BlockType.NextValue();
			
			UpdateBlockType(blockType);
		}

		private void UpdateBlockType(BlockType blockType)
		{
			_cellModel.SetBlockType(blockType);
			
			var typeSprite = _blockViewData.FirstOrDefault(viewData => viewData.BlockType == _cellModel.BlockType)?.BlockSprite;
			_cellButton.image.sprite = typeSprite;
		}
	}
}