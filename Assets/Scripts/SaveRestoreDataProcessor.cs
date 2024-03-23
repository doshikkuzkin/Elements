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

		public SaveRestoreDataProcessor(IGridViewModel gridViewModel, ISaveRestoreDataObserver saveRestoreDataObserver)
		{
			_gridViewModel = gridViewModel;
			_saveRestoreDataObserver = saveRestoreDataObserver;
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
			var currentGridState = _gridViewModel.GridModel;
			var currentGridStateJson = JsonConvert.SerializeObject(currentGridState);
			
			PlayerPrefs.SetString("LevelState", currentGridStateJson);
			PlayerPrefs.Save();
		}
		
		private void OnClearRequested()
		{
			PlayerPrefs.DeleteKey("LevelState");
			PlayerPrefs.Save();
		}
	}
}