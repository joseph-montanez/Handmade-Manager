using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        string _consumerKey = "crgneh7f95n5clhjb30wex9r";
        string _consumerSecretKey = "w556ftx4za";
        string _linkedInRequestTokenUrl = "https://openapi.etsy.com/v2/oauth/request_token";
        string _linkedInAccessTokenUrl = "https://openapi.etsy.com/v2/oauth/access_token";

        string _requestPeopleUrl = "http://api.linkedin.com/v1/people/~";
        string _requestConnectionsUrl = "http://api.linkedin.com/v1/people/~/connections";
        string _requestPositionsUrl = "http://api.linkedin.com/v1/people/~:(positions)";

        string _accessToken;
        string _accessTokenSecretKey;

        string _loginUrl;
        string _requestToken;
        string _requestTokenSecretKey;
        string _OAuthVerifier = "N/A";

        OAuthUtil oAuthUtil = new OAuthUtil();


        /*
         * Handmade Manager KEYSTRING crgneh7f95n5clhjb30wex9r SHARED SECRETw556ftx4za
         * */


        public MainPage()
        {
            this.InitializeComponent();
        }


        private async void getRequestToken_Click_1(object sender, RoutedEventArgs e)
        {
            string nonce = oAuthUtil.GetNonce();
            string timeStamp = oAuthUtil.GetTimeStamp();

            string sigBaseStringParams = "oauth_consumer_key=" + _consumerKey;

            //sigBaseStringParams += "&" + "scope=" + "email_r%20listings_r";
            sigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            sigBaseStringParams += "&" + "oauth_signature_method=" + "HMAC-SHA1";
            sigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            sigBaseStringParams += "&" + "oauth_version=1.0";
            string sigBaseString = "POST&";
            sigBaseString += Uri.EscapeDataString(_linkedInRequestTokenUrl) + "&" + Uri.EscapeDataString(sigBaseStringParams);

            string signature = oAuthUtil.GetSignature(sigBaseString, _consumerSecretKey);

            var responseText = await oAuthUtil.PostData(_linkedInRequestTokenUrl, sigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(signature));

            if (!string.IsNullOrEmpty(responseText))
            {
                string oauth_token = null;
                string login_url = null;
                string oauth_token_secret = null;
                string oauth_authorize_url = null;
                string[] keyValPairs = responseText.Split('&');

                for (int i = 0; i < keyValPairs.Length; i++)
                {
                    String[] splits = keyValPairs[i].Split('=');
                    switch (splits[0])
                    {
                        case "oauth_token":
                            oauth_token = splits[1];
                            break;
                        case "login_url":
                            login_url = WebUtility.UrlDecode(splits[1]);
                            Debug.WriteLine("login_url: {0}", login_url);
                            break;
                        case "oauth_token_secret":
                            oauth_token_secret = splits[1];
                            break;
                        case "xoauth_request_auth_url":
                            oauth_authorize_url = splits[1];
                            break;
                    }
                }

                _requestToken = oauth_token;
                _requestTokenSecretKey = oauth_token_secret;
                _loginUrl = login_url;
                oAuthAuthorizeLink.Content = Uri.UnescapeDataString(oauth_authorize_url + "?oauth_token=" + oauth_token);
            }
        }

        private async void getAccessToken_Click_1(object sender, RoutedEventArgs e)
        {
            string nonce = oAuthUtil.GetNonce();
            string timeStamp = oAuthUtil.GetTimeStamp();

            string sigBaseStringParams = "oauth_consumer_key=" + _consumerKey;
            sigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            sigBaseStringParams += "&" + "oauth_signature_method=" + "HMAC-SHA1";
            sigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            sigBaseStringParams += "&" + "oauth_token=" + _requestToken;
            sigBaseStringParams += "&" + "oauth_verifier=" + _OAuthVerifier;
            sigBaseStringParams += "&" + "oauth_version=1.0";
            string sigBaseString = "POST&";
            sigBaseString += Uri.EscapeDataString(_linkedInAccessTokenUrl) + "&" + Uri.EscapeDataString(sigBaseStringParams);

            // LinkedIn requires both consumer secret and request token secret
            string signature = oAuthUtil.GetSignature(sigBaseString, _consumerSecretKey, _requestTokenSecretKey);

            var responseText = await oAuthUtil.PostData(_linkedInAccessTokenUrl, sigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(signature));

            if (!string.IsNullOrEmpty(responseText))
            {
                string oauth_token = null;
                string oauth_token_secret = null;
                string[] keyValPairs = responseText.Split('&');

                for (int i = 0; i < keyValPairs.Length; i++)
                {
                    String[] splits = keyValPairs[i].Split('=');
                    switch (splits[0])
                    {
                        case "oauth_token":
                            oauth_token = splits[1];
                            break;
                        case "oauth_token_secret":
                            oauth_token_secret = splits[1];
                            break;
                    }
                }

                _accessToken = oauth_token;
                _accessTokenSecretKey = oauth_token_secret;
            }
        }


        private void oAuthAuthorizeLink_Click_1(object sender, RoutedEventArgs e)
        {
            WebViewHost.Visibility = Windows.UI.Xaml.Visibility.Visible;
            WebViewHost.Navigate(new Uri(oAuthAuthorizeLink.Content.ToString()));
        }

        private async void requestUserProfile_Click_1(object sender, RoutedEventArgs e)
        {
            requestLinkedInApi(_requestPeopleUrl);
        }

        private void requestConnections_Click_1(object sender, RoutedEventArgs e)
        {
            requestLinkedInApi(_requestConnectionsUrl);
        }

        private void requestPositions_Click_1(object sender, RoutedEventArgs e)
        {
            requestLinkedInApi(_requestPositionsUrl);
        }


        private async void check_Click_1(object sender, RoutedEventArgs e)
        {
            if (_requestToken == null)
            {
                Debug.WriteLine("Getting request token");
                getRequestToken_Click_1(sender, e);
            }
            else if (_accessToken == null)
            {
                Debug.WriteLine("Sending User to grant access");
                WebViewHost.Visibility = Windows.UI.Xaml.Visibility.Visible;
                WebViewHost.Navigate(new Uri(_loginUrl));
                //getAccessToken_Click_1(sender, e);
            }
            else
            {
                Debug.WriteLine("Done getting everything");
            }
        }

        private async void requestLinkedInApi(string url)
        {
            string nonce = oAuthUtil.GetNonce();
            string timeStamp = oAuthUtil.GetTimeStamp();

            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.MaxResponseContentBufferSize = int.MaxValue;
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                HttpRequestMessage requestMsg = new HttpRequestMessage();
                requestMsg.Method = new HttpMethod("GET");
                requestMsg.RequestUri = new Uri(url, UriKind.Absolute);

                string sigBaseStringParams = "oauth_consumer_key=" + _consumerKey;
                sigBaseStringParams += "&" + "oauth_nonce=" + nonce;
                sigBaseStringParams += "&" + "oauth_signature_method=" + "HMAC-SHA1";
                sigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
                sigBaseStringParams += "&" + "oauth_token=" + _accessToken;
                sigBaseStringParams += "&" + "oauth_verifier=" + _OAuthVerifier;
                sigBaseStringParams += "&" + "oauth_version=1.0";
                string sigBaseString = "GET&";
                sigBaseString += Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(sigBaseStringParams);

                // LinkedIn requires both consumer secret and request token secret
                string signature = oAuthUtil.GetSignature(sigBaseString, _consumerSecretKey, _accessTokenSecretKey);

                string data = "realm=\"https://openapi.etsy.com/\", oauth_consumer_key=\"" + _consumerKey
                              +
                              "\", oauth_token=\"" + _accessToken +
                              "\", oauth_verifier=\"" + _OAuthVerifier +
                              "\", oauth_nonce=\"" + nonce +
                              "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + timeStamp +
                              "\", oauth_version=\"1.0\", oauth_signature=\"" + Uri.EscapeDataString(signature) + "\"";
                requestMsg.Headers.Authorization = new AuthenticationHeaderValue("OAuth", data);
                var response = await httpClient.SendAsync(requestMsg);
                var text = await response.Content.ReadAsStringAsync();
                WebViewHost.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                LinkedInResponse.Visibility = Windows.UI.Xaml.Visibility.Visible;
                LinkedInResponse.Text = text;
            }
            catch (Exception Err)
            {
                throw;
            }
        }
    }
}
