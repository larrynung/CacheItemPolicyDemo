using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CacheItemPolicyDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			ContentProvider textFile = new ContentProvider();
			Stopwatch sw = new Stopwatch();
			while (true)
			{
				sw.Reset();
				sw.Start();
				Console.WriteLine(textFile.Content);
				sw.Stop();
				Console.WriteLine("Elapsed Time: {0} ms", sw.ElapsedMilliseconds);
				Console.WriteLine(new string('=', 50));
				Console.ReadLine();
			}
		}
	}

	public class ContentProvider
	{
		public String Content
		{
			get
			{
				const string CACHE_KEY = "Content";
				string content = m_Cache[CACHE_KEY] as string;
				if (content == null)
				{
					CacheItemPolicy policy = new CacheItemPolicy();
					//policy.AbsoluteExpiration = DateTime.Now.AddMilliseconds(1500);
					policy.SlidingExpiration = TimeSpan.FromMilliseconds(1500);

					//policy.UpdateCallback = new CacheEntryUpdateCallback((args) => 
					//{
					//	Console.WriteLine("MyCachedItemUpdatedCallback...");
					//	Console.WriteLine(string.Format("Remove {0}...Because {1}",args.Key,args.RemovedReason.ToString()));
					//	Console.WriteLine(new string('=', 50));
					//});

					policy.RemovedCallback = new CacheEntryRemovedCallback((args) => 
					{
						Console.WriteLine("CacheEntryRemovedCallback...");
						Console.WriteLine(string.Format("Remove {0}...Because {1}", args.CacheItem.Key, args.RemovedReason.ToString()));
						Console.WriteLine(new string('=', 50));
					});

					content = Guid.NewGuid().ToString();
					Thread.Sleep(1000);
					m_Cache.Set(CACHE_KEY, content, policy);
				}
				return content;
			}
		}


		private ObjectCache _cache;
		private ObjectCache m_Cache
		{
			get
			{
				if (_cache == null)
					_cache = MemoryCache.Default;
				return _cache;
			}
		}
	}
}
