using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Game.Player
{
	public sealed class PlayerHealth : MonoBehaviour, IDamageable
	{
		[ReadOnly]
		[SerializeField]
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
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
