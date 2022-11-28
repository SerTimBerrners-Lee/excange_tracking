
using System.Dynamic;
using Fiss.Enums;
using Fiss.Extensions;
using Fiss.Request;
using Fiss.Response;
using Newtonsoft.Json;

// MASK-USDT
// MRST-USDT

namespace OKX_API
{
	class Program
	{
		static float oldPrices;
		static string symbwol = "";

		static async Task Main(string[] args)
		{
	
			Console.Write("\nEnter Exchange: MOEX (1) or OKX (2): ");
			int enterExchange = Convert.ToInt32(Console.ReadLine());

			Console.Write("\nEnter a coin pair or sidId to track:  ");
			string enterPair = Console.ReadLine();
			
			Console.Write("\nEnter a float to track:  ");
			string enterPrice = Console.ReadLine();
			float setFloat = (float) Convert.ToDouble(enterPrice);

			Console.Write("\nEnter track to low (1) or to high (2): ");
			int enterDirection = Convert.ToInt32(Console.ReadLine());

			dynamic newTask = enterExchange == 1 ?
				new MoexApiRunner(enterPair) : 
					new OkxApiRunner(enterPair);

			var summNoLow = true;

			while (summNoLow) 
			{
				float price = await newTask.GetResponse();
				DateTime now = DateTime.Now;

				if (price != oldPrices && price > oldPrices && symbwol.Length < 30) {
					if ((symbwol?.Length != 0) && (symbwol[0] == '+'))
					{
						symbwol += '+';
					}
					else symbwol = "+";
				} else if (price != oldPrices && price < oldPrices && symbwol.Length < 30)
				{
					if ((symbwol.Length != 0) && (symbwol[0] == '-'))
					{
						symbwol += '-';
					}
					else symbwol = "-";
				}
				
				Console.WriteLine(
					"{0}:{1}:{2} ({3})  {4}|{5}", 
					now.Hour, now.Minute, now.Second, 
					enterPair,
					price, symbwol);

				oldPrices = price;

				bool checkStatus = false;
				if (enterDirection == 1)
				{
					checkStatus = price <= setFloat;
				} else if (enterDirection == 2)
				{
					checkStatus = price >= setFloat;
				}
				
				if (checkStatus) {
					summNoLow = false;
					string msg = enterPair + "|" + price;
					string title = enterDirection == 1 ? "PriceBelow<" : "PriceHighter>";
					title += setFloat;
					var notifications = new Notification(title, msg);
					notifications.SongPlay();
					bool isSendMail = notifications.MailSend();
					Console.WriteLine("Result send mailer is: {0}", isSendMail.ToString());

					/* string workingDirectory = Environment.CurrentDirectory + "/media/voennoytrevoga.wav";
					string projectDirectory = workingDirectory; */
				}

				System.Threading.Thread.Sleep(600);
			}
		}

	}

	public class MoexApiRunner
	{

		private string uri;
		private string secId;
		public float price;
		public MoexApiRunner(string secId)
		{
			this.secId = secId;
			uri = "engines/stock/markets/shares/securities/" + this.secId;
		}

		public async Task<float> GetResponse()
		{
			var tables = new IssRequest()
				.Path(this.uri); //history/engines/stock/markets/shares/securities/AFKS");

			var tab = await  tables.Fetch();
			IAsyncEnumerable<IDictionary<string, Fiss.Response.Table>> tabs = tab.ToCursor().Iterator();

			await foreach (var response in tabs)
			{
				var table = response["Marketdata"].Rows.ElementAt(2);
				price = (float) Convert.ToDouble(table.Values["Bid"]);
			}
			
			return price;
		}
	}
	

	public class OkxApiRunner
	{
		public string uri = "https://www.okex.com";

		public OkxApiRunner(string coinPair = "MASK-USDT")
		{
			string querty = "/api/v5/market/ticker?instId=";
			uri = uri + querty + coinPair;
			Console.WriteLine("query: {0}", uri);
		}

		public async Task<float> GetResponse()
		{
			using var client = new HttpClient();
			string response = await client.GetStringAsync(uri);
			dynamic parse = JsonConvert.DeserializeObject<dynamic>(response);
			float price = parse["data"][0]["bidPx"];
			return price;
		}
	}
}
