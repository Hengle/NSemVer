/*

namespace Api
{
	using System;
	using System.Collections;

	public class Foo
	{
		//public void Bar(IComparable x) { }
		//public void Bar(IFormattable x) { }

		//public void Baz(A x) { }
		//public void Baz(B x) { }

		public bool Bar(int i) { }
	}
}

namespace ApiConsumer
{
	using System;
	using Api;

	public class Consumer
	{
		public void Example()
		{
			//int i = 99;
			//new Foo().Bar(i);

			Action<int> test = new Foo().Bar;
			test(99);
		}
	}

	public static class ConsumerExtensions
	{
		public static void Bar(this Foo foo, ICloneable x) { }
	}
}
*/
