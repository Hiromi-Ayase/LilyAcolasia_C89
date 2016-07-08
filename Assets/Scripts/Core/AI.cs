using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LilyAcolasia;

namespace LilyAcolasia_AI
{

    class AI
    {
        private int TRASH_LEVEL = 50;
        private int EVAL_DISCARD_WIN = 1000;
        private int EVAL_CARD_LOST = 20;
        private int EVAL_DISCARD_LOST_DECISION = 300;
        private GameRandom random;

        private const int EVAL_SPECIAL1_A = 110;
        private const int EVAL_SPECIAL3_A = 180;
        private const int EVAL_SPECIAL5_A = 40;
        private const int EVAL_SPECIAL7_A = 40;
        private const int EVAL_SPECIAL9_A = 0;
        private const int EVAL_SPECIAL1_B = 8;
        private const int EVAL_SPECIAL3_B = 20;
        private const int EVAL_SPECIAL5_B = 3;
        private const int EVAL_SPECIAL7_B = 8;
        private const int EVAL_SPECIAL9_B = 30;
        private const int EVAL_SPECIALX = 10;
        private const int EVAL_TRASH6 = 45;
        private const int EVAL_TRASH4 = 60;
        private const int EVAL_TRASH2 = 80;
        private const int EVAL_DISCARD_SPECIAL = 30;
        private const int EVAL_DISCARD_FIELD_SCORE = 1;
        private const int EVAL_DISCARD_CARD_SCORE = 10;
        private const int EVAL_DISCARD_IMPOSSIBLE = -10000;

        public AI(int level)
        {
            if (level == 1)
            {
                EVAL_DISCARD_WIN = 10;
            }
            else if (level == 2)
            {
                TRASH_LEVEL = 80;
            }
            else if (level == 3)
            {
                EVAL_DISCARD_LOST_DECISION = 10;
            }
            this.random = new GameRandom(31415);
        }

        public AITuple<LilyAcolasia.Command, string, int> next(CardGame game)
        {
            AITuple<LilyAcolasia.Command, string, int> line = null;
            if (game.Status == GameStatus.Status.End)
            {
                line = AITuple.Create(LilyAcolasia.Command.Next, "", -1);
            }
            else
            {
                if (game.Status == GameStatus.Status.WaitSpecialInput)
                {
                    var eval = getSpecialEval(game, game.LastTrashedField, game.LastTrashed);
                    line = AITuple.Create(LilyAcolasia.Command.Special, eval.Item2, eval.Item3);
                }
                else if (game.Status == GameStatus.Status.First)
                {
                    var trashList = getSpecialCards(game);
                    if (trashList.Count() > 0 && trashList[0].Item3 > TRASH_LEVEL)
                    {
                        line = AITuple.Create(LilyAcolasia.Command.Trash, trashList[0].Item1, trashList[0].Item2);
                    }
                }

                if (line == null)
                {
                    List<AITuple<string, int, int>> cardEval = getCardEval(game);

                    if (cardEval.Count() > 0)
                    {
                        line = AITuple.Create(LilyAcolasia.Command.Discard, cardEval[0].Item1, cardEval[0].Item2);
                    }
                    else
                    {
                        List<int> emptyFieldList = game.Fields.Where(f => f.CardList[game.Turn].Count() < 3).Select(f => f.Number).ToList();
                        if (emptyFieldList.Count() == 0)
                        {
                            line = AITuple.Create(LilyAcolasia.Command.Next, "", -1);
                        }
                        else
                        {
                            CardDeck deck = game.Player.Hand;
                            int randomIndex = random.Next() % deck.Count();
                            int randomField = random.Next() % emptyFieldList.Count();
                            line = AITuple.Create(LilyAcolasia.Command.Discard, deck[randomIndex].Name, emptyFieldList[randomField]);
                        }
                    }
                }
            }
            return line;
        }

        private List<AITuple<string, int, int>> getSpecialCards(CardGame game)
        {
            List<AITuple<string, int, int>> list = new List<AITuple<string, int, int>>();

            foreach (Field field in game.Fields)
            {
                CardDeck deck = field.CardList[game.Turn];
                if (!field.IsFixed && deck.Count() > 0)
                {
                    Card card = deck.Last();
                    if (card.Power % 2 == 1)
                    {
                        var eval = getSpecialEval(game, field, card);
                        list.Add(AITuple.Create(card.Name, field.Number, eval.Item1));
                    }
                    else if (card.Power == 2)
                    {
                        list.Add(AITuple.Create(card.Name, field.Number, EVAL_TRASH2));
                    }
                    else if (card.Power == 4)
                    {
                        list.Add(AITuple.Create(card.Name, field.Number, EVAL_TRASH4));
                    }
                    else if (card.Power == 6)
                    {
                        list.Add(AITuple.Create(card.Name, field.Number, EVAL_TRASH6));
                    }

                    if (field.CardList.Count() == 3)
                    {
                        list.Last().Item1 += EVAL_SPECIALX;
                    }
                }
            }
            list.Sort((c1, c2) => c2.Item3 - c1.Item3);
            return list;

        }

        private AITuple<int, string, int> getSpecialEval(CardGame game, Field field, Card card)
        {
            AITuple<int, string, int> result = AITuple.Create(-1, "", -1);
            if (card.Power == 1)
            {
                var t1 = calcFieldScore(game.Turn, field, card);
                int min = Int32.MaxValue;
                foreach (Field f in game.Fields)
                {
                    CardDeck deck = f.CardList[game.Turn];
                    if (!f.IsFixed && deck.Count() > 0)
                    {
                        Card c = deck.Last();
                        if (c == card)
                        {
                            if (deck.Count() > 1)
                            {
                                c = deck[deck.Count() - 2];
                            }
                            else
                            {
                                continue;
                            }
                        }
                        var t2 = calcFieldScore(game.Turn, f, c);
                        if (t2.Item1 < min)
                        {
                            min = t2.Item1;
                            result = AITuple.Create(EVAL_SPECIAL1_A - (min + t1.Item1) * EVAL_SPECIAL1_B, c.Name, f.Number);
                        }
                    }
                }
            }
            else if (card.Power == 3)
            {
                int max = game.Player.Hand.Max(c => c.Power);
                result = AITuple.Create(EVAL_SPECIAL3_A - (max * EVAL_SPECIAL3_B), "", -1);
            }
            else if (card.Power == 5)
            {
                int enemyTurn = (game.Turn + 1) % 2;
                int myTurn = game.Turn;

                Dictionary<Color, Int32> score = new Dictionary<Color, Int32>();
                score[Color.Red] = 0;
                score[Color.White] = 0;
                score[Color.Blue] = 0;
                score[Color.Black] = 0;
                score[Color.Green] = 0;

                foreach (Card c in field.CardList[enemyTurn])
                {
                    score[c.Color]--;
                }
                foreach (Card c in field.CardList[myTurn])
                {
                    score[c.Color]++;
                }
                int maxScore = Int32.MinValue;
                Color maxColor = null;
                foreach (Color c in Color.List)
                {
                    int s = score[c] - score[field.Color];
                    if (maxScore < s)
                    {
                        maxScore = s;
                        maxColor = c;
                    }
                }
                result = AITuple.Create(EVAL_SPECIAL5_A + maxScore * EVAL_SPECIAL5_B, maxColor.ToString(), -1);

            }
            else if (card.Power == 7)
            {
                var t1 = calcFieldScore(game.Turn, field, card);
                int max = -1;
                foreach (Field f in game.Fields)
                {
                    CardDeck deck = f.CardList[(game.Turn + 1) % 2];
                    if (!f.IsFixed && deck.Count() > 0)
                    {
                        Card c = deck.Last();

                        if (c == card)
                        {
                            if (deck.Count() > 1)
                            {
                                c = deck[deck.Count() - 2];
                            }
                            else
                            {
                                continue;
                            }
                        }
                        var t = calcFieldScore(game.Turn, f, c);
                        if (t.Item1 > max)
                        {
                            max = t.Item1;
                            result = AITuple.Create((max - t1.Item1) * EVAL_SPECIAL7_B + EVAL_SPECIAL7_A, c.Name, f.Number);
                        }
                    }
                }
            }
            else if (card.Power == 9)
            {
                int count = 0;
                foreach (Field f in game.Fields)
                {
                    CardDeck deck = f.CardList[(game.Turn + 1) % 2];
                    if (deck.Count() == 2)
                    {
                        count++;
                    }
                }
                result = AITuple.Create(count * EVAL_SPECIAL9_B + EVAL_SPECIAL9_A, "", -1);
            }
            return result;
        }


        private List<AITuple<string, int, int>> getCardEval(CardGame game)
        {
            List<AITuple<string, int, int>> list = new List<AITuple<string, int, int>>();
            foreach (Field field in game.Fields)
            {
                foreach (Card card in game.Player.Hand)
                {
                    int score = calcEval(game.Turn, field, card);
                    if (score != EVAL_DISCARD_IMPOSSIBLE)
                    {
                        list.Add(AITuple.Create(card.Name, field.Number, score));
                    }
                }
            }

            list.Sort((c1, c2) => c2.Item3 - c1.Item3);
            return list;
        }

        private int calcEval(int turn, Field field, Card card)
        {
            int result = -card.Power;

            int enemyTurn = (turn + 1) % 2;
            var t1 = calcFieldScore(turn, field, card);
            int cardScore = t1.Item1;

            if (t1.Item1 < 0)
            {
                return EVAL_DISCARD_IMPOSSIBLE;
            }
            int score1 = field.getScore(turn) + cardScore;
            int score2 = field.getScore(enemyTurn);
            int specialActive1 = field.CardList[turn].Count(c => c.HasSpecial);
            int specialActive2 = field.CardList[enemyTurn].Count(c => c.HasSpecial);

            int enemyCount = field.CardList[enemyTurn].Count();
            int myCount = field.CardList[turn].Count();


            if (enemyCount == 3 && myCount == 2)
            {
                // 勝敗決定
                if (score1 > score2)
                {
                    result += EVAL_DISCARD_WIN - cardScore * EVAL_CARD_LOST;
                }
                else if (score1 < score2)
                {
                    result -= EVAL_DISCARD_WIN - cardScore * EVAL_CARD_LOST;
                }
                else
                {
                    result += 0;
                }

                result += (specialActive2 - specialActive1) * EVAL_DISCARD_SPECIAL;
            }
            else if (enemyCount < 3 && myCount == 2)
            {
                result -= EVAL_DISCARD_LOST_DECISION;
            }
            else
            {
                result += card.HasSpecial ? EVAL_DISCARD_SPECIAL : 0;
            }

            result += score1 * EVAL_DISCARD_FIELD_SCORE;
            result += cardScore * EVAL_DISCARD_CARD_SCORE;

            return result;
        }

        private const int SCORE_SAME_COLOR = 3;
        private const int SCORE_SAME_NUMBER = 3;
        private const int SCORE_MATCH_COLOR = 2;

        // score sameColor, sameNumber, matchColor
        private AITuple<int, bool, bool, bool> calcFieldScore(int turn, Field field, Card card)
        {
            CardDeck fieldDeck = field.CardList[turn];
            Color fieldColor = field.Color;

            bool sameColor = false;
            bool sameNumber = false;
            if (!fieldDeck.Contains(card) && fieldDeck.Count() == 3)
            {
                return AITuple.Create(-1, false, false, false);
            }
            if (fieldDeck.Count() == (fieldDeck.Contains(card) ? 3 : 2))
            {
                sameColor = true;
                sameNumber = true;
                foreach (Card c in fieldDeck)
                {
                    sameColor = sameColor && c.Color == card.Color;
                    sameNumber = sameNumber && c.Power == card.Power;
                }
            }
            bool matchColor = fieldColor == card.Color;

            int score = card.Power;
            score += sameColor ? SCORE_SAME_COLOR : 0;
            score += sameNumber ? SCORE_SAME_NUMBER : 0;
            score += matchColor ? SCORE_MATCH_COLOR : 0;

            return AITuple.Create(score, sameColor, sameNumber, matchColor);
        }
    }
}
