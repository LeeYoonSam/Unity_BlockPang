using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInfo : MonoBehaviour
{
	public int maxHealthPoint;	// 블록의 최대 체력
	private int healthPoint;		// 블록의 현제 체력

	private float xHalfSize;
	private float yHalfSize;
	private Vector3 thisBlockPos;
	private Vector2 thisBlockScale;

	private Vector3 tempBallPos;
	private Vector3 tempBallScale;
	private static float ballRadius = 1.2f;
	private static BlockManager blockM;

	private Color normalColor;		// 일반 생성(공통 사용)
	private Color beAttackedColor;	// 피격시 잠깐 바뀔 색상
	private SpriteRenderer thisSR;

	private Transform gaugeTf;

	private void Awake()
	{
		if (blockM == null)
		{
			blockM = GameObject.Find("GameManager").GetComponent<BlockManager>();
		}
		
		var tempCol = gameObject.GetComponent<BoxCollider2D>();
		xHalfSize = transform.localScale.x * (tempCol.size.x * 0.5f);
		yHalfSize = transform.localScale.y * (tempCol.size.y * 0.5f);
		thisBlockScale = new Vector2(xHalfSize * 2, yHalfSize * 2);

		thisSR = gameObject.GetComponent<SpriteRenderer>();
		normalColor = thisSR.color;
		beAttackedColor = normalColor;
		beAttackedColor.r -= 0.1f;
		beAttackedColor.g -= 0.1f;
		beAttackedColor.b -= 0.1f;

		gaugeTf = transform.FindChild("HealthBar");
	}

	private bool gaugeOn = false;
	
	public void SetBlockInfo()
	{
		healthPoint = maxHealthPoint;
		thisBlockPos = transform.position;

		gaugeOn = false;
		// 체력 게이지가 활성화된 상태이면 비활성화 처리
		if (gaugeTf.gameObject.activeSelf)
		{
			gaugeTf.gameObject.SetActive(false);
		}
	}

	private float shortestDist;	// 가장 짧은 값 저장
	private int shortestSide = 0;	// 1:왼쪽 2:오른쪽 3:위 4:아래
	private float tempDist;	// 각 면에서의 거리 확인

	// 충돌한 면이 어떤 면이지 확인하는 함수
	void CheckCollisionSide()
	{
		// x - 좌, 우
		shortestDist = Mathf.Abs(tempBallPos.x + (tempBallScale.x * ballRadius) - thisBlockPos.x - xHalfSize); // 왼쪽면에 대한 값
		shortestSide = 1;
		tempDist = Mathf.Abs(tempBallPos.x - (tempBallScale.x * ballRadius) - thisBlockPos.x + xHalfSize); // 오른쪽면에 대한 값
		if (shortestDist > tempDist)
		{
			shortestDist = tempDist;
			shortestSide = 2;	// 왼쪽
		}
		
		// y - 위, 아래
		tempDist = Mathf.Abs(tempBallPos.y - (tempBallScale.y * ballRadius) - thisBlockPos.y + yHalfSize);	// 위쪽면에 대한 값
		if (shortestDist > tempDist)
		{
			shortestDist = tempDist;
			shortestSide = 3;	// 위쪽
		}

		tempDist = Mathf.Abs(tempBallPos.y + (tempBallScale.y * ballRadius) - thisBlockPos.y - yHalfSize);	// 아래면에 대한 값

		if (shortestDist > tempDist)
		{
			shortestDist = tempDist;
			shortestSide = 4;	// 아래쪽
		}
	}

	private Vector3 tempScale;
	void GetDamage(int dam)
	{
		healthPoint -= dam; // 피해량에 따라 체력 감소

		if (healthPoint < 1)
		{
			healthPoint = 0;
			blockM.BlockDestroy();
			gameObject.SetActive(false);
		}
		else
		{
			StartCoroutine("BeAttackedEffect");

			if (gaugeOn)
			{
				StopCoroutine("GaugeOnRoutine");	// 재충돌 시 시간을 늘리기 위해 기존 코루틴은 종료
			}
			StartCoroutine("GaugeOnRoutine");

			tempScale = gaugeTf.localScale;		// 체력게이지 줄임
			tempScale.x = (float) healthPoint / maxHealthPoint;
			gaugeTf.localScale = tempScale;
		}
	}

	IEnumerator BeAttackedEffect()
	{
		thisSR.color = beAttackedColor;
		
		yield return new WaitForSeconds(0.1f);

		thisSR.color = normalColor;
	}

	IEnumerator GaugeOnRoutine()
	{
		gaugeOn = true;
		if (!gaugeTf.gameObject.activeSelf)
		{
			gaugeTf.gameObject.SetActive(true);
		}
	
		yield return new WaitForSeconds(3f);
		
		gaugeTf.gameObject.SetActive(false);
		gaugeOn = false;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Ball"))
		{
			Debug.Log("충돌");

			tempBallPos = other.transform.position;
			tempBallScale = other.transform.localScale;
		
			CheckCollisionSide();

			MoveBall tempMoveBall = other.gameObject.GetComponent<MoveBall>();
			if (!tempMoveBall.frameCollisionCheck)
			{
				if (shortestSide < 3)
				{
					other.gameObject.GetComponent<MoveBall>().TurnTheBall(2);
				}
				else
				{
					other.gameObject.GetComponent<MoveBall>().TurnTheBall(1);
				}

				tempMoveBall.frameCollisionCheck = true;

//				gameObject.SetActive(false);
				GetDamage(other.gameObject.GetComponent<BallInfo>().damage);
				
			}
			
			shortestSide = 0;
		}
	}
}
