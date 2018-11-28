using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public static PlayerController Instance;

	public static float speed = 4f;
	public bool isPause = false;
	public bool canMove = true;
	public State state = State.Normal;

	Vector3 targetPos;

    static int life = 3;

	int laneNumber = 0;
	float jumpForce = 6.0f;
	float gravity = 12.0f;
	float vertical;

    [System.NonSerialized]
    public float starttimer = 0.0f;
	
    bool gameover = false;
	bool jumping = false;
	bool sliding = false;

	//Rigidbody rb;
	Animator anim;
	CharacterController controller;
	CapsuleCollider box;

	void Start()
	{
		Instance = this;
		anim = GetComponent<Animator> ();
        controller = GetComponent<CharacterController>();
        MenuManager.Instance.lifeText.text = "(Life: " + life + ")";
		//rb = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () 
	{
        starttimer += Time.fixedDeltaTime;
        
        #if UNITY_EDITOR
        if (starttimer > 3)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) { Jump(); }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) { MoveLeft(); }

            if (Input.GetKeyDown(KeyCode.RightArrow)) { MoveRight(); }

            if (Input.GetKeyDown(KeyCode.DownArrow)) { Slide(); }
        }
        #endif
        if (canMove)
			Move ();
	}

	void Move() {
		targetPos = transform.position.z * Vector3.forward;

		if (laneNumber == -1)
			targetPos += Vector3.left;
		else if (laneNumber == 1)
			targetPos += Vector3.right;

		Vector3 moveVector = Vector3.zero;
		moveVector.x = (targetPos - transform.position).normalized.x * 10;

		bool grounded = isGrounded ();
		anim.SetBool ("isGrounded", grounded);

		if(grounded) {
			vertical = 0.0f;

			if (jumping) {
				anim.SetTrigger ("isJumping");
				vertical = jumpForce;
				jumping = false;
			} 

			else if (sliding) {
				StartSliding ();
				Invoke ("StopSliding", 1.0f);
			}
		}
		else {
			vertical -= (gravity * Time.deltaTime);
		}

		moveVector.y = vertical;
		moveVector.z = speed;

		controller.Move(moveVector * Time.deltaTime);
	}

	public void GoToDir(string text) 
	{
		switch (text) {
		case "left":
			MoveLeft();
			break;

		case "right":
			MoveRight ();
			break;

		case "jump":
			Jump ();
			break;

		case "slide":
			Slide ();
			break;

		case "pause":
			if(!gameover)
				Pause ();
			break;

		case "resume":
			if(isPause)
				Resume();
			break;
		
		case "retry":
            if (gameover)
                Retry();
			break;

		case "home":
            if (gameover || isPause)
                Home();
			break;

		default:
			break;
		}
	}

	public void Gameover()
	{
		gameover = true;

	}

    public void Dead() {
        canMove = false;
        anim.speed = 0;
        isPause = true;
        life -= 1;
        MenuManager.Instance.lifeText.text = "(Life: " + life + ")";
        if (life < 1)
        {
            PlayerPrefs.SetInt("coins", 0);
            MenuManager.Instance.DisplayGameOver();
            ScoreManager.Instance.SaveScore();
            Gameover();
        }
        else
        {
            speed = 4;
            PlayerPrefs.SetInt("coins", ScoreManager.Instance.coins);
            MenuManager.Instance.Continue();
        }
    }

	void MoveLeft()
	{
		if (!canMove)
			return;

		if (laneNumber > -1) {
			laneNumber -= 1;
			//transform.position = new Vector3 (pointX, transform.position.y, transform.position.z);
		}
	}

	void MoveRight()
	{
		if (!canMove)
			return;

		if (laneNumber < 1) {
			laneNumber += 1;
			//transform.position = new Vector3 (pointX, transform.position.y, transform.position.z);
		}
	}

	void Jump()
	{
		if (isGrounded ())
			jumping = true;
	}

	void Slide()
	{
		sliding = true;
	}

	public void Pause()
	{
		isPause = true;
		canMove = false;
		anim.speed = 0;
		MenuManager.Instance.Pause ();
	}

    public void Resume()
    {
        isPause = false;
        canMove = true;
        anim.speed = 1;
        MenuManager.Instance.Resume();
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void Retry() {
        MenuManager.Instance.Retry();
    }

	void StartSliding()
	{
		anim.SetBool ("isSliding", sliding);
		controller.radius = 0.2f;
		controller.height = 0.2f;
		controller.center = new Vector3 (0, 0.15f, 0);
	}

	void StopSliding()
	{
		controller.radius = 0.5f;
		controller.height = 2.1f;
		controller.center = new Vector3 (0, 1, 0);
		sliding = false;
		anim.SetBool ("isSliding", sliding);
	}

	bool isGrounded()
	{
		Ray groundRay = new Ray (new Vector3(controller.bounds.center.x, 
			(controller.bounds.center.y - controller.bounds.extents.y) + 0.2f,
			controller.bounds.center.z), Vector3.down);
		Debug.DrawRay (groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

		return Physics.Raycast (groundRay, 0.3f );
	}
}
