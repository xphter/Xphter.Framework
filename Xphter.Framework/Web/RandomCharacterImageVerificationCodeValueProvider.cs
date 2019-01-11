using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Web {
    public class RandomCharacterImageVerificationCodeValueProvider : IImageVerificationCodeValueProvider {
        /// <summary>
        /// Initialize a instance of RandomCharacterVerificationCodeProvider class.
        /// </summary>
        /// <param name="option"></param>
        public RandomCharacterImageVerificationCodeValueProvider(RandomCharacterVerificationCodeValueOption option) {
            if(option == null) {
                throw new ArgumentNullException("option");
            }

            this.m_random = option.RandomNumber ?? new Random(Environment.TickCount);
            this.m_characterCount = option.CharactersCount != null ? new List<int>(option.CharactersCount.Select((item) => (int) item)) : new List<int>(0);
            this.m_characters = option.Characters != null ? new List<char>(option.Characters) : new List<char>(0);
        }

        protected Random m_random;
        protected IList<int> m_characterCount;
        protected IList<char> m_characters;

        protected virtual T GetRandomValue<T>(IList<T> range) {
            if(range.Count == 0) {
                return default(T);
            }
            if(range.Count == 1) {
                return range[0];
            }

            return range[this.m_random.Next(0, range.Count)];
        }

        protected virtual int GetCharacterCount() {
            return this.GetRandomValue<int>(this.m_characterCount);
        }

        protected virtual IEnumerable<char> GetCharacters(int count) {
            if(count == 0 || this.m_characters.Count == 0) {
                return Enumerable.Empty<char>();
            }
            if(this.m_characters.Count <= count) {
                return this.m_characters;
            }

            ICollection<char> characters = new List<char>();
            for(int i = 0; i < count; i++) {
                characters.Add(this.GetRandomValue<char>(this.m_characters));
            }

            return characters;
        }

        #region IImageVerificationCodeValueProvider Members

        /// <inheritdoc />
        public string GetValue() {
            return new string(this.GetCharacters(this.GetCharacterCount()).ToArray());
        }

        #endregion
    }

    /// <summary>
    /// Provides options of RandomCharacterImageVerificationCodeValueProvider class.
    /// </summary>
    public class RandomCharacterVerificationCodeValueOption {
        public Range CharactersCount {
            get;
            set;
        }

        public Random RandomNumber {
            get;
            set;
        }

        public IEnumerable<char> Characters {
            get;
            set;
        }
    }
}
