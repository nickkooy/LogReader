using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
	public struct CoOrd
	{
		public int x, y;

		public CoOrd(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static CoOrd operator +(CoOrd a, CoOrd b)
		{
			return new LogReader.CoOrd(a.x + b.x, a.y + b.y);
		}

		public static CoOrd operator -(CoOrd a, CoOrd b)
		{
			return new CoOrd(a.x - b.x, a.y - b.y);
		}
	}
}
