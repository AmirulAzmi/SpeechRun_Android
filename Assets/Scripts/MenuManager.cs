using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
	public static MenuManager Instance;

	//public TileManager tm;
    public GameObject pause;
    public GameObject gameover;
    public GameObject bumped;
    public Text countDown;
    public Text lifeText;
    public int life;
	void Start()
	{
		Instance = this;
	}

	public void PlayGame()
	{
		//GameObject go = GameObject.Find ("MainMenu");
        //go.SetActive (false);
		
        PlayerController.Instance.canMove = true;
	}

    public void Retry()
    {
        //GameObject go = GameObject.Find("GameOver");
        //go.SetActive(false);

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");

        foreach (GameObject g in objects)
        {
            Destroy(g);
        }
        ScoreManager.Instance.ResetScore();
        PlayerController.Instance.canMove = true;
        SceneManager.LoadScene("_SCENE", LoadSceneMode.Single);
    }

    public void Continue()
    {
        bumped.SetActive(true);

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Object");

        foreach (GameObject g in objects)
        {
            Destroy(g);
        }
        StartCoroutine("ContinueTimer");
    }

    public void Pause() { pause.SetActive(true); }

    public void Resume() { pause.SetActive(false); }

    public void DisplayGameOver() { gameover.SetActive(true); }

    public void HideGameOver() { gameover.SetActive(false); }

    public IEnumerator ContinueTimer()
    {
        yield return new WaitForSeconds(1f);
        countDown.text = "Continue in 2..";
        yield return new WaitForSeconds(1f);
        countDown.text = "Continue in 1..";
        yield return new WaitForSeconds(1f);
        countDown.text = "Continue in 0..";
        yield return new WaitForSeconds(0.3f);
        countDown.text = "PLAY!";
        PlayerController.Instance.canMove = true;
        SceneManager.LoadScene("_SCENE", LoadSceneMode.Single);
    }
}
