using System.Text;

namespace mogate
{
    public static class NameGenerator
	{
		private readonly static char[] m_opened = {'e', 'y', 'u', 'i', 'o', 'a', 'a', 'a', 'e', 'e', 'y', 'u', 'i', 'i'};
		private readonly static char[] m_closed = {'q', 'w', 'r', 't', 'p', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm'};

		private readonly static int m_chanceToStartClosed = 70;
		private readonly static int m_chanceToEndOpened = 30;
		private readonly static int m_minPairs = 2;
		private readonly static int m_randPairs = 1;

		public static string Generate ()
		{
			var name = new StringBuilder ();

			if (Utils.DropChance (m_chanceToStartClosed)) {
				name.Append (m_closed [Utils.ThrowDice (m_closed.Length)]);
			}
			int pairs = Utils.ThrowDice (m_randPairs) + m_minPairs;
			for (int idx = 0; idx < pairs; ++idx) {
				name.Append (m_opened [Utils.ThrowDice (m_opened.Length)]);
				name.Append (m_closed [Utils.ThrowDice (m_closed.Length)]);
			}
			if (Utils.DropChance (m_chanceToEndOpened) || name.Length < 3) {
				name.Append (m_opened [Utils.ThrowDice (m_opened.Length)]);
			}
			name [0] = name [0].ToString ().ToUpper ().ToCharArray()[0];
			return name.ToString();
		}
	}
}

