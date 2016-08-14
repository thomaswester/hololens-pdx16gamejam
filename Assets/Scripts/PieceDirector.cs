﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PieceDirector : MonoBehaviour {
	public static int NUMBER_OF_PLAYERS = 3;
	public static int NUMBER_OF_TILE_PER_PLAYER = 3;
	public static int TOTAL_TILES = NUMBER_OF_PLAYERS * NUMBER_OF_TILE_PER_PLAYER;

	private GameTile centerBoard1;
	private GameTile centerBoard2;

	public int maxTurnsPerGame = 15;

	private HandTile activePiece;

	private int currentPlayersTurn = 0;
	private int totalTurnCounter = 0;

	private HandTile[] allPlayerPieces = new HandTile[9];
	private Text scoreText;
	private Text gameOverText;

	// Use this for initialization
	void Start () {
		centerBoard1 = (GameTile)GameObject.Find ("CenterGameBoard1").GetComponent<GameTile> ();
		centerBoard2 = (GameTile)GameObject.Find ("CenterGameBoard2").GetComponent<GameTile> ();

		allPlayerPieces [0] = (HandTile) GameObject.Find ("Player1HandTile1").GetComponent<HandTile>();
		allPlayerPieces [1] = (HandTile) GameObject.Find ("Player1HandTile2").GetComponent<HandTile>();
		allPlayerPieces [2] = (HandTile) GameObject.Find ("Player1HandTile3").GetComponent<HandTile>();
        /*
		allPlayerPieces [3] = (HandTile) GameObject.Find ("Player2HandTile1").GetComponent<HandTile>();
		allPlayerPieces [4] = (HandTile) GameObject.Find ("Player2HandTile2").GetComponent<HandTile>();
		allPlayerPieces [5] = (HandTile) GameObject.Find ("Player2HandTile3").GetComponent<HandTile>();
		allPlayerPieces [6] = (HandTile) GameObject.Find ("Player3HandTile1").GetComponent<HandTile>();
		allPlayerPieces [7] = (HandTile) GameObject.Find ("Player3HandTile2").GetComponent<HandTile>();
		allPlayerPieces [8] = (HandTile) GameObject.Find ("Player3HandTile3").GetComponent<HandTile>();

		scoreText = GameObject.Find("Score").GetComponent<Text>();
		gameOverText = GameObject.Find ("GameOver").GetComponent<Text> ();
		gameOverText.enabled = false;*/
	}

	void ResetGame() {
		for (int i=0; i< TOTAL_TILES; i++) {
			allPlayerPieces [i].init();
		}

		centerBoard1.init ();
		centerBoard2.init ();

		currentPlayersTurn = 0;
		totalTurnCounter = 0;
		activePiece = null;
		gameOverText.enabled = false;
	}

	//Resets the current Player's piece.  They cannot match the pieces just
	void ResetPieces(VirtualTile lastPlayedPiece) {
		for (int i = 0; i < NUMBER_OF_TILE_PER_PLAYER; i++) {
			int index = i + currentPlayersTurn * NUMBER_OF_PLAYERS;

			if (lastPlayedPiece.hasRed () && allPlayerPieces [index].GetData ().hasRed ()) {
				allPlayerPieces [index].init();
			} else if (lastPlayedPiece.hasBlue () && allPlayerPieces [index].GetData ().hasBlue ()) {
				allPlayerPieces [index].init();
			} else if (lastPlayedPiece.hasYellow () && allPlayerPieces [index].GetData ().hasYellow ()) {
				allPlayerPieces [index].init();
			}
		}
	}

	public void MergeRequested(HandTile piece, GameTile board, VirtualTile.Orientation orientation) {
		//todo animate merge.
		activePiece.SetActive (false);
		activePiece = piece;
		activePiece.SetActive (true);
		activePiece = null;
	
		if (board.canMergeWith (piece.GetData (), orientation)) {
			totalTurnCounter++;
			VirtualTile lastPlayedPiece = new VirtualTile(piece.GetData());

			piece.MergeWithBoard (board, orientation);
			currentPlayersTurn++;
			currentPlayersTurn = currentPlayersTurn % NUMBER_OF_PLAYERS;

			ResetPieces (lastPlayedPiece);

			RecalculateScore ();

			CheckForGameOver ();
		} else {
			piece.SetActive (false);
		}
	}

	void RecalculateScore() {
		int score = centerBoard1.GetData ().Score () + centerBoard2.GetData ().Score ();
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
	}

	void CheckForGameOver() {
		bool playable = false;
		for (int i = 0; i < NUMBER_OF_TILE_PER_PLAYER; i++) {
			int index = i + currentPlayersTurn * NUMBER_OF_PLAYERS;

			VirtualTile pieceToCheck = allPlayerPieces [index].GetData();

			playable |= centerBoard1.canMergeWith (pieceToCheck, VirtualTile.Orientation.Up);
			playable |= centerBoard1.canMergeWith (pieceToCheck, VirtualTile.Orientation.UpsideDown);
			playable |= centerBoard1.canMergeWith (pieceToCheck, VirtualTile.Orientation.Clockwise90);
			playable |= centerBoard1.canMergeWith (pieceToCheck, VirtualTile.Orientation.CounterClockwise90);

			playable |= centerBoard2.canMergeWith (pieceToCheck, VirtualTile.Orientation.Up);
			playable |= centerBoard2.canMergeWith (pieceToCheck, VirtualTile.Orientation.UpsideDown);
			playable |= centerBoard2.canMergeWith (pieceToCheck, VirtualTile.Orientation.Clockwise90);
			playable |= centerBoard2.canMergeWith (pieceToCheck, VirtualTile.Orientation.CounterClockwise90);

			if (playable) {
				break;
			}
		}

		if (!playable || totalTurnCounter >= maxTurnsPerGame) {
			gameOverText.enabled = true;
		}
	}

	void AdjustActivePiece (int position)
	{
		if (activePiece != null) {
			activePiece.SetActive (false);
		}
		int index = position + currentPlayersTurn * NUMBER_OF_PLAYERS;
		activePiece = allPlayerPieces [index];
		activePiece.SetActive (true);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return) && gameOverText != null && gameOverText.enabled) {
			ResetGame ();
		}
		if (gameOverText != null && gameOverText.enabled) {
			return;
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			AdjustActivePiece (0);
		} else if (Input.GetKeyDown (KeyCode.S)) {
			AdjustActivePiece (1);
		} else if (Input.GetKeyDown (KeyCode.D)) {
			AdjustActivePiece (2);
		}

		if (activePiece == null) {
			return;
		}
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			MergeRequested (activePiece, centerBoard1, VirtualTile.Orientation.Up);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2) ) {
			MergeRequested (activePiece, centerBoard2, VirtualTile.Orientation.Up);
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			activePiece.Reorient ( VirtualTile.Orientation.CounterClockwise90);
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			activePiece.Reorient(VirtualTile.Orientation.Clockwise90);
		}
		if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.DownArrow)) {
			activePiece.Reorient (VirtualTile.Orientation.UpsideDown);
		}
	}
}
