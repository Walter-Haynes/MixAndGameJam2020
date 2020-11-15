using UnityEngine;

namespace Scripts.Game.Player
{
	public sealed class PlayerHealth : MonoBehaviour, IDamageable
	{
		private int _lives;
		public int Lives 
		{ 
			get => _lives;
			set
			{
				_lives = value;
				
				if (_lives <= 0)
				{
					Kill();	
				}
			}
		}

		public void Kill()
		{
			
		}
	}
}
