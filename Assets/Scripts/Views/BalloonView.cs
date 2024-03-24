using UnityEngine;

namespace Views
{
	public class BalloonView : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;

		private float _frequency;
		private float _amplitude;
		private float _speed;

		private float _startX;
		private float _startY;
		private float _x;
		private float _y;
		private float _targetXPosition;
		private float _direction;

		public void SetSprite(Sprite sprite)
		{
			_spriteRenderer.sprite = sprite;
		}

		public void Initialize(float startX, float startY, float distance, float direction, float frequency, float amplitude, float speed)
		{
			_startX = startX * direction;
			_x = _startX;
			_startY = startY;
			_y = startY;
			_targetXPosition = _startX + direction * distance;
			_direction = direction;
			
			_frequency = frequency;
			_amplitude = amplitude;
			_speed = speed;
		}

		private void Update()
		{
			_x += _speed * Time.deltaTime * _direction;
			_y = _startY + Mathf.Sin(_x * _frequency) * _amplitude;
			
			transform.position = new Vector2(_x, _y);

			if ((_direction > 0 && _x >= _targetXPosition) || (_direction < 0 && _x <= _targetXPosition))
			{
				(_targetXPosition, _startX) = (_startX, _targetXPosition);
				_direction *= -1;
			}
		}
	}
}