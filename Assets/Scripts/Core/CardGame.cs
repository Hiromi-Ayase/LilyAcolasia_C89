using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEngine;

namespace LilyAcolasia
{
    public class Constants
    {
        public const string DECK_TALON = "Talon";
        public const string DECK_PLAYER0 = "NorthHand";
        public const string DECK_PLAYER1 = "SouthHand";
        public const string DECK_TRASH = "Trash";
        public const string DECK_NORTH_FIELD = "NorthField";
        public const string DECK_SOUTH_FIELD = "SouthField";

        public const int CARD9_AFFECTED = 2;
        public const int CARD9_NONE = 0;
    }

    /// <summary>
    /// Random class for the Game.
    /// </summary>
    public class GameRandom
    {
		private const long A = 3572345621;
		private const long B = 1000000007;
		private long now;

		public GameRandom(long seed)
		{
			this.now = seed % B;
		}

		/// <summary>
		/// Get the next color.
		/// </summary>
		/// <returns></returns>
		public int Next()
		{
			now = now * A % B;
			Debug.Log (now);
			return (int)now;
		}
    }

    /// <summary>
    /// Color class.
    /// </summary>
    public class Color
    {
        /// <summary>
        /// Red.
        /// </summary>
        public static readonly Color Red = new Color("R");
        /// <summary>
        /// Green.
        /// </summary>
        public static readonly Color Green = new Color("G");
        /// <summary>
        /// Blue.
        /// </summary>
        public static readonly Color Blue = new Color("B");
        /// <summary>
        /// Yellow.
        /// </summary>
        public static readonly Color Black = new Color("Y");
        /// <summary>
        /// Purple.
        /// </summary>
        public static readonly Color White = new Color("W");
        /// <summary>
        /// All colors list.
        /// </summary>
        public static readonly Color[] List = { Red, Green, Blue, Black, White };

        private readonly string name;
        private Color(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Get the string representation.
        /// </summary>
        /// <returns>Color string representation.</returns>
        public override string ToString()
        {
            return this.name;
        }

        /// <summary>
        /// Get the color instance from the color character.
        /// </summary>
        /// <param name="c">Character of color.</param>
        /// <returns>Color instance.</returns>
        public static Color Get(char c)
        {
            switch (c)
            {
                case 'R':
                    return Red;
                case 'G':
                    return Green;
                case 'B':
                    return Blue;
                case 'Y':
                    return Black;
                case 'W':
                    return White;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get random color.
        /// </summary>
        /// <returns>Color instance.</returns>
        public static Color GetRandom(GameRandom random)
        {
            return Color.List[random.Next() % Color.List.Length];
        }
    }

    /// <summary>
    /// Card class.
    /// </summary>
    public class Card : IComparable<Card>
    {
        private static readonly Regex regex;
        private readonly int power;
        private readonly Color color;
        private readonly string name;
        private readonly bool hasSpecialInput;
        private readonly bool hasSpecial;

        /// <summary>
        /// Card power.
        /// </summary>
        public int Power { get { return this.power; } }
        /// <summary>
        /// Card color.
        /// </summary>
        public Color Color { get { return this.color; } }
        /// <summary>
        /// Card name.
        /// </summary>
        public string Name { get { return this.name; } }
        /// <summary>
        /// Has special input?
        /// </summary>
        public bool HasSpecialInput { get { return this.hasSpecialInput; } }
        /// <summary>
        /// Has special?
        /// </summary>
        public bool HasSpecial { get { return this.hasSpecial; } }

        static Card()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Color color in Color.List)
            {
                sb.Append(color.ToString());
            }
            regex = new Regex("[" + sb.ToString() + "][1-9]");
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="power">Power.</param>
        /// <param name="color">Color.</param>
        public Card(int power, Color color)
        {
            this.power = power;
            this.color = color;
            this.name = String.Format("{0}{1}", color.ToString(), power);
            this.hasSpecialInput = power == 1 || power == 7 || power == 5;
            this.hasSpecial = power % 2 == 1;
        }

        /// <summary>
        /// The special skill of this card.
        /// </summary>
        /// <param name="game">Game instance.</param>
        /// <param name="opt">Input option parameter.</param>
        /// <returns>Result.</returns>
        public bool Special(CardGame game, params string[] opt)
        {
            bool ret = true;
            if (this.power == 1)
            {
                Field field = game.Fields[Int32.Parse(opt[1])];
                if (field.IsFixed)
                {
                    throw GameException.getCardAlreadyFixedException();
                }
                string cardStr = opt[0];
                game.Trash(field.Number, cardStr);
                //CardDeck fieldDeck = field.CardList[game.Turn];
                //fieldDeck.Move(cardStr, game.Talon.Trash);
                ret = false;
            }
            else if (this.power == 3)
            {
                int count = game.Player.Hand.Count();
                game.Player.Hand.MoveAll(game.Talon.Trash);
                for (int i = 0; i < count; i++)
                {
                    game.Draw();
                }
            }
            else if (this.power == 5)
            {
                Field field = game.LastTrashedField;
                char c = ((string)opt[0])[0];
                field.Color = Color.Get(c);
            }
            else if (this.power == 7)
            {
                Field field = game.Fields[Int32.Parse(opt[1])];
                if (field.IsFixed)
                {
                    throw GameException.getCardAlreadyFixedException();
                }
                string cardStr = (string)opt[0];
                CardDeck fieldDeck = field.CardList[(game.Turn + 1) % 2];
                fieldDeck.Move(cardStr, game.Talon.Trash);
            }
            else if (this.power == 9)
            {
				game.Card9[(game.Turn + 1) % 2] = Constants.CARD9_AFFECTED;
            }
            return ret;
        }

        /// <summary>
        /// String representation for the card.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return String.Format("[{0}]", this.name);
        }

        /// <summary>
        /// Judge if the card string equals to this card.
        /// </summary>
        /// <param name="cardStr">The card name.</param>
        /// <returns>True: same  False: different.</returns>
        public bool IsMatch(string cardStr)
        {
            if (!regex.IsMatch(cardStr))
            {
                throw GameException.getCardStrException(cardStr);
            }
            return this.name == cardStr;
        }

        /// <summary>
        /// Comparator.
        /// </summary>
        /// <param name="other">Other card instance.</param>
        /// <returns>Comparison result.</returns>
        public int CompareTo(Card other)
        {
            char c1 = this.color.ToString()[0];
            char c2 = other.color.ToString()[0];
            if (c1 == c2)
            {
                return this.power - other.power;
            }
            else
            {
                return c1 - c2;
            }
        }
    }

    /// <summary>
    /// Player class.
    /// </summary>
    public class Player
    {
        private readonly CardDeck hand;
        private readonly String name;

        /// <summary>
        /// Player's hand.
        /// </summary>
        public CardDeck Hand { get { return this.hand; } }
        /// <summary>
        /// Player's name.
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Player's name.</param>
        public Player(string name, int turn)
        {
            this.name = name;
            this.hand = new CardDeck(turn == 0 ? Constants.DECK_PLAYER0 : Constants.DECK_PLAYER1);
        }

        /// <summary>
        /// String represantation for this player.
        /// </summary>
        /// <returns>String represantation.</returns>
        public override string ToString()
        {
            return String.Format("{0}: {1}", name, this.hand.ToString());
        }

        /// <summary>
        /// Initialize the player hand.
        /// </summary>
        /// <param name="trash">Trash.</param>
        public void Init(CardDeck trash)
        {
            this.hand.MoveAll(trash);
        }
    }

    /// <summary>
    /// Card deck class.
    /// </summary>
    public class CardDeck : IEnumerable<Card>
    {
        public delegate void CardMoveHandler(CardDeck from, CardDeck to, Card card);
        private static CardMoveHandler handler = (from, to, Card) => { };
        private static Dictionary<string, CardDeck> deckList = new Dictionary<string, CardDeck>();

        private readonly int size;
        private readonly string name;
        private List<Card> list = new List<Card>();

        /// <summary>
        /// Constructor by the default card.
        /// </summary>
        /// <param name="size">Size.</param>
        public CardDeck(List<Card> list, string name)
        {
            this.size = list.Count;
            this.list = list;
            this.name = name;
            CardDeck.List[name] = this;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="size">Size.</param>
        /// <param name="name">Name.</param>
        public CardDeck(int size, string name)
        {
            this.size = size;
            this.name = name;
            CardDeck.List[name] = this;
        }

        /// <summary>
        /// Constructor for infinite size deck.
        /// </summary>
        /// <param name="name">Name.</param>
        public CardDeck(string name)
        {
            this.size = -1;
            this.name = name;
        }

        /// <summary>
        /// Get the card by index.
        /// </summary>
        /// <param name="i">Index.</param>
        /// <returns>Card instance.</returns>
        public Card this[int i]
        {
            get { return this.list[i]; }
        }

        /// <summary>
        /// Draw a card from this talon and send it to another deck.
        /// </summary>
        /// <param name="to">The deck to send the card.</param>
        /// <returns>Drawn card.</returns>
        public Card Draw(CardDeck to)
        {
            if (this.list.Count() == 0)
            {
                throw GameException.getNoCardException();
            }
            Card card = this.list.First();
            CardDeck.handler(this, to, card);
            this.Move(card.Name, to);
            return card;
        }

        /// <summary>
        /// Move the specified card to the another deck.
        /// </summary>
        /// <param name="cardStr">card string.</param>
        /// <param name="to">Target deck.</param>
        /// <returns>Card instance.</returns>
        public Card Move(string cardStr, CardDeck to)
        {
            if (to.size >= 0 && to.list.Count() == to.size)
            {
                throw GameException.getCardTooManyException();
            }

            Card card = this.Get(cardStr);

            if (card == null)
            {
                throw GameException.getNoCardException();
            }

            if (!this.list.Remove(card))
            {
                throw new Exception();
            }
            CardDeck.handler(this, to, card);
            to.list.Add(card);
            return card;
        }

        /// <summary>
        /// Move all card to the deck.
        /// </summary>
        /// <param name="to">Target deck.</param>
        public void MoveAll(CardDeck to)
        {
            if (to.size >= 0 && to.list.Count() > to.size - this.list.Count)
            {
                throw GameException.getCardTooManyException();
            }
            List<string> list = new List<string>();
            foreach (Card card in this.list)
            {
                list.Add(card.Name);
            }
            foreach (string name in list)
            {
                this.Move(name, to);
            }
        }

        /// <summary>
        /// Get the card by string representation.
        /// </summary>
        /// <param name="cardStr">String representation.</param>
        /// <returns>Card instance.</returns>
        public Card Get(string cardStr)
        {
            foreach (Card card in this.list)
            {
                if (card.IsMatch(cardStr))
                {
                    return card;
                }
            }
            return null;
        }

        /// <summary>
        /// Shuffle the card.
        /// </summary>
        public void Shuffle(GameRandom random)
        {
            this.list = this.list.OrderBy(i => random.Next()).ToList();
        }

        /// <summary>
        /// String represantation for this deck.
        /// </summary>
        /// <returns>String represantation.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Math.Max(this.size, this.list.Count()); i++)
            {
                if (i < this.list.Count())
                {
                    sb.Append(this.list[i].ToString());
                }
                else
                {
                    sb.Append("    ");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// IEnumerable implementation.
        /// </summary>
        /// <returns>IEnumerable instance.</returns>
        public IEnumerator<Card> GetEnumerator()
        {
            foreach (Card card in this.list)
            {
                yield return card;
            }
        }

        /// <summary>
        /// IEnumerable implementation.
        /// </summary>
        /// <returns>IEnumerable instance.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Set the handler.
        /// </summary>
        public static CardMoveHandler Handler { set { CardDeck.handler = value; } }

        /// <summary>
        /// Get the all decks.
        /// </summary>
        public static Dictionary<string, CardDeck> List { get { return CardDeck.deckList; } }

        /// <summary>
        /// Get the name.
        /// </summary>
        public string Name { get { return this.name; } }
    }

    /// <summary>
    /// Talon class.
    /// </summary>
    public class Talon
    {
        private CardDeck deck = null;
        private CardDeck trash = new CardDeck(Constants.DECK_TRASH);
        private GameRandom random;

        public CardDeck Deck { get { return this.deck; } }
        public CardDeck Trash { get { return this.trash; } }

        public Talon(GameRandom random)
        {
            List<Card> list = new List<Card>();
            foreach (Color color in Color.List)
            {
                for (int i = 1; i <= 9; i++)
                {
                    list.Add(new Card(i, color));
                }
            }
            this.deck = new CardDeck(list, Constants.DECK_TALON);
            this.random = random;
            deck.Shuffle(random);
        }

        /// <summary>
        /// Initialize the talon.
        /// </summary>
        public void Init()
        {
            trash.MoveAll(deck);
            deck.Shuffle(this.random);
        }

        /// <summary>
        /// String represantation for this deck.
        /// </summary>
        /// <returns>String represantation.</returns>
        public override string ToString()
        {
            return String.Format("Talon: {0}", deck.ToString());
        }

        /// <summary>
        /// True: empty deck  False: other.
        /// </summary>
        public bool IsEmpty
        {
            get { return this.deck.Count() == 0; }
        }
    }

    /// <summary>
    /// Field class.
    /// </summary>
    public class Field
    {
        private const int MAX_DISCARD = 3;
        private const int PLAYER_NUM = 2;

        private const int SCORE_FIELD_MATCH = 2;
        private const int SCORE_SAME_COLOR = 3;
        private const int SCORE_SAME_NUMBER = 3;

        private readonly int number;
        private readonly CardDeck[] cardList = new CardDeck[PLAYER_NUM];
        private int win = -1;
        private Color color;

        /// <summary>
        /// Card list.
        /// </summary>
        public CardDeck[] CardList { get { return this.cardList; } }
        /// <summary>
        /// Winner(-1:Not fixed  0:Player1 won  1:Player2 won  2:Even).
        /// </summary>
        public int Winner { get { return this.win; } }
        /// <summary>
        /// Is the field result fixed? (True: fixed. False: Not fixed).
        /// </summary>
        public bool IsFixed { get { return this.win >= 0; } }
        /// <summary>
        /// Field number.
        /// </summary>
        public int Number { get { return this.number; } }
        /// <summary>
        /// Color.
        /// </summary>
        public Color Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="number">Field number.</param>
        public Field(int number, GameRandom random)
        {
            this.number = number;
            this.color = Color.GetRandom(random);
            cardList[0] = new CardDeck(MAX_DISCARD, Constants.DECK_NORTH_FIELD + number);
            cardList[1] = new CardDeck(MAX_DISCARD, Constants.DECK_SOUTH_FIELD + number);
        }

        /// <summary>
        /// Get the card list.
        /// </summary>
        /// <param name="turn">Turn number.</param>
        /// <returns>CardDeck instance.</returns>
        public CardDeck GetCardList(int turn)
        {
            return this.CardList[turn];
        }

        /// <summary>
        /// Initialize the field.
        /// </summary>
        /// <param name="trash">Trash</param>
        public void Init(CardDeck trash)
        {
            this.win = -1;
            this.cardList[0].MoveAll(trash);
            this.cardList[1].MoveAll(trash);
        }

        /// <summary>
        /// Get the score.
        /// </summary>
        /// <param name="turn">Turn number.</param>
        /// <returns>Score.</returns>
        public int getScore(int turn)
        {
            int score = 0;
            CardDeck list = this.CardList[turn];

            foreach (Card card in this.CardList[turn])
            {
                score += card.Power;
                if (card.Color == this.color)
                {
                    score += SCORE_FIELD_MATCH;
                }
            }

            if (list.Count() == MAX_DISCARD)
            {
                Color color = list[0].Color;
                int power = list[0].Power;
                for (int i = 0; i < MAX_DISCARD; i++)
                {
                    if (color != list[i].Color)
                    {
                        color = null;
                    }
                    if (power != list[i].Power)
                    {
                        power = -1;
                    }
                }
                if (color != null)
                {
                    score += SCORE_SAME_COLOR;
                }
                if (power >= 0)
                {
                    score += SCORE_SAME_NUMBER;
                }
            }
            return score;
        }

        /// <summary>
        /// Evaluate this field.
        /// If there are 3 cards in the both side, the field will be fixed.
        /// </summary>
        /// <returns>-1:Not fixed, 0:Player1 won, 1:Player2 won, 2:Even</returns>
        public int Eval()
        {
            if (Math.Min(this.cardList[0].Count(), this.cardList[1].Count()) < MAX_DISCARD)
            {
                return this.win;
            }

            if (this.win < 0)
            {
                int score1 = getScore(1);
                int score0 = getScore(0);

                if (score1 == score0)
                {
                    this.win = 2;
                }
                else if (score1 > score0)
                {
                    this.win = 1;
                }
                else
                {
                    this.win = 0;
                }
            }
            return this.win;
        }

        /// <summary>
        /// Get the string representation of the Field.
        /// </summary>
        /// <param name="before">The before field for cascade.</param>
        /// <returns>String representation.</returns>
        public string GetString(string before)
        {
            if (before == null)
            {
                before = "\n\n\n\n\n";
            }
            string[] lines = before.Split('\n');

            StringBuilder sb1 = new StringBuilder();
            sb1.Append(" ");
            for (int i = 0; i < MAX_DISCARD; i++)
            {
                if (i < cardList[0].Count())
                {
                    sb1.Append(cardList[0][i].ToString());
                }
                else
                {
                    sb1.Append("    ");
                }
            }
            sb1.Append("  ");
            lines[0] += sb1.ToString();

            string winner;
            switch (win)
            {
                case 0:
                    winner = "0";
                    break;
                case 1:
                    winner = "1";
                    break;
                case PLAYER_NUM:
                    winner = "E";
                    break;
                default:
                    winner = "-";
                    break;
            }

            lines[1] += "-------------- ";
            // R: 02 vs 03 (-) 
            lines[2] += String.Format("[{0}] {1}:{2, 2}v{3,-2}({4}) ", number, color.ToString(), getScore(0), getScore(1), winner);
            lines[3] += "-------------- ";

            StringBuilder sb3 = new StringBuilder();
            sb3.Append(" ");
            for (int i = 0; i < MAX_DISCARD; i++)
            {
                if (i < cardList[1].Count())
                {
                    sb3.Append(cardList[1][i].ToString());
                }
                else
                {
                    sb3.Append("    ");
                }
            }
            sb3.Append("  ");
            lines[4] += sb3.ToString();
            return String.Join("\n", lines);
        }
    }

    /// <summary>
    /// Game status class.
    /// </summary>
    public class GameStatus
    {
        /// <summary>
        /// Action enum.
        /// </summary>
        public enum Action
        {
            Discard, Trash, Special, Next,
        }
        /// <summary>
        /// Status enum.
        /// </summary>
        public enum Status
        {
            First, Trashed, WaitSpecialInput, Discarded, End, RoundEnd, Error
        }

        private Status status = Status.First;

        /// <summary>
        /// Current status.
        /// </summary>
        public Status Current
        {
            get { return this.status; }
            set { this.status = value; }
        }

        /// <summary>
        /// Initialize the status.
        /// </summary>
        public void Init()
        {
            this.status = Status.First;
        }

        /// <summary>
        /// Transition the status.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <returns>Next status.</returns>
        public Status Act(Action action)
        {
            Status next = Status.Error;

            switch (this.status)
            {
                case Status.First:
                    switch (action)
                    {
                        case Action.Trash:
                            next = Status.WaitSpecialInput;
                            break;
                        case Action.Discard:
                            next = Status.Discarded;
                            break;
                    }
                    break;
                case Status.WaitSpecialInput:
                    switch (action)
                    {
                        case Action.Special:
                            next = Status.Trashed;
                            break;
                        case Action.Trash:
                            next = Status.WaitSpecialInput;
                            break;
                    }
                    break;
                case Status.Trashed:
                    switch (action)
                    {
                        case Action.Discard:
                            next = Status.Discarded;
                            break;
                    }
                    break;
                case Status.Discarded:
                    switch (action)
                    {
                        case Action.Discard:
                            next = Status.End;
                            break;
                    }
                    break;
                case Status.End:
                    switch (action)
                    {
                        case Action.Next:
                            next = Status.First;
                            break;
                    }
                    break;
            }
            return next;
        }
    }

    /// <summary>
    /// Card game class.
    /// </summary>
    public class CardGame
    {
        private const int PLAYER_NUM = 2;
        private const int FIELD_NUM = 5;
        private const int INIT_CARD = 3;

        private readonly Field[] fields = new Field[FIELD_NUM];
        private readonly Player[] players = new Player[PLAYER_NUM];
        private readonly GameStatus status = new GameStatus();
        private readonly Talon talon;
        private readonly GameRandom random;
		private readonly bool rev;

        private int turn;
		private int[] card9 = {Constants.CARD9_NONE, Constants.CARD9_NONE};
        private Card lastTrashed = null;
        private Field lastTrashedField = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="playerA">Player1 name</param>
        /// <param name="playerB">Player2 name</param>
		public CardGame(string playerA, string playerB, GameRandom random, bool rev)
        {
            this.players[0] = new Player(playerA, 0);
            this.players[1] = new Player(playerB, 1);
            for (int i = 0; i < FIELD_NUM; i++)
            {
                this.fields[i] = new Field(i, random);
            }
            this.talon = new Talon(random);
            this.random = random;
			this.rev = rev;
        }

        /// <summary>
        /// Initialize the game.
        /// </summary>
        public void init()
        {
			this.turn = (random.Next() + (rev ? 1 : 0)) % PLAYER_NUM;
            this.lastTrashed = null;
            this.lastTrashedField = null;

            for (int i = 0; i < FIELD_NUM; i++)
            {
                this.fields[i].Init(this.talon.Trash);
            }
            this.players[rev ? 1 : 0].Init(this.talon.Trash);
			this.players[rev ? 0 : 1].Init(this.talon.Trash);
            talon.Init();
            for (int i = 0; i < INIT_CARD; i++)
            {
				this.talon.Deck.Draw(this.players[rev ? 1 : 0].Hand);
				this.talon.Deck.Draw(this.players[rev ? 0 : 1].Hand);
            }
            this.talon.Deck.Draw(this.players[this.turn].Hand);
            this.talon.Deck.Draw(this.players[this.turn].Hand);
            this.status.Current = GameStatus.Status.First;
        }

        /// <summary>
        /// Proceed to the next turn.
        /// </summary>
        public void nextTurn()
        {
            foreach (Field field in this.fields)
            {
                field.Eval();
            }

            if (this.Winner >= 0)
            {
                this.status.Current = GameStatus.Status.RoundEnd;
                return;
            }

            GameStatus.Status nextStatus = this.IsFilled ? GameStatus.Status.First : status.Act(GameStatus.Action.Next);
            if (GameStatus.Status.Error == nextStatus)
            {
                throw GameException.getNotTrashedException();
            }
			this.card9 [this.turn] = Constants.CARD9_NONE;
            this.turn = (this.turn + 1) % 2;
            this.lastTrashedField = null;
            this.lastTrashed = null;

            int drawNum = 5 - this.Player.Hand.Count();
            for (int i = 0; i < drawNum; i++)
            {
                this.Draw();
            }
            this.status.Current = nextStatus;
        }

        /// <summary>
        /// Talon.
        /// </summary>
        public Talon Talon { get { return this.talon; } }
        /// <summary>
        /// Players.
        /// </summary>
        public Player[] Players { get { return this.players; } }
        /// <summary>
        /// Current player.
        /// </summary>
        public Player Player { get { return this.players[this.turn]; } }
        /// <summary>
        /// Fileds.
        /// </summary>
        public Field[] Fields { get { return this.fields; } }
        /// <summary>
        /// Turn.
        /// </summary>
        public int Turn { get { return this.turn; } }
        /// <summary>
        /// Trashed flag.
        /// </summary>
        public GameStatus.Status Status { get { return this.status.Current; } }
        /// <summary>
        /// Card9 flg.
        /// </summary>
		public int[] Card9 { get { return this.card9; } }
        /// <summary>
        /// Last trashed card.
        /// </summary>
        public Card LastTrashed { get { return this.lastTrashed; } }
        /// <summary>
        /// Last trashed field.
        /// </summary>
        public Field LastTrashedField { get { return this.lastTrashedField; } }
        /// <summary>
        /// Is my field filled?
        /// </summary>
        public bool IsFilled { get { return fields.Where(f => f.CardList[turn].Count() == 3).Count() == FIELD_NUM; } }

        /// <summary>
        /// Winner.
        /// </summary>
        public int Winner
        {
            get
            {
                int before = -1;
                int[] evals = new int[2];
                int fixedField = 0;
                for (int i = 0; i < FIELD_NUM; i++)
                {
                    int eval = this.fields[i].Winner;
                    if (eval >= 0 && eval < PLAYER_NUM)
                    {
                        if (eval == before)
                        {
                            return eval;
                        }

                        evals[eval]++;
                        if (evals[eval] > Math.Floor((double)FIELD_NUM / 2 + 1))
                        {
                            return eval;
                        }
                    }
                    before = eval;

                    if (eval >= 0)
                    {
                        fixedField++;
                    }
                }

                if (fixedField == FIELD_NUM || Math.Max(this.players[0].Hand.Count(), this.players[1].Hand.Count()) == 0 && this.talon.IsEmpty && this.talon.Trash.Count() == 0)
                {
                    if (evals[0] == evals[1])
                    {
                        return PLAYER_NUM;
                    }
                    else
                    {
                        return evals[0] > evals[1] ? 0 : 1;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Draw the card from the talon.
        /// </summary>
        public void Draw()
        {
            if (this.talon.IsEmpty)
            {
                if (this.talon.Trash.Count() == 0)
                {
                    return;
                }
                else
                {

                    this.talon.Init();
                }
            }
            this.talon.Deck.Draw(this.players[this.turn].Hand);
        }

        /// <summary>
        /// Discard the card to the field.
        /// </summary>
        /// <param name="fieldIndex">Field index.</param>
        /// <param name="cardStr">Card string.</param>
        /// <returns>Discarded card instance.</returns>
        public Card Discard(int fieldIndex, string cardStr)
        {
            GameStatus.Status nextStatus = status.Act(GameStatus.Action.Discard);
            if (GameStatus.Status.Error == nextStatus)
            {
                throw GameException.getAlreadyDiscardedException();
            }

            if (fieldIndex >= FIELD_NUM || fieldIndex < 0)
            {
                throw GameException.getFieldIndexException();
            }

            CardDeck hand = this.players[turn].Hand;
            Card card = hand.Get(cardStr);
            if (card == null)
            {
                throw GameException.getCardNotFoundException(cardStr);
            }
            hand.Move(cardStr, this.fields[fieldIndex].CardList[turn]);

            if (this.players[turn].Hand.Count() == 0 || this.IsFilled)
            {
                status.Current = GameStatus.Status.End;
            }
            else if (this.card9[turn] == Constants.CARD9_AFFECTED)
            {
                status.Current = GameStatus.Status.Trashed;
            }
            else
            {
                status.Current = nextStatus;
            }
            return card;
        }

        /// <summary>
        /// Trash the card from the field.
        /// </summary>
        /// <param name="fieldIndex">Field index.</param>
        /// <param name="cardStr">Card string.</param>
        /// <returns>Trashed card instance.</returns>
        public Card Trash(int fieldIndex, string cardStr)
        {
            GameStatus.Status nextStatus = status.Act(GameStatus.Action.Trash);
            if (GameStatus.Status.Error == nextStatus)
            {
                throw GameException.getAlreadyTrashedException();
            }
            if (fieldIndex >= FIELD_NUM || fieldIndex < 0)
            {
                throw GameException.getFieldIndexException();
            }

            CardDeck deck = this.fields[fieldIndex].CardList[this.turn];
            if (this.fields[fieldIndex].IsFixed || deck.Last().Name != cardStr)
            {
                throw GameException.getCardAlreadyFixedException();
            }


            Card card = deck.Move(cardStr, this.talon.Trash);
            this.lastTrashed = card;
            this.lastTrashedField = this.fields[fieldIndex];

            status.Current = card.HasSpecial ? nextStatus : GameStatus.Status.Trashed;
            return card;
        }

        /// <summary>
        /// Fire the special effect of the card.
        /// </summary>
        /// <param name="opt">Special effect option.</param>
        public void Special(params string[] opt)
        {
            GameStatus.Status nextStatus = status.Act(GameStatus.Action.Special);
            if (GameStatus.Status.Error == nextStatus)
            {
                throw GameException.getAlreadyTrashedException();
            }
            bool ret = this.lastTrashed.Special(this, opt);
            if (ret)
            {
                status.Current = nextStatus;
            }
        }

        /// <summary>
        /// String representation for this game.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.talon.ToString());
            sb.AppendLine();
            sb.AppendLine();

            string fieldStr = null;
            foreach (Field field in this.fields)
            {
                fieldStr = field.GetString(fieldStr);
            }
            sb.Append(fieldStr);
            sb.AppendLine();

            sb.Append(this.turn == 0 ? "* " : "  ");
            sb.Append(this.players[0].ToString());
            sb.AppendLine();

            sb.Append(this.turn == 1 ? "* " : "  ");
            sb.Append(this.players[1].ToString());
            sb.AppendLine();

            return sb.ToString();
        }
    }

    /// <summary>
    /// Game round class.
    /// </summary>
    public class GameRound
    {
        private const int MAX = 1;

        private readonly CardGame game;
        private int point1 = 0;
        private int point2 = 0;
        private int round = 0;
        private bool evaluated = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="n">Round number.</param>
        /// <param name="name1">Player1 name.</param>
        /// <param name="name2">Player2 name.</param>
		public GameRound(int n, string name1, string name2, GameRandom random, bool rev)
        {
            this.game = new CardGame(name1, name2, random, rev);
        }

        /// <summary>
        /// Player1's point.
        /// </summary>
        public int Point1 { get { return this.point1; } }
        /// <summary>
        /// Player2's point.
        /// </summary>
        public int Point2 { get { return this.point2; } }
        /// <summary>
        /// Current round.
        /// </summary>
        public int Round { get { return this.round; } }
        /// <summary>
        /// Current game.
        /// </summary>
        public CardGame Current { get { return this.game; } }

        /// <summary>
        /// Start the game.
        /// </summary>
        public void NextRound()
        {
            this.round++;
            this.evaluated = false;
            this.Current.init();
        }

        /// <summary>
        /// Return if the round has a next turn or not.
        /// </summary>
        /// <returns>True: yes  False: no</returns>
        public bool HasNext()
        {
            if (this.Current.Status == GameStatus.Status.RoundEnd)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Return if a next round exists or not.
        /// </summary>
        /// <returns>True: yes  False: all games are finished.</returns>
        public bool HasNextRound()
        {
            if (!this.evaluated)
            {
                this.evaluated = true;
                int winner = this.Current.Winner;
                if (winner == 1)
                {
                    this.point2++;
                }
                else if (winner == 0)
                {
                    this.point1++;
                }
            }
            return this.round < MAX;
        }

        /// <summary>
        /// Initialize the game round.
        /// </summary>
        public void Init()
        {
            this.point1 = 0;
            this.point2 = 0;
            this.round = 0;
            this.game.init();
        }
    }


	/// <summary>
    /// Game observer interface.
    /// </summary>
    public interface IGameObserver
    {
		/// <summary>
		/// Called when the input is called.
		/// </summary>
		/// <param name="round">Game round instance.</param>
		/// <param name="input">Game input.</param>
		void Input(GameRound round, GameInput input);
		/// <summary>
		/// Called when the game is started.
		/// </summary>
		/// <param name="round">Game round instance.</param>
		void GameStart(GameRound round);
        /// <summary>
        /// Called when the game is finished.
        /// </summary>
        /// <param name="round">Game round instance.</param>
        void GameEnd(GameRound round);
        /// <summary>
        /// Called when the round is started.
        /// </summary>
        /// <param name="round">Game round instance.</param>
        void RoundStart(GameRound round);
        /// <summary>
        /// Called when the round is finished.
        /// </summary>
        /// <param name="round">Game round instance.</param>
        void RoundEnd(GameRound round);
        /// <summary>
        /// Called when the turn is started.
        /// </summary>
        /// <param name="round">Game round instance.</param>
        void TurnStart(GameRound round);
        /// <summary>
        /// Called when the turn is finished.
        /// </summary>
        /// <param name="round">Game round instance.</param>
        void TurnEnd(GameRound round);
        /// <summary>
        /// Called when the command is finished.
        /// </summary>
        /// <param name="round">Game round instance.</param>
        void CmdEnd(GameRound round);
        /// <summary>
        /// Called when some exception happens.
        /// </summary>
        /// <param name="round">Game round instance.</param>
        /// <param name="ex">GameException instance.</param>
        void CmdError(GameRound round, GameException ex);
    }

    /// <summary>
    /// Command enum.
    /// </summary>
    public enum Command
    {
        Trash, Discard, Next, Special, Null
    }

    /// <summary>
    /// Game input class.
    /// </summary>
    [Serializable]
    public class GameInput
    {
        private string[] opt;
        private Command command = Command.Null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="opt">Option</param>
        public void input(Command command, params string[] opt)
        {
            this.opt = opt;
            this.command = command;
        }

        /// <summary>
        /// Options.
        /// </summary>
        public string[] Options { get { return this.opt; } }
        /// <summary>
        /// Command.
        /// </summary>
        public Command Command { get { return this.command; } }

        /// <summary>
        /// Serializer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return command + ":" + String.Join(":", opt);
        }

        /// <summary>
        /// Deserializer
        /// </summary>
        /// <param name="xml">Serialized string.</param>
        public void input(string data)
        {
            string[] elem = data.Split(':');
            command = (Command)Enum.Parse(typeof(Command), elem[0]);
            opt = new string[elem.Length - 1];
            for (int i = 0; i < opt.Length; i++)
            {
                opt[i] = elem[i + 1];
            }
        }
    }

    /// <summary>
    /// Game operator class.
    /// </summary>
    public class GameOperator
    {
        private const int ROUND = 3;
        private GameRound round;
        private IGameObserver observer;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name1">Player1 name.</param>
        /// <param name="name2">Player2 name.</param>
        /// <param name="observer">Observer.</param>
		public GameOperator(IGameObserver observer, string name1, string name2, long seed, bool rev)
        {
            this.round = new GameRound(ROUND, name1, name2, new GameRandom(seed), rev);
            this.observer = observer;
        }

        /// <summary>
        /// Get the current 
        /// </summary>
        public GameRound Round { get { return this.round; } }

        /// <summary>
        /// Start the game.
        /// </summary>
        public IEnumerable<GameInput> Iterator()
        {
            observer.GameStart(round);
            while (true)
            {
                observer.RoundStart(round);
                round.NextRound();
                while (round.HasNext())
                {
                    foreach (GameInput gi in Turn())
                    {
                        yield return gi;
                    }
                }
                observer.RoundEnd(round);

                if (!round.HasNextRound())
                {
                    break;
                }
                yield return null;
            }
            observer.GameEnd(round);
        }

        private IEnumerable<GameInput> Turn()
        {
            CardGame game = round.Current;
            observer.TurnStart(round);

            while (true)
            {
                GameInput gi = new GameInput();
                yield return gi;
				observer.Input (round, gi);
                try
                {
                    Command mode = gi.Command;
                    string[] opt = gi.Options;
                    if (mode == Command.Discard)
                    {
                        string cardStr = opt[0];
                        int field = Int32.Parse(opt[1]);
                        game.Discard(field, cardStr);
                        observer.CmdEnd(round);
                    }
                    else if (mode == Command.Trash)
                    {
                        string cardStr = opt[0];
                        int field = Int32.Parse(opt[1]);
                        game.Trash(field, cardStr);
                        observer.CmdEnd(round);
                    }
                    else if (mode == Command.Special)
                    {
                        game.Special(opt);
                        observer.CmdEnd(round);
                    }
                    else if (mode == Command.Next)
                    {
                        observer.TurnEnd(round);
                        game.nextTurn();
                        break;
                    }
                }
                catch (GameException ex)
                {
                    observer.CmdError(round, ex);
                }
            }
        }
    }
}
