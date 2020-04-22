using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipManager{
	//判断两个玩家的关系
	public static Relationship RelationshipBetween(int origin, int target) {
		if (origin != target) {
			return Relationship.Enemy;
		}

		return Relationship.Self;
	}
	//判断和目标单位的关系
	public static Relationship RelationshipBetween(Unit origin, Unit target) {
		return RelationshipBetween(origin.player, target.player);
	}
}
