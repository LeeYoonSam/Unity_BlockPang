using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    // 위치 설정 변수
    public Vector2 minPos;
    public Vector2 maxPos;

    private Transform ballTf;
    private Vector3 ballPos;
    private Vector2 moveValue; // 이동값 +1 -1 두 방향
    private float moveSpeed = 0.2f;

    public bool frameCollisionCheck = false;
    
    private void Awake()
    {
        ballTf = transform;
//        ballPos = ballTf.position;
        moveValue = Vector2.one; // == new Vector2(1,1);
    }

    private void FixedUpdate()
    {
        ballPos.x += moveSpeed * moveValue.x;
        ballPos.y += moveSpeed * moveValue.y;

        // x 축이 최소치를 넘어가면
        if (ballPos.x < minPos.x)
        {
            moveValue.x *= -1;
            ballPos.x += minPos.x - ballPos.x; // 반대 방향으로 차이 만큼 밀어 이동거리 일정하게 해줌.
        } 
        // x 축이 최대치를 넘어가면
        else if (ballPos.x > maxPos.x)
        {
            moveValue.x *= -1;
            ballPos.x += maxPos.x - ballPos.x;
        }

        
        // y축이 최대치를 넘어가면
        if (ballPos.y > maxPos.y)
        {
            moveValue.y *= -1;
            ballPos.y += maxPos.y - ballPos.y;
        }
        // y 축이 최소치를 넘어가면
        else if (ballPos.y < minPos.y - 3f)
        {
//            moveValue.y *= -1;
//            ballPos.y += minPos.y - ballPos.y;
            
            gameObject.SetActive(false);
            // 볼이 없어질때 생존 한 공의개수 업데이트
            GameObject.Find("GameManager").GetComponent<BallManager>().MinusAliveBallCount();
        }
        

        ballTf.position = ballPos;

        if (frameCollisionCheck)
        {
            frameCollisionCheck = false;
        }
    }

    public void TurnTheBall(int type) // 1은 x축, 2는 y축
    {
        // 벽돌이나 다른 물체와 충돌 시 튕겨냄
        if (type == 1)
        {
            moveValue.x *= -1;
        } 
        else if (type == 2)
        {
            moveValue.y *= -1;
        }
        else if (type == -1)
        {
            if (moveValue.y < 0)
            {
                moveValue.y *= -1;
            }
        }
    }

    public void SetDirecMoveValue(Vector2 dir)
    {
        moveValue.x = dir.x;
        moveValue.y = dir.y;
        ballPos = ballTf.position;
    }
}
