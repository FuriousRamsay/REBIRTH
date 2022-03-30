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

}