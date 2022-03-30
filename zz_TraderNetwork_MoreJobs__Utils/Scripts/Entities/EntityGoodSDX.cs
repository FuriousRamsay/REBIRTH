using System;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public abstract class EntityGoodSDX : EntityAlive
{
	// Token: 0x060020F1 RID: 8433 RVA: 0x000DED3B File Offset: 0x000DCF3B
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		this.emodel.SetVisible(false, false);
	}

	// Token: 0x060020F2 RID: 8434 RVA: 0x000DED51 File Offset: 0x000DCF51
	public override void InitFromPrefab(int _entityClass)
	{
		base.InitFromPrefab(_entityClass);
		this.emodel.SetVisible(false, false);
	}

	// Token: 0x060020F3 RID: 8435 RVA: 0x000DED68 File Offset: 0x000DCF68
	public override void VisiblityCheck(float _distanceSqr, bool _masterIsZooming)
	{
		bool bVisible = this.ticksUntilVisible <= 0 && _distanceSqr < (float)(_masterIsZooming ? 14400 : 8100);
		this.emodel.SetVisible(bVisible, false);
	}

	// Token: 0x060020F4 RID: 8436 RVA: 0x000DEDA2 File Offset: 0x000DCFA2
	public override void PostInit()
	{
		base.PostInit();
		if (!this.isEntityRemote)
		{
			this.IsBloodMoon = this.world.aiDirector.BloodMoonComponent.BloodMoonActive;
		}
	}

	// Token: 0x060020F5 RID: 8437 RVA: 0x000DEDCD File Offset: 0x000DCFCD
	public override bool IsDrawMapIcon()
	{
		return true;
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x000DEDD0 File Offset: 0x000DCFD0
	public override Vector3 GetMapIconScale()
	{
		return new Vector3(0.75f, 0.75f, 1f);
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x000DEDE6 File Offset: 0x000DCFE6
	public override bool IsSavedToFile()
	{
		return (base.GetSpawnerSource() != EnumSpawnerSource.Dynamic || this.IsDead()) && base.IsSavedToFile();
	}

	// Token: 0x060020F8 RID: 8440 RVA: 0x000DEE01 File Offset: 0x000DD001
	protected override bool canDespawn()
	{
		return (!this.IsHordeZombie || this.world.GetPlayers().Count == 0) && base.canDespawn();
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x000DEE25 File Offset: 0x000DD025
	public override void OnUpdateLive()
	{
		base.OnUpdateLive();
		if (this.ticksUntilVisible > 0)
		{
			this.ticksUntilVisible--;
		}
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x000DEE44 File Offset: 0x000DD044
	protected override bool isRadiationSensitive()
	{
		return false;
	}

	// Token: 0x060020FB RID: 8443 RVA: 0x000DEE47 File Offset: 0x000DD047
	protected override bool isDetailedHeadBodyColliders()
	{
		return true;
	}

	// Token: 0x060020FC RID: 8444 RVA: 0x000DEE4A File Offset: 0x000DD04A
	protected override void OnEntityTargeted(EntityAlive target)
	{
		base.OnEntityTargeted(target);
		if (!this.isEntityRemote && base.GetSpawnerSource() != EnumSpawnerSource.Dynamic && target is EntityPlayer)
		{
			this.world.aiDirector.NotifyIntentToAttack(this, target as EntityPlayer);
		}
	}

	// Token: 0x060020FD RID: 8445 RVA: 0x000DEE83 File Offset: 0x000DD083
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale)
	{
		return base.DamageEntity(_damageSource, _strength, _criticalHit, _impulseScale);
	}

	// Token: 0x060020FE RID: 8446 RVA: 0x000DEE90 File Offset: 0x000DD090
	protected override bool isGameMessageOnDeath()
	{
		return false;
	}

	// Token: 0x04001525 RID: 5413
	public bool IsHordeZombie;

	// Token: 0x04001526 RID: 5414
	private int ticksUntilVisible = 2;
}