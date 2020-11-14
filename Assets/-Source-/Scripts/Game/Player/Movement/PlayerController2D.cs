using System;
using System.Collections.Generic;
using UnityEngine;

using JetBrains.Annotations;
using Sirenix.OdinInspector;

namespace Scripts.Game.Player.Movement
{
    using Utilities;
    
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider;

        [SerializeField] private LayerMask groundLayer;

        [SerializeField] internal Transform visuals;

        #region Properties

        [PublicAPI]
        public PlayerInputs InputActions { get; private set; }
        
        [PublicAPI]
        public PlayerInputs.GameplayActions Inputs { get; private set; }
        
        private bool _isGrounded = false;
        [PublicAPI]
        public bool IsGrounded 
        {
            //If we're not grounded do an extra check, just in case.
            //get => _isGrounded = (_isGrounded || ExplicitlyGrounding());
            get
            {
                if (_isGrounded) return true;

                return (_isGrounded = CheckGrounding());
            }
            internal set => _isGrounded = value;
        }
        
        [PublicAPI]
        public Vector2 Gravity { get; internal set; } = new Vector2(0, -50);

        [PublicAPI]
        public bool HasNormalGravity => (Gravity.y < 0);

        /// <summary> Player is not moving up (So it's either standing still, or falling) </summary>
        private bool NotJumping => HasNormalGravity ? (_velocity.y < 0) : (_velocity.y > 0);

        #endregion
        

        private Vector2 _velocity;

        private IEnumerable<PlayerAbility> Abilities => GetComponents<PlayerAbility>();

        #region Methods

        private void Awake()
        {
            if (boxCollider != null) return;
        
            if(TryGetComponent(component: out BoxCollider2D __boxCollider2D))
            {
                boxCollider = __boxCollider2D;
            }
        }

        private void Reset()
        {
            if(TryGetComponent(component: out BoxCollider2D __boxCollider2D))
            {
                boxCollider = __boxCollider2D;
            }
        }

        private void OnEnable()
        {
            InputActions = new PlayerInputs();
            Inputs = InputActions.Gameplay;

            InputActions.Enable();
        }
        private void OnDisable()
        {
            InputActions.Disable();
        }

        [PublicAPI]
        public void Move(in Vector2 velocity)
        {
            _velocity = velocity;
        }
        [PublicAPI]
        public void Move(in float? x = null, in float? y = null)
        {
            bool __hasX = (x != null); 
            bool __hasY = (y != null);
            
            if     (__hasX && !__hasY)
            {
                _velocity.x = (float)x;
            }
            else if(!__hasX && __hasY)
            {
                _velocity.y = (float)y;
            }
            if (__hasX && __hasY)
            {
                Move(velocity: new Vector2(x: (float)x, y: (float)y)); 
            }
        }
        
        private void FixedUpdate()
        {
            if(IsGrounded && NotJumping)
            {
                _velocity.y = 0;
            }

            foreach (PlayerAbility __ability in Abilities)
            {
                __ability.AbilityFixedUpdate();
            }
            
            if (!IsGrounded)
            {
                Debug.Log(Gravity);
                _velocity.y += Gravity.y * Time.deltaTime;
            }

            transform.Translate(translation: _velocity * Time.deltaTime);

            ResolveCollisions();
        }

        private readonly Collider2D[] _overlappingColliders = new Collider2D[16];
        private void ResolveCollisions()
        {
            IsGrounded = false;
            
            int __overlappingCollidersCount = Physics2D.OverlapBoxNonAlloc(point: transform.position, size: boxCollider.size, angle: 0, results: _overlappingColliders);

            for(uint __colliderIndex = 0; __colliderIndex < __overlappingCollidersCount; __colliderIndex++)
            {
                Collider2D __overlappingCollider = _overlappingColliders[__colliderIndex];
            
                if (__overlappingCollider == boxCollider) continue;
            
                ColliderDistance2D __colliderDistance = __overlappingCollider.Distance(boxCollider);

                // Skip if we are no longer overlapping with this collider. (could be pushed out already)
                if (!__colliderDistance.isOverlapped) continue;

                Transform __transform;
                (__transform = transform).Translate(translation: __colliderDistance.pointA - __colliderDistance.pointB);

                bool __colliderBeneathUs = (__colliderDistance.normal.AngleTo(__transform.up) < 90);

                if(__colliderBeneathUs && NotJumping)
                {
                    Debug.Log("Doot");
                    IsGrounded = true;
                }
            }
        }

        private bool CheckGrounding()
        {
            Bounds __playerBounds = boxCollider.bounds;
            
            RaycastHit2D __hit = Physics2D.BoxCast(
                origin: __playerBounds.center, 
                size: __playerBounds.size, 
                angle: 0, 
                direction: -transform.up, 
                distance: (0.01f), 
                layerMask: groundLayer);

            return (__hit.collider != null);
        }
        
        #endregion
        
    }
}
