/*
 * Class: EntityBanditSDX
 * Author:  FuriousRamsay 
 * Category: Entity
 * Description:
 *      This mod is an extension of the base entityAlive. This is meant to be a base class, where other classes can extend
 *      from, giving them the ability to accept quests and buffs.
 * 
 * Usage:
 *      Add the following class to entities that are meant to use these features. 
 *
 *      <property name="Class" value="EntityBanditSDX, SCore" />
 */

using Random = System.Random;

public class EntityMeleeBanditSDX : EntityBandit
{
    public float flEyeHeight = -1f;
    public Random random = new Random();
    public ulong timeToDie;

	public override void OnUpdateLive()
    {
        base.OnUpdateLive();
		if (this.ticksUntilVisible > 0)
		{
			this.ticksUntilVisible--;
		}
		if (!this.isEntityRemote)
        {
            if (!this.IsDead() && this.world.worldTime >= this.timeToDie && !this.attackTarget)
            {
                this.Kill(DamageResponse.New(true));
            }
        }
    }
    public override bool CanDamageEntity(int _sourceEntityId)
    {
        return true;
    }

    public override void OnAddedToWorld()
    {
        base.OnAddedToWorld();
        this.timeToDie = this.world.worldTime + 1800UL + (ulong)(22000f * this.rand.RandomFloat);
        if (this.IsFeral && base.GetSpawnerSource() == EnumSpawnerSource.Biome)
        {
            int num = (int)SkyManager.GetDawnTime();
            int num2 = (int)SkyManager.GetDuskTime();
            int num3 = GameUtils.WorldTimeToHours(this.WorldTimeBorn);
            if (num3 < num || num3 >= num2)
            {
                int num4 = GameUtils.WorldTimeToDays(this.world.worldTime);
                if (GameUtils.WorldTimeToHours(this.world.worldTime) >= num2)
                {
                    num4++;
                }
                this.timeToDie = GameUtils.DayTimeToWorldTime(num4, num, 0);
            }
        }
    }
    public override bool IsSavedToFile()
    {
        // If they have a cvar persist, keep them around.
        if (Buffs.HasCustomVar("Persist")) return true;

        // If its dynamic spawn, don't let them stay.
        if (GetSpawnerSource() == EnumSpawnerSource.Dynamic) return false;

        // If its biome spawn, don't let them stay.
        //if (GetSpawnerSource() == EnumSpawnerSource.Biome) return false;
        return true;
    }

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

	private int ticksUntilVisible = 2;
}