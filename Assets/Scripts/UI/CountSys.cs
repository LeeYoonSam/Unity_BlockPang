using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountSys : MonoBehaviour
{

	private int count;
	private Text countTx;

	private void Awake()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Screen.SetResolution(720, 1280, true);
	}

	private void Start()
	{
		count = 0;
		countTx = GameObject.Find("Canvas").transform.FindChild("CountTx").GetComponent<Text>();
		StartCoroutine("CountRoutine");

	}

	IEnumerator CountRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.1f);

			count += 1;
			countTx.text = (count / 10).ToString() + "." + (count % 10).ToString();
		}
	}
	
}
