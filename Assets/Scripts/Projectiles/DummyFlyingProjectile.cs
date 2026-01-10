using UnityEngine;

namespace Projectiles
{
    public class DummyFlyingProjectile : MonoBehaviour
    {
        // PRIVATE METHODS

        [SerializeField]
        private float _speed = 80f;
        [SerializeField]
        private float _maxDistance = 100f;
        [SerializeField]
        private GameObject _hitEffect;
        [SerializeField]
        private float _lifeTimeAfterHit = 2f;
        [SerializeField]
        private GameObject _visualRoot;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private bool _showHitEffect;

        private float _startTime;
        private float _duration;

        public void SetHitPosition(Vector3 hitPosition)
        {
            _targetPosition = hitPosition;
            _showHitEffect = hitPosition != Vector3.zero;
        }

        protected void Awake()
        {
            if (_hitEffect != null)
            {
                _hitEffect.SetActive(false);
            }
        }

        protected void Start()
        {
            _startPosition = transform.position;

            if (_targetPosition == Vector3.zero)
            {
                _targetPosition = _startPosition + transform.forward * _maxDistance;
            }

            _duration = Vector3.Distance(_startPosition, _targetPosition) / _speed;
            _startTime = Time.timeSinceLevelLoad;
        }

        protected void Update()
        {
            float time = Time.timeSinceLevelLoad - _startTime;

            if (time < _duration)
            {
                transform.position = Vector3.Lerp(_startPosition, _targetPosition, time / _duration);
            }
            else
            {
                transform.position = _targetPosition;
                enabled = false;

                if (_showHitEffect == true && _hitEffect != null)
                {
                    if (_visualRoot != null)
                    {
                        _visualRoot.SetActive(false);
                    }

                    _hitEffect.SetActive(true);
                    Destroy(gameObject, _lifeTimeAfterHit);
                }
                else
                {
                    // No hit effect, destroy immediately
                    Destroy(gameObject);
                }
            }
        }
    }
}
