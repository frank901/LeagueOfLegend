using UnityEngine;
using System.Collections;


//写一些公共类型
/// <summary>
///装备存储位置类型 
/// </summary>
public enum LocationType{
	//0代表什么也没有。1代表bag有装备没有。2代表装备栏有背包没有。3代表所有都有
	Null=0,
	Bag,
	Equip,
	All
}

/// <summary>
/// 玩家人物属性4个类型
/// </summary>
public enum PropertyType
{
	AD,
	AP,
	Armor,
	Magic
}

/// <summary>
/// 装备属性类型
/// </summary>
public enum EquipType
{
	AD,
	AP,
	Armor,
	Magic,
	Hemophagia,
	ReduceCD
}


public class Util : MonoBehaviour {


}
