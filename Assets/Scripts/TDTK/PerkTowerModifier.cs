namespace TDTK
{
	public class PerkTowerModifier
	{
		public int prefabID = -1;

		public float HP;

		public float HPRegen;

		public float HPStagger;

		public float shield;

		public float shieldRegen;

		public float shieldStagger;

		public float buildCost;

		public float upgradeCost;

		public UnitStat stats = new UnitStat();

		public PerkTowerModifier()
		{
			stats.damageMin = 0f;
			stats.cooldown = 0f;
			stats.clipSize = 0f;
			stats.reloadDuration = 0f;
			stats.range = 0f;
			stats.aoeRadius = 0f;
			stats.hit = 0f;
			stats.crit.chance = 0f;
			stats.crit.dmgMultiplier = 0f;
		}
	}
}
