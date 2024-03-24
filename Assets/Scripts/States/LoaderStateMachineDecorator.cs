using Cysharp.Threading.Tasks;
using Views;

namespace States
{
	public class LoaderStateMachineDecorator : BaseStateMachineDecorator
	{
		private readonly ILoaderView _loaderView;

		public LoaderStateMachineDecorator(ILoaderView loaderView)
		{
			_loaderView = loaderView;
		}

		public override async UniTask OnBeforeInitialize()
		{
			await ShowLoader();

			await base.OnBeforeInitialize();
		}

		public override async UniTask OnBeforeExecute()
		{
			await HideLoader();

			await base.OnBeforeExecute();
		}

		public override async UniTask OnBeforeStop()
		{
			await ShowLoader();

			await base.OnBeforeStop();
		}

		private UniTask ShowLoader()
		{
			if (_loaderView.IsLoaderShown)
			{
				return UniTask.CompletedTask;
			}
			
			_loaderView.ShowLoader();

			return UniTask.WaitUntil(() => _loaderView.IsLoaderShown);
		}

		private UniTask HideLoader()
		{
			if (_loaderView.IsLoaderHidden)
			{
				return UniTask.CompletedTask;
			}
			
			_loaderView.HideLoader();

			return UniTask.WaitUntil(() => _loaderView.IsLoaderHidden);
		}
	}
}