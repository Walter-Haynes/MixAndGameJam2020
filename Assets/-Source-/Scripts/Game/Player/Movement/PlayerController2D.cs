using System;
using UnityEngine;

using JetBrains.Annotations;

namespace Scripts.Game.Player.Movement
{
    using Utilities;
    
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider;

        [PublicAPI]
        public PlayerInputs InputActions { get; private set; }
        
        [PublicAPI]
        public PlayerInputs.GameplayActions Inputs { get; private set; }
        
        [PublicAPI]
        public bool IsGrounded { get; private set; }

        private Vector2 _velocity;

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
            bool __hasX = (x == null), __hasY = (y == null);
            
            if     (__hasX && !__hasY)
            {
                _velocity.x = (float)x;
                
                //Move(velocity: new Vector2(x: (float)x, y: 0));
            }
            else if(!__hasX && __hasY)
            {
                _velocity.y = (float)y;
                //Move(velocity: new Vector2(x: 0, y: (float)y));
            }
            else if (__hasX && __hasY)
            {
                _velocity.x = (float)x;
                _velocity.y = (float)y;
                
                //Move(velocity: new Vector2(x: (float)x, y: (float)y)); 
            }
        }
        
        private void FixedUpdate()
        {
            /*
            if(IsGrounded)
            {
                _velocity.y = 0;
            }
            */
            _velocity.y += Physics2D.gravity.y * Time.deltaTime;

            transform.Translate(translation: _velocity * Time.deltaTime);

            ResolveCollisions();
        }

        private readonly Collider2D[] _overlappingColliders = new Collider2D[16];
        private void ResolveCollisions()
        {
            IsGrounded = false;

            // Retrieve all colliders we have intersected after velocity has been applied.
            int __overlappingCollidersCount = Physics2D.OverlapBoxNonAlloc(point: transform.position, size: boxCollider.size, angle: 0, results: _overlappingColliders);

            for(uint __colliderIndex = 0; __colliderIndex < __overlappingCollidersCount; __colliderIndex++)
            {
                Collider2D __overlappingCollider = _overlappingColliders[__colliderIndex];
            
                if (__overlappingCollider == boxCollider) continue;
            
                ColliderDistance2D __colliderDistance = __overlappingCollider.Distance(boxCollider);

                // Skip if we are no longer overlapping with this collider. (could be pushed out already)
                if (!__colliderDistance.isOverlapped) continue;
            
                transform.Translate(translation: __colliderDistance.pointA - __colliderDistance.pointB);

                bool __colliderBeneathUs = (__colliderDistance.normal.AngleTo(Vector2.up) < 90);
                bool __notJumping = (_velocity.y < 0);
            
                if(__colliderBeneathUs && __notJumping)
                {
                    IsGrounded = true;
                }
            }
        }
        
        #endregion
        
    }
}
