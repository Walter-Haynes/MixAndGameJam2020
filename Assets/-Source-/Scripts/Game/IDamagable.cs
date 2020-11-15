namespace Scripts.Game
{
	public interface IDamageable
	{
		int Lives { get; set; }
		void Kill();
	}
}