using System.Collections;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class CharacterController2D : MonoBehaviour 
{
	// Properties
	[SerializeField] private float skinWidth = 0.01f;
	[SerializeField] private float minMoveDistance = 0.001f;

	[SerializeField] private  float slopeLimit = 30f;

	[SerializeField] private LayerMask blockerMask;
	[SerializeField] private string oneWayPlatformTag = "One Way Platform";
	
	[SerializeField] private bool ignoreOneWayPlatforms = false;
	[SerializeField] private bool ignoreOneWayPlatformsOnce = false;
	
	[PublicAPI]
	public Vector2 GroundNormal { get; private set;}
	private Vector2 _maxSlopeNormal;

	// Flags
	[PublicAPI]
	public bool isGrounded;
	[PublicAPI]
	public bool isOnCeiling;
	[PublicAPI]
	public bool isOnOneWayPlatform; 

	// References
	private Rigidbody2D _rigidbody2D;

	// Misc
	[HideInInspector][NonSerialized]
	public ContactFilter2D contactFilter;

	private void Awake() 
	{
		if(_rigidbody2D == null)
		{
			_rigidbody2D = GetComponent<Rigidbody2D>();	
		}

		contactFilter.useTriggers = false;
		contactFilter.SetLayerMask(blockerMask);
		contactFilter.useLayerMask = true;
		
		_maxSlopeNormal = Quaternion.AngleAxis(slopeLimit, Vector3.forward) * Vector3.up;
	}

	private void Reset()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();

		_rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
		_rigidbody2D.useFullKinematicContacts = true;
		_rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		_rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
		_rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

		blockerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
	}

	// Should be called in FixedUpdate.
	[PublicAPI]
	public void Move(in Vector2 movement, in bool slideAlongGroundNormal = false) 
	{
		isGrounded = false;
		isOnCeiling = false;
		isOnOneWayPlatform = false;
		
		Vector2 __movementX = Vector2.Scale(movement, Vector2.right);
		Vector2 __movementY = Vector2.Scale(movement, transform.up);
		
		if(slideAlongGroundNormal)
		{
			__movementX = new Vector2(GroundNormal.y, -GroundNormal.x) * movement.x;
		}
		
		DoMovement(__movementX);	
		DoMovement(__movementY);
	}
	
	
	private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
	
	private void DoMovement(in Vector2 velocity) 
	{
		float __distance = velocity.magnitude;
		
		if (__distance > minMoveDistance) 
		{
			int __count = _rigidbody2D.Cast(velocity.normalized, contactFilter, _hitBuffer, distance: __distance + skinWidth);
	
			for(int __index = 0; __index < __count; __index++) 
			{
				bool __isOneWayPlatform = _hitBuffer[__index].transform.CompareTag(oneWayPlatformTag);
				
				Vector2 __currentNormal = _hitBuffer[__index].normal;
				
				if(!__isOneWayPlatform)
				{
					if(__currentNormal.y >= _maxSlopeNormal.y) 
					{
						isGrounded = true;
						GroundNormal = __currentNormal;
					}
					else if(__currentNormal.y <= -_maxSlopeNormal.y) 
					{
						isOnCeiling = true;
					}

					float __modifiedDistance = _hitBuffer[__index].distance - skinWidth;
					__distance = (__modifiedDistance < __distance) ? __modifiedDistance : __distance;
					
					continue;
				}

				if(ignoreOneWayPlatforms || ignoreOneWayPlatformsOnce)
				{
					ignoreOneWayPlatformsOnce = false;
					continue;
				}
					
				if(__currentNormal.y >= _maxSlopeNormal.y && velocity.y <= 0 && _hitBuffer[0].distance > 0) 
				{
					isGrounded = true;
					GroundNormal = __currentNormal;
				}
					
				if(isGrounded && velocity.x == 0)
				{
					isOnOneWayPlatform = true;
					float __modifiedDistance = _hitBuffer[__index].distance - skinWidth;
					__distance = __modifiedDistance < __distance ? __modifiedDistance : __distance;
				}
			}
		}

		_rigidbody2D.position += velocity.normalized * __distance;
	}
}