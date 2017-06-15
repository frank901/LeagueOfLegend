using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

/// <summary>
///数据库单例 
/// </summary>
public class SqlManager {
	#region 单例
	private static SqlManager instance;
	//多性的实例化
	//通过构造方法实例化一个私有化对象，外部不能调用
	public static SqlManager GetInstans(){
		if (instance == null) {
			instance = new SqlManager ();
		}
		return instance;
	}
	//构造私有化
	private SqlManager(){}

		#endregion

		#region 数据库对象
		//创建数据库的三个类
	private SqliteConnection con;
	private SqliteCommand command;
	private SqliteDataReader reader;
		#endregion
	#region 数据库框架
	//关于数据库的操作

	/// <summary>
	///打开数据库 
	/// </summary>
	/// <param name="databaseName">Database name.</param>
	public void OpenDatabase(string databaseName){
//数据库装备的识别
		//是否包含.sqlite
		if(!databaseName.Contains(".sqlite"))
//如果不包含.sqlite,则加上---数据库的名称优化
		{
			databaseName+=".sqlite";
		}
		//路径的默认在strenmAccess里所以直接写
		//打开数据库--字符串路径---数据库连接字符串
		//三部曲 找路径 连接 打开
		string dataStr="Data Source"+Application.streamingAssetsPath+"/"+databaseName;
		//实例化连接对象
		con = new SqliteConnection (dataStr);
	//实例化指令对象
		command=con.CreateCommand();
		//打开连接
		con.Open();
	}
	/// <summary>
	///关闭数据库 
	/// </summary>
	public void CloseDatabase(){
		//有可能读写流没开---异常处理
		try {
			reader.Close();//读写流
			con.Close();
		} catch (SqliteException ex) {
			Debug.Log (ex.ToString ());
		}
	}
	/// <summary>
	///执行Sql语句---基础的封装---增删改
	/// </summary>
	/// <param name="quert">Quert.</param>
	public void RunSql(string quert){
		try {
			command.CommandText=quert;
			command.ExecuteNonQuery();
		} catch (SqliteException ex) {
			Debug.Log (ex.ToString ());
		}
	}
	/// <summary>
	///查询单个数据 
	/// </summary>
	/// <param name="query">Query.</param>
	public object SelectSingleData(string query){
		try {
			command.CommandText=query;
			object obj=command.ExecuteScalar();
			return obj;
		} catch (SqliteException ex) {
			Debug.Log (ex.ToString ());
			return null;
		}
	}
	/// <summary>
	///查找多个数据 
	/// </summary>
	/// <returns>The all data.</returns>
	/// <param name="query">Query.</param>
	/// 因为有行有列--所以建立一个arr类型的list
	public List<ArrayList> SelectAllData(string query){
		try {
			//创建一个结构
			List<ArrayList> result=new List<ArrayList>();//实例化
			command.CommandText=query;//数据添上去
			//执行语句，返回所有结果--返回reader
			reader=command.ExecuteReader();
			while(reader.Read()){
				ArrayList temp=new ArrayList();
				//遍历所有列
				for(int i=0;i<reader.FieldCount;i++){//FieldCount列数
					temp.Add(reader.GetValue(i));
				}
				//添加行数据到结构中
				result.Add(temp);//把temp加到list里
			}
			//关闭流
			reader.Close();
			//返回结构
			return result;
		} catch (SqliteException ex) {
			Debug.Log (ex.ToString ());
			return null;
		}
	}
	//总结：封装了三个函数，一个是增删改，获取单个数据，获取多个数据，三个基础函数，后期会变为private
	//以后会在这个基础之上建立函数，用于装备人物获取的函数
	#endregion

	#region 项目数据库框架
	//---------做了一个封装location和count 做了一个get，set方法
	//获取某个装备类，location是谁--获取装备类型
	public LocationType GetEquipLocation(string equipName){
		try {//以免数据库语句执行的时候出现异常
			string tempQuery="Select Location From EquipTable Where EquipName='"+equipName+"'";
			object obj=SelectSingleData(tempQuery);
			return(LocationType)obj;
		} catch (SqliteException ex) {
			return LocationType.Null;//如果出现异常-调成location.null-就是没有被买过的一种情况
		}
	}
	//位置更新---传装备参数
	public void SetEquipLocation(string equipName,int location){
		//位置类型
		string query="Update EquipTable Set Location="+location+"where EquipName='"+equipName+"'";
		RunSql (query);
	}
	//查询装备数量
	public int GetEquipCount(string equipName){
		string query="Select Count From EquipName Where EquipName'"+equipName+"'";
		return System.Convert.ToInt32 (SelectSingleData (query));
	}
	//更新数量
	public void SetEquipCount(string equipName,int count){
		string query="Update EquipTable set Count="+count+"Where EquipName='"+equipName+"'";
		RunSql (query);
	}
	/// <summary>
	///获取装备的花费---花钱 
	/// </summary>
	public int GetEquipCost(string equipName){
		string query="Select Cost From EquipTable Where EquipName'"+equipName+"'";
		int cost = System.Convert.ToInt32 (SelectSingleData (query));
		return cost;
	}
	/// <summary>
	///设置玩家金钱-----卖装备加钱 
	/// </summary>
	public void SetplayerMoney(int money){
		string query = "Update PlayerTable Set PlayerMoney=" + money;
		RunSql (query);
	}
	/// <summary>
	/// 获取玩家金钱
	/// </summary>
	public int GetPlayerMoney(){//只有一个玩家所以没有加任何参数，多个玩家去吧玩家的名称传回来
		string query="Select Player From PlayerTable";
		return System.Convert.ToInt32 (SelectSingleData (query));
	}
	/// <summary>
	/// 获取某个装备的单个属性（装备名称，属性类型）
	/// </summary>
	public float GetEquipProperty(string equipName,PropertyType type){
		string query="Select"+type.ToString()+"From EquipTable Where EquipName='"+equipName+"'";
	//获取属性值
		float value=System.Convert.ToSingle(SelectSingleData(query));
		//返回
		return value;
	}
	/// <summary>
	/// 获取玩家属性(属性类型)
	/// </summary>
	public float GetPlayerProperty(PropertyType type)
	{
		string query = "Select " + type.ToString() + "From PlayerTable";
		return System.Convert.ToSingle (SelectSingleData (query));
	}
	/// <summary>
	/// 添加装备属性
	/// </summary>
	/// <param name="equipName">Equip name.</param>
	public void AddEquipProperties(string equipName)
	{
		string query = string.Format ("Update PlayerTable Set AD={0},AP={1},Armor={2},Magic={3}",
			GetPlayerProperty(PropertyType.AD) + GetEquipProperty(equipName,PropertyType.AD),
			GetPlayerProperty(PropertyType.AP) + GetEquipProperty(equipName,PropertyType.AP),
			GetPlayerProperty(PropertyType.Armor) + GetEquipProperty(equipName,PropertyType.Armor),
			GetPlayerProperty(PropertyType.Magic) + GetEquipProperty(equipName,PropertyType.Magic));
		RunSql (query);
	}
	/// <summary>
	/// 移除装备属性
	/// </summary>
	/// <param name="equipName">Equip name.</param>
	public void RemoveEquipProperties(string equipName)
	{
		string query = string.Format ("Update PlayerTable Set AD={0},AP={1},Armor={2},Magic={3}",
			GetPlayerProperty(PropertyType.AD) - GetEquipProperty(equipName,PropertyType.AD),
			GetPlayerProperty(PropertyType.AP) - GetEquipProperty(equipName,PropertyType.AP),
			GetPlayerProperty(PropertyType.Armor) - GetEquipProperty(equipName,PropertyType.Armor),
			GetPlayerProperty(PropertyType.Magic) - GetEquipProperty(equipName,PropertyType.Magic));
		RunSql (query);
	}
	/// <summary>
	///能否购买此装备 
	/// </summary>
	public bool CanBuyEquip(string equipName){
		if (GetPlayerMoney () >= GetEquipCost (equipName)) {
			return true;
		}
		return false;
	}
	/// <summary>
	///买装备 
	/// </summary>
	/// <param name="equipName">Equip name.</param>
	//存在角色装备位置，Null，Bag，EquipBox，All
	public void BuyEquip(string equipName){
        //获取当前装备位置字段，看有没有买过装备，装备在哪里
		//string tempQuery="Select Location From EquipTable Where EquipName='"+equipName+"'";
			//转换成int
		//int result=System.Convert.ToInt32(SelectSingleData(tempQuery));


		//检测装备位置类型--把装备的信息存在装备栏里
		//switch那util脚本那里面的几个类型,result是int，把int转化成locationType类型
		switch(GetEquipLocation(equipName)){
		//sql语句需要封装一下，直接调用
		case LocationType.Null:
			//设置装备值类型为背包类型--原来是null无是0，后来变为背包为1
			SetEquipLocation(equipName,1);
			//设置装备数量为1
			SetEquipCount(equipName,1);
			break;
		case LocationType.Equip://装备栏
			//设置装备位置类型为背包类型以及装备栏类型
			SetEquipLocation(equipName,3);//即在背包里也在装备栏里 
			//设置装备数量为1
			SetEquipCount(equipName,1);
			break;
		case LocationType.Bag:
		case LocationType.All:
			//无论是在背包里还是在all 数量加1,状态不变
			SetEquipCount (equipName, GetEquipCount (equipName) + 1);
			break;
		default:
			break;
		}
		//设置玩家剩余金钱
		SetplayerMoney (GetPlayerMoney () - GetEquipCost (equipName));
	}
	/// <summary>
	///卖装备 
	/// </summary>
	/// <param name="equipName">Equip name.</param>
	public void SellEquip(string equipName){
		//装备数量减1
		SetEquipCount (equipName, GetEquipCount (equipName) - 1);
	//如果装备数量为0
		if(GetEquipCount(equipName)==0){
			//如果装备之前在背包中，其他地方都没有--装备就在背包里
			if (GetEquipLocation (equipName) == LocationType.Bag) {
			//设置装备位置为Null
				SetEquipLocation(equipName,0);
				//如果唯一的装备在装备栏里？？？？？？
			}else if (GetEquipLocation(equipName)==LocationType.All) {
				//设置装备位置为equip
				SetEquipCount (equipName, 2);
			}
		}
		//玩家获取装备一半的费用
		SetplayerMoney(GetPlayerMoney()+GetEquipCost(equipName)/2);
	}
	/// <summary>
	///背包到装备栏 
	/// </summary>
	/// <param name="equipName">Equip name.</param>
	public void Bag2EquipBox(string equipName){
	//设置装备背包数量
		SetEquipCount(equipName,GetEquipCount(equipName)-1);
		//背包是否为空
		if (GetEquipCount (equipName) == 0) {
			//更新为装备类型equip
			SetEquipLocation (equipName, 2);
		} else {
			//更新为装备背包类型all
			SetEquipLocation (equipName, 3);
		}
		//将当前装备的属性添加到玩家身上，之前的属性移除掉
	}

	//三层封装，第一个是数据库框架的封装，第二个是数据获取查询的语句封装，第三个是具体需求方法的封装


	/// <summary>
	///获取玩家多个信息 
	/// </summary>
	public List<ArrayList> GetPlayerMsg(){
		string query="Select * from PlayerTable";
		//查询多个数据
		List<ArrayList> result = SelectAllData (query);
		return result;
	}




	#endregion
}
