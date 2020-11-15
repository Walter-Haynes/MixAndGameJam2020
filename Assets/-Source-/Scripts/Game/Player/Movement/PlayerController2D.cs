using System.Collections.Generic;
using UnityEngine;

using JetBrains.Annotations;

namespace Scripts.Game.Player.Movement
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class PlayerController2D : MonoBehaviour
    {
        #region Fields

        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] internal Transform visuals;

        [SerializeField] private LayerMask groundLayer = 1 << 0;

        [SerializeField] private float skinWidth = 0.01f;
        private const float MIN_MOVE_DISTANCE = 0.001f;

        private Rigidbody2D _rigidbody2D;
        
        private Vector2 _movement;

        #endregion

        #region Properties

        #region Inputs
        
        [PublicAPI]
        public PlayerInputs InputActions { get; private set; }
        
        [PublicAPI]
        public PlayerInputs.GameplayActions Inputs { get; private set; }

        #endregion

        private IEnumerable<PlayerAbility> Abilities => GetComponents<PlayerAbility>();
        
        [PublicAPI]
        public bool IsGrounded 
        {
            get
            {
                Bounds __playerBounds = boxCollider.bounds;
                Vector3 __offset = new Vector3(x: 0, y: __playerBounds.size.y / 2.0f, z: 0) * (HasNormalGravity ? -1 : 1);

                Vector3 __groundCheckPos = __playerBounds.center + __offset;
            
                return Physics2D.OverlapCircle(point: __groundCheckPos, radius: 0.3f, layerMask: groundLayer);
            }
        }

        [field: SerializeField]
        [PublicAPI]
        public Vector2 Gravity { get; internal set; } = new Vector2(0, -50);

        [PublicAPI]
        public bool HasNormalGravity => (Gravity.y < 0);

        ///// <summary> Player is not moving up (So it's either standing still, or falling) </summary>
        //private bool NotJumping => HasNormalGravity ? (_movement.y < 0) : (_movement.y > 0);

        #endregion

        #region Methods

        private void Awake()
        {
            if (boxCollider == null)
            {
                if(TryGetComponent(component: out BoxCollider2D __result))
                {
                    boxCollider = __result;
                }   
            }

            if (_rigidbody2D == null)
            {
                if(TryGetComponent(component: out Rigidbody2D __result))
                {
                    _rigidbody2D = __result;
                }   
            }
            
            InputActions = new PlayerInputs();
            Inputs = InputActions.Gameplay;
        }

        private void Reset()
        {
            if(TryGetComponent(component: out BoxCollider2D __boxCollider2D))
            {
                boxCollider = __boxCollider2D;
            }

            visuals = transform.GetChild(0).transform;
            
            _rigidbody2D = GetComponent<Rigidbody2D>();

            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody2D.useFullKinematicContacts = true;
            _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rigidbody2D.sleepMode = RigidbodySleepMode2D.NeverSleep;
            _rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void OnEnable() => InputActions.Enable();
        private void OnDisable() => InputActions.Disable();

        [PublicAPI]
        public void Move(in Vector2 velocity)
        {
            _movement = velocity;
        }
        [PublicAPI]
        public void Move(in float? x = null, in float? y = null)
        {
            bool __hasX = (x != null); 
            bool __hasY = (y != null);
            
            if     (__hasX && !__hasY)
            {
                _movement.x = (float)x;
            }
            else if(!__hasX && __hasY)
            {
                _movement.y = (float)y;
            }
            if (__hasX && __hasY)
            {
                Move(velocity: new Vector2(x: (float)x, y: (float)y)); 
            }
        }
        
        private void FixedUpdate()
        {
            //visuals.GetComponent<SpriteRenderer>().color = IsGrounded ? Color.green : Color.yellow; 

            foreach (PlayerAbility __ability in Abilities)
            {
                __ability.AbilityFixedUpdate();
            }
            
            if (!IsGrounded)
            {
                _movement.y += Gravity.y * Time.deltaTime;   
            }
            
            Translate(velocity: _movement * Time.deltaTime);
        }

        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[32];
        private void Translate(in Vector2 velocity)
        {
            float __distance = velocity.magnitude;

            if (__distance > MIN_MOVE_DISTANCE)
            {
                int __hits = _rigidbody2D.Cast(direction: velocity.normalized, results: _hitBuffer, distance: __distance + skinWidth);

                for (int __index = 0; __index < __hits; __index++)
                {
                    RaycastHit2D __hit = _hitBuffer[__index];
                    Debug.DrawRay(__hit.point, __hit.normal);

                    float __modifiedDistance = __hit.distance - skinWidth;
                    __distance = (__modifiedDistance < __distance) ? __modifiedDistance : __distance;
                }
            }

            _rigidbody2D.position += velocity.normalized * __distance;

        }

        private void OnDrawGizmos()
        {
            Bounds __colliderBounds = boxCollider.bounds;
            Vector3 __offset = new Vector3(x: 0, y: __colliderBounds.size.y / 2.0f, z: 0) * (HasNormalGravity ? -1 : 1);

            Vector3 __groundCheckPos = __colliderBounds.center + __offset;

            Gizmos.DrawSphere(center: __groundCheckPos, radius: 0.3f);
        }

        #endregion
        
    }
}
