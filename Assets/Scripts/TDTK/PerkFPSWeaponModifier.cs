namespace TDTK
{
	public class PerkFPSWeaponModifier
	{
		public int prefabID = -1;

		public UnitStat stats = new UnitStat();

		public PerkFPSWeaponModifier()
		{
			stats.damageMin = 0f;
			stats.cooldown = 0f;
			stats.clipSize = 0f;
			stats.reloadDuration = 0f;
			stats.aoeRadius = 0f;
			stats.aoeRadius = 0f;
			stats.crit.chance = 0f;
			stats.crit.dmgMultiplier = 0f;
		}
	}
}
