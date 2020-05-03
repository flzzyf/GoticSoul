using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseData{
	public string ID;
	public string Name;
	public string Info;

	public override string ToString() {
		return string.Format("ID:{0}, Name:{1}, Info:{2}", ID, Name, Info);
	}
}
