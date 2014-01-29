using System;
using System.Diagnostics;
using System.Web.Mvc;

using Basic.Models;

using D2L.Extensibility.AuthSdk;
using D2L.Extensibility.AuthSdk.Restsharp;

using RestSharp;

namespace Basic.Controllers {
	public class HomeController : Controller {
		private const string APP_ID = "G9nUpvbZQyiPrk3um2YAkQ";
		private const string APP_KEY = "ybZu7fm_JKJTFwKEHfoZ7Q";

		private const string LMS_URL = "lms.valence.desire2learn.com";

		private const string LP_VERSION = "1.2";

		private const string WHOAMI_ROUTE = "/d2l/api/lp/" + LP_VERSION + "/users/whoami";

		private readonly ID2LAppContext m_valenceAppContext;
		private readonly HostSpec m_valenceHost;
		private ID2LUserContext m_valenceUserContext;

		public HomeController( ) {
			var appFactory = new D2LAppContextFactory();
			m_valenceAppContext = appFactory.Create( APP_ID, APP_KEY );
			m_valenceHost = new HostSpec( "https", LMS_URL, 443 );
		}

		private WhoAmIUser WhoAmI( ) {
			Debug.Assert( m_valenceUserContext != null, "This call can only be used by an authenticated user" );

			var client = new RestClient( "https://" + LMS_URL );
			var authenticator = new ValenceAuthenticator( m_valenceUserContext );
			var request = new RestRequest( WHOAMI_ROUTE );
			authenticator.Authenticate( client, request );
			var response = client.Execute<WhoAmIUser>( request );

			return response.Data;
		}

		public ActionResult Index( ) {
			m_valenceUserContext = Session["valenceUserContext"] as ID2LUserContext;
			if (m_valenceUserContext == null) {
				return RedirectToAction( "NeedsAuth" );
			}

			ViewBag.user = WhoAmI();
			return View();
		}

		public ActionResult NeedsAuth( ) {
			var returnUrl = Url.Action( "AuthReturn", null, null, Request.Url.Scheme );
			Debug.Assert( returnUrl != null, "Missing AuthReturn action" );
			ViewBag.valenceAuthUrl = m_valenceAppContext.CreateUrlForAuthentication( m_valenceHost, new Uri( returnUrl ) );
			return View();
		}

		public ActionResult AuthReturn( ) {
			Session["valenceUserContext"] = m_valenceAppContext.CreateUserContext( Request.Url, m_valenceHost );
			return RedirectToAction( "Index" );
		}

		public ActionResult Logout( ) {
			Session["valenceUserContext"] = null;
			return RedirectToAction( "Index" );
		}
	}
}