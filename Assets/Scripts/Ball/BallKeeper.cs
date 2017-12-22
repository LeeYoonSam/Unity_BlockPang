using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallKeeper : MonoBehaviour
{
	private BallManager bm;

	private void Awake()
	{
		bm = GameObject.Find("GameManager").GetComponent<BallManager>();
		
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Ball"))
		{
			// 무조건 올라가는 방향으로 설정(아래 방향은 설정 안됨)
			other.GetComponent<MoveBall>().TurnTheBall(-1);
//			Debug.Log("키퍼와 충돌");
		}
		else
		{
			bm.checkEmptyNormalBallCollider = true;
		}
	}
}
