using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2.0f;
	public static GameManager instance = null;
	public BoardManager bm;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playerTurn = true;
	public float turnDelay = 0.1f;

	public int startLevel = 3; 

	private int level = 3;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private Text levelText;
	private bool doingSetup;
	private GameObject levelImage;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}

		enemies = new List<Enemy> ();

		SceneManager.sceneLoaded += OnSceneLoaded;

		bm= GetComponent<BoardManager>();
		DontDestroyOnLoad(gameObject);
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		level++;

		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();

		levelText.text = "Day " + getDay();
		levelImage.SetActive (true);
		InitGame ();

		Invoke ("HideLevelImage", levelStartDelay);
	}

	private void HideLevelImage() {
		levelImage.SetActive (false);
		doingSetup = false;
	}

	private int getDay() {
		return level - startLevel;
	}

	public void GameOver() {
		levelText.text = "After " + getDay() + " Days you die...";
		levelImage.SetActive (true);
		enabled = false;
	}


	void Update() {
		if (playerTurn || enemiesMoving || doingSetup)
			return;

		StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList(Enemy script) {
		enemies.Add (script);
	}

	void InitGame() {

		doingSetup = true;
		levelImage = GameObject.Find ("LevelImage");

		enemies.Clear ();
		bm.SetupScene (level);
	}

	IEnumerator MoveEnemies() {
		enemiesMoving = true;

		yield return new WaitForSeconds (turnDelay);

		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++) {
			enemies [i].MoveEnemy ();
			yield return new WaitForSeconds (enemies [i].moveTime);
		}

		playerTurn = true;
		enemiesMoving = false;
	}

}
