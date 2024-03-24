using System.Collections.Generic;
using Data;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
	public class EditorGridGenerator : MonoBehaviour
	{
		[SerializeField] private Transform _gridParent;
		[SerializeField] private GameObject _columsPrefab;
		[SerializeField] private GameObject _cellPrefab;

		[SerializeField] private TMP_InputField _columnsField;
		[SerializeField] private TMP_InputField _rowsField;
		[SerializeField] private TMP_InputField _levelField;
		[SerializeField] private Button _generateButton;
		[SerializeField] private Button _saveButton;
		[SerializeField] private Button _loadButton;

		private readonly List<GameObject> _cells = new ();
		private GridModel _grid;

#if UNITY_EDITOR
		private void Awake()
		{
			_generateButton.onClick.AddListener(GenerateGrid);
			_saveButton.onClick.AddListener(GenerateLevelConfig);
			_loadButton.onClick.AddListener(LoadLevelConfig);
		}

		private void LoadLevelConfig()
		{
			if (!int.TryParse(_levelField.text, out var levelIndex))
			{
				return;
			}
			
			var scriptableObject = AssetDataBaseTool.LoadAssetAtPath<LevelConfig>($"Assets/LevelConfigs/Level{levelIndex}Config.asset");

			if (scriptableObject != null)
			{
				LoadGrid(scriptableObject);
			}
		}

		private void GenerateLevelConfig()
		{
			if (_grid == null)
			{
				return;
			}

			if (!int.TryParse(_levelField.text, out var levelIndex))
			{
				return;
			}

			AssetDataBaseTool.AssertHasFolder("Assets", "LevelConfigs");

			var existingAsset = AssetDataBaseTool.LoadAssetAtPath<LevelConfig>($"Assets/LevelConfigs/Level{levelIndex}Config.asset");
			
			if (existingAsset == null)
			{
				var levelConfig = ScriptableObject.CreateInstance<LevelConfig>();
				levelConfig.GridModel = (GridModel) _grid.Clone();
				
				AssetDataBaseTool.CreateAsset(levelConfig, $"Assets/LevelConfigs/Level{levelIndex}Config.asset");
			}
			else
			{
				existingAsset.GridModel = (GridModel) _grid.Clone();
			}
			
			AssetDataBaseTool.Save();
		}

		private void LoadGrid(LevelConfig scriptableObject)
		{
			ClearGrid();
			
			_grid = (GridModel) scriptableObject.GridModel.Clone();
			
			InstantiateGrid();
		}

		private void GenerateGrid()
		{
			ClearGrid();
			
			var hasColumns = int.TryParse(_columnsField.text, out var columns) && columns > 0;
			var hasRows = int.TryParse(_rowsField.text, out var rows) && rows > 0;
			
			if (!hasColumns || !hasRows)
			{
				if (!hasColumns)
				{
					_columnsField.text = string.Empty;
				}

				if (!hasRows)
				{
					_rowsField.text = string.Empty;
				}
				
				return;
			}

			_grid = new GridModel(rows, columns);
			
			InstantiateGrid();
		}

		private void ClearGrid()
		{
			foreach (var cell in _cells)
			{
				Destroy(cell.gameObject);
			}
			
			_cells.Clear();
			_grid = null;
		}

		private void InstantiateGrid()
		{
			foreach (var column in _grid.Grid)
			{
				var columnObject = Instantiate(_columsPrefab, _gridParent);
				_cells.Add(columnObject);

				foreach (var cell in column.Cells)
				{
					var cellObject = Instantiate(_cellPrefab, columnObject.transform);

					var cellHolder = cellObject.GetComponent<EditorCellHolder>();
					cellHolder.SetCellModel(cell);
				}
			}
		}
#endif
	}
}