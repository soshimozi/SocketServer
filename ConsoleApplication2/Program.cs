using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoPPSharp;

namespace ConsoleApplication2
{
	class Program
	{
		static void Main(string[] args)
		{
			CryptoWrapper wrapper = new CryptoWrapper();

			Console.WriteLine(wrapper.Encrypt("dada", "baba"));
		}
	}
}
