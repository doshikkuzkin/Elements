using UnityEngine;

namespace Views
{
	public class BackgroundView : MonoBehaviour
	{
		[SerializeField] private int _balloonsCount;

		[SerializeField] private Vector2 _startXPositionRange;
		[SerializeField] private Vector2 _startYPositionRange;
		[SerializeField] private Vector2 _distanceRange;
		[SerializeField] private Vector2 _frequencieRange;
		[SerializeField] private Vector2 _amplitudeRange;
		[SerializeField] private Vector2 _speedRange;

		[SerializeField] private GameObject _balloonPrefab;
		[SerializeField] private Sprite[] _balloonsSprites;

		private readonly float[] _availableDirections = {-1, 1};

		private void Start()
		{
			for (var i = 0; i < _balloonsCount; i++)
			{
				CreateBalloon();
			}
		}

		private void CreateBalloon()
		{
			var balloon = Instantiate(_balloonPrefab, transform).GetComponent<BalloonView>();
			balloon.SetSprite(_balloonsSprites[Random.Range(0, _balloonsSprites.Length)]);

			var startX = Random.Range(_startXPositionRange.x, _startXPositionRange.y);
			var startY = Random.Range(_startYPositionRange.x, _startYPositionRange.y);
			var direction = _availableDirections[Random.Range(0, _availableDirections.Length)];
			var distance = Random.Range(_distanceRange.x, _distanceRange.y);
			var frequency = Random.Range(_frequencieRange.x, _frequencieRange.y);
			var amplitude = Random.Range(_amplitudeRange.x, _amplitudeRange.y);
			var speed = Random.Range(_speedRange.x, _speedRange.y);

			balloon.Initialize(startX, startY, distance, direction, frequency, amplitude, speed);
		}
	}
}