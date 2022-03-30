/*
 * Class: EntityZombieSDX
 * Author:  FuriousRamsay 
 * Category: Entity
 * Description:
 *      This mod is an extension of entityZombie.
 * 
 * Usage:
 *      Add the following class to entities. 
 *
 *      <property name="Class" value="EntityZombieSDX, SCore" />
 */

using Random = System.Random;
using System;
using UnityEngine;

public class EntityZombieSDX : EntityZombie
{

    public override bool CanDamageEntity(int _sourceEntityId)
    {
        return true;
    }

	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale = 1f)
	{

		if (_damageSource.GetDamageType() == EnumDamageTypes.Falling)
		{
			_strength = (_strength + 1) / 2;
			int num = (this.GetMaxHealth() + 2) / 3;
			if (_strength > num)
			{
				_strength = num;
			}
		}

		EnumDamageSource source = _damageSource.GetSource();
		if (_damageSource.IsIgnoreConsecutiveDamages() && source != EnumDamageSource.Internal)
		{
			if (this.damageSourceTimeouts.ContainsKey(source) && GameTimer.Instance.ticks - this.damageSourceTimeouts[source] < 30UL)
			{
				return 0;
			}
			this.damageSourceTimeouts[source] = GameTimer.Instance.ticks;
		}
		EntityAlive entityAlive = this.world.GetEntity(_damageSource.getEntityId()) as EntityAlive;
		if (!this.FriendlyFireCheck(entityAlive))
		{
			return 0;
		}
		bool flag = _damageSource.GetDamageType() == EnumDamageTypes.Heat;
		if (this.IsGodMode.Value)
		{
			return 0;
		}
		if (!this.IsDead() && entityAlive)
		{
			float value = EffectManager.GetValue(PassiveEffects.DamageBonus, null, 0f, entityAlive, null, default(FastTags), true, true, true, true, 1, true);
			if (value > 0f)
			{
				_damageSource.DamageMultiplier = value;
				_damageSource.BonusDamageType = EnumDamageBonusType.Sneak;
			}
		}
		DamageResponse damageResponse = this.damageEntityLocal(_damageSource, _strength, _criticalHit, _impulseScale);
		NetPackage package = NetPackageManager.GetPackage<NetPackageDamageEntity>().Setup(this.entityId, damageResponse);
		if (this.world.IsRemote())
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
		}
		else
		{
			int excludePlayer = -1;
			if (!flag && _damageSource.CreatorEntityId != -2)
			{
				excludePlayer = _damageSource.getEntityId();
				if (_damageSource.CreatorEntityId != -1)
				{
					Entity entity = this.world.GetEntity(_damageSource.CreatorEntityId);
					if (entity && !entity.isEntityRemote)
					{
						excludePlayer = -1;
					}
				}
			}
			this.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entityId, excludePlayer, package);
		}
		return damageResponse.ModStrength;
	}

}