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
using System;
using UnityEngine;

public class EntityMeleeBanditSDX : EntityAlive
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

	protected override void Awake()
	{
		base.Awake();
		this.MaxLedgeHeight = 20;
	}

	// Token: 0x0600238A RID: 9098 RVA: 0x000F5688 File Offset: 0x000F3888
	/*public override void OnUpdateLive()
	{
		base.OnUpdateLive();
		if (this.moveSpeedRagePer > 0f && this.bodyDamage.CurrentStun == EnumEntityStunType.None)
		{
			this.moveSpeedScaleTime -= 0.05f;
			if (this.moveSpeedScaleTime <= 0f)
			{
				this.StopRage();
			}
		}
		if (!this.isEntityRemote)
		{
			if (!this.IsDead() && this.world.worldTime >= this.timeToDie && !this.attackTarget)
			{
				this.Kill(DamageResponse.New(true));
			}
			if (this.emodel)
			{
				AvatarController avatarController = this.emodel.avatarController;
				if (avatarController)
				{
					bool flag = this.onGround || this.isSwimming || this.bInElevator;
					if (flag)
					{
						this.fallTime = 0f;
						this.fallThresholdTime = 0f;
						if (this.bInElevator)
						{
							this.fallThresholdTime = 0.6f;
						}
					}
					else
					{
						if (this.fallThresholdTime == 0f)
						{
							this.fallThresholdTime = 0.1f + this.rand.RandomFloat * 0.3f;
						}
						this.fallTime += 0.05f;
					}
					bool canFall = !this.emodel.IsRagdollActive && this.bodyDamage.CurrentStun == EnumEntityStunType.None && !this.isSwimming && !this.bInElevator && this.jumpState == EntityAlive.JumpState.Off && !this.IsDead();
					if (this.fallTime <= this.fallThresholdTime)
					{
						canFall = false;
					}
					avatarController.SetFallAndGround(canFall, flag);
				}
			}
		}
	}*/

	// Token: 0x0600238B RID: 9099 RVA: 0x000F581C File Offset: 0x000F3A1C
	/*public override Ray GetLookRay()
	{
		Ray result;
		if (base.IsBreakingBlocks)
		{
			result..ctor(this.position + new Vector3(0f, this.GetEyeHeight(), 0f), this.GetLookVector());
		}
		else if (base.GetWalkType() == 8)
		{
			result..ctor(this.getHeadPosition(), this.GetLookVector());
		}
		else
		{
			result..ctor(this.getHeadPosition(), this.GetLookVector());
		}
		return result;
	}*/

	// Token: 0x0600238C RID: 9100 RVA: 0x000F5892 File Offset: 0x000F3A92
	public override Vector3 GetLookVector()
	{
		if (this.lookAtPosition.Equals(Vector3.zero))
		{
			return base.GetLookVector();
		}
		return this.lookAtPosition - this.getHeadPosition();
	}

	// Token: 0x170004C2 RID: 1218
	// (get) Token: 0x0600238D RID: 9101 RVA: 0x000F58C0 File Offset: 0x000F3AC0
	public override bool IsRunning
	{
		get
		{
			EnumGamePrefs eProperty = EnumGamePrefs.ZombieMove;
			if (this.IsBloodMoon)
			{
				eProperty = EnumGamePrefs.ZombieBMMove;
			}
			else if (this.IsFeral)
			{
				eProperty = EnumGamePrefs.ZombieFeralMove;
			}
			else if (this.world.IsDark())
			{
				eProperty = EnumGamePrefs.ZombieMoveNight;
			}
			return GamePrefs.GetInt(eProperty) >= 2;
		}
	}

	// Token: 0x0600238E RID: 9102 RVA: 0x000F5908 File Offset: 0x000F3B08
	public override float GetMoveSpeedAggro()
	{
		EnumGamePrefs eProperty = EnumGamePrefs.ZombieMove;
		if (this.IsBloodMoon)
		{
			eProperty = EnumGamePrefs.ZombieBMMove;
		}
		else if (this.IsFeral)
		{
			eProperty = EnumGamePrefs.ZombieFeralMove;
		}
		else if (this.world.IsDark())
		{
			eProperty = EnumGamePrefs.ZombieMoveNight;
		}
		int @int = GamePrefs.GetInt(eProperty);
		float num = moveSpeeds[@int];
		if (this.moveSpeedRagePer > 1f)
		{
			num = moveSuperRageSpeeds[@int];
		}
		else if (this.moveSpeedRagePer > 0f)
		{
			float num2 = moveRageSpeeds[@int];
			num = num * (1f - this.moveSpeedRagePer) + num2 * this.moveSpeedRagePer;
		}
		if (num < 1f)
		{
			num = this.moveSpeedAggro * (1f - num) + this.moveSpeedAggroMax * num;
		}
		else
		{
			num = this.moveSpeedAggroMax * num;
		}
		return EffectManager.GetValue(PassiveEffects.RunSpeed, null, num, this, null, default(FastTags), true, true, true, true, 1, true);
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x000F59E0 File Offset: 0x000F3BE0
	protected override float getNextStepSoundDistance()
	{
		if (!this.IsRunning)
		{
			return 0.5f;
		}
		return 1.5f;
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x000F59F5 File Offset: 0x000F3BF5
	protected override void updateStepSound(float _distX, float _distZ)
	{
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x000F59F8 File Offset: 0x000F3BF8
	public override void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
		if (this.walkType == 4 && this.Jumping)
		{
			this.motion = this.accumulatedRootMotion;
			this.accumulatedRootMotion = Vector3.zero;
			this.IsRotateToGroundFlat = true;
			if (this.moveHelper != null)
			{
				Vector3 vector = this.moveHelper.JumpToPos - this.position;
				if (Utils.FastAbs(vector.y) < 0.2f)
				{
					this.motion.y = vector.y * 0.2f;
				}
				if (Utils.FastAbs(vector.x) < 0.3f)
				{
					this.motion.x = vector.x * 0.2f;
				}
				if (Utils.FastAbs(vector.z) < 0.3f)
				{
					this.motion.z = vector.z * 0.2f;
				}
				if (vector.sqrMagnitude < 0.010000001f)
				{
					if (this.emodel && this.emodel.avatarController)
					{
						this.emodel.avatarController.StartAnimationJump(AnimJumpMode.Land);
					}
					this.Jumping = false;
				}
			}
			this.entityCollision(this.motion);
			return;
		}
		base.MoveEntityHeaded(_direction, _isDirAbsolute);
	}

	// Token: 0x06002392 RID: 9106 RVA: 0x000F5B34 File Offset: 0x000F3D34
	protected override void UpdateJump()
	{
		if (this.walkType == 4 && !this.isSwimming)
		{
			base.FaceJumpTo();
			this.jumpState = EntityAlive.JumpState.Climb;
			if (!this.emodel.avatarController || !this.emodel.avatarController.IsAnimationJumpRunning())
			{
				this.Jumping = false;
			}
			if (this.jumpTicks == 0 && this.accumulatedRootMotion.y > 0.005f)
			{
				this.jumpTicks = 30;
			}
			return;
		}
		base.UpdateJump();
		if (this.isSwimming)
		{
			return;
		}
		this.accumulatedRootMotion.y = 0f;
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x000F5BCC File Offset: 0x000F3DCC
	protected override void EndJump()
	{
		base.EndJump();
		this.IsRotateToGroundFlat = false;
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x000F5BDC File Offset: 0x000F3DDC
	protected override bool ExecuteFallBehavior(EntityAlive.FallBehavior behavior, float _distance, Vector3 _fallMotion)
	{
		if (behavior == null || !this.emodel)
		{
			return false;
		}
		AvatarController avatarController = this.emodel.avatarController;
		if (!avatarController)
		{
			return false;
		}
		avatarController.SetInt("RandomSelector", this.rand.RandomRange(0, 64));
		switch (behavior.ResponseOp)
		{
			case EntityAlive.FallBehavior.Op.None:
				avatarController.SetInt(AvatarController.jumpLandResponseHash, -1);
				break;
			case EntityAlive.FallBehavior.Op.Land:
				avatarController.SetInt(AvatarController.jumpLandResponseHash, 0);
				break;
			case EntityAlive.FallBehavior.Op.LandLow:
				avatarController.SetInt(AvatarController.jumpLandResponseHash, 1);
				break;
			case EntityAlive.FallBehavior.Op.LandHard:
				avatarController.SetInt(AvatarController.jumpLandResponseHash, 2);
				break;
			case EntityAlive.FallBehavior.Op.Stumble:
				avatarController.SetInt(AvatarController.jumpLandResponseHash, 3);
				break;
			case EntityAlive.FallBehavior.Op.Ragdoll:
				this.emodel.DoRagdoll(this.rand.RandomFloat * 2f, EnumBodyPartHit.None, _fallMotion * 20f, Vector3.zero, false);
				break;
		}
		if (this.attackTarget != null && behavior.RagePer.IsSet() && behavior.RageTime.IsSet() && this.StartRage(behavior.RagePer.Random(this.rand), behavior.RageTime.Random(this.rand)))
		{
			avatarController.StartAnimationRaging();
		}
		return true;
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x000F5D2C File Offset: 0x000F3F2C
	protected override bool ExecuteDestroyBlockBehavior(EntityAlive.DestroyBlockBehavior behavior, ItemActionAttack.AttackHitInfo attackHitInfo)
	{
		if (behavior == null || attackHitInfo == null || this.moveHelper == null || this.emodel == null || this.emodel.avatarController == null)
		{
			return false;
		}
		if (this.walkType == 4)
		{
			return false;
		}
		this.moveHelper.ClearBlocked();
		this.moveHelper.ClearTempMove();
		this.emodel.avatarController.SetInt("RandomSelector", this.rand.RandomRange(0, 64));
		switch (behavior.ResponseOp)
		{
			case EntityAlive.DestroyBlockBehavior.Op.Ragdoll:
				this.emodel.avatarController.BeginStun(EnumEntityStunType.StumbleBreakThroughRagdoll, EnumBodyPartHit.LeftLowerLeg, Utils.EnumHitDirection.None, false, 1f);
				base.SetStun(EnumEntityStunType.StumbleBreakThroughRagdoll);
				break;
			case EntityAlive.DestroyBlockBehavior.Op.Stumble:
				this.emodel.avatarController.BeginStun(EnumEntityStunType.StumbleBreakThrough, EnumBodyPartHit.LeftLowerLeg, Utils.EnumHitDirection.None, false, 1f);
				base.SetStun(EnumEntityStunType.StumbleBreakThrough);
				this.bodyDamage.StunDuration = 1f;
				break;
		}
		if (this.attackTarget != null && behavior.RagePer.IsSet() && behavior.RageTime.IsSet())
		{
			this.StartRage(behavior.RagePer.Random(this.rand), behavior.RageTime.Random(this.rand));
		}
		return true;
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x000F5E7C File Offset: 0x000F407C
	/*public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale)
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
		return base.DamageEntity(_damageSource, _strength, _criticalHit, impulseScale);
	}*/

	// Token: 0x06002397 RID: 9111 RVA: 0x000F5EB8 File Offset: 0x000F40B8
	public override void ProcessDamageResponseLocal(DamageResponse _dmResponse)
	{
		base.ProcessDamageResponseLocal(_dmResponse);
		if (!this.isEntityRemote)
		{
			int @int = GameStats.GetInt(EnumGameStats.GameDifficulty);
			float num = (float)_dmResponse.Strength / 40f;
			if (num > 1f)
			{
				num = Mathf.Pow(num, 0.29f);
			}
			float num2 = rageChances[@int] * num;
			if (this.rand.RandomFloat < num2)
			{
				if (this.rand.RandomFloat < superRageChances[@int])
				{
					this.StartRage(2f, 30f);
					this.PlayOneShot(this.GetSoundAlert(), false);
					return;
				}
				this.StartRage(0.5f + this.rand.RandomFloat * 0.5f, 4f + this.rand.RandomFloat * 6f);
			}
		}
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x000F5F81 File Offset: 0x000F4181
	public bool StartRage(float speedPercent, float time)
	{
		if (speedPercent >= this.moveSpeedRagePer)
		{
			this.moveSpeedRagePer = speedPercent;
			this.moveSpeedScaleTime = time;
			return true;
		}
		return false;
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x000F5F9D File Offset: 0x000F419D
	public void StopRage()
	{
		this.moveSpeedRagePer = 0f;
		this.moveSpeedScaleTime = 0f;
	}

	// Token: 0x0600239A RID: 9114 RVA: 0x000F5FB5 File Offset: 0x000F41B5
	public override void OnEntityDeath()
	{
		base.OnEntityDeath();
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x000F5FC0 File Offset: 0x000F41C0
	protected override Vector3i dropCorpseBlock()
	{
		if (this.lootContainer != null && this.lootContainer.IsUserAccessing())
		{
			return Vector3i.zero;
		}
		Vector3i vector3i = base.dropCorpseBlock();
		if (vector3i == Vector3i.zero)
		{
			return Vector3i.zero;
		}
		TileEntityLootContainer tileEntityLootContainer = this.world.GetTileEntity(0, vector3i) as TileEntityLootContainer;
		if (tileEntityLootContainer == null)
		{
			return Vector3i.zero;
		}
		if (this.lootContainer != null)
		{
			tileEntityLootContainer.CopyLootContainerDataFromOther(this.lootContainer);
		}
		else
		{
			tileEntityLootContainer.lootListName = this.lootListOnDeath;
			tileEntityLootContainer.SetContainerSize(LootContainer.GetLootContainer(this.lootListOnDeath, true).size, true);
		}
		tileEntityLootContainer.SetModified();
		return vector3i;
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x000F6060 File Offset: 0x000F4260
	protected override void AnalyticsSendDeath(DamageResponse _dmResponse)
	{
		DamageSource source = _dmResponse.Source;
		if (source == null)
		{
			return;
		}
		string name;
		if (source.BuffClass != null)
		{
			name = source.BuffClass.Name;
		}
		else
		{
			if (source.ItemClass == null)
			{
				return;
			}
			name = source.ItemClass.Name;
		}
		GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.ZombiesKilledBy, name, 1, true, GameSparksCollector.GSDataCollection.SessionUpdates);
	}

	// Token: 0x0600239E RID: 9118 RVA: 0x000F612E File Offset: 0x000F432E
	public override void BuffAdded(BuffValue _buff)
	{
		if (_buff.BuffClass.DamageType == EnumDamageTypes.Electrical)
		{
			this.Electrocuted = true;
		}
	}

	private int ticksUntilVisible = 2;

	// Token: 0x04001719 RID: 5913
	private float moveSpeedRagePer;

	// Token: 0x0400171A RID: 5914
	private float moveSpeedScaleTime;

	// Token: 0x0400171B RID: 5915
	private float fallTime;

	// Token: 0x0400171C RID: 5916
	private float fallThresholdTime;

	// Token: 0x0400171D RID: 5917
	private static float[] moveSpeeds = new float[]
	{
		0f,
		0.35f,
		0.7f,
		1f,
		1.35f
	};

	// Token: 0x0400171E RID: 5918
	private static float[] moveRageSpeeds = new float[]
	{
		0.75f,
		0.8f,
		0.9f,
		1.15f,
		1.7f
	};

	// Token: 0x0400171F RID: 5919
	private static float[] moveSuperRageSpeeds = new float[]
	{
		0.88f,
		0.92f,
		1f,
		1.2f,
		1.7f
	};

	// Token: 0x04001720 RID: 5920
	private static float[] rageChances = new float[]
	{
		0f,
		0.15f,
		0.3f,
		0.35f,
		0.4f,
		0.5f
	};

	// Token: 0x04001721 RID: 5921
	private static float[] superRageChances = new float[]
	{
		0f,
		0.01f,
		0.03f,
		0.05f,
		0.08f,
		0.15f
	};

}