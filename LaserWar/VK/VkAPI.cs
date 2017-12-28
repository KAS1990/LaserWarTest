using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Net;
using LaserWar.VK;

namespace LaserWar.Vk
{
	public static class VkAPI
	{
		private const string APPID = "ApplicationId";  //ID приложения
		private const string VKAPIURL = "https://api.vk.com/method/";  //Ссылка для запросов
		public static string m_Token;  //Токен, использующийся при запросах
		private const string VERSION_VK_API = "5.69";

		public static Dictionary<string, string> GetInformation(string UserId, string[] Fields)  //Получение заданной информации о пользователе с заданным ID 
		{
			HttpRequest request = new HttpRequest();
			request.AddUrlParam("user_ids", UserId);
			request.AddUrlParam("v", VERSION_VK_API);
			request.AddUrlParam("access_token", m_Token);
			string Params = "";
			foreach (string i in Fields)
			{
				Params += i + ",";
			}
			Params = Params.Remove(Params.Length - 1);
			request.AddUrlParam("fields", Params);
			string Result = request.Get(VKAPIURL + "users.get").ToString();
			StdVkAnswer<List<Dictionary<string, object>>> resp = JsonConvert.DeserializeObject<StdVkAnswer<List<Dictionary<string, object>>>>(Result);
			if (resp.response[0].ContainsKey("country"))
			{
				resp.response[0]["country"] = (resp.response[0]["country"] as JObject)["title"];
				resp.response[0]["city"] = (resp.response[0]["city"] as JObject)["title"];
			}

			Dictionary<string, string> result = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> item in resp.response[0])
				result.Add(item.Key, item.Value.ToString());
			return result;
		}

		/// <summary>
		/// Получение информации о группах пользователя
		/// </summary>
		/// <param name="UserId"></param>
		/// <returns></returns>
		public static List<VKGroup> GetGroups(string UserId)
		{
			HttpRequest request = new HttpRequest();
			request.AddUrlParam("user_id", UserId);
			request.AddUrlParam("extended", 1);
			request.AddUrlParam("offset", 0);
			request.AddUrlParam("count", 1000);
			request.AddUrlParam("v", VERSION_VK_API);
			request.AddUrlParam("access_token", m_Token);
			string Result = request.Get(VKAPIURL + "groups.get").ToString();
			StdVkAnswer<Dictionary<string, object>> resp = JsonConvert.DeserializeObject<StdVkAnswer<Dictionary<string, object>>>(Result);
			
			return JsonConvert.DeserializeObject<List<VKGroup>>(resp.response["items"].ToString());
		}

		class photos_getWallUploadServerAns
		{
			public string upload_url;
			public long album_id;
			public long user_id;
		}
		class docs_getWallUploadServerAns
		{
			public string upload_url;
		}
		class wall_PostAns
		{
			public long post_id;
		}
		/// <summary>
		/// Загрузка фотки на сервер ВК
		/// </summary>
		/// <param name="GroupId"></param>
		/// <param name="ScreenshotFullFilePath"></param>
		/// <returns>
		/// </returns>
		public static photos_saveWallPhotoAns LoadImageToServer(int GroupId, string ScreenshotFullFilePath)
		{
			try
			{
				// Получаем идентификатор сервера
				HttpRequest request = new HttpRequest();
				request.AddUrlParam("group_id", GroupId);
				request.AddUrlParam("v", VERSION_VK_API);
				request.AddUrlParam("access_token", m_Token);
				string Result = request.Get(VKAPIURL + "photos.getWallUploadServer").ToString();
				StdVkAnswer<Dictionary<string, object>> resp = JsonConvert.DeserializeObject<StdVkAnswer<Dictionary<string, object>>>(Result);
				photos_getWallUploadServerAns Ans = new photos_getWallUploadServerAns()
				{
					upload_url = resp.response["upload_url"].ToString(),
					album_id = (long)resp.response["album_id"],
					user_id = (long)resp.response["user_id"],
				};

				// Загружаем картинку на сервер
				string wcans = null;
				using (WebClient wc = new WebClient())
				{
					wcans = wc.Encoding.GetString(wc.UploadFile(Ans.upload_url, "POST", ScreenshotFullFilePath));
				}
				Dictionary<string, object> dictWcans = JsonConvert.DeserializeObject<Dictionary<string, object>>(wcans);

				request = new HttpRequest();
				request.AddUrlParam("group_id", GroupId);
				request.AddUrlParam("server", (long)dictWcans["server"]);
				request.AddUrlParam("photo", dictWcans["photo"].ToString());
				request.AddUrlParam("hash", dictWcans["hash"].ToString());
				request.AddUrlParam("v", VERSION_VK_API);
				request.AddUrlParam("access_token", m_Token);
				Result = request.Get(VKAPIURL + "photos.saveWallPhoto").ToString();
				StdVkAnswer < List < photos_saveWallPhotoAns > > Photos = JsonConvert.DeserializeObject < StdVkAnswer < List< photos_saveWallPhotoAns > > >(Result);

				return Photos.response[0];
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		/// <summary>
		/// Загрузка фотки на сервер ВК
		/// </summary>
		/// <param name="UserId"></param>
		/// <param name="PDFFullFilePath"></param>
		/// <returns>
		/// </returns>
		public static docs_SaveAns LoadPDFToServer(string PDFFullFilePath)
		{
			try
			{
				// Получаем идентификатор сервера
				HttpRequest request = new HttpRequest();
				request.AddUrlParam("v", VERSION_VK_API);
				request.AddUrlParam("access_token", m_Token);
				string Result = request.Get(VKAPIURL + "docs.getWallUploadServer").ToString();
				StdVkAnswer<Dictionary<string, object>> resp = JsonConvert.DeserializeObject<StdVkAnswer<Dictionary<string, object>>>(Result);
				docs_getWallUploadServerAns Ans = new docs_getWallUploadServerAns()
				{
					upload_url = resp.response["upload_url"].ToString()
				};

				// Загружаем картинку на сервер
				string wcans = null;
				using (WebClient wc = new WebClient())
				{
					wcans = System.Text.Encoding.UTF8.GetString(wc.UploadFile(Ans.upload_url, PDFFullFilePath));
				}
				Dictionary<string, object> dictWcans = JsonConvert.DeserializeObject<Dictionary<string, object>>(wcans);

				request = new HttpRequest();
				request.AddUrlParam("file", dictWcans["file"].ToString());
				request.AddUrlParam("v", VERSION_VK_API);
				request.AddUrlParam("access_token", m_Token);
				Result = request.Get(VKAPIURL + "docs.save").ToString();
				StdVkAnswer<List<docs_SaveAns>> Docs = JsonConvert.DeserializeObject<StdVkAnswer<List<docs_SaveAns>>>(Result);

				return Docs.response[0];
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		public static long CreatePost(int GroupId, string Text, photos_saveWallPhotoAns PhotoInfo, docs_SaveAns DocInfo)
		{
			try
			{
				HttpRequest request = new HttpRequest();
				request.AddUrlParam("owner_id", "-" + GroupId.ToString());
				request.AddUrlParam("message", Text);
				request.AddUrlParam("attachments",
									string.Format("photo{0}_{1},doc{2}_{3}", PhotoInfo.owner_id, PhotoInfo.id, DocInfo.owner_id, DocInfo.id));
				request.AddUrlParam("v", VERSION_VK_API);
				request.AddUrlParam("access_token", m_Token);
				string Result = request.Get(VKAPIURL + "wall.post").ToString();
				StdVkAnswer<wall_PostAns>  resp = JsonConvert.DeserializeObject<StdVkAnswer<wall_PostAns>>(Result);

				return resp.response.post_id;
			}
			catch (Exception ex)
			{
				return -1;
			}
		}
	}
}
