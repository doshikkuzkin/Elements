using System;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
	public class SaveRestoreDataProcessor : ISaveRestoreDataProcessor, IInitializable, IDisposable
	{
		private readonly IGridViewModel _gridViewModel;
		private readonly ISaveRestoreDataObserver _saveRestoreDataObserver;
		private readonly ILevelIndexProvider _levelIndexProvider;

		public SaveRestoreDataProcessor(IGridViewModel gridViewModel, ISaveRestoreDataObserver saveRestoreDataObserver, ILevelIndexProvider levelIndexProvider)
		{
			_gridViewModel = gridViewModel;
			_saveRestoreDataObserver = saveRestoreDataObserver;
			_levelIndexProvider = levelIndexProvider;
		}


		public void Initialize()
		{
			_saveRestoreDataObserver.SaveRequested += OnSaveRequested;
			_saveRestoreDataObserver.ClearRequested += OnClearRequested;
		}

		public void Dispose()
		{
			_saveRestoreDataObserver.SaveRequested -= OnSaveRequested;
			_saveRestoreDataObserver.ClearRequested -= OnClearRequested;
		}
		
		private void OnSaveRequested()
		{
			int levelIndexToSave = 0;
			string gridStateToSave;

			if (_gridViewModel.AreAllBlocksDestroyed())
			{
				levelIndexToSave = _levelIndexProvider.NextLevelIndex;
				gridStateToSave = string.Empty;
			}
			else
			{
				levelIndexToSave = _levelIndexProvider.CurrentLevelIndex;
				gridStateToSave = JsonConvert.SerializeObject(_gridViewModel.GridModel);
			}
			
			PlayerPrefs.SetInt("LevelIndex", levelIndexToSave);
			PlayerPrefs.SetString("LevelState", gridStateToSave);
			
			PlayerPrefs.Save();
		}
		
		private void OnClearRequested()
		{
			PlayerPrefs.SetInt("LevelIndex", _levelIndexProvider.CurrentLevelIndex);
			PlayerPrefs.DeleteKey("LevelState");
			
			PlayerPrefs.Save();
		}
	}
}