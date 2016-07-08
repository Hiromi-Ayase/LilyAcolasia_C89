using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyAcolasia
{
    /// <summary>
    /// Game exception type class.
    /// </summary>
    public class GameExceptionType
    {
        /// <summary>
        /// Field index out of range.
        /// </summary>
        public const string FIELD_INDEX_EXCEPTION = "FIELD_INDEX_EXCEPTION";
        /// <summary>
        /// The card representation string is invalid.
        /// </summary>
        public const string CARD_STRING_EXCEPTION = "CARD_STRING_EXCEPTION";
        /// <summary>
        /// Already discarded a card.
        /// </summary>
        public const string ALREADY_DISCARDED_EXCEPTION = "ALREADY_DISCARDED_EXCEPTION";
        /// <summary>
        /// Not discarded any card yet.
        /// </summary>
        public const string NOT_DISCARDED_EXCEPTION = "NOT_DISCARDED_EXCEPTION";
        /// <summary>
        /// Already trashed a card.
        /// </summary>
        public const string ALREADY_TRASHED_EXCEPTION = "ALREADY_TRASHED_EXCEPTION";
        /// <summary>
        /// The card list in the field is already fixed.
        /// </summary>
        public const string CARD_ALREADY_FIXED_EXCEPTION = "CARD_ALREADY_FIXED_EXCEPTION";
        /// <summary>
        /// The card is not found
        /// </summary>
        public const string CARD_NOT_FOUND_EXCEPTION = "CARD_NOT_FOUND_EXCEPTION";
        /// <summary>
        /// The number of cards is too many.
        /// </summary>
        public const string CARD_TOO_MANY_EXCEPTION = "CARD_TOO_MANY_EXCEPTION";
        /// <summary>
        /// "No such card exists."
        /// </summary>
        public const string NO_CARD_EXCEPTION = "NO_CARD_EXCEPTION";

        private readonly string type;

        /// <summary>
        /// Type string.
        /// </summary>
        /// <returns>Type.</returns>
        public override string ToString()
        {
            return this.type;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="args">Arguments.</param>
        public GameExceptionType(string type, params object[] args)
        {
            this.type = type;
        }
    }

    /// <summary>
    /// Game exception class.
    /// </summary>
    public class GameException : Exception
    {
        private readonly GameExceptionType type;

        /// <summary>
        /// Type.
        /// </summary>
        public string Type { get { return this.type.ToString();  } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">GameExceptionType.</param>
        public GameException(GameExceptionType type)
        {
            this.type = type;
        }

        /// <summary>
        /// FIELD_INDEX_EXCEPTION
        /// </summary>
        /// <returns>GameExceptionType.</returns>
        public static GameException getFieldIndexException()
        {
            return new GameException(new GameExceptionType(GameExceptionType.FIELD_INDEX_EXCEPTION));
        }

        /// <summary>
        /// ALREADY_DISCARDED_EXCEPTION
        /// </summary>
        /// <returns>GameExceptionType.</returns>
        public static GameException getAlreadyDiscardedException()
        {
            return new GameException(new GameExceptionType(GameExceptionType.ALREADY_DISCARDED_EXCEPTION));
        }

        /// <summary>
        /// ALREADY_TRASHED_EXCEPTION
        /// </summary>
        /// <returns>GameExceptionType.</returns>
        public static GameException getAlreadyTrashedException()
        {
            return new GameException(new GameExceptionType(GameExceptionType.ALREADY_TRASHED_EXCEPTION));
        }

        /// <summary>
        /// CARD_ALREADY_FIXED_EXCEPTION
        /// </summary>
        /// <returns>GameExceptionType.</returns>
        public static GameException getCardAlreadyFixedException()
        {
            return new GameException(new GameExceptionType(GameExceptionType.CARD_ALREADY_FIXED_EXCEPTION));
        }

        /// <summary>
        /// CARD_NOT_FOUND_EXCEPTION
        /// </summary>
        /// <param name="cardStr">Card string.</param>
        /// <returns>GameExceptionType.</returns>
        public static Exception getCardNotFoundException(string cardStr)
        {
            return new GameException(new GameExceptionType(GameExceptionType.CARD_NOT_FOUND_EXCEPTION, cardStr));
        }

        /// <summary>
        /// NOT_DISCARDED_EXCEPTION
        /// </summary>
        /// <returns>GameExceptionType.</returns>
        public static Exception getNotTrashedException()
        {
            return new GameException(new GameExceptionType(GameExceptionType.NOT_DISCARDED_EXCEPTION));
        }

        /// <summary>
        /// CARD_STRING_EXCEPTION
        /// </summary>
        /// <param name="cardStr">Card string.</param>
        /// <returns>GameExceptionType.</returns>
        public static Exception getCardStrException(string cardStr)
        {
            return new GameException(new GameExceptionType(GameExceptionType.CARD_STRING_EXCEPTION, cardStr));
        }

        /// <summary>
        /// CARD_TOO_MANY_EXCEPTION
        /// </summary>
        /// <returns>GameExceptionType.</returns>
        public static Exception getCardTooManyException()
        {
            return new GameException(new GameExceptionType(GameExceptionType.CARD_TOO_MANY_EXCEPTION));
        }

        /// <summary>
        /// NO_CARD_EXCEPTION
        /// </summary>
        /// <returns>GameExceptionType.</returns>
        public static Exception getNoCardException()
        {
            return new GameException(new GameExceptionType(GameExceptionType.NO_CARD_EXCEPTION));
        }
    }
}
