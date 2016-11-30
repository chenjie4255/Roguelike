using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {
	
	[Serializable]
	public class Count
	{
		public int minimun;
		public int maximun;

		public Count(int min, int max) {
			minimun = min;
			maximun = max;
		}
	}

	public int columns = 8;
	public int rows = 8;

	public Count wallCount = new Count(5,9);
	public Count foodCount = new Count (1, 5);
	public Count sodaCount = new Count (0, 2);
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] sodaTiles;
	public GameObject[] outerWallTiles;

	private Transform boardHolder;
	private List<Vector3> gridPostions = new List<Vector3> ();

	void InitialiseList() { 
		gridPostions.Clear ();

		for (int x = 1; x < columns - 1; x++) {
			for (int y = 1; y < rows - 1; y++) {
				gridPostions.Add (new Vector3 (x, y, 0.0f));
			}
		}
	}

	void BoardSetup() {
		boardHolder = new GameObject ("Board").transform;

		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				if (x == -1 || x == columns || y == -1 || y == rows) {
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
				}

				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent (boardHolder);
			}
		}
	}

	Vector3 RandomPostion() {
		int randomIndex = Random.Range (0, gridPostions.Count);
		Vector3 randomPos = gridPostions [randomIndex];
		Debug.Log (randomPos);
		gridPostions.RemoveAt (randomIndex);
		return randomPos;
	}

	void LayoutObjectAtRandom(GameObject[] tiles, int min, int max) {
		int objectCount = Random.Range (min, max + 1);
		Debug.Log ("layout count:" + objectCount);
		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPos = RandomPostion ();
			GameObject tileChoice = tiles [Random.Range (0, tiles.Length)];
			Instantiate (tileChoice, randomPos, Quaternion.identity);
		}
	}

	public void SetupScene(int level) {
		Debug.Log ("SetupScene:" + level);
		BoardSetup ();
		InitialiseList ();
		LayoutObjectAtRandom (wallTiles, wallCount.minimun, wallCount.maximun);
		LayoutObjectAtRandom (foodTiles, foodCount.minimun, foodCount.maximun);
		LayoutObjectAtRandom (sodaTiles, sodaCount.minimun, sodaCount.maximun);
		Debug.Log ("start layout enemy");
		int enemyCount = (int)Mathf.Log (level);
		LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
		Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0.0f), Quaternion.identity);
	}
}
