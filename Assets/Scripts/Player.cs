using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

	public int wallDamge = 1;
	public int pointPerFood = 20;
	public int pointPerSoda = 10;
	public float restartLevelDelay = 1;

	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	private int food;
	public Text foodText;


	// Use this for initialization
	protected override void Start () {
		animator = GetComponent<Animator> ();

		food = GameManager.instance.playerFoodPoints;

		foodText.text = "Food: " + food;

		base.Start ();
	}

	private void OnDisable() {
		GameManager.instance.playerFoodPoints = food;
	}

	private void CheckIfGameOver() {
		if (food <= 0) {
			SoundManager.instance.PlayerSignle (gameOverSound);
			GameManager.instance.GameOver ();
		}
	}
	 
	protected override void AttemptMove <T> (int xDir, int yDir) {
		food--;
		foodText.text = "Food: " + food;
		base.AttemptMove <T> (xDir, yDir);
		 
		RaycastHit2D hit;
		if (Move (xDir, yDir, out hit)) {
			SoundManager.instance.RandomSfx (moveSound1, moveSound2);
		}

		CheckIfGameOver ();
		GameManager.instance.playerTurn = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameManager.instance.playerTurn)
			return;

		int hor = 0;
		int ver = 0;

		hor = (int)Input.GetAxisRaw ("Horizontal");
		ver = (int)Input.GetAxisRaw("Vertical");

		if (hor != 0)
			ver = 0;

		if (hor != 0 || ver != 0)
			AttemptMove<Wall> (hor, ver);
	}

	protected override void OnCantMove<T> (T compoent) {
		Wall hitWall = compoent as Wall;
		hitWall.DamageWall (wallDamge);

		animator.SetTrigger ("playerChop");
	}

	private void Restart() {
		GameManager.instance.playerFoodPoints = food;
		SceneManager.LoadScene ("Main");
//		Application.load (Application.loadedLevel);
	}

	public void LoseFood(int loss) {
		animator.SetTrigger ("playerHit");

		food -= loss;
		foodText.text = "-" + loss + " Food:" + food;
		CheckIfGameOver ();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
		} else if (other.tag == "Food") {
			food += pointPerFood;
			foodText.text = "+" + pointPerFood + " Food:" + food;
			other.gameObject.SetActive(false);
			SoundManager.instance.RandomSfx (eatSound1, eatSound2);
		} else if (other.tag == "Soda") {
			food += pointPerSoda;
			foodText.text = "+" + pointPerSoda + " Food:" + food;
			other.gameObject.SetActive(false);
			SoundManager.instance.RandomSfx (drinkSound1, drinkSound2);
		}
	}
}
