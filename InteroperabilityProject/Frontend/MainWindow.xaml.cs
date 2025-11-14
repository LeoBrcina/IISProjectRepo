using DAL.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProfileSoapRef;
using System.IO;
using System.Xml.Serialization;
using System.Net.Http.Headers;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string AccessToken = string.Empty;
        private DAL.DTOs.SimilarProfileDto _selectedProfile;

        public MainWindow()
        {
            InitializeComponent();
            CrudTab.Visibility = Visibility.Collapsed;
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text.Trim();
            var password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                AuthStatusText.Text = "Enter both username and password";
                return;
            }

            var dto = new RegisterRequestDto
            {
                Username = username,
                Password = password
            };

            var client = new HttpClient();

            try
            {
                var response = await client.PostAsJsonAsync("http://localhost:5150/api/Auth/Register", dto);

                if (response.IsSuccessStatusCode)
                {
                    AuthStatusText.Text = "Registration success";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    AuthStatusText.Text = "Username already exists";
                }
                else
                {
                    AuthStatusText.Text = "Registration unsuccessful";
                }
            }
            catch (Exception ex)
            {
                AuthStatusText.Text = $"Error: {ex.Message}";
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text.Trim();
            var password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                AuthStatusText.Text = "Enter both username and password";
                return;
            }

            var dto = new
            {
                username,
                password
            };

            var client = new HttpClient();
            try
            {
                var response = await client.PostAsJsonAsync("http://localhost:5150/api/Auth/Login", dto);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var tokenObj = JsonSerializer.Deserialize<AuthResponseDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    AccessToken = tokenObj?.AccessToken ?? "";
                    if (!string.IsNullOrEmpty(AccessToken))
                    {
                        AuthStatusText.Text = "Login successful";
                        CrudTab.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        AuthStatusText.Text = "Login unsuccessful, JWT invalid";
                    }
                }
                else
                {
                    AuthStatusText.Text = "Login failed, invalid username or pass";
                }
            }
            catch (Exception ex)
            {
                AuthStatusText.Text = $"Error: {ex.Message}";
            }
        }

        private async void ValidateUpload_Click(object sender, RoutedEventArgs e)
        {
            var url = ProfileUrlBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("https://www.linkedin.com/in/"))
            {
                UploadResultText.Text = "Enter a valid Linkedin link";
                return;
            }

            var endpoint = XsdRadio.IsChecked == true
                ? "xsd-upload"
                : "rng-upload";

            var requestUrl = $"http://localhost:5150/api/entity/{endpoint}?profileUrl={Uri.EscapeDataString(url)}";

            var client = new HttpClient();
            try
            {
                var response = await client.PostAsync(requestUrl, null); 

                if (response.IsSuccessStatusCode)
                {
                    UploadResultText.Text = "Upload and validation success";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    UploadResultText.Text = $"Error: {error}";
                }
            }
            catch (Exception ex)
            {
                UploadResultText.Text = $"Error: {ex.Message}";
            }
        }

        private void ClearUploadForm_Click(object sender, RoutedEventArgs e)
        {
            ProfileUrlBox.Text = "";
            XsdRadio.IsChecked = true;
            UploadResultText.Text = "";
        }

        private async void SearchSoap_Click(object sender, RoutedEventArgs e)
        {
            var keyword = SearchKeywordBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("Enter a keyword to search");
                return;
            }

            try
            {
                var client = new ProfileSearchServiceClient(
                    ProfileSearchServiceClient.EndpointConfiguration.BasicHttpBinding_IProfileSearchService
                );

                var result = await client.SearchByKeywordAsync(keyword);

                if (result != null && result.Length > 0)
                {
                    SoapResultsBox.ItemsSource = result;
                }
                else
                {
                    SoapResultsBox.ItemsSource = null;
                    MessageBox.Show("No profiles found");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SOAP search failed:\n{ex.Message}");
            }
        }

        private void ClearSoapSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchKeywordBox.Text = string.Empty;
            SoapResultsBox.ItemsSource = null;
            XmlPreviewBox.Text = string.Empty;
        }

        private void SoapResultsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SoapResultsBox.SelectedItem is ProfileSoapRef.SimilarProfileDto selectedProfile)
            {
                var xml = SerializeToXml(selectedProfile);
                XmlPreviewBox.Text = xml;
            }
        }

        private string SerializeToXml(ProfileSoapRef.SimilarProfileDto profile)
        {
            var serializer = new XmlSerializer(typeof(ProfileSoapRef.SimilarProfileDto));
            using var sw = new StringWriter();
            serializer.Serialize(sw, profile);
            return sw.ToString();
        }

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
            {
                if (tabControl.SelectedItem is TabItem tab && tab.Header.ToString() == "Manage Profiles")
                {
                    await LoadProfilesAsync();
                }
            }
        }

        private async Task LoadProfilesAsync()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                var response = await client.GetAsync("http://localhost:5150/api/userprofiles");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var profiles = JsonSerializer.Deserialize<List<DAL.DTOs.SimilarProfileDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    CrudProfileList.ItemsSource = profiles;
                    CrudStatusText.Text = $"Loaded {profiles.Count} profiles";
                }
                else
                {
                    CrudStatusText.Text = $"Error: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                CrudStatusText.Text = $"Error: {ex.Message}";
            }
        }

        private void CrudProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CrudProfileList.SelectedItem is DAL.DTOs.SimilarProfileDto profile)
            {
                _selectedProfile = profile;
                CrudFirstNameBox.Text = profile.FirstName;
                CrudLastNameBox.Text = profile.LastName;
                CrudPublicIdBox.Text = profile.PublicIdentifier;
                CrudTitleBox.Text = profile.TitleV2;
                CrudSubtitleBox.Text = profile.Subtitle;
            }
        }

        private async void CreateProfile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                CrudStatusText.Text = "Please log in before creating a profile, JWT invalid.";
                return;
            }

            var dto = new CreateProfileDto
            {
                FirstName = CrudFirstNameBox.Text.Trim(),
                LastName = CrudLastNameBox.Text.Trim(),
                TitleV2 = CrudTitleBox.Text.Trim(),
                Subtitle = CrudSubtitleBox.Text.Trim(),
                SubtitleV2 = CrudSubtitleBox.Text.Trim(),
                IsCreator = true,
                ProfilePictures = new List<CreateProfilePictureDto>()
            };

            if (string.IsNullOrEmpty(dto.FirstName) || string.IsNullOrEmpty(dto.LastName) || string.IsNullOrEmpty(dto.TitleV2))
            {
                CrudStatusText.Text = "First Name, Last Name, and Title are required";
                return;
            }

            var alreadyExists = ((List<DAL.DTOs.SimilarProfileDto>)CrudProfileList.ItemsSource)
                .Any(p => p.FirstName.Equals(dto.FirstName, StringComparison.OrdinalIgnoreCase)
                        && p.LastName.Equals(dto.LastName, StringComparison.OrdinalIgnoreCase)
                        && p.TitleV2.Equals(dto.TitleV2, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
            {
                CrudStatusText.Text = "A profile with the same name exists";
                return;
            }

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                var response = await client.PostAsJsonAsync("http://localhost:5150/api/userprofiles", dto);
                CrudStatusText.Text = response.IsSuccessStatusCode ? "Profile created" : $"Error: {await response.Content.ReadAsStringAsync()}";
                await LoadProfilesAsync();
            }
            catch (Exception ex)
            {
                CrudStatusText.Text = $"Error: {ex.Message}";
            }
        }

        private async void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                CrudStatusText.Text = "Please log in before updating a profile, JWT invalid.";
                return;
            }

            if (_selectedProfile == null)
            {
                CrudStatusText.Text = "Select a profile.";
                return;
            }

            var dto = new CreateProfileDto
            {
                FirstName = CrudFirstNameBox.Text.Trim(),
                LastName = CrudLastNameBox.Text.Trim(),
                TitleV2 = CrudTitleBox.Text.Trim(),
                Subtitle = CrudSubtitleBox.Text.Trim(),
                SubtitleV2 = CrudSubtitleBox.Text.Trim(),
                TextActionTarget = _selectedProfile.TextActionTarget,
                IsCreator = true,
                ProfilePictures = new List<CreateProfilePictureDto>()
            };

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                var response = await client.PutAsJsonAsync($"http://localhost:5150/api/userprofiles/{_selectedProfile.PublicIdentifier}", dto);
                CrudStatusText.Text = response.IsSuccessStatusCode ? "Profile updated." : $"Error: {await response.Content.ReadAsStringAsync()}";
                await LoadProfilesAsync();
            }
            catch (Exception ex)
            {
                CrudStatusText.Text = $"Error: {ex.Message}";
            }
        }

        private async void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                CrudStatusText.Text = "Please log in before deleting a profile.";
                return;
            }

            if (_selectedProfile == null)
            {
                CrudStatusText.Text = "Select a profile.";
                return;
            }

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                var response = await client.DeleteAsync($"http://localhost:5150/api/userprofiles/{_selectedProfile.PublicIdentifier}");
                CrudStatusText.Text = response.IsSuccessStatusCode ? "Deleted successfully." : $"Error: {await response.Content.ReadAsStringAsync()}";
                _selectedProfile = null;
                ClearCrudForm_Click(sender, e);
                await LoadProfilesAsync();
            }
            catch (Exception ex)
            {
                CrudStatusText.Text = $"Error: {ex.Message}";
            }
        }
        private void ClearCrudForm_Click(object sender, RoutedEventArgs e)
        {
            _selectedProfile = null;
            CrudFirstNameBox.Text = "";
            CrudLastNameBox.Text = "";
            CrudPublicIdBox.Text = "";
            CrudTitleBox.Text = "";
            CrudSubtitleBox.Text = "";
            CrudProfileList.SelectedItem = null;
            CrudStatusText.Text = "";
        }
    }
}