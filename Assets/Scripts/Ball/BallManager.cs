using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public GameObject NormalBall;
    public GameObject EmptyNormalBall;

    private int aliveBallCount;    // 현재 생존한 볼 수
    private int nowBallCount;    // 현재 생성된 볼 수 
    private int normalBallNum;
    
    private GameObject[] normalBalls;
//    private GameObject normalBall;
    private GameObject emptyNormalBall;

    public bool checkEmptyNormalBallCollider;

    private bool activeBall = false;    // 활성화된 상태인지 확인하는 용도
    private Vector3 startTouchedPos;
    private Vector3 endTouchedPos;
    
    private void Awake()
    {
        aliveBallCount = 0;
        nowBallCount = 0;
        normalBallNum = 3;
        normalBalls = new GameObject[normalBallNum];
        for (int i = 0; i < normalBallNum; i++)
        {
            normalBalls[i] = GameObject.Instantiate(NormalBall, Vector3.zero, Quaternion.identity) as GameObject;
            normalBalls[i].SetActive(false);
        }
//        normalBall = GameObject.Instantiate(NormalBall, Vector3.zero, Quaternion.identity) as GameObject;
//        normalBall.SetActive(false);
        
        emptyNormalBall = GameObject.Instantiate(EmptyNormalBall, Vector3.zero, Quaternion.identity) as GameObject;
        emptyNormalBall.SetActive(false);
    }

    private bool touchOn = false;
    private Touch tempTouch;
    
    private void Update()
    {
        // 볼이 생성되지 않은 상태라면 마우스 클릭 확인
        if (Input.touchCount > 0)    //버튼 누름
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                tempTouch = Input.GetTouch(i);
                if (tempTouch.phase == TouchPhase.Began)    // 터치 시작
                {
                    // 클릭된 위치를 메인 카메라 기준으로 월드 포지션을 가져옴
                    startTouchedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    // 메인 카메라에서 포지션을 받아왔으니 z값이 -10이므로 0으로 초기화
                    startTouchedPos.z = 0;

                    emptyNormalBall.SetActive(true);
                    emptyNormalBall.transform.position = startTouchedPos;

                    checkEmptyNormalBallCollider = false;
                    touchOn = true;
                }
                else if (tempTouch.phase == TouchPhase.Ended)    // 터치 끝
                {
                    if (nowBallCount < normalBallNum)
                    {
                        // 클릭이 끝난 위치를 메인 카메라 기준으로 월드 포지션을 가져옴
                        endTouchedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        // 메인 카메라에서 포지션을 받아왔으니 z값이 -10이므로 0으로 초기화
                        endTouchedPos.z = 0;

                        ActiveBall();
                    }
                    else
                    {
                        emptyNormalBall.SetActive(false);
                    }

                    touchOn = false; 
                }
            }
        }
        
        // 볼이 모두 만들어진 상태
        if (nowBallCount >= normalBallNum)
        {
            if (touchOn)
            {
                // 드래그 중일때
                endTouchedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                endTouchedPos.z = 0;
                // 위치 계속 바꿔줘서 emptyNomalBall이 터치된 위치 따라가게 함.
                emptyNormalBall.transform.position = endTouchedPos;
            }
        }


    }

    private Vector2 direcVector;

    // 볼 활성화. 이때 각도와 위치에 맞게 보내도록 설정
    void ActiveBall()
    {
        // 어치 시작지점부터 종료지점 거리가 1보다 크면 터치 생성으로 간주
        if (Vector3.Distance(startTouchedPos, endTouchedPos) > 1f)
        {
            if (!checkEmptyNormalBallCollider)
            {
                direcVector.x = endTouchedPos.x - startTouchedPos.x;
                direcVector.y = endTouchedPos.y - startTouchedPos.y;

                // Normalize를 통해 항상 길이가 1이 되도록 맞춰준다.
                direcVector.Normalize();

                for (int i = 0; i < normalBallNum; i++)
                {
                    // 볼이 비활성화 상태
                    if (!normalBalls[i].activeSelf)
                    {
                        normalBalls[i].SetActive(true);
                        normalBalls[i].transform.position = startTouchedPos;
                        normalBalls[i].GetComponent<MoveBall>().SetDirecMoveValue(direcVector);
                        break;
                    }
                }
                nowBallCount++;
                aliveBallCount++;   
            }
        }
        else
        {
            Debug.Log("볼 생성 안 됨");
        }
        
        emptyNormalBall.SetActive(false);
    }

    public void MinusAliveBallCount()
    {
        aliveBallCount--;
        if (aliveBallCount == 0)
        {
            if (nowBallCount >= normalBallNum)
            {
                Debug.Log("Game Over");
                
                /*
                 * 바로 GameOverPanel을 찾지 않은 것은
                    비활성화 상태인 오브젝트는 GameObject.Find 함수로 찾을 수 없기 때문입니다.
                    그래서 항상 활성화되어 있는 Canvas를 먼저 찾고,
                    자식인 GameOverPanel을 FindChild로 찾아준 거죠.
                    
                    
                    출처: http://prosto.tistory.com/164?category=482059 [Prosto]
                 */
                GameObject.Find("Canvas").transform.FindChild("GameOverPanel").gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }
}
