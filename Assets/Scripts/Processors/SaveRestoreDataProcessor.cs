using System;
using Newtonsoft.Json;
using Observers;
using Providers;
using UnityEngine;
using Views.ViewModels;
using Zenject;

namespace Processors
{
	public class SaveRestoreDataProcessor : IInitializable, IDisposable
	{
		private readonly IGridViewModel _gridViewModel;
		private readonly ILevelIndexProvider _levelIndexProvider;
		private readonly ILevelStateProvider _levelStateProvider;
		private readonly ISaveRestoreDataObserver _saveRestoreDataObserver;

		public SaveRestoreDataProcessor(
			IGridViewModel gridViewModel,
			ISaveRestoreDataObserver saveRestoreDataObserver,
			ILevelIndexProvider levelIndexProvider,
			ILevelStateProvider levelStateProvider)
		{
			_gridViewModel = gridViewModel;
			_saveRestoreDataObserver = saveRestoreDataObserver;
			_levelIndexProvider = levelIndexProvider;
			_levelStateProvider = levelStateProvider;
		}

		public void Dispose()
		{
			_saveRestoreDataObserver.SaveRequested -= OnSaveRequested;
			_saveRestoreDataObserver.ClearRequested -= OnClearRequested;
		}

		public void Initialize()
		{
			_saveRestoreDataObserver.SaveRequested += OnSaveRequested;
			_saveRestoreDataObserver.ClearRequested += OnClearRequested;
		}

		private void OnSaveRequested()
		{
			var levelIndexToSave = 0;
			string gridStateToSave;

			if (_levelStateProvider.IsLevelCleared)
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