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
        #region Fields

        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] internal Transform visuals;

        [SerializeField] private LayerMask groundLayer = 1 << 0;
        [SerializeField] private float slopeLimit = 30f;

        [SerializeField] private float skinWidth = 0.01f;
        private const float MIN_MOVE_DISTANCE = 0.001f;

        private Rigidbody2D _rigidbody2D;
        
        [SerializeField]
        private Vector2 _movement;

        // Misc
        [HideInInspector][NonSerialized] 
        private ContactFilter2D _contactFilter;

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

        //[PublicAPI]
        //public bool IsGrounded { get; internal set; } = false;

        [PublicAPI]
        public bool HitsCeiling { get; private set; }
        
        [field: SerializeField]
        [PublicAPI]
        public Vector2 Gravity { get; internal set; } = new Vector2(0, -50);

        [PublicAPI]
        public bool HasNormalGravity => (Gravity.y < 0);

        /// <summary> Player is not moving up (So it's either standing still, or falling) </summary>
        private bool NotJumping => HasNormalGravity ? (_movement.y < 0) : (_movement.y > 0);
        
        //private Vector2 MaxSlopeNormal => Quaternion.AngleAxis(slopeLimit, transform.forward) * transform.up;
        
        //public Vector2 GroundNormal { get; private set; }

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

            _contactFilter.useTriggers = false;
            _contactFilter.SetLayerMask(~groundLayer);
            _contactFilter.useLayerMask = true;
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

            //blockerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
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
            if(IsGrounded) //&& NotJumping
            {
                _movement.y = 0;
            }

            visuals.GetComponent<SpriteRenderer>().color = IsGrounded ? Color.green : Color.yellow; 

            foreach (PlayerAbility __ability in Abilities)
            {
                __ability.AbilityFixedUpdate();
            }
            
            if (!IsGrounded)
            {
                if (HasNormalGravity) //I don't get why I have to do this. 10 + -40 = -30, right?!
                {
                    _movement.y += Gravity.y * Time.deltaTime;   
                }
                else
                {
                    _movement.y -= Gravity.y * Time.deltaTime;   
                }
            }

            Vector2 __movementX = Vector2.Scale(_movement, Vector2.right);
            Vector2 __movementY = Vector2.Scale(_movement, transform.up);

            //_rigidbody2D.velocity = new Vector2(__movementX.x, __movementY.y);

            //IsGrounded = false;
            HitsCeiling = false;
            MoveWithResolve(velocity: new Vector2(__movementX.x, __movementY.y) * Time.deltaTime);
            
            //ResolveCollisions();
        }

        private readonly Collider2D[] _overlappingColliders = new Collider2D[16];
        private void ResolveCollisions()
        {
            int __overlappingCollidersCount = Physics2D.OverlapBoxNonAlloc(point: transform.position, size: boxCollider.size, angle: 0, results: _overlappingColliders);

            for(uint __colliderIndex = 0; __colliderIndex < __overlappingCollidersCount; __colliderIndex++)
            {
                Collider2D __overlappingCollider = _overlappingColliders[__colliderIndex];
            
                if (__overlappingCollider == boxCollider) continue;
            
                ColliderDistance2D __colliderDistance = __overlappingCollider.Distance(boxCollider);

                // Skip if we are no longer overlapping with this collider. (could be pushed out already)
                if (!__colliderDistance.isOverlapped) continue;
                
                transform.Translate(translation: __colliderDistance.pointA - __colliderDistance.pointB);
            }
        }
        
        /*
        private void Translate(in Vector2 velocity) 
        {
            float __distance = velocity.magnitude;

            if(__distance > MIN_MOVE_DISTANCE)
            {
                int __count = _rigidbody2D.Cast(velocity.normalized, _contactFilter, _hitBuffer,
                    distance: __distance + skinWidth);

                for(int __index = 0; __index < __count; __index++)
                {
                    Vector2 __hitNormal = _hitBuffer[__index].normal;

                    Debug.Log(MaxSlopeNormal);
                    
                    if(__hitNormal.y >= MaxSlopeNormal.y)
                    {
                        IsGrounded = true;
                        GroundNormal = __hitNormal;
                    }
                    else if(__hitNormal.y <= -MaxSlopeNormal.y)
                    {
                        HitsCeiling = true;
                    }

                    float __modifiedDistance = _hitBuffer[__index].distance - skinWidth;
                    __distance = (__modifiedDistance < __distance) ? __modifiedDistance : __distance;
                }
            }

            _rigidbody2D.position += velocity.normalized * __distance;
        }
        */

        //private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
        
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[32];
        private void Translate(in Vector2 velocity)
        {
            //Vector2 __velocity = _rigidbody2D.velocity;
            float __distance = velocity.magnitude;

            if (__distance > MIN_MOVE_DISTANCE)
            {
                int __hits = _rigidbody2D.Cast(direction: velocity.normalized, results: _hitBuffer, distance: __distance + skinWidth);

                for (int __index = 0; __index < __hits; __index++)
                {
                    RaycastHit2D __hit = _hitBuffer[__index];

                    //Vector2 __hitNormal = __hit.normal;
                    //float __hitAngle = __hitNormal.NormalToAngle();

                    Debug.DrawRay(__hit.point, __hit.normal);

                    //if (__hitAngle <= slopeLimit) //ground

                    float __modifiedDistance = __hit.distance - skinWidth;
                    __distance = (__modifiedDistance < __distance) ? __modifiedDistance : __distance;
                }
            }

            _rigidbody2D.position += velocity.normalized * __distance;

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
            
            Debug.DrawRay(start: __playerBounds.center, dir: -transform.up * (__playerBounds.size.y + 0.01f), Color.magenta);

            if (__hit.collider != null)
            {
                Vector3 __point = __hit.point;
                Vector3 __vertical   = __point + new Vector3(0, -0.1f);
                Vector3 __horizontal = __point + new Vector3(-0.1f, 0);
                
                Debug.DrawRay(start: __vertical,   dir: transform.right * 0.2f, Color.red, duration: 10);
                Debug.DrawRay(start: __horizontal, dir: transform.up * 0.2f,    Color.red, duration: 10);
            }

            return (__hit.collider != null);
        }

        private bool CheckGroundingCircle()
        {
            Bounds __playerBounds = boxCollider.bounds;
            Vector3 __offset = new Vector3(x: 0, y: __playerBounds.size.y / 2.0f, z: 0) * (HasNormalGravity ? -1 : 1);

            Vector3 __groundCheckPos = __playerBounds.center + __offset;
            
            return Physics2D.OverlapCircle(point: __groundCheckPos, radius: 0.3f, layerMask: groundLayer);
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
