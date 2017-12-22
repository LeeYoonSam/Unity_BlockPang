using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIgameOver : MonoBehaviour {

	public void RetryButton()
	{
		SceneManager.LoadScene("GameScene");
		Time.timeScale = 1;
	}
}
