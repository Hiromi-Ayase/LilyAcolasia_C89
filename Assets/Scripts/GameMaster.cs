using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



public class GameMaster : Photon.MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite areaWhite;
    public Sprite areaBlack;
    public Sprite areaGreen;
    public Sprite areaBlue;
    public Sprite areaRed;
    public Sprite judgeDefeat;
    public Sprite judgeEven;
    public Sprite judgeConquer;
    public Sprite turnSet1;
    public Sprite turnSet2;
    public Sprite turnSet3;
    public Sprite turnSet4;
    public Sprite turnSet5;
    public Sprite turnEnemy;
    public Sprite turnYour;
    public Sprite turnRemove;
    public Sprite equivBonus;
    public Sprite concolorBonus;

    public AudioClip soundDraw;
    public AudioClip soundDiscard;
    public AudioClip soundTrash;
    public AudioClip soundCutIn;
    public AudioClip soundSkillLink;
    public AudioClip soundSkillColor;
    public AudioClip soundSkillSnipe;
	public AudioClip soundJudge;
	public AudioClip soundGameWin;
	public AudioClip soundGameLost;
	public AudioClip soundGameDraw;
	public AudioClip soundAllSet;

	public AudioClip bgm1;
	public AudioClip bgm2;

	public Achievements achievements;

    private GameObject talon;
    private GameObject trash;
    private GameObject[] northField = new GameObject[5];
    private GameObject[] southField = new GameObject[5];
    private GameObject[] FieldWrapper = new GameObject[5];
    private GameObject southHand;
    private GameObject northHand;
    private GameObject colorChoice;
    private SpriteRenderer[] statusField = new SpriteRenderer[5];
    private SpriteRenderer[] northBonus = new SpriteRenderer[5];
    private SpriteRenderer[] southBonus = new SpriteRenderer[5];
    private SpriteRenderer statusTurn;
    private SpriteRenderer statusSet;
    private Digit[] northScore = new Digit[5];
    private Digit[] southScore = new Digit[5];
    private GameObject[] northScoreObject = new GameObject[5];
    private GameObject[] southScoreObject = new GameObject[5];
    private Digit talonCount;
    private Digit trashCount;
    private AudioSource audioSource;
    private AudioSource bgmSource;

    private LilyAcolasia.GameOperator gameOperator;
    private GameObserverImpl observer;
    private IEnumerator<LilyAcolasia.GameInput> gameEnumerator = null;
    private Dictionary<String, GameObject> cards = new Dictionary<String, GameObject>();
    private Dictionary<String, DragUI> cardScripts = new Dictionary<String, DragUI>();

    private LilyAcolasia_AI.AI ai;
	private PhotonNetworkPlayer netuser;
    private int level;

    /// <summary>
    /// Game State enum.
    /// </summary>
    public enum GameState
    {
        UserAction,
        AIAction,
        CmdWait,
        ColorChoice,
        RoundStart,
        TurnStart,
        RoundEnd,
        GameEnd,
		GameEnding,
        CutIn,
    };

    private const int CMD_WAIT = 50;
    private const int AI_WAIT = 50;
    private const int TURN_WAIT = 50;
	private const int GAME_ENDING_WAIT = 220;
	private int USER_TURN = 1;
    private const int AI_TURN = 0;
    private int frameCounter = 0;

    public GameState NextGameState { get; set; }
    public List<GameObject> CardParents { get { return this.cardParents; } }
    public List<GameObject> cardParents = new List<GameObject>();

    private GameState currentGameState;
	private int lastTurn;
	private int gameEndingState;
	private LilyAcolasia_AI.AI demoAi;

    void Awake()
    {
		long rand;
		bool rev = false;
		int isOnline = PlayerPrefs.GetInt ("network", 0);
		Debug.Log ("isOnline" + isOnline);

		if (isOnline != 0 && NetworkStart.user != null) {
			this.netuser = NetworkStart.user;
			if (this.netuser.IsServer) {
				rev = true;
			}
			rand = this.netuser.Rand;
			Debug.Log ("Seed:" + rand);
			this.level = 0;
		} else {
			this.level = PlayerPrefs.GetInt("level", 1);
			if (this.level == 0) {
				this.level = 1;
			}
			this.ai = new LilyAcolasia_AI.AI (this.level);

			this.demoAi = null;//new LilyAcolasia_AI.AI (4);
			this.currentGameState = GameState.AIAction;

			rand = new System.Random ().Next();
		}
        Debug.Log("Level:" + level);
		this.observer = new GameObserverImpl(this, this.netuser);
        this.gameOperator = new LilyAcolasia.GameOperator(this.observer, "AI", "You", rand, rev);

        LilyAcolasia.CardDeck.Handler = this.CardMove;
        FindObjects();
        CreateCards();
    }

    void Start ()
    {
        this.colorChoice.SetActive(false);
        Init();
        this.bgmSource.loop = true;
		this.bgmSource.clip = this.gameOperator.Round.Current.Turn == USER_TURN ? bgm1 : bgm2;
        this.bgmSource.Play();
		//GameEffect.Special(5, 1);
		//GameEffect.Achievement(1);
    }


	void OnApplicationQuit() {
		if (this.netuser != null) {
			this.netuser.Close ();
			DestroyObject (this.netuser.gameObject);
		}
	}

	void Update ()
    {
        processEvent();

        foreach (SpriteRenderer rd in this.statusField)
        {
            if (rd.transform.localScale.x > 1000)
            {
                rd.transform.localScale -= new Vector3(100, 100, 100);
                if (rd.transform.localScale.x < 1000)
                {
                    rd.transform.localScale = new Vector3(1000, 1000, 1000);
                }
            }
        }
    }
   
    private void processEvent()
    {
        int turn = this.gameOperator.Round.Current.Turn;
        LilyAcolasia.CardGame game = this.gameOperator.Round.Current;
        LilyAcolasia.GameInput input = this.gameEnumerator.Current;

        this.frameCounter++;

		if (this.currentGameState == GameState.RoundEnd) {
			if (!GameEffect.IsRoundEnd) {
				if (this.gameOperator.Round.HasNextRound ()) {
					StartRound ();
				} else {
					this.currentGameState = GameState.GameEnding;
					var round = this.gameOperator.Round;
					this.frameCounter = 0;
					if (round.Point1 > round.Point2) {
						if (level == 4) {
							achievements.WarfareLost ();
						} else if (level == 1) {
							achievements.MimicLost ();
						} else if (level == 0) {
							achievements.HumanLost ();
						}
						gameEndingState = 0;
					} else if (round.Point1 < round.Point2) {
						if (level == 4) {
							achievements.WarfareWin ();
						} else if (level == 1) {
							achievements.MimicWin ();
						} else if (level == 0) {
							achievements.HumanWin ();
						}

						bool isPerfect = true;
						foreach (LilyAcolasia.Field f in game.Fields) {
							if (f.Winner != USER_TURN) {
								isPerfect = false;
								break;
							}
						}
						if (isPerfect) {
							achievements.Perfect ();
						}

						gameEndingState = 1;
					} else {
						if (level == 4) {
							achievements.WarfareDraw ();
						} else if (level == 1) {
							achievements.MimicDraw ();
						} else if (level == 0) {
							achievements.HumanDraw ();
						}

						gameEndingState = 2;
					}
				}
			}
		} else if (this.currentGameState == GameState.GameEnding) {
			this.bgmSource.volume *= 0.95f;
			if (frameCounter > GAME_ENDING_WAIT) {
				if (gameEndingState == 0) {
					GameEffect.GameEnd (0);
					audioSource.clip = soundGameLost;
					audioSource.Play ();
				} else if (gameEndingState == 1) {
					GameEffect.GameEnd (1);
					audioSource.clip = soundGameWin;
					audioSource.Play ();
				} else if (gameEndingState == 2) {
					GameEffect.GameEnd (2);
					audioSource.clip = soundGameDraw;
					audioSource.Play ();
				}
				this.currentGameState = GameState.GameEnd;
				this.frameCounter = 0;
			}
		}
		else if (this.currentGameState == GameState.GameEnd)
        {
            if (!GameEffect.IsGameEnd)
            {
				if (Input.GetMouseButtonDown (0)) {
					CameraFade.StartAlphaFade (Color.black, false, 0.5f, 0.5f, () => {
						if (this.netuser != null) {
							this.netuser.Close ();
							DestroyObject (this.netuser.gameObject);
						}
						SceneManager.LoadScene ("Title");
					});
				} else if (demoAi != null) {
					demoAi = null;
					CameraFade.StartAlphaFade (Color.black, false, 0.5f, 0.5f, () => {
						if (this.netuser != null) {
							this.netuser.Close ();
							DestroyObject (this.netuser.gameObject);
						}
						SceneManager.LoadScene ("Game");
					});
				}
            }
        }
        else if (this.currentGameState == GameState.CutIn)
        {
            if (!GameEffect.IsCutIn)
            {
                int n = game.LastTrashed.Power;

                if (n == 5)
                {
                    if (demoAi == null && game.Turn == USER_TURN)
                    {
                        this.currentGameState = GameState.ColorChoice;
                        ColorChoice.Show((color) =>
                        {
                            LilyAcolasia.GameInput ipt = this.gameEnumerator.Current;
                            ipt.input(LilyAcolasia.Command.Special, color);
                            this.gameEnumerator.MoveNext();
                            this.currentGameState = GameState.UserAction;
                            audioSource.clip = soundSkillColor;
                            audioSource.Play();
                        });
                    }
                    else
                    {
                        this.currentGameState = GameState.AIAction;
                    }
                }
                else if (n == 9)
                {
					this.currentGameState = demoAi == null && turn == USER_TURN ? GameState.UserAction : GameState.AIAction;
                }
                else
                {
                    GameEffect.Special(n, game.Turn);
					this.currentGameState = demoAi == null && turn == USER_TURN ? GameState.UserAction : GameState.AIAction;
                }
                this.frameCounter = 0;
            }
        }
        else if (this.currentGameState == GameState.RoundStart)
        {
            if (this.frameCounter >= TURN_WAIT)
            {
                this.frameCounter = 0;
                this.currentGameState = GameState.TurnStart;
            }
        }
        else if (this.currentGameState == GameState.TurnStart)
        {
            if (this.frameCounter >= TURN_WAIT)
            {
                this.frameCounter = 0;
				this.currentGameState = demoAi == null && turn == USER_TURN ? GameState.UserAction : GameState.AIAction;
            }
        }
        else if (this.currentGameState == GameState.CmdWait)
        {
            if (this.frameCounter >= CMD_WAIT)
            {
                this.frameCounter = 0;
 

                if (game.Status == LilyAcolasia.GameStatus.Status.End)
                {
                    input.input(LilyAcolasia.Command.Next);
                    this.gameEnumerator.MoveNext();

                    if (game.Status == LilyAcolasia.GameStatus.Status.RoundEnd)
                    {
                        this.currentGameState = GameState.RoundEnd;
                        GameEffect.RoundEnd(game.Winner);
                    }
                    else
                    {
                        this.currentGameState = GameState.TurnStart;
                    }
                }
                else if (game.Status == LilyAcolasia.GameStatus.Status.WaitSpecialInput)
                {
                    GameEffect.Special(-1, game.Turn);
                    input.input(LilyAcolasia.Command.Special);
                    this.gameEnumerator.MoveNext();
                }
                else
                {
					this.currentGameState = demoAi == null &&  turn == USER_TURN ? GameState.UserAction : GameState.AIAction;
                }
            }
        }
        else if (this.currentGameState == GameState.UserAction)
        {
			if (this.demoAi != null) {
				if (game.Status != LilyAcolasia.GameStatus.Status.WaitSpecialInput || this.frameCounter >= AI_WAIT)
				{
					this.frameCounter = 0;

					var next = demoAi.next (game);
					input.input (next.Item1, next.Item2, next.Item3.ToString());

					this.gameEnumerator.MoveNext();
					this.frameCounter = 0;
					this.currentGameState = GameState.CmdWait;
					GameEffect.Special(-1, game.Turn);
					if (game.Status == LilyAcolasia.GameStatus.Status.WaitSpecialInput)
					{
						GameEffect.CutIn(game.LastTrashed.Power);
						audioSource.clip = soundCutIn;
						audioSource.Play();
						this.currentGameState = GameState.CutIn;
					}
				}
			} else {
				if (game.Status == LilyAcolasia.GameStatus.Status.WaitSpecialInput && !game.LastTrashed.HasSpecialInput) {
					this.currentGameState = GameState.CmdWait;
					this.frameCounter = 0;
					GameEffect.Special (game.LastTrashed.Power, game.Turn);
				}
			}

            this.frameCounter = 0;
        }
        else if (this.currentGameState == GameState.AIAction)
        {
            if (game.Status != LilyAcolasia.GameStatus.Status.WaitSpecialInput || this.frameCounter >= AI_WAIT)
            {
                this.frameCounter = 0;

				if (this.level == 0) {
					if (!netuser.IsConnected) {
						GameEffect.GameEnd (3);
						this.netuser.Close ();
						DestroyObject (this.netuser.gameObject);
						this.currentGameState = GameState.GameEnd;
						return;
					}
					string next = netuser.message ();

					if (next == null) {
						GameEffect.GameEnd (3);
						this.netuser.Close ();
						DestroyObject (this.netuser.gameObject);
						this.currentGameState = GameState.GameEnd;
						return;
					} else if (next == "") {
						return;
					}
					input.input (next);
				} else {
					var next = ai.next (game);
					input.input (next.Item1, next.Item2, next.Item3.ToString());
				}
                this.gameEnumerator.MoveNext();
                this.frameCounter = 0;
                this.currentGameState = GameState.CmdWait;
                GameEffect.Special(-1, game.Turn);
                if (game.Status == LilyAcolasia.GameStatus.Status.WaitSpecialInput)
                {
                    GameEffect.CutIn(game.LastTrashed.Power);
                    audioSource.clip = soundCutIn;
                    audioSource.Play();
                    this.currentGameState = GameState.CutIn;
                }
            }
        }
    }

    public void UpdateStatus()
    {
        var game = this.gameOperator.Round.Current;
        var bonus = new HashSet<string>();

        for (int i = 0; i < 5; i++)
        {
            Sprite beforeSprite = this.statusField[i].sprite;
            if (game.Fields[i].Winner == 2)
            {
                this.statusField[i].sprite = judgeEven;
            }
            else if (game.Fields[i].Winner == USER_TURN)
            {
                this.statusField[i].sprite = judgeConquer;
            }
            else if (game.Fields[i].Winner == AI_TURN)
            {
                this.statusField[i].sprite = judgeDefeat;
            }
            else
            {
                this.statusField[i].sprite = null;
            }
            if (beforeSprite == null && this.statusField[i].sprite != null)
            {
				if (game.Fields [i].Winner == USER_TURN) {
					var deck = game.Fields [i].CardList [USER_TURN];
					if (deck [0].Color == deck [1].Color && deck [1].Color == deck [2].Color) {
						achievements.Concolor ();
					}
					else if (deck[0].Power == deck[1].Power && deck[1].Power == deck[2].Power)
					{
						achievements.Equiv ();
					}
				}

				this.statusField[i].transform.localScale = new Vector3(3000, 3000, 1);
                audioSource.clip = soundJudge;
                audioSource.Play();
            }

            this.southScore[i].Number = game.Fields[i].getScore(USER_TURN);
            this.northScore[i].Number = game.Fields[i].getScore(AI_TURN);

            this.southScoreObject[i].SetActive(this.southScore[i].Number > 0);
            this.northScoreObject[i].SetActive(this.northScore[i].Number > 0);

            var userDeck = game.Fields[i].CardList[USER_TURN];
            var aiDeck = game.Fields[i].CardList[AI_TURN];
            foreach (LilyAcolasia.Card card in userDeck.Concat(aiDeck))
            {
                if (card.Color == game.Fields[i].Color)
                {
                    bonus.Add(card.Name);
                }
            }

            southBonus[i].sprite = null;
            northBonus[i].sprite = null;
            if (userDeck.Count() == 3)
            {
                if (userDeck[0].Color == userDeck[1].Color && userDeck[1].Color == userDeck[2].Color)
                {
                    southBonus[i].sprite = concolorBonus;
                }
                else if (userDeck[0].Power == userDeck[1].Power && userDeck[1].Power == userDeck[2].Power)
                {
                    southBonus[i].sprite = equivBonus;
                }
            }
            if (aiDeck.Count() == 3)
            {
                if (aiDeck[0].Color == aiDeck[1].Color && aiDeck[1].Color == aiDeck[2].Color)
                {
                    northBonus[i].sprite = concolorBonus;
                }
                else if (aiDeck[0].Power == aiDeck[1].Power && aiDeck[1].Power == aiDeck[2].Power)
                {
                    northBonus[i].sprite = equivBonus;
                }
            }
        }

        switch (game.Status) {
            case LilyAcolasia.GameStatus.Status.First:
                if (game.Fields.Count(f => f.CardList[game.Turn].Count() > 0) == 0)
                {
                    this.statusSet.sprite = turnSet1;
                }
                else
                {
                    this.statusSet.sprite = turnRemove;
                }
                break;
            case LilyAcolasia.GameStatus.Status.Trashed:
                this.statusSet.sprite = turnSet1;
                break;
            case LilyAcolasia.GameStatus.Status.Discarded:
                this.statusSet.sprite = turnSet2;
                break;
        }
		if (this.lastTurn != game.Turn) {
			GameEffect.Special9 (-1);
			this.lastTurn = game.Turn;
		}
		if (game.Card9[game.Turn] == LilyAcolasia.Constants.CARD9_AFFECTED)
        {
			if (game.Status != LilyAcolasia.GameStatus.Status.First && game.Status != LilyAcolasia.GameStatus.Status.WaitSpecialInput) {
				GameEffect.Special9 (game.Turn);
			}

            if (game.Player.Hand.Count() == 3)
            {
                this.statusSet.sprite = turnSet3;
            }
            else if (game.Player.Hand.Count() == 2)
            {
                this.statusSet.sprite = turnSet4;
            }
            else if (game.Player.Hand.Count() == 1)
            {
                this.statusSet.sprite = turnSet5;
            }
        }

        if (game.Turn == USER_TURN)
        {
            this.statusTurn.sprite = turnYour;
        }
        else
        {
            this.statusTurn.sprite = turnEnemy;
        }

        foreach (string c in this.cardScripts.Keys)
        {
            cardScripts[c].SameBonus = bonus.Contains(c);
        }
        this.talonCount.Number = game.Talon.Deck.Count();
        this.trashCount.Number = game.Talon.Trash.Count();
        UpdateField();
    }

    private void CardMove(LilyAcolasia.CardDeck from, LilyAcolasia.CardDeck to, LilyAcolasia.Card card)
    {
        GameObject toObj = GameObject.Find(to.Name);
        DragUI ui = this.cards[card.Name].GetComponent<DragUI>();
        ui.Move(toObj.transform);

        this.frameCounter = 0;
        this.currentGameState = GameState.CmdWait;

        if (to.Name == "Trash")
        {
            audioSource.clip = soundTrash;
            if (this.gameOperator.Round.Current.Status == LilyAcolasia.GameStatus.Status.WaitSpecialInput)
            {
                if (this.gameOperator.Round.Current.LastTrashed.Power == 1)
                {
                    audioSource.clip = soundSkillLink;

                }
                else if (this.gameOperator.Round.Current.LastTrashed.Power == 7)
                {
                    audioSource.clip = soundSkillSnipe;
                }
            }
            audioSource.Play();
        }
        else if (from.Name == "Talon")
        {
            audioSource.clip = soundDraw;
            audioSource.Play();
        }
        else if (to.Name.Contains("Field"))
        {
            audioSource.clip = soundDiscard;
            audioSource.Play();
        }
    }

    private void CreateCards()
    {
        cardParents.Add(this.talon);
        cardParents.Add(this.trash);
        cardParents.Add(this.northHand);
        cardParents.Add(this.southHand);
        for (int i = 0; i < 5; i++)
        {
            cardParents.Add(this.northField[i]);
            cardParents.Add(this.southField[i]);
        }

        Vector3 last = this.talon.transform.position + new Vector3(0, 1, 0);
        foreach (LilyAcolasia.Card card in LilyAcolasia.CardDeck.List[LilyAcolasia.Constants.DECK_TALON])
        {
            string name = card.Name;
            this.cards[name] = Instantiate(cardPrefab);
            this.cards[name].transform.parent = this.talon.transform;
            this.cards[name].transform.position = last;

            DragUI ui = this.cards[name].GetComponent<DragUI>();
            ui.Init(name, operationHandler, this);
            this.cardScripts[name] = ui;

            last += new Vector3(0, 0.1f, 0);
        }
    }

    private bool operationHandler(string from, string to, string cardStr)
    {
        if (this.currentGameState != GameState.UserAction)
        {
            return false;
        }
        LilyAcolasia.GameInput input = this.gameEnumerator.Current;

        if (from.Contains("Hand") && to.Contains("Field")) {
            int field = to[to.IndexOf("Field") + 5] - '0';
			input.input(LilyAcolasia.Command.Discard, cardStr, field.ToString());
        }
        else if (from.Contains("Field") && to == "Trash")
        {
            int field = from[from.IndexOf("Field") + 5] - '0';
            LilyAcolasia.CardGame game = this.gameOperator.Round.Current;
            if (game.Status == LilyAcolasia.GameStatus.Status.WaitSpecialInput)
            {
                if (game.LastTrashed.Power == 5)
                {
                    return false;
                }
				input.input(LilyAcolasia.Command.Special, cardStr, field.ToString());
            }
            else
            {
				input.input(LilyAcolasia.Command.Trash, cardStr, field.ToString());
            }
        }
        else if (from.Contains("Hand") && to == "Trash" && this.gameOperator.Round.Current.IsFilled)
        {
            input.input(LilyAcolasia.Command.Next);
            this.gameEnumerator.MoveNext();
            return false;
        }
        else
        {
            return false;
        }

        this.gameEnumerator.MoveNext();
        Debug.Log(gameOperator.Round.Current.ToString());
        Debug.Log(gameOperator.Round.Current.Status);

        GameEffect.Special(-1, gameOperator.Round.Current.Turn);
        if (this.gameOperator.Round.Current.Status == LilyAcolasia.GameStatus.Status.WaitSpecialInput)
        {
            GameEffect.CutIn(this.gameOperator.Round.Current.LastTrashed.Power);
            audioSource.clip = soundCutIn;
            audioSource.Play();
            this.currentGameState = GameState.CutIn;
        }
        else
        {
            this.currentGameState = GameState.CmdWait;
        }

        return this.observer.IsCmdSuccess;
    }

    private void FindObjects()
    {
        this.talon = GameObject.Find("Talon");
        this.trash = GameObject.Find("Trash");
        this.northHand = GameObject.Find("NorthHand");
        this.southHand = GameObject.Find("SouthHand");
        this.colorChoice = GameObject.Find("ColorChoice");

        for (int i = 0; i < 5; i++)
        {
            string northName = "NorthField" + i;
            string southName = "SouthField" + i;
            string wrapperName = "FieldWrapper" + i;
            this.northField[i] = GameObject.Find(northName);
            this.southField[i] = GameObject.Find(southName);
            this.FieldWrapper[i] = GameObject.Find(wrapperName);

            var obj = GameObject.Find("StatusField" + i);
            this.statusField[i] = obj.GetComponent<SpriteRenderer>();

            this.southScoreObject[i] = GameObject.Find("SouthScore" + i);
            this.northScoreObject[i] = GameObject.Find("NorthScore" + i);

            this.northScore[i] = northScoreObject[i].GetComponent<Digit>();
            this.southScore[i] = southScoreObject[i].GetComponent<Digit>();
            this.northBonus[i] = GameObject.Find("NorthBonus" + i).GetComponent<SpriteRenderer>();
            this.southBonus[i] = GameObject.Find("SouthBonus" + i).GetComponent<SpriteRenderer>();
        }

        this.statusSet = GameObject.Find("StatusSet").GetComponent<SpriteRenderer>();
        this.statusTurn = GameObject.Find("StatusTurn").GetComponent<SpriteRenderer>();
        this.talonCount = GameObject.Find("TalonCount").GetComponent<Digit>();
        this.trashCount = GameObject.Find("TrashCount").GetComponent<Digit>();

        AudioSource[] audioSources = GetComponents<AudioSource>();
        this.audioSource = audioSources[0];
        this.bgmSource = audioSources[1];
    }

    private void UpdateField()
    {
        LilyAcolasia.CardGame game = this.gameOperator.Round.Current;
        for (int i = 0; i < 5; i++)
        {
            SpriteRenderer wrapperMaterial = this.FieldWrapper[i].GetComponent<SpriteRenderer>();

            if (game.Fields[i].Color == LilyAcolasia.Color.Blue)
            {
                wrapperMaterial.sprite = areaBlue;
            }
            else if (game.Fields[i].Color == LilyAcolasia.Color.Green)
            {
                wrapperMaterial.sprite = areaGreen;
            }
            else if (game.Fields[i].Color == LilyAcolasia.Color.White)
            {
                wrapperMaterial.sprite = areaWhite;
            }
            else if (game.Fields[i].Color == LilyAcolasia.Color.Red)
            {
                wrapperMaterial.sprite = areaRed;
            }
            else if (game.Fields[i].Color == LilyAcolasia.Color.Black)
            {
                wrapperMaterial.sprite = areaBlack;
            }
        }
    }

    public void Init()
    {
        //this.gameOperator.Round.Init();
        this.gameEnumerator = this.gameOperator.Iterator().GetEnumerator();
        StartRound();
    }

    private void StartRound()
    {
        this.gameEnumerator.MoveNext();
        LilyAcolasia.CardGame game = this.gameOperator.Round.Current;
		this.lastTurn = this.gameOperator.Round.Current.Turn;
        Debug.Log(game.ToString());
        currentGameState = GameState.RoundStart;
    }
}

class GameObserverImpl : LilyAcolasia.IGameObserver
{
    private readonly GameMaster master;
	private readonly PhotonNetworkPlayer user;
	public GameObserverImpl(GameMaster master, PhotonNetworkPlayer user)
    {
        this.master = master;
		this.user = user;
    }

    public bool IsCmdSuccess { get; set; }

    private static readonly Dictionary<string, string> ErrorMessages = new Dictionary<string, string>() 
        {
            { LilyAcolasia.GameExceptionType.FIELD_INDEX_EXCEPTION, "The field index is out of range."},
            { LilyAcolasia.GameExceptionType.CARD_STRING_EXCEPTION, "The card representation string is invalid."},
            { LilyAcolasia.GameExceptionType.ALREADY_DISCARDED_EXCEPTION, "You have already discarded a card."},
            { LilyAcolasia.GameExceptionType.NOT_DISCARDED_EXCEPTION, "You have not discarded any card yet."},
            { LilyAcolasia.GameExceptionType.ALREADY_TRASHED_EXCEPTION, "You have already trashed a card."},
            { LilyAcolasia.GameExceptionType.CARD_ALREADY_FIXED_EXCEPTION, "The card list in the field is already fixed."},
            { LilyAcolasia.GameExceptionType.CARD_NOT_FOUND_EXCEPTION, "The card is not found."},
            { LilyAcolasia.GameExceptionType.CARD_TOO_MANY_EXCEPTION, "The number of cards is too many."},
            { LilyAcolasia.GameExceptionType.NO_CARD_EXCEPTION, "No such card exists."},
        };

	public void Input(LilyAcolasia.GameRound round, LilyAcolasia.GameInput input) {
		if (this.user != null && round.Current.Turn == 1) {
			this.user.send (input.ToString ());
		}
	}

	public void RoundStart(LilyAcolasia.GameRound round)
    {
        Debug.Log("Round" + round.Round + " start!");
        this.master.UpdateStatus();
    }

    public void GameStart(LilyAcolasia.GameRound round)
    {
        Debug.Log("Game start!");
        this.master.UpdateStatus();
    }

    public void GameEnd(LilyAcolasia.GameRound round)
    {
        Debug.Log("Result: " + round.Point1 + ":" + round.Point2);
        this.master.UpdateStatus();
    }

    public void RoundEnd(LilyAcolasia.GameRound round)
    {
        Debug.Log(round.Current.ToString());
        int winner = round.Current.Winner;
        string mesage;
        if (winner == 2)
        {
            mesage = "Round" + round.Round + " Even.";
        }
        else
        {
            mesage = "Round" + round.Round + " " + round.Current.Players[winner].Name + " won!";
        }
        Debug.Log(mesage);
        this.master.UpdateStatus();
    }

    public void TurnStart(LilyAcolasia.GameRound round)
    {
        Debug.Log("============ Turn Start ============");
        this.master.UpdateStatus();
    }

    public void TurnEnd(LilyAcolasia.GameRound round)
    {
        Debug.Log("============ Turn End ============");
        this.master.UpdateStatus();
    }

    public void CmdEnd(LilyAcolasia.GameRound round)
    {

        if (round.Current.Status == LilyAcolasia.GameStatus.Status.WaitSpecialInput)
        {
        }
        IsCmdSuccess = true;
        this.master.UpdateStatus();
    }

    public void CmdError(LilyAcolasia.GameRound round, LilyAcolasia.GameException ex)
    {
        IsCmdSuccess = false;
        Debug.Log("Error: " + ErrorMessages[ex.Type]);
        this.master.UpdateStatus();
    }
}