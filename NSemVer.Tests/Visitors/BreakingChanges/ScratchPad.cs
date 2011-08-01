
namespace Api
{
	using System;
	using System.Collections;

	public class A { }
	public class B : A { }
	public class C : B { }

	public class Foo
	{
		public void Bar(IComparable x) { }
		public void Bar(IFormattable x) { }

		public void Baz(A x) { }
		public void Baz(B x) { }
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

			new Foo().Baz(new C());
		}
	}

	public static class ConsumerExtensions
	{
		public static void Bar(this Foo foo, ICloneable x) { }
	}
}
