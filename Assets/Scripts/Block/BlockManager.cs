using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{

	public GameObject NormalBlock;

//	private GameObject[] normalBlocks;
	private int nowAliveBlockNum = 0;
	private int normalBlockNum = 20;

	private Vector3 nBlockCenterTopPos;
	private Vector3 nBlockSize;

	struct Block
	{
		public GameObject obj;
		public BlockInfo info;
	}

	private Block[] normalBlocks;

	private void Awake()
	{
		Transform tempTf = GameObject.Find("NormalBlockCenterTop").transform;
		nBlockCenterTopPos = tempTf.position;
		nBlockSize = tempTf.GetComponent<BoxCollider2D>().size;
		nBlockSize.x *= tempTf.localScale.x;
		nBlockSize.y *= tempTf.localScale.y;
		nBlockSize.z = 1;

		CreateNormalBlock();
		// block size
		// x: 5.12  / y: 2.56

		TempMapSetting();
	}

	void CreateNormalBlock()
	{
		normalBlocks = new Block[normalBlockNum];

		for (int i = 0; i < normalBlockNum; i++)
		{
			normalBlocks[i].obj = GameObject.Instantiate(NormalBlock, Vector3.zero, Quaternion.identity) as GameObject;
			normalBlocks[i].info = normalBlocks[i].obj.GetComponent<BlockInfo>();
			normalBlocks[i].obj.SetActive(false); // 비활성화 대기
			 
		}
	}

	void SetNormalBlock(Vector3 pos)
	{
		for (int i = 0; i < normalBlockNum; i++)
		{
			if (!normalBlocks[i].obj.activeSelf)
			{
				normalBlocks[i].obj.SetActive(true);
				normalBlocks[i].obj.transform.position = pos;
				normalBlocks[i].info.SetBlockInfo();
				nowAliveBlockNum++;
				break;
			}
		}
	}

	public void BlockDestroy()
	{
		nowAliveBlockNum--;

		if (nowAliveBlockNum < 1)
		{
			Debug.Log("게임 클리어!");
			GameObject.Find("Canvas").transform.FindChild("GameClearPanel").gameObject.SetActive(true);
			
			// 0이하면 게임 클리어
			Time.timeScale = 0;
		}

	}
	
	void TempMapSetting()
	{
		Vector3 tempVec;
		
		for(int i  = 0; i < 4 ; i ++)
		{
			for (int j = -2; j < 3; j++) 
			{
				tempVec = nBlockCenterTopPos;
				tempVec.x += j * nBlockSize.x;
				tempVec.y -= i * nBlockSize.y;
				
				SetNormalBlock(tempVec);
			}
			
		}
	}
}
