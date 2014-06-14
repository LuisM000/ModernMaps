using Maps.NET.GoogleMaps.GooglePlaces;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Maps.NET
{
     
    public partial class MainWindow : Window
    {
        bool flagSeeAll=true;

        public MainWindow()
        {
            InitializeComponent();
            InitializeControls();
        }

        
        private async void InitializeControls()
        {
            listboxTypesPlaces.ItemsSource = Static.StaticPlaces.listPlaces;
            listboxTypesPlaces.DataContext = Static.StaticPlaces.listPlaces;
            comboTypesPlace.ItemsSource = Static.StaticPlaces.listPlaces;
            comboTypesPlace.DataContext = Static.StaticPlaces.listPlaces;
            comboTypesPlace.SelectedIndex = 33;
            GoogleMaps.GoogleStaticMaps.StaticMap.ScaleMap = GoogleMaps.GoogleStaticMaps.StaticMap.Scale.ADVANCED;
            comboMarkerColor.ItemsSource = Enum.GetNames(typeof(GoogleMaps.GoogleStaticMaps.Marker.ColorMarker)).ToList(); comboMarkerColor.SelectedIndex = 0;
            comboMarkerSize.ItemsSource = Enum.GetNames(typeof(GoogleMaps.GoogleStaticMaps.Marker.SizeMarker)).ToList(); comboMarkerSize.SelectedIndex = 0;
            comboColorRoute.ItemsSource = Enum.GetNames(typeof(GoogleMaps.GoogleStaticMaps.Marker.ColorMarker)).ToList(); comboColorRoute.SelectedIndex = 0;
            comboFillColorRoute.ItemsSource = Enum.GetNames(typeof(GoogleMaps.GoogleStaticMaps.Marker.ColorMarker)).ToList(); comboFillColorRoute.SelectedIndex = 0;
            listboxFeatures.ItemsSource = Enum.GetNames(typeof(GoogleMaps.GoogleStaticMaps.Style.FeatureStyle)).ToList(); listboxFeatures.SelectedIndex = 0;
            comboElement.ItemsSource = Enum.GetNames(typeof(GoogleMaps.GoogleStaticMaps.Style.ElementStyle)).ToList(); comboElement.SelectedIndex = 0;
            comboVisibility.ItemsSource = Enum.GetNames(typeof(GoogleMaps.GoogleStaticMaps.Style.RulesStyle.VisibilityRule)).ToList(); comboVisibility.SelectedIndex = 0;
            comboColorStyle.ItemsSource = Enum.GetNames(typeof(GoogleMaps.GoogleStaticMaps.Style.RulesStyle.ColorRule)).ToList(); comboColorStyle.SelectedIndex = 0;
            comboPlacesAlternative.ItemsSource = Static.StaticPlaces.listPlaces;
            comboPlacesAlternative.DataContext = Static.StaticPlaces.listPlaces;
            comboPlacesAlternative.SelectedIndex = 33;
            await Task.Delay(500);
            zoomME(new Location(40.4168, -3.7038), "Madrid", 15);
            if (Properties.Settings.Default.ApiKey == "") { GoogleMaps.GoogleStaticInfo.Key = ""; } else { GoogleMaps.GoogleStaticInfo.Key = Properties.Settings.Default.ApiKey; }
            if (Properties.Settings.Default.BingKey == "") { myMap.CredentialsProvider = new ApplicationIdCredentialsProvider(""); } else { myMap.CredentialsProvider = new ApplicationIdCredentialsProvider(Properties.Settings.Default.BingKey); }
            if (Properties.Settings.Default.ApiKey == "" || Properties.Settings.Default.BingKey == "") { viewApiKey(true); }
        } 

        

        #region "ProgressBar"
        private void startProgress(string message)
        {
            txtProgressBar.Text = message;
            progressBar.IsIndeterminate = true;
        }
        private void stopProgress()
        {
            txtProgressBar.Text = "Modern Maps";
            progressBar.IsIndeterminate = false;
        }

        private async void bussyMessage(TimeSpan time,string message)
        {
            txtProgressBar.Text = message;
            await Task.Delay(time);
            stopProgress();
        }
        #endregion

        #region "control events"
        private void ContentPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            //Show the popup if mouse is hovering over it
            ContentPopup.Visibility = Visibility.Visible;
            Canvas.SetZIndex(ContentPopup, 10);
        }
        private void ContentPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            //Hide the popup if mouse leaves it
            ContentPopup.Visibility = Visibility.Collapsed;
        }
       

        private void RibbonToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (myMap != null) { viewAll(true); }
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtSearch.Text != "")
            {
                geocoding(txtSearch.Text);
            }

        }
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter && txtSearch.Text != "")
            {
                geocoding(txtSearch.Text);
            }
        }
        private void txtGeocoding_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtGridGeocoding.Text != "")
            {
                GeocodingGrid(txtGridGeocoding.Text);
            }
        }
        private void RibbonButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            copyPosition();
            (this.FindResource("storyMessageCopyON") as Storyboard).Begin(); 
        }
        private void RibbonButton_Click_7(object sender, RoutedEventArgs e)
        {
            if (gridRequest.Visibility == Visibility.Visible) { viewRequest(false); } else { viewRequest(true); }
        }
        private void RibbonButton_Click_FeedBack(object sender, RoutedEventArgs e)
        {
            if (gridFeedback.Visibility == Visibility.Visible) { viewFeedBack(false); } else { viewFeedBack(true); }
        }

        private void RibbonButton_Click_8(object sender, RoutedEventArgs e)
        {
            if (gridApiKey.Visibility == Visibility.Visible) { viewApiKey(false); } else { viewApiKey(true); }
        }
        private void RibbonButton_Click_RemoveAll(object sender, RoutedEventArgs e)
        {
            Static.MapObject.removeMapPushpin(myMap, "place");
            Static.MapObject.removeMapPushpin(myMap, "pushAlternative");
            Static.MapObject.removeRoute(myMap, "route");
            Static.MapObject.removeRoute(myMap, "routeGoogle");
            bussyMessage(TimeSpan.FromSeconds(2), "eliminado contenido del mapa");
        }
        private void RibbonButton_Click_RemoveAlternative(object sender, RoutedEventArgs e)
        {
            Static.MapObject.removeMapPushpin(myMap, "pushAlternative");
            bussyMessage(TimeSpan.FromSeconds(2), "eliminado marcadores secundarios");
        }
        private void RibbonToggleButton_Checked_1(object sender, RoutedEventArgs e)
        {
            ribbon.IsMinimized = false;
        }

        private void RibbonToggleButton_Unchecked_1(object sender, RoutedEventArgs e)
        {
            ribbon.IsMinimized = true;

        }
 
        private void Image_MouseDown3(object sender, MouseButtonEventArgs e)
        {
            checkReverseGeocoding();
        }
        private void txtReverseGeocoding_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                checkReverseGeocoding();
            }
        }

        private void txtReverseLngGeocoding_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                checkReverseGeocoding();
            }
        }
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeZoom(2);
        }

        private void Image_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            changeZoom(-2);
        }
        private void RibbonButton_Click(object sender, RoutedEventArgs e)
        {
            zoom(pushME.Location, 17);
        }
        private void listboxGeocoding_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxGeocoding.SelectedIndex !=-1)
            {
                GoogleMaps.GoogleGeocoding.Geoco geoC=((List<GoogleMaps.GoogleGeocoding.Geoco>)this.listboxGeocoding.DataContext)[0];
                zoomME(stringToLocation(geoC.Latitude, geoC.Longitude), geoC.FormatedAddress, 15);
            }
        }

        private void RibbonButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (gridGeocoding.Visibility == Visibility.Visible) { viewGeocoding(false); } else { viewGeocoding(true); }
        }
        private void RibbonButton_Click_2(object sender, RoutedEventArgs e)
        {
            if (gridReverseGeocoding.Visibility == Visibility.Visible) { viewReverseGeocoding(false); } else { viewReverseGeocoding(true); }
        }
        private void listboxReverseGeocoding_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxReveGeocoding.SelectedIndex != -1)
            {
                GoogleMaps.GoogleGeocoding.ReverseGeoco geoC = ((List<GoogleMaps.GoogleGeocoding.ReverseGeoco>)this.listboxReveGeocoding.DataContext)[listboxReveGeocoding.SelectedIndex];
                zoomME(stringToLocation(geoC.Latitude, geoC.Longitude), geoC.FormatedAddress, 15);
            }
        }
        private void RibbonButton_Click_3(object sender, RoutedEventArgs e)
        {
            if (gridPostalCode.Visibility == Visibility.Visible) { viewPostalCode(false); } else { viewPostalCode(true); }

        }
        private void txtGridPostalCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtGridPostalCode.Text != "")
            {
                PostalCode(txtGridPostalCode.Text);
            }
        }
        private void imgSearchPostalCode_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(txtGridPostalCode.Text!="")
            {
                PostalCode(txtGridPostalCode.Text);
            }
        }
        private void listboxPostalCode_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxPostalCode.SelectedIndex != -1 && this.listboxPostalCode.DataContext != null)
            {
                GoogleMaps.GoogleGeocoding.PostalCod postalC = ((List<GoogleMaps.GoogleGeocoding.PostalCod>)this.listboxPostalCode.DataContext)[0];
                GoogleMaps.GoogleGeocoding.Geocoding geocoding = new GoogleMaps.GoogleGeocoding.Geocoding();
                geocoding.FinishedReading += (e1, s1) =>
                    {
                        if(geocoding.resultGeocoding!=null)
                        {
                            zoomME(stringToLocation(geocoding.resultGeocoding.Latitude, geocoding.resultGeocoding.Longitude), geocoding.resultGeocoding.FormatedAddress, 15);
                        }
                    };
                geocoding.getGeocoding(postalC.FormatedAddress);
            }
        }

        private void ImageAltitude_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(txtGridAltitude.Text!="")
            {
                altitude(txtGridAltitude.Text);
            }
        }
        private void txtAltitude_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtGridAltitude.Text != "")
            {
                altitude(txtGridAltitude.Text);
            }
        }
        private void listboxAltitude_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxAltitude.SelectedIndex!=-1)
            {
                GoogleMaps.GoogleElevation.Elevat elevat = ((List<GoogleMaps.GoogleElevation.Elevat>)this.listboxAltitude.DataContext)[listboxAltitude.SelectedIndex];
                zoomME(stringToLocation(elevat.Latitude, elevat.Longitude), elevat.Elevation + " metros", 15);
            }
        }
        private void RibbonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (gridAltitude.Visibility == Visibility.Visible) { viewAltitude(false); } else { viewAltitude(true); }
        }
        private void ImageAltitudeCoor_MouseDown3(object sender, MouseButtonEventArgs e)
        {
            checkAltitudeCoord();
        }
        private void listboxAltitudeCoo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxAltitudeCoo.SelectedIndex != -1)
            {
                GoogleMaps.GoogleElevation.Elevat elevat = ((List<GoogleMaps.GoogleElevation.Elevat>)this.listboxAltitudeCoo.DataContext)[listboxAltitudeCoo.SelectedIndex];
                zoomME(stringToLocation(elevat.Latitude, elevat.Longitude), elevat.Elevation + " metros", 15);
            }
        }
        private void txtReverseLngAltitude_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                checkAltitudeCoord();
            }
        }

        private void txtReverseAltitude_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                checkAltitudeCoord();
            }
        }
        private void RibbonMenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (gridAltitudeCoo.Visibility == Visibility.Visible) { viewAltitudeCoo(false); } else { viewAltitudeCoo(true); }
        }

        private void RibbonButtonStreet_Click_2(object sender, RoutedEventArgs e)
        {
            if (gridStreetView.Visibility == Visibility.Visible) { viewAStreetView(false); } else { viewAStreetView(true); loadGridStreet(); }
        }
        private void ImageStreet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            checkStreetView();
        }

        private void txtStreetView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                checkStreetView();
            }
        }
        private void imgMaxiStreet_MouseUp(object sender, MouseButtonEventArgs e)
        {
            maximizeStreet(true);
        }

        private void RibbonButtonPlaces_Click_2(object sender, RoutedEventArgs e)
        {
            if (gridTypesPlaces.Visibility == Visibility.Visible) { viewTypesPlacesView(false,"gridTypesPlaces"); } else { viewTypesPlacesView(true,"gridTypesPlaces"); }
        }
        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(listboxTypesPlaces.SelectedIndex!=-1)
            {
                string typePlace = ((Static.Place)listboxTypesPlaces.Items[listboxTypesPlaces.SelectedIndex]).Name.Replace(" ", "_");
                loadPlaces(typePlace, pushME.Location);
            }
        }
        private void txtFilterPlaces_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterPlaces(txtFilterPlaces.Text);
        }
        private void ImageBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridTypesPlaces.Visibility == Visibility.Visible) { viewTypesPlacesView(false, this.txtFlagReturnGridPlaces.Text); } else { viewTypesPlacesView(true, this.txtFlagReturnGridPlaces.Text); }
        }
        private void Border_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            if (places.NextResults.NextPage != "no_token")
            {
                loadNextPlaces(places.NextResults.NextPage,pushME.Location);
            }
            else
            {
                bussyMessage(TimeSpan.FromSeconds(2), "no hay más lugares");
            }
        }

        private void txtFilterSearchPlaces_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterSearchPlaces(txtFilterSearchPlaces.Text);
        }

        private void listboxSelectedPlace_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxPlaces.SelectedIndex != -1)
            {
                Place selectedPlace = ((Place)listboxPlaces.Items[listboxPlaces.SelectedIndex]);
                zoom(selectedPlace.GeoCoordinates, 15);
                detailsPlace(selectedPlace);
            }
        }
        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(txtWebsite.Text!="")
            {
                openWeb(txtWebsite.Text);
            }
        }

        private void TextBlock_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            if (txtWebGoogle.Text != "")
            {
                openWeb(txtWebGoogle.Text);
            }
        }
        private void txtWebAuthor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(listboxReviews.SelectedIndex!=-1)
            {
                var reviews = (List<GoogleMaps.GooglePlaces.Details.Review>)listboxReviews.DataContext;
                if (reviews[listboxReviews.SelectedIndex].GooglePlusAuthor!="")
                {
                    openWeb(reviews[listboxReviews.SelectedIndex].GooglePlusAuthor);
                }
            }
          
        }

        private void RibbonButtonPlacesSearch_Click_2(object sender, RoutedEventArgs e)
        {
            if (gridPlaceSearch.Visibility == Visibility.Visible) { viewPlacesSearch(false); } else { viewPlacesSearch(true); }
        }
        private void listboxImagesPlace_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxImagesPlace.SelectedIndex != -1)
            {
                var source = (List<GoogleMaps.GooglePlaces.Details.Photo>)this.listboxImagesPlace.DataContext;
                maximizePlaceImage(true, source[listboxImagesPlace.SelectedIndex].PhotoReference);
            }

        }
        private void ImageBackImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int itemSelected = (listboxImagesPlace.SelectedIndex+1<listboxImagesPlace.Items.Count)?listboxImagesPlace.SelectedIndex+1:0;
            this.listboxImagesPlace.SelectedIndex = itemSelected;
            var source = (List<GoogleMaps.GooglePlaces.Details.Photo>)this.listboxImagesPlace.DataContext;
            maximizePlaceImage(true, source[listboxImagesPlace.SelectedIndex].PhotoReference);
        }
        private void ImageImagePlaceBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int itemSelected = (listboxImagesPlace.SelectedIndex - 1 >= 0) ? listboxImagesPlace.SelectedIndex - 1 : listboxImagesPlace.Items.Count-1;
            this.listboxImagesPlace.SelectedIndex = itemSelected;
            var source = (List<GoogleMaps.GooglePlaces.Details.Photo>)this.listboxImagesPlace.DataContext;
            maximizePlaceImage(true, source[listboxImagesPlace.SelectedIndex].PhotoReference);
        }
        private void sliderHeadingPlace_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            streetViewPlaces(((GoogleMaps.GooglePlaces.Details.Place)this.tabDetailsPlace.DataContext).GeoCoordinates,(int)sliderHeadingPlace.Value);
        }
        private void orderRating_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(listboxPlaces.Items.Count>0)
            { 
                listboxPlaces.DataContext = GoogleMaps.GooglePlaces.Places.sortRating((List<Place>)listboxPlaces.DataContext);
                listboxPlaces.ItemsSource = GoogleMaps.GooglePlaces.Places.sortRating((List<Place>)listboxPlaces.DataContext);
            }
            gridSort.Visibility = Visibility.Collapsed;
        }
        private void orderDistance_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxPlaces.Items.Count > 0)
            { 
                listboxPlaces.DataContext = GoogleMaps.GooglePlaces.Places.sortDistance((List<Place>)listboxPlaces.DataContext);
                listboxPlaces.ItemsSource = GoogleMaps.GooglePlaces.Places.sortDistance((List<Place>)listboxPlaces.DataContext);
            }
            gridSort.Visibility = Visibility.Collapsed;
        }
        private void orderHorary_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxPlaces.Items.Count > 0)
            {
                listboxPlaces.DataContext = GoogleMaps.GooglePlaces.Places.sortOpening((List<Place>)listboxPlaces.DataContext);
                listboxPlaces.ItemsSource = GoogleMaps.GooglePlaces.Places.sortOpening((List<Place>)listboxPlaces.DataContext);
            }
            gridSort.Visibility = Visibility.Collapsed;
    }
        private void RibbonButton_Click_DeletePlaces(object sender, RoutedEventArgs e)
        {
            Static.MapObject.removeMapPushpin(myMap, "place");
            bussyMessage(TimeSpan.FromSeconds(2), "lugares eliminados");
        }

        private void txtPlaceSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.txtPlaceSearch.Text != "")
            {
                loadPlaces(this.txtPlaceSearch.Text, pushME.Location, checkTypeSearch(),(Static.Place)comboTypesPlace.SelectedItem,(bool)chbLocation.IsChecked);
            }
        }
        private void ImageSearchPlace_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(this.txtPlaceSearch.Text!="")
            {
                loadPlaces(this.txtPlaceSearch.Text, pushME.Location, checkTypeSearch(),(Static.Place)comboTypesPlace.SelectedItem,(bool)chbLocation.IsChecked);
            }
        }
        private void chbLocation_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtPlaceSearch.Text != "")
            {
                loadPlaces(this.txtPlaceSearch.Text, pushME.Location, checkTypeSearch(),(Static.Place)comboTypesPlace.SelectedItem, (bool)chbLocation.IsChecked);
            }
        }
        private void TextBlockSearchTerm_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxPlaceSearch.SelectedIndex != -1)
            {
                List<Autocomplete> autocompleteList = (List<Autocomplete>)listboxPlaceSearch.DataContext;
                Autocomplete autocomplete = autocompleteList[listboxPlaceSearch.SelectedIndex];
                loadPlaces(autocomplete.Term, pushME.Location, checkTypeSearch(),(Static.Place)comboTypesPlace.SelectedItem, (bool)chbLocation.IsChecked);
                this.txtPlaceSearch.Text = autocomplete.Term;
            }
        }
        private void TextBlock_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            if (listboxPlaceSearch.SelectedIndex != -1)
            {
                List<Autocomplete> autocompleteList = (List<Autocomplete>)listboxPlaceSearch.DataContext;
                Autocomplete autocomplete = autocompleteList[listboxPlaceSearch.SelectedIndex];
                detailsPlace(autocomplete.Description, autocomplete.Reference);
                

            }
        }


        private void sliderRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            changeRadius((int)sliderRadius.Value);
        }
        private void sliderRadiusR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            changeRadius((int)sliderRibbonRadius.Value);
        }
        private void RibbonButtonSettingsPlaces_Click_2(object sender, RoutedEventArgs e)
        {
            if (gridSettingsPlaces.Visibility == Visibility.Visible) { viewSettingsPlace(false); } else { viewSettingsPlace(true); }

        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            changeSort(Places.SortBy.None);
        }
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            changeSort(Places.SortBy.Rating);
        }
        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            changeSort(Places.SortBy.Distance);
        }
        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            changeSort(Places.SortBy.Opening);
        }

        private void imgRoute_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Static.MapObject.removeRoute(myMap, "route");
            bussyMessage(TimeSpan.FromSeconds(2), "ruta eliminada");
            if (gridRoute.Height > 259) { (this.FindResource("storyRouteCollapseOFF") as Storyboard).Begin(); } else { (this.FindResource("storyRouteCollapseReduceOFF") as Storyboard).Begin(); }
            
        }
        private void imgSearchStart_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (txtAddressStart.Text != "")
            {
                geocodingRoute(txtAddressStart.Text, true);
            }
        }
        private void imgSearchFinish_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (txtAddressFinish.Text != "")
            {
                geocodingRoute(txtAddressFinish.Text, false);
            }
        }
        private void txtAddresStart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtAddressStart.Text != "")
            {
                geocodingRoute(txtAddressStart.Text, true);
            }
        }
        private void txtAddressFinish_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtAddressFinish.Text != "")
            {
                geocodingRoute(txtAddressFinish.Text, false);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            checkRoute();
        }
        private void imgWarnHints_MouseUp(object sender, MouseButtonEventArgs e)
        {
            gridWarniHints.Visibility = Visibility.Collapsed;
        }

        private void RibbonButton_Click_4(object sender, RoutedEventArgs e)
        {
            if (gridRouteSelect.Visibility == Visibility.Visible) { viewRouteSelect(false); } else { viewRouteSelect(true); }
            if (gridRoute.Visibility==Visibility.Collapsed) { 
                txtGeoAddressStart.Text = pushME.Location.ToString();
                var tooltip = pushME.ToolTip;
                if(tooltip.GetType()==typeof(string))
                {
                    txtAddressStart.Text = (string)tooltip;
                }
                if (tooltip.GetType() == typeof(ToolTip))
                {
                    txtAddressStart.Text = ((ToolTip)tooltip).Content.ToString();
                }
            }
        }
        private void Image_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            if (gridRoute.Height > 259)
            {
                (this.FindResource("storyRouteHeightOFF") as Storyboard).Begin();
            }
            else
            {
                (this.FindResource("storyRouteHeightON") as Storyboard).Begin();
            }

        }
        private void RibbonRadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            BingMaps.BingRoute.Route.Mode = BingServices.TravelMode.Driving;
        }
        private void RibbonRadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            BingMaps.BingRoute.Route.Mode = BingServices.TravelMode.Walking;
        }
        private void RibbonRadioButton_Checked_OptimizeTime(object sender, RoutedEventArgs e)
        {
            BingMaps.BingRoute.Route.RouteOptimization = BingServices.RouteOptimization.MinimizeTime;
        }
        private void RibbonRadioButton_Checked_OptimizeDistance(object sender, RoutedEventArgs e)
        {
            BingMaps.BingRoute.Route.RouteOptimization = BingServices.RouteOptimization.MinimizeDistance;
        }
        private void RibbonRadioButton_Checked_TrafficNone(object sender, RoutedEventArgs e)
        {
            BingMaps.BingRoute.Route.TrafficUsage = BingServices.TrafficUsage.None;
        }
        private void RibbonRadioButton_Checked_TrafficTime(object sender, RoutedEventArgs e)
        {
            BingMaps.BingRoute.Route.TrafficUsage = BingServices.TrafficUsage.TrafficBasedTime;
        }
        private void RibbonRadioButton_Checked_TrafficTimeRoute(object sender, RoutedEventArgs e)
        {
            BingMaps.BingRoute.Route.TrafficUsage = BingServices.TrafficUsage.TrafficBasedRouteAndTime;
            ribbonradButOptimizeTime.IsChecked = true;
        }
        private void RibbonButton_Click_5(object sender, RoutedEventArgs e)
        {
            Static.MapObject.removeRoute(myMap,"route");
            bussyMessage(TimeSpan.FromSeconds(2), "ruta eliminada");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewDetailsRoute(true);
        }
        private void RibbonButton_Click_Google(object sender, RoutedEventArgs e)
        {
            if (gridRouteGoogleSelect.Visibility == Visibility.Visible) { viewRouteGoogleSelect(false); } else { viewRouteGoogleSelect(true); }
            if (gridRouteGoogle.Visibility == Visibility.Collapsed)
            { 
                txtGeoAddressGoogleStart.Text = pushME.Location.Latitude.ToString() + "," + pushME.Location.Longitude.ToString();
                var tooltip = pushME.ToolTip;
                if(tooltip.GetType()==typeof(string))
                {
                    txtAddressGoogleStart.Text = (string)tooltip;
                }
                if (tooltip.GetType() == typeof(ToolTip))
                {
                    txtAddressGoogleStart.Text = ((ToolTip)tooltip).Content.ToString();
                }
            }
        }
        private void Button_Click_1_GoogleRoute(object sender, RoutedEventArgs e)
        {
            checkRouteGoogle();
        }
        private void RibbonButton_Click_RemoveGoogle(object sender, RoutedEventArgs e)
        {
            Static.MapObject.removeRoute(myMap, "routeGoogle");
            bussyMessage(TimeSpan.FromSeconds(2), "ruta eliminada");
        }
        private void RibbonRadioButton_Checked_Driving(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleRoute.Route.ModeRoute = GoogleMaps.GoogleRoute.Route.Mode.driving;
        }
        private void RibbonRadioButton_Checked_Walking(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleRoute.Route.ModeRoute = GoogleMaps.GoogleRoute.Route.Mode.walking;
        }
        private void RibbonRadioButton_Checked_Bicycling(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleRoute.Route.ModeRoute = GoogleMaps.GoogleRoute.Route.Mode.bicycling;
        }
        private void RibbonRadioButton_Checked_None(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleRoute.Route.AvoidRoute = GoogleMaps.GoogleRoute.Route.Avoid.none;
            modeRouteGoogle.IsChecked = true;
        }
        private void RibbonRadioButton_Checked_Highways(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleRoute.Route.AvoidRoute = GoogleMaps.GoogleRoute.Route.Avoid.highways;
            modeRouteGoogle.IsChecked = true;
        }
        private void RibbonRadioButton_Checked_Tolls(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleRoute.Route.AvoidRoute = GoogleMaps.GoogleRoute.Route.Avoid.tolls;
            modeRouteGoogle.IsChecked = true;
        }
        
        private void sliderRibbonWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderRibbonHeight != null)
            {
                GoogleMaps.GoogleStaticMaps.StaticMap.SizeMap = new Size((int)((Slider)sender).Value, (int)sliderRibbonHeight.Value);
            }
        }
        private void sliderRibbonHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderRibbonWidth != null)
            {
                GoogleMaps.GoogleStaticMaps.StaticMap.SizeMap = new Size((int)((Slider)sender).Value, (int)sliderRibbonWidth.Value);
            }
        }
        private void RibbonRadioButton_Click(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.ScaleMap = GoogleMaps.GoogleStaticMaps.StaticMap.Scale.NORMAL;
        }
        private void RibbonRadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.ScaleMap = GoogleMaps.GoogleStaticMaps.StaticMap.Scale.ADVANCED;

        }
        private void RibbonRadioButton_Checked_4(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.FormatMap = GoogleMaps.GoogleStaticMaps.StaticMap.Format.PNG;
        }
        private void RibbonRadioButton_Checked_5(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.FormatMap = GoogleMaps.GoogleStaticMaps.StaticMap.Format.PNG32;
        }
        private void RibbonRadioButton_Checked_6(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.FormatMap = GoogleMaps.GoogleStaticMaps.StaticMap.Format.GIF;
        }
        private void RibbonRadioButton_Checked_7(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.FormatMap = GoogleMaps.GoogleStaticMaps.StaticMap.Format.JPG;
        }
        private void RibbonRadioButton_Checked_8(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.FormatMap = GoogleMaps.GoogleStaticMaps.StaticMap.Format.JPG_BASELINE;
        }
        private void RibbonRadioButton_Checked_9(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.Maptype = GoogleMaps.GoogleStaticMaps.StaticMap.MapType.ROADMAP;
        }
        private void RibbonRadioButton_Checked_10(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.Maptype = GoogleMaps.GoogleStaticMaps.StaticMap.MapType.SATELLITE;
        }
        private void RibbonRadioButton_Checked_11(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.Maptype = GoogleMaps.GoogleStaticMaps.StaticMap.MapType.TERRAIN;
        }
        private void RibbonRadioButton_Checked_12(object sender, RoutedEventArgs e)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap.Maptype = GoogleMaps.GoogleStaticMaps.StaticMap.MapType.HYBRID;
        }
        private void imgHideStaticBasicMap_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridStaticMapBasic.Width > 349) { viewGoWidthStaticMap(false); } else { viewGoWidthStaticMap(true); }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            checkStaticBasicMap();
        }
        private void imgStaticMap_MouseUp(object sender, MouseButtonEventArgs e)
        {
            hideBrowserStaticMap();
        }
        private void RibbonButton_Click_6(object sender, RoutedEventArgs e)
        {
            if (gridStaticMapBasic.Visibility == Visibility.Visible) { viewStaticMap(false); } else { viewStaticMap(true); }
            txtGeoStaticMap.Text = pushME.Location.Latitude.ToString() + "," + pushME.Location.Longitude.ToString();
        }

        private void RibbonButton_Click_StaticAdvanced(object sender, RoutedEventArgs e)
        {
            if (gridStaticMapAdvanced.Visibility == Visibility.Visible) { viewStaticMapAdvanced(false); } else { viewStaticMapAdvanced(true); }
        }
        private void Image_MouseUp_6(object sender, MouseButtonEventArgs e)
        {
            addVisibilityZone(txtVisibilityZones.Text);
        }
        private void Image_MouseUp_7(object sender, MouseButtonEventArgs e)
        {
            removeVisibilityZone(listboxVisibilityZones.SelectedIndex);
        }
        private void Image_MouseUp_8(object sender, MouseButtonEventArgs e)
        {
            removeAllVisibility();
        }
        private void Image_MouseUpMarker_7(object sender, MouseButtonEventArgs e)
        {
            addMarker();
        }
        private void Image_MouseUp_RemoveMarker(object sender, MouseButtonEventArgs e)
        {
            removeMarker(listboxMarkers.SelectedIndex);
        }
        private void Image_MouseUp_removeAllAMrker(object sender, MouseButtonEventArgs e)
        {
            removeAllMarker();
        }
        private void Image_MouseUp_addNodeRoute(object sender, MouseButtonEventArgs e)
        {
            addNodeRoute(txtNodeRoute.Text);
        }
        private void Image_MouseUp_NodeRoute(object sender, MouseButtonEventArgs e)
        {
            removeNodeRoute(listboxNodesRoute.SelectedIndex);
        }
        private void Image_MouseUp_deleteallNodeRoute(object sender, MouseButtonEventArgs e)
        {
            removeAllNodeRoute();
        }
        private void sliderGamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtsliderGamma != null)
            {
                txtsliderGamma.Text = (sliderGamma.Value / 100).ToString("0.0");
            }
        }
        private void Image_MouseUp_AddFeatures(object sender, MouseButtonEventArgs e)
        {
            if(listboxFeatures.SelectedIndex!=-1)
            {
                addFeature(listboxFeatures.SelectedItem.ToString());
            }
        }
        private void Image_MouseUp_RemoveFeature(object sender, MouseButtonEventArgs e)
        {
            if (listboxFeaturesSelected.SelectedIndex != -1)
            {
                removeFeature(listboxFeaturesSelected.SelectedIndex);
            }
        }
        private void Image_MouseUp_RemoveAllFeature(object sender, MouseButtonEventArgs e)
        {
            removeAllFeature();
        }
        private void Image_MouseUp_AddStyle(object sender, MouseButtonEventArgs e)
        {
            addStyle();
        }
        private void Image_MouseUp_removeStyle(object sender, MouseButtonEventArgs e)
        {
            if(listboxStyles.SelectedIndex!=-1)
            {
                removeStyle(listboxStyles.SelectedIndex);
            }
        }
        private void Image_MouseUp_removeAllStyles(object sender, MouseButtonEventArgs e)
        {
            removeAllStyle();
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            createStaticMap();
        }
        private void imgStaticMap_CloseBrowserAdvanced(object sender, MouseButtonEventArgs e)
        {
            viewBroserStaticAdvanced(false);
        }
        private void Image_MouseUp_9(object sender, MouseButtonEventArgs e)
        {
            copySelectedRequest(listboxRequest.SelectedIndex);
        }
        private void Image_MouseUp_10(object sender, MouseButtonEventArgs e)
        {
            copyAllRequest();
        }
        private void Image_MouseUp_11(object sender, MouseButtonEventArgs e)
        {
            saveApiKey(txtApiKey.Text);
        }
        private void Image_MouseUp_BingKey(object sender, MouseButtonEventArgs e)
        {
            saveBingKey(txtBingKey.Text);
        }
        private void TextBlock_MouseUp_4(object sender, MouseButtonEventArgs e)
        {
            openWeb("https://accounts.google.com/ServiceLogin?service=devconsole&passive=1209600&continue=https%3A%2F%2Fcode.google.com%2Fapis%2Fconsole%2F%3Fhl%3Dbn&followup=https%3A%2F%2Fcode.google.com%2Fapis%2Fconsole%2F%3Fhl%3Dbn&hl=bn");
        }

        private void TextBlock_MouseUp_BingWeb(object sender, MouseButtonEventArgs e)
        {
            openWeb("https://www.bingmapsportal.com/");
        }
        private void TextBlock_MouseUp_5(object sender, MouseButtonEventArgs e)
        {
            openWeb(((TextBlock)sender).Text);
        }
        private void RibbonButton_Click_9(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RibbonApplicationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (gridRequest.Visibility == Visibility.Visible) { viewRequest(false); } else { viewRequest(true); }
        }
        private void RibbonApplicationMenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (gridApiKey.Visibility == Visibility.Visible) { viewApiKey(false); } else { viewApiKey(true); }
        }
        private void RibbonApplicationMenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            copyPosition();
            (this.FindResource("storyMessageCopyON") as Storyboard).Begin();
        }
        private void RibbonApplicationMenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Static.MapObject.removeMapPushpin(myMap, "place");
            Static.MapObject.removeMapPushpin(myMap, "pushAlternative");
            Static.MapObject.removeMapPushpin(myMap, "place");
            Static.MapObject.removeRoute(myMap, "route");
            Static.MapObject.removeRoute(myMap, "routeGoogle");
            bussyMessage(TimeSpan.FromSeconds(2), "eliminado contenido del mapa");
        }
        private void RibbonApplicationMenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Static.MapObject.removeMapPushpin(myMap, "place");
            bussyMessage(TimeSpan.FromSeconds(2), "lugares eliminados");
        }
        private void RibbonApplicationMenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            Static.MapObject.removeRoute(myMap, "route");
            bussyMessage(TimeSpan.FromSeconds(2), "ruta eliminada");
        }
        private void RibbonApplicationMenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            Static.MapObject.removeRoute(myMap, "routeGoogle");
            bussyMessage(TimeSpan.FromSeconds(2), "ruta eliminada");
        }
        private void TextBlock_MouseUp_6(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText("luis.m.r@outlook.com");
            (this.FindResource("storyMessageCopyON") as Storyboard).Begin();
        }
        private void RibbonApplicationMenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            if (gridAbout.Visibility == Visibility.Visible) { viewAbout(false); } else { viewAbout(true); }
        }

        private void RibbonButton_Click_10(object sender, RoutedEventArgs e)
        {
            if (gridAbout.Visibility == Visibility.Visible) { viewAbout(false); } else { viewAbout(true); }
        }
        #endregion
       
        private void geocoding(string address)
        {
            startProgress("buscando coordenadas");
            GoogleMaps.GoogleGeocoding.Geocoding geocoding = new GoogleMaps.GoogleGeocoding.Geocoding();
            geocoding.FinishedReading += (e1, s1) =>
            {
                stopProgress();
                if (geocoding != null)
                {
                    zoomME(stringToLocation(geocoding.resultGeocoding.Latitude, geocoding.resultGeocoding.Longitude), geocoding.resultGeocoding.FormatedAddress, 15);
                }
            };
            geocoding.getGeocoding(address);
        }
        private void GeocodingGrid(string address)
        {
            startProgress("buscando coordenadas");
            GoogleMaps.GoogleGeocoding.Geocoding geocoding = new GoogleMaps.GoogleGeocoding.Geocoding();
            geocoding.FinishedReading += (e1, s1) =>
                {
                    stopProgress();
                    if (geocoding != null && geocoding.resultGeocoding!=null)
                    {
                        List<GoogleMaps.GoogleGeocoding.Geoco> listGeo = new List<GoogleMaps.GoogleGeocoding.Geoco>();
                        listGeo.Add(geocoding.resultGeocoding);
                        listboxGeocoding.ItemsSource = listGeo;
                        listboxGeocoding.DataContext = listGeo;
                        bussyMessage(TimeSpan.FromSeconds(2), geocoding.resultGeocoding.FormatedAddress);
                    }
                };
            geocoding.getGeocoding(address);
        }
        private void ReverseGeocodingGrid(Location coordinates)
        {
            startProgress("buscando localizaciones");
            GoogleMaps.GoogleGeocoding.ReverseGeocoding reverse = new GoogleMaps.GoogleGeocoding.ReverseGeocoding();
            reverse.getReverseGeocoding(coordinates);
            reverse.FinishedReading += (e1, s1) =>
            {
                stopProgress();
                if (reverse != null && reverse.resultReverseGeocoding != null)
                {
                    listboxReveGeocoding.ItemsSource = reverse.resultReverseGeocoding;
                    listboxReveGeocoding.DataContext = reverse.resultReverseGeocoding;
                    bussyMessage(TimeSpan.FromSeconds(2), reverse.resultReverseGeocoding[0].FormatedAddress);
                }

            };
        }
        private void PostalCode(string address)
        {
            startProgress("buscando código postal");
            GoogleMaps.GoogleGeocoding.PostalCode postalCode = new GoogleMaps.GoogleGeocoding.PostalCode();
            postalCode.FinishedReading += (e1, s1) =>
            {
                stopProgress();
                if (postalCode != null && postalCode.resultPostal != null)
                {
                    List<GoogleMaps.GoogleGeocoding.PostalCod> listPostal = new List<GoogleMaps.GoogleGeocoding.PostalCod>();
                    listPostal.Add(postalCode.resultPostal);
                    listboxPostalCode.DataContext = null;
                    if(listPostal[0].PostalCode!=null)
                    {
                        listboxPostalCode.DataContext = listPostal;
                    }
                    else
                    {
                        listPostal[0].PostalCode = "No se han encontrado resultados. Sea más específico.";

                    }
                    listboxPostalCode.ItemsSource = listPostal;
                    bussyMessage(TimeSpan.FromSeconds(2), postalCode.resultPostal.PostalCode);
                }
            };
            postalCode.getPostalCode(address);
        }
        private void altitude(string address)
        {
            startProgress("buscando coordenadas");
            GoogleMaps.GoogleGeocoding.Geocoding geocoding = new GoogleMaps.GoogleGeocoding.Geocoding();
            geocoding.FinishedReading += (e1, s1) =>
            {
                stopProgress();
                if (geocoding != null)
                {
                    startProgress("buscando altitud");
                    GoogleMaps.GoogleElevation.Elevation elevation=new GoogleMaps.GoogleElevation.Elevation();
                    elevation.FinishedReading += (e2, s2) =>
                        {
                            stopProgress();
                            if(elevation.resultElevation!=null && elevation.resultElevation.Count>0)
                            {
                                this.listboxAltitude.ItemsSource = elevation.resultElevation;
                                this.listboxAltitude.DataContext = elevation.resultElevation;
                                bussyMessage(TimeSpan.FromSeconds(2), elevation.resultElevation[0].Elevation + " metros");
                            }
                        };
                    elevation.getElevation(new List<Location>() { stringToLocation(geocoding.resultGeocoding.Latitude, geocoding.resultGeocoding.Longitude) });
                }
            };
            geocoding.getGeocoding(address);
        }
        private void altitude(Location coordinate)
        {
            startProgress("buscando altitud");
            GoogleMaps.GoogleElevation.Elevation elevation = new GoogleMaps.GoogleElevation.Elevation();
            elevation.FinishedReading += (e2, s2) =>
            {
                stopProgress();
                if (elevation.resultElevation != null && elevation.resultElevation.Count > 0)
                {
                    this.listboxAltitudeCoo.ItemsSource = elevation.resultElevation;
                    this.listboxAltitudeCoo.DataContext = elevation.resultElevation;
                    bussyMessage(TimeSpan.FromSeconds(2), elevation.resultElevation[0].Elevation + " metros");
                }
            };
            elevation.getElevation(new List<Location>() { coordinate});
        }
        private void checkReverseGeocoding()
        {
            if (txtLatReveGeo.Text != "" && txtLngReveGeo.Text != "")
            {
                Location location = stringToLocation(txtLatReveGeo.Text, txtLngReveGeo.Text);
                if (location != null) { ReverseGeocodingGrid(location); }
            }
        }
        private void checkAltitudeCoord()
        {
            if (txtAltitudeLat.Text != "" && txtAltitudeLng.Text != "")
            {
                Location location = stringToLocation(txtAltitudeLat.Text, txtAltitudeLng.Text);
                if (location != null) { altitude(location); }
            }
        }
        private void loadGridStreet()
        {
            this.txtStreetView.Text = pushME.Location.Latitude + "," + pushME.Location.Longitude;
            checkStreetView();
        }
        private void checkStreetView()
        {
            if(txtStreetView.Text!="")
            {
                streetViewImage(txtStreetView.Text, new Size((int)sliderStreetWidth.Value, (int)sliderStreetHeight.Value), (int)sliderHeading.Value, (int)sliderPictch.Value, (int)sliderZoom.Value);
            }
        }
        private void maximizeStreet(bool view)
        {
            if(view)
            {
                gridMaxStreet.Visibility = Visibility;
                this.imgStreetMaxView.Source = this.imgStreetView.Source;
            }
            else
            {
                gridMaxStreet.Visibility = Visibility.Collapsed;
            }
        }
        private void streetViewImage(string address, Size size, int heading, int pitch, int fov)
        {
            GoogleMaps.GoogleStreet.StreetView streetView = new GoogleMaps.GoogleStreet.StreetView();
            string urlStreet = streetView.getImage(address, size, heading, pitch, fov);
            startProgress("cargando imagen");
            BitmapImage image=new BitmapImage(new Uri(urlStreet,UriKind.Absolute));
            imgStreetView.Source = image;

            image.DownloadCompleted += (e1, s1) =>
                {
                    stopProgress();
                    bussyMessage(TimeSpan.FromSeconds(2), "imagen " + address + " cargada");
                    imgStreetView.Source = image;
                };
        }

        GoogleMaps.GooglePlaces.Places places;
        private void loadPlaces(string typePlace,Location coordinates)
        {
            startProgress("buscando lugares");
            places = new Places();
            GoogleMaps.GooglePlaces.Places.TypesPlaces placeSelected = (GoogleMaps.GooglePlaces.Places.TypesPlaces)Enum.Parse(typeof(GoogleMaps.GooglePlaces.Places.TypesPlaces), typePlace, true);
            places.FinishedReading += (e1, s1) =>
                {
                    stopProgress();
                    if(places.PlaceList!=null && places.PlaceList.Count>0)
                    {
                        loadPushPinPlace(places.PlaceList);
                        viewPlacesView(true, "gridTypesPlaces");
                    }
                    bussyMessage(TimeSpan.FromSeconds(2), places.PlaceList.Count.ToString() + " lugares (" + typePlace + ") encontrados");
                };
            places.getPlaces(coordinates, new List<Places.TypesPlaces> { placeSelected }, "es");

        }
        private void loadNextPlaces(string token, Location coordinates)
        {
            startProgress("buscando más lugares");
            places = new Places();
            places.FinishedReading += (e1, s1) =>
            {
                stopProgress();
                if (places.PlaceList != null && places.PlaceList.Count > 0)
                {
                    loadPushPinPlace(places.PlaceList);
                }
                bussyMessage(TimeSpan.FromSeconds(2), places.PlaceList.Count.ToString() + " lugares encontrados");
                viewPlacesView(true, this.txtFlagReturnGridPlaces.Text);
            };
            places.getPlaces(coordinates,token);
        }
        private void loadPlaces(string input, Location coordinates, Maps.NET.GoogleMaps.GooglePlaces.Places.TypeSearch typeSearch, Static.Place typePlace, bool location = true)
        {
            startProgress("buscando " + input);
            places = new Places();
            GoogleMaps.GooglePlaces.Places.TypesPlaces placeSelected = (GoogleMaps.GooglePlaces.Places.TypesPlaces)Enum.Parse(typeof(GoogleMaps.GooglePlaces.Places.TypesPlaces), typePlace.Name.Replace(" ","_"), true);

            places.FinishedReading += (e1, s1) =>
            {
                stopProgress();
                if (places.PlaceList != null && places.PlaceList.Count > 0)
                {
                    loadPushPinPlace(places.PlaceList);
                    bussyMessage(TimeSpan.FromSeconds(2), places.PlaceList.Count.ToString() + " " + input + " encontrados");
                    viewPlacesView(true,"gridPlaceSearch");
                }
                else
                {
                    loadAutocomplete(input, coordinates,location);
                }
                
            };
            places.getPlaces(coordinates, new List<Places.TypesPlaces> { placeSelected }, "es", input, typeSearch);
        }
        private void detailsPlace(Place place)
        {
            startProgress("buscando detalles de " + place.Name);
            GoogleMaps.GooglePlaces.Details.DetailsPlace detailsPlace = new GoogleMaps.GooglePlaces.Details.DetailsPlace();
            detailsPlace.FinishedReading += (e1, s1) =>
            {
                stopProgress();
                bussyMessage(TimeSpan.FromSeconds(2), "detalles de lugar cargados");
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                {
                    tabDetailsPlace.DataContext = detailsPlace.PlaceDetails;
                    if (detailsPlace.PlaceDetails.WebGoogle != "")
                    {
                        webBrowserWebgoogle.Source = new Uri(detailsPlace.PlaceDetails.WebGoogle, UriKind.Absolute);
                    }
                    if (detailsPlace.PlaceDetails.Website != "")
                    {
                        webBrowserWebsite.Source = new Uri(detailsPlace.PlaceDetails.Website, UriKind.Absolute);
                    }
                    listboxReviews.DataContext = detailsPlace.Reviews;
                    this.listboxImagesPlace.DataContext = detailsPlace.Photos;
                    streetViewPlaces(detailsPlace.PlaceDetails.GeoCoordinates, (int)sliderHeadingPlace.Value);
                    viewDetailsPlacesView(true);
                }));
            };
            detailsPlace.getDetails(place.PlaceReference, pushME.Location);
        }
        private void detailsPlace(string name, string reference)
        {
            startProgress("buscando detalles de " + name);
            GoogleMaps.GooglePlaces.Details.DetailsPlace detailsPlace = new GoogleMaps.GooglePlaces.Details.DetailsPlace();
            detailsPlace.FinishedReading += (e1, s1) =>
            {
                stopProgress();
                bussyMessage(TimeSpan.FromSeconds(2), "detalles de lugar cargados");
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                {
                    tabDetailsPlace.DataContext = detailsPlace.PlaceDetails;
                    if (detailsPlace.PlaceDetails.WebGoogle != "")
                    {
                        webBrowserWebgoogle.Source = new Uri(detailsPlace.PlaceDetails.WebGoogle, UriKind.Absolute);
                    }
                    if (detailsPlace.PlaceDetails.Website != "")
                    {
                        webBrowserWebsite.Source = new Uri(detailsPlace.PlaceDetails.Website, UriKind.Absolute);
                    }
                    listboxReviews.DataContext = detailsPlace.Reviews;
                    this.listboxImagesPlace.DataContext = detailsPlace.Photos;
                    streetViewPlaces(detailsPlace.PlaceDetails.GeoCoordinates, (int)sliderHeadingPlace.Value);
                    viewDetailsPlacesView(true);
                    Place place = new Place(); place.Name = detailsPlace.PlaceDetails.Name; place.PlaceReference = reference; place.GeoCoordinates = detailsPlace.PlaceDetails.GeoCoordinates; place.Activity = detailsPlace.PlaceDetails.Activity; place.Rating = detailsPlace.PlaceDetails.Rating;
                    loadPushPinPlace(place);
                    zoom(detailsPlace.PlaceDetails.GeoCoordinates, 15);
                }));
            };
            detailsPlace.getDetails(reference, pushME.Location);
        }
        private void maximizePlaceImage(bool view, string urlImage = "")
        {
            if (view)
            {
                this.imgPlaceMaxView.Source = null;
                startProgress("cargando imagen");
                gridMaxImagePlace.Visibility = Visibility;
                BitmapImage bmp = new BitmapImage(new Uri(urlImage, UriKind.Absolute));
                bmp.DownloadCompleted += (e1, s1) =>
                {
                    this.imgPlaceMaxView.Source = bmp;
                    bussyMessage(TimeSpan.FromSeconds(2), "imagen del lugar cargada");
                };
                bmp.DownloadFailed += (e1, s1) =>
                {
                    bussyMessage(TimeSpan.FromSeconds(2), "no se ha podido cargar la imagen. Inténtelo más tarde");
                };
            }
            else
            {
                gridMaxImagePlace.Visibility = Visibility.Collapsed;
            }
        }
        private void filterPlaces(string textFilter)
        {
            this.listboxTypesPlaces.ItemsSource = Static.StaticPlaces.listPlaces.Where(w => w.Name.ToUpper().Contains(textFilter.ToUpper()));
        }
        private void filterSearchPlaces(string textFilter)
        {
            this.listboxPlaces.ItemsSource = ((List<Place>)this.listboxPlaces.DataContext).Where(w => w.Name.ToUpper().Contains(textFilter.ToUpper()));
        }
        private void streetViewPlaces(Location coordinates, int valueHeading)
        {
            GoogleMaps.GoogleStreet.StreetView street = new GoogleMaps.GoogleStreet.StreetView();
            BitmapImage bmp = new BitmapImage(new Uri(street.getImage(coordinates.Latitude.ToString().Replace(",", ".") + "," + coordinates.Longitude.ToString().Replace(",", "."),
                new Size(574, 387), valueHeading, 0, 90), UriKind.Absolute));
            this.imgStreetPlace.Source = bmp;
        }
        private void loadAutocomplete(string input, Location coordinates, bool location)
        {
            startProgress("buscando coincidencias con " + input);
            places = new Places();
            places.FinishedReadingAutocomplete += (e1, s1) =>
            {
                stopProgress();
                if (places.AutocompleteList != null && places.AutocompleteList.Count > 0)
                {
                    this.listboxPlaceSearch.DataContext=places.AutocompleteList;
                    this.listboxPlaceSearch.ItemsSource = places.AutocompleteList;
                    if (this.gridPlaceSearch.Height < 370) { (this.FindResource("storySearchPlaceHeightON") as Storyboard).Begin(); }
                }
                bussyMessage(TimeSpan.FromSeconds(2), places.AutocompleteList.Count.ToString() + " " + input + " encontrados");

            };
            places.getAutocomplete(input,coordinates,location,"es");
        }
        private void loadPushPinPlace(List<GoogleMaps.GooglePlaces.Place> listPlaces)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                this.listboxPlaces.ItemsSource = listPlaces;
                this.listboxPlaces.DataContext = listPlaces;
                Static.MapObject.removeMapPushpin(myMap, "place");
                Pushpin pushAdd;
                ToolTip tool;
                foreach (var item in listPlaces)
                {
                    pushAdd = new Pushpin();
                    pushAdd.Tag = "place";
                    pushAdd.Location = new Location(item.GeoCoordinates.Latitude, item.GeoCoordinates.Longitude);
                    pushAdd.DataContext = item;
                    tool = new System.Windows.Controls.ToolTip();
                    tool.Content = item.Name;
                    pushAdd.ToolTip = tool;
                    pushAdd.Content = item.Rating;
                    myMap.Children.Add(pushAdd);
                    pushAdd.MouseEnter+=push_MouseEnter;
                    pushAdd.MouseLeave += pushAdd_MouseLeave;
                }
                //zoom(listPlaces[listPlaces.Count - 1].GeoCoordinates, 15);
            }));

        }
        private void loadPushPinPlace(GoogleMaps.GooglePlaces.Place place)
        {
            Pushpin pin = new Pushpin();
            pin.Location = place.GeoCoordinates;
            pin.Background = Brushes.Yellow;
            pin.Content = place.Rating;
            pin.Foreground = Brushes.Black;
            pin.Tag = "place";
            ToolTip tool = new System.Windows.Controls.ToolTip();
            tool.Content = place.Name;
            pin.ToolTip = tool;
            pin.DataContext = place;
            pin.MouseEnter += push_MouseEnter;
            pin.MouseLeave += pushAdd_MouseLeave;
            // Adds the pushpin to the map.
            myMap.Children.Add(pin);
        }
        private void changeRadius(int radius)
        {
            GoogleMaps.GooglePlaces.Places.distance = radius;
        }
        private void changeSort(GoogleMaps.GooglePlaces.Places.SortBy sort)
        {
            GoogleMaps.GooglePlaces.Places.sortBy = sort;
        }

        #region "Route Google"
        GoogleMaps.GoogleRoute.GeneralDirections generalDatesRoute;
        private void checkRouteGoogle()
        {
            if ((txtAddressGoogleStart.Text != "" || txtGeoAddressGoogleStart.Text != "") && (txtAddressGoogleFinish.Text != "" || txtGeoAddressGoogleFinish.Text != ""))
            {
                string start = (txtGeoAddressGoogleStart.Text == "") ? txtAddressGoogleStart.Text : txtGeoAddressGoogleStart.Text;
                string finish = (txtGeoAddressGoogleFinish.Text == "") ? txtAddressGoogleFinish.Text : txtGeoAddressGoogleFinish.Text;
                loadRoute(start, finish);
            }
        }
        private void loadRoute(string startPoint,string endPoint)
        {
            Static.MapObject.removeRoute(myMap, "routeGoogle");
            startProgress("calculando ruta");
            GoogleMaps.GoogleRoute.Route route = new GoogleMaps.GoogleRoute.Route();
            route.FinishedReading += (e1, s1) =>
                    {
                stopProgress();
                if (route.RouteList != null && route.RouteList.Count > 0)
                {
                    loadRouteInMap(route.RouteList);
                    this.listboxRouteGoogle.DataContext = route.RouteList; this.listboxRouteGoogle.ItemsSource = route.RouteList;
                    loadGeneralDatesRoute(route.GeneralDatesRoute);
                    this.generalDatesRoute = route.GeneralDatesRoute;
                    refreshEventsMap();
                }
                bussyMessage(TimeSpan.FromSeconds(2), "ruta calculada");
            };
            route.getRoute(startPoint, endPoint);
        }
        private void loadRouteInMap(List<GoogleMaps.GoogleRoute.Direction> directions)
        {
            viewDetailsRouteGoogle(true);
            MapLayer mapLayer = new MapLayer();
            var line = new MapPolyline
            {
                Locations = new LocationCollection(),
                Stroke = Brushes.OrangeRed,
                Opacity = 0.6,
                StrokeThickness = 8,
                Tag = "routeGoogle"
            };
            foreach (var item in directions)
            {
                line.MouseEnter += line_MouseEnter;
                line.MouseLeave += line_MouseLeave;
                line.MouseMove += line_MouseMove;
                foreach (var item2 in item.DecodePolyline)
                {
                    line.Locations.Add(item2);
                }
            }

            int i = 0;
            foreach (var item in directions)
            {
                Border brd = new Border(); brd.Width = 20; brd.Height = 20; brd.BorderBrush = Brushes.White; brd.CornerRadius = new CornerRadius(10); brd.Background = Brushes.Black; brd.Opacity = 1; brd.HorizontalAlignment = HorizontalAlignment.Center; brd.VerticalAlignment = VerticalAlignment.Center;
                brd.Background = gradienteBorder(); brd.Tag = "routeGoogle";
                TextBlock txt = new TextBlock(); txt.Text = i.ToString(); txt.Foreground = Brushes.White; txt.FontWeight = FontWeights.Bold; txt.HorizontalAlignment = HorizontalAlignment.Center; txt.VerticalAlignment = VerticalAlignment.Center;
                if (i == 0)
                {
                    brd.Background = Brushes.Green; brd.Width = 25; brd.Height = 20; brd.CornerRadius = new CornerRadius(2); brd.BorderBrush = Brushes.White; brd.BorderThickness = new Thickness(1);
                    Image img = new Image(); img.Source = createIcon(new Uri("/Assets/Route/Start.png", UriKind.Relative)); img.Stretch = Stretch.None;
                    brd.Child = img;
                    Canvas.SetZIndex(brd, 500);
                    zoom(item.StartGeocoordinate, 15);
                }
                else if (i == directions.Count-1)
                {
                    brd.Background = Brushes.Red; brd.Width = 25; brd.Height = 20; brd.CornerRadius = new CornerRadius(2); brd.BorderBrush = Brushes.White; brd.BorderThickness = new Thickness(1);
                    Image img = new Image(); img.Source = createIcon(new Uri("/Assets/Route/Start.png", UriKind.Relative)); img.Stretch = Stretch.None;
                    brd.Child = img;
                    Canvas.SetZIndex(brd, 500);
                }
                else
                {
                    brd.Child = txt;
                }
                brd.DataContext = item;
                brd.MouseEnter+=brdGoogle_MouseEnter;
                brd.MouseLeave += brdGoogle_MouseLeave;
                mapLayer.AddChild(brd, item.StartGeocoordinate, PositionOrigin.Center);
                i++;
            }
            line.Tag = "routeGoogle";
            myMap.Children.Add(line);
            mapLayer.Tag = "routeGoogle";
            myMap.Children.Add(mapLayer);
        }
        private void loadGeneralDatesRoute(GoogleMaps.GoogleRoute.GeneralDirections resultRoute)
        {
            txtCopyRightGoogle.Text=resultRoute.Copyright;
            txtDistanceGoogle.Text=((int)(resultRoute.DistanceMeters)/1000).ToString();
            txtTimeGoogle.Text = resultRoute.Duration.ToString();
        }
        private void altitudeRoute(List<GoogleMaps.GoogleRoute.Direction> routeList)
        {
            startProgress("buscando altitud");
            List<Location> coordinates = new List<Location>();
            foreach (var item in routeList)
            {
                coordinates.Add(item.StartGeocoordinate);
            }
            GoogleMaps.GoogleElevation.Elevation elevation = new GoogleMaps.GoogleElevation.Elevation();
            elevation.FinishedReading += (e2, s2) =>
            {

                stopProgress();
                List<KeyValuePair<int, double>> listChar = new List<KeyValuePair<int, double>>();
                int i = 0;
                foreach (var item in elevation.resultElevation)
                {
                    listChar.Add(new KeyValuePair<int, double>(i, Convert.ToDouble(item.Elevation)));
                    i++;
                }
                chartAltitude.ItemsSource = listChar;
            };
            elevation.getElevation(coordinates);
        }
        void brdGoogle_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopupRoute.Visibility = Visibility.Collapsed;
        }
        void brdGoogle_MouseEnter(object sender, MouseEventArgs e)
        {
            GoogleMaps.GoogleRoute.Direction direction = (GoogleMaps.GoogleRoute.Direction)(((Border)sender).DataContext);
            ContentPopupRoute.DataContext = ((Border)sender).DataContext;
            ContentPopupRouteText.Text = direction.Description;
            Point point = e.GetPosition(myMap);
            Location location = direction.StartGeocoordinate;
            MapLayer.SetPosition(ContentPopupRoute, location);
            ContentPopupHint.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
            {
                MapLayer.SetPositionOffset(ContentPopupRoute, new Point(3, -ContentPopupRoute.ActualHeight));
                return null;
            }), null);
            Canvas.SetZIndex(ContentPopupLayerRoute, 100);
            ContentPopupRoute.Visibility = Visibility.Visible;
        }

        private void listboxRouteGoogle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxRouteGoogle.SelectedIndex != -1)
            {
                List<GoogleMaps.GoogleRoute.Direction> directions = (List<GoogleMaps.GoogleRoute.Direction>)listboxRouteGoogle.DataContext;
                GoogleMaps.GoogleRoute.Direction direction = directions[listboxRouteGoogle.SelectedIndex];
                zoom(direction.StartGeocoordinate, 15);
            }
        }
        private void TextBlock_MouseUp_Google(object sender, MouseButtonEventArgs e)
        {
            var routeList = (List<GoogleMaps.GoogleRoute.Direction>)listboxRouteGoogle.DataContext;
            if (routeList != null)
            {
                viewDetailsRoute(false);
                viewAltitudeRoute(true);
                altitudeRoute(routeList);
            }
        }

        private void TextBlock_MouseUp_StaticRouteGoogle(object sender, MouseButtonEventArgs e)
        {
            showStaticMapRoute(generalDatesRoute.EncodeOverviewPolyline, generalDatesRoute.StartPoint, generalDatesRoute.EndPoint, new Size(540, 390));
        }
        private void Image_MouseUp_4(object sender, MouseButtonEventArgs e)
        {
            if (listboxRouteGoogle.SelectedIndex != -1)
            {
                List<GoogleMaps.GoogleRoute.Direction> directions = (List<GoogleMaps.GoogleRoute.Direction>)listboxRouteGoogle.DataContext;
                GoogleMaps.GoogleRoute.Direction direction = directions[listboxRouteGoogle.SelectedIndex];
                string address = direction.StartLatitude + "," + direction.StartLongitude;
                this.txtStreetView.Text = address;
                viewAStreetView(true);
                checkStreetView();
            }
        }
        private void Image_MouseUp_5(object sender, MouseButtonEventArgs e)
        {
            if (listboxRouteGoogle.SelectedIndex != -1)
            {
                List<GoogleMaps.GoogleRoute.Direction> directions = (List<GoogleMaps.GoogleRoute.Direction>)listboxRouteGoogle.DataContext;
                GoogleMaps.GoogleRoute.Direction direction = directions[listboxRouteGoogle.SelectedIndex];
                showStaticMapRoute(direction.EncodePolyline, direction.StartGeocoordinate, direction.FinishGeocoordinate, new Size(540, 390));
            }
        }
        #endregion
        #region "Route Bing"
        BingServices.RouteResponse resultRoute;
        private async void getRoute(Location coordinatesStart, Location coordinatesEnd)
        {
            Static.MapObject.removeRoute(myMap, "route");
            try
            {
            startProgress("calculando ruta");
            BingMaps.BingRoute.Route route = new BingMaps.BingRoute.Route();
            Task<BingServices.RouteResponse> taskRoute = route.getRoute(coordinatesStart, coordinatesEnd);
            BingServices.RouteResponse resultRoute = await taskRoute;
            this.resultRoute = resultRoute;
            bussyMessage(TimeSpan.FromSeconds(2), "ruta calculada");
            if (resultRoute.Result != null)
            {
                refreshEventsMap();
                listboxRoute.DataContext = route.listDirections; listboxRoute.ItemsSource = route.listDirections;
                MapLayer mapLayer = new MapLayer(); 
                var line = new MapPolyline
                {
                    Locations = new LocationCollection(),
                    Stroke = Brushes.Blue,
                    Opacity = 0.6,
                    StrokeThickness = 8,
                    Tag="route"
                };
                foreach (var item in resultRoute.Result.RoutePath.Points)
                {
                    line.MouseEnter += line_MouseEnter;
                    line.MouseLeave += line_MouseLeave;
                    line.MouseMove += line_MouseMove;
                    line.Locations.Add(new Location(item.Latitude, item.Longitude));
                }
               


                int i = 0;
                foreach (var item in resultRoute.Result.Legs[0].Itinerary)
                {
                    Border brd = new Border(); brd.Width = 20; brd.Height = 20; brd.BorderBrush = Brushes.White; brd.CornerRadius = new CornerRadius(10); brd.Background = Brushes.Black; brd.Opacity = 1; brd.HorizontalAlignment = HorizontalAlignment.Center; brd.VerticalAlignment = VerticalAlignment.Center;
                    brd.Background = gradienteBorder(); brd.Tag = "route";
                    TextBlock txt = new TextBlock(); txt.Text = i.ToString(); txt.Foreground = Brushes.White; txt.FontWeight = FontWeights.Bold; txt.HorizontalAlignment = HorizontalAlignment.Center; txt.VerticalAlignment = VerticalAlignment.Center;
                    if (i == 0)
                    {
                        brd.Background = Brushes.Green; brd.Width = 25; brd.Height = 20; brd.CornerRadius = new CornerRadius(2); brd.BorderBrush = Brushes.White; brd.BorderThickness = new Thickness(1);
                        Image img = new Image(); img.Source = createIcon(new Uri("/Assets/Route/Start.png", UriKind.Relative)); img.Stretch = Stretch.None;
                        brd.Child = img;
                        Canvas.SetZIndex(brd, 500);
                        zoom(new Location(item.Location.Latitude, item.Location.Longitude), 15);
                    }
                    else if (i == resultRoute.Result.Legs[0].Itinerary.Length - 1)
                    {
                        brd.Background = Brushes.Red; brd.Width = 25; brd.Height = 20; brd.CornerRadius = new CornerRadius(2); brd.BorderBrush = Brushes.White; brd.BorderThickness = new Thickness(1);
                        Image img = new Image(); img.Source = createIcon(new Uri("/Assets/Route/Start.png", UriKind.Relative)); img.Stretch = Stretch.None;
                        brd.Child = img;
                        Canvas.SetZIndex(brd, 500);
                    }
                    else
                    {
                        brd.Child = txt;
                    }
                    brd.DataContext = route.listDirections[i];
                    brd.MouseEnter += brd_MouseEnter;
                    brd.MouseLeave += brd_MouseLeave;
                    mapLayer.AddChild(brd, new Location(item.Location.Latitude, item.Location.Longitude), PositionOrigin.Center);
                    i++;
                }


                MapLayer mapLayerWarnings = new MapLayer();
                foreach (var item in route.listDirections)
                {
                    if(item.Warnings.Count>0)
                    {
                        Image img = new Image(); img.Source = createIcon(new Uri(item.ImageWarnings, UriKind.Relative)); img.Width = 20; img.Height = 20; 
                        Canvas.SetZIndex(img, 499);
                        img.DataContext = item;
                        img.MouseEnter += img_MouseEnter;
                        img.MouseLeave += img_MouseLeave;
                        mapLayerWarnings.AddChild(img, new Location(item.Location.Latitude, item.Location.Longitude), new Point(10, -10));
                    }
                }
                MapLayer mapLayerHints = new MapLayer(); 
                foreach (var item in route.listDirections)
                {
                    if (item.Hints.Count > 0)
                    {
                        Image imgH = new Image(); imgH.Source = createIcon(new Uri(item.ImageHints, UriKind.Relative)); imgH.Width = 20;
                        Canvas.SetZIndex(imgH, 499);
                        imgH.DataContext = item;
                        imgH.MouseEnter += imgH_MouseEnter;
                        imgH.MouseLeave += imgH_MouseLeave;
                        mapLayerHints.AddChild(imgH, new Location(item.Location.Latitude, item.Location.Longitude), new Point(10,-10));
                    }
                }
                line.Tag = "route";
                myMap.Children.Add(line);
                mapLayerWarnings.Tag = "route";
                myMap.Children.Add(mapLayerWarnings);
                mapLayerHints.Tag = "route";
                myMap.Children.Add(mapLayerHints);
                mapLayer.Tag = "route";
                myMap.Children.Add(mapLayer);
                loadGeneralDatesRoute(resultRoute);
                viewDetailsRoute(true);
            }
            }
            catch (Exception)
            {
                MessageBox.Show("No se ha podido calcular la ruta :(", "Error");
                bussyMessage(TimeSpan.FromSeconds(2), "Error en la ruta");
            }
        }
        private void checkRoute()
        {
            if(txtGeoAddressStart.Text!="" && txtGeoAddressFinish.Text!="")
            {
                var startSplit = txtGeoAddressStart.Text.Split(',');
                var finishSplit = txtGeoAddressFinish.Text.Split(',');
                Location start = new Location(Convert.ToDouble(startSplit[0]), Convert.ToDouble(startSplit[1]));
                Location finish = new Location(Convert.ToDouble(finishSplit[0]), Convert.ToDouble(finishSplit[1]));
                getRoute(start, finish);
            }
        }
        private void geocodingRoute(string address,bool start)
        {
            startProgress("buscando coordenadas");
            GoogleMaps.GoogleGeocoding.Geocoding geocoding = new GoogleMaps.GoogleGeocoding.Geocoding();
            geocoding.FinishedReading += (e1, s1) =>
            {
                stopProgress();
                if (geocoding != null)
                {
                    if(start)
                    {
                        txtGeoAddressStart.Text = geocoding.resultGeocoding.Latitude + "," + geocoding.resultGeocoding.Longitude;
                    }
                    else
                    {
                        txtGeoAddressFinish.Text = geocoding.resultGeocoding.Latitude + "," + geocoding.resultGeocoding.Longitude;
                    }
                    zoom(new Location(Convert.ToDouble(geocoding.resultGeocoding.Latitude),Convert.ToDouble(geocoding.resultGeocoding.Longitude)),15);
                }
            };
            geocoding.getGeocoding(address);
        }
        private void altitudeRoute(List<BingMaps.BingRoute.Direction> routeList)
        {
            startProgress("buscando altitud");
            List<Location> coordinates = new List<Location>();
            foreach (var item in routeList)
            {
                coordinates.Add(item.Location);
            }
            GoogleMaps.GoogleElevation.Elevation elevation = new GoogleMaps.GoogleElevation.Elevation();
            elevation.FinishedReading += (e2, s2) =>
            {

                stopProgress();
                List<KeyValuePair<int, double>> listChar = new List<KeyValuePair<int, double>>();
                int i = 0;
                foreach (var item in elevation.resultElevation)
                {
                    listChar.Add(new KeyValuePair<int, double>(i, Convert.ToDouble(item.Elevation)));
                    i++;
                }
                chartAltitude.ItemsSource = listChar;
                gridAltitudeRoute.Visibility = Visibility.Visible;
            };
            elevation.getElevation(coordinates);
        }
        void imgH_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopupHint.Visibility = Visibility.Collapsed;
        }
        void imgH_MouseEnter(object sender, MouseEventArgs e)
        {
            BingMaps.BingRoute.Direction hints = (BingMaps.BingRoute.Direction)((Image)sender).DataContext;
            ContentPopupRouteHint.Text = "";
            List<BingMaps.BingRoute.ItineraryHint> hint = hints.Hints;
            foreach (var item in hint)
            {
                ContentPopupRouteHint.Text += "-" + item.Description + "\n";
            }
            ContentPopupRouteHint.Text = ContentPopupRouteHint.Text.Substring(0, ContentPopupRouteHint.Text.Length - 1);
            Point relativePoint = ((Image)sender).TransformToAncestor(Application.Current.MainWindow)
                          .Transform(new Point(0, 0));

            Point mousePosition = e.GetPosition(this);
            mousePosition.X -= relativePoint.X;
            mousePosition.Y -= relativePoint.Y;
            Location location = new Location(hints.Location.Latitude, hints.Location.Longitude);
            MapLayer.SetPosition(ContentPopupHint, location);
            ContentPopupHint.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
            {
                MapLayer.SetPositionOffset(ContentPopupHint, new Point(20, -ContentPopupHint.ActualHeight));
                return null;
            }), null);
            Canvas.SetZIndex(ContentPopupHint, 100);
            ContentPopupHint.Visibility = Visibility.Visible;
        }
        void brd_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopupRoute.Visibility = Visibility.Collapsed;
        }
        void brd_MouseEnter(object sender, MouseEventArgs e)
        {
            BingMaps.BingRoute.Direction direction = (BingMaps.BingRoute.Direction)(((Border)sender).DataContext);
            ContentPopupRoute.DataContext = ((Border)sender).DataContext;
            ContentPopupRouteText.Text = direction.Description;
            Point point = e.GetPosition(myMap);
            Location location = new Location(direction.Location.Latitude, direction.Location.Longitude);
            MapLayer.SetPosition(ContentPopupRoute, location);
            //MapLayer.SetPositionOffset(ContentPopupRoute, new Point(3, 3));
            ContentPopupHint.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
            {
                MapLayer.SetPositionOffset(ContentPopupRoute, new Point(3, -ContentPopupRoute.ActualHeight));
                return null;
            }), null);
            Canvas.SetZIndex(ContentPopupLayerRoute, 100);
            ContentPopupRoute.Visibility = Visibility.Visible;
        }
        void img_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopupWarningHint.Visibility = Visibility.Collapsed;
        }
        void img_MouseEnter(object sender, MouseEventArgs e)
        {
            BingMaps.BingRoute.Direction warnings = (BingMaps.BingRoute.Direction)((Image)sender).DataContext;
            ContentPopupRouteWarningHint.Text = "";
            List<BingMaps.BingRoute.ItineraryWarning> warning=warnings.Warnings;
            foreach (var item in warning)
            {
                ContentPopupRouteWarningHint.Text += "-" + item.Description + "\n";
            }
            ContentPopupRouteWarningHint.Text = ContentPopupRouteWarningHint.Text.Substring(0, ContentPopupRouteWarningHint.Text.Length - 1);
            Point relativePoint = ((Image)sender).TransformToAncestor(Application.Current.MainWindow)
                          .Transform(new Point(0, 0));

            Point mousePosition = e.GetPosition(this);
            mousePosition.X -= relativePoint.X;
            mousePosition.Y -= relativePoint.Y;

            Location location = new Location(warnings.Location.Latitude, warnings.Location.Longitude);
            MapLayer.SetPosition(ContentPopupWarningHint, location);
            ContentPopupWarningHint.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
            {
                MapLayer.SetPositionOffset(ContentPopupWarningHint, new Point(20, -ContentPopupWarningHint.ActualHeight));
                return null;
            }), null);
            Canvas.SetZIndex(ContentPopupWarningHint, 100);
            ContentPopupWarningHint.Visibility = Visibility.Visible;
        }


        private void loadGeneralDatesRoute(BingServices.RouteResponse resultRoute)
        {
            imgBing.Source = createIcon(resultRoute.BrandLogoUri);
            ToolTip tool = new ToolTip(); tool.Content = resultRoute.ResponseSummary.Copyright;
            imgBing.ToolTip = tool;
            txtDistance.Text = resultRoute.Result.Summary.Distance.ToString();
            txtTime.Text = TimeSpan.FromSeconds(Convert.ToDouble(resultRoute.Result.Summary.TimeInSeconds)).ToString();
        }
        private void positionMouseRoute(MapPolyline control, MouseEventArgs e)
        {
            Point relativePoint = control.TransformToAncestor(Application.Current.MainWindow)
                          .Transform(new Point(0, 0));

            Point mousePosition = e.GetPosition(this);
            mousePosition.X -= relativePoint.X;
            mousePosition.Y -= relativePoint.Y;
            Location location = myMap.ViewportPointToLocation(mousePosition);
            ContentPopupRouteLine.Text = location.Latitude.ToString("0.0000") + "," + location.Longitude.ToString("0.0000");
            Point point = e.GetPosition(myMap);
            MapLayer.SetPosition(ContentPopupLine, location);
            MapLayer.SetPositionOffset(ContentPopupLine, new Point(5, 5));
            ContentPopupLine.Visibility = Visibility.Visible;
        }
        void line_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopupLine.Visibility = Visibility.Collapsed;
            ContentPopupLine.Opacity = 0.8;
        }
        void line_MouseMove(object sender, MouseEventArgs e)
        {
            positionMouseRoute((MapPolyline)sender, e);
        }
        void line_MouseEnter(object sender, MouseEventArgs e)
        {
            positionMouseRoute((MapPolyline)sender, e);
        }
        private void listboxRoute_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxRoute.SelectedIndex != -1)
            {
                List<BingMaps.BingRoute.Direction> directions = (List<BingMaps.BingRoute.Direction>)listboxRoute.DataContext;
                BingMaps.BingRoute.Direction direction = directions[listboxRoute.SelectedIndex];
                zoom(direction.Location, 15);
            }
        }
        private void ImageHint_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            if (listboxRoute.SelectedIndex != -1)
            {
                List<BingMaps.BingRoute.Direction> directions = (List<BingMaps.BingRoute.Direction>)listboxRoute.DataContext;
                BingMaps.BingRoute.Direction direction = directions[listboxRoute.SelectedIndex];
                if(direction.Warnings.Count>0)
                {
                    listboxWarnHint.ItemsSource = direction.Warnings;
                    gridWarniHints.Visibility = Visibility.Visible;
                }
            }
        }
        private void Image_MouseUp_3(object sender, MouseButtonEventArgs e)
        {
            if (listboxRoute.SelectedIndex != -1)
            {
                List<BingMaps.BingRoute.Direction> directions = (List<BingMaps.BingRoute.Direction>)listboxRoute.DataContext;
                BingMaps.BingRoute.Direction direction = directions[listboxRoute.SelectedIndex];
                if(direction.Hints.Count>0)
                {
                    listboxWarnHint.ItemsSource = direction.Hints;
                    gridWarniHints.Visibility=Visibility.Visible;
                }
            }
        }
        private void TextBlock_MouseUp_3(object sender, MouseButtonEventArgs e)
        {
            var routeList = (List<BingMaps.BingRoute.Direction>)listboxRoute.DataContext;
            if (routeList != null)
            {
                viewDetailsRoute(false);
                viewAltitudeRoute(true);
                altitudeRoute(routeList);
            }
        }
        private void TextBlock_MouseUp_StaticRoute(object sender, MouseButtonEventArgs e)
        {
            if(resultRoute!=null)
            {
                List<Location> routePoints = new List<Location>();

                foreach (var item in resultRoute.Result.RoutePath.Points)
                {
                    routePoints.Add(new Location(item.Latitude,item.Longitude));
                }
                Location start = new Location(resultRoute.Result.RoutePath.Points[0].Latitude, resultRoute.Result.RoutePath.Points[0].Longitude);
                Location finish = new Location(resultRoute.Result.RoutePath.Points[resultRoute.Result.RoutePath.Points.Length-1].Latitude, resultRoute.Result.RoutePath.Points[resultRoute.Result.RoutePath.Points.Length-1].Longitude);
                showStaticMapRoute(GoogleMaps.GoogleRoute.Route.encodePolyline(routePoints), start,finish, new Size(540, 390));
            }
        }
        private void Image_MouseUp_RouteStreet(object sender, MouseButtonEventArgs e)
        {
            if (listboxRoute.SelectedIndex != -1)
            {
                List<BingMaps.BingRoute.Direction> directions = (List<BingMaps.BingRoute.Direction>)listboxRoute.DataContext;
                BingMaps.BingRoute.Direction direction = directions[listboxRoute.SelectedIndex];
                string address = direction.Latitude + "," + direction.Longitude;
                this.txtStreetView.Text = address;
                viewAStreetView(true);
                checkStreetView();
            }
        }
        private void Image_MouseUp_RouteMap(object sender, MouseButtonEventArgs e)
        {
            if (listboxRoute.SelectedIndex != -1)
            {
                List<BingMaps.BingRoute.Direction> directions = (List<BingMaps.BingRoute.Direction>)listboxRoute.DataContext;
                BingMaps.BingRoute.Direction direction = directions[listboxRoute.SelectedIndex];
                List<BingServices.Location> listDirections = new List<BingServices.Location>(resultRoute.Result.RoutePath.Points);
                int index = getIndexLocation(listDirections, direction.Location);
                int index2 = (listboxRoute.SelectedIndex + 1 <= directions.Count - 1) ? getIndexLocation(listDirections, directions[listboxRoute.SelectedIndex + 1].Location) : listDirections.Count - 1;
                int indexPoint1 = (listboxRoute.SelectedIndex + 1 <= directions.Count - 1) ? listboxRoute.SelectedIndex + 1 : directions.Count - 1;
                var locationsAux = listDirections.GetRange(index, index2 - index + 1);
                List<Location> locations = new List<Location>();
                foreach (var item in locationsAux)
                {
                    locations.Add(new Location(item.Latitude, item.Longitude));
                }

                showStaticMapRoute(GoogleMaps.GoogleRoute.Route.encodePolyline(locations), direction.Location, directions[indexPoint1].Location, new Size(540, 390));
            }
        }
        private int getIndexLocation(List<BingServices.Location> listDirections, Location location)
        {
            return listDirections.FindIndex(loc => loc.Latitude.ToString("0.0000") == location.Latitude.ToString("0.0000") && loc.Longitude.ToString("0.0000") == location.Longitude.ToString("0.0000"));
        }
        #endregion
        #region "Static Map Advanced"
        private void addVisibilityZone(string zone)
        {
            if (zone != "")
            {
                List<string> listVisibility = (List<string>)listboxVisibilityZones.DataContext;
                if (listVisibility == null) { listVisibility = new List<string>(); }
                listVisibility.Add(zone);
                listboxVisibilityZones.DataContext = null; listboxVisibilityZones.ItemsSource = null;
                listboxVisibilityZones.DataContext = listVisibility; listboxVisibilityZones.ItemsSource = listVisibility;
            }
        }
        private void removeVisibilityZone(int zone)
        {
            if (zone >= 0)
            {
                List<string> listVisibility = (List<string>)listboxVisibilityZones.DataContext;
                if (listVisibility == null) { listVisibility = new List<string>(); }
                listVisibility.RemoveAt(zone);
                listboxVisibilityZones.DataContext = null; listboxVisibilityZones.ItemsSource = null;
                listboxVisibilityZones.DataContext = listVisibility; listboxVisibilityZones.ItemsSource = listVisibility;
            }
        }
        private void removeAllVisibility()
        {
            listboxVisibilityZones.DataContext = null; listboxVisibilityZones.ItemsSource = null;
        }

        private void addMarker()
        {
            if (txtGeoStaticMarker.Text != "")
            {
                GoogleMaps.GoogleStaticMaps.Marker marker = new GoogleMaps.GoogleStaticMaps.Marker();
                marker.Center = txtGeoStaticMarker.Text;
                marker.Color = (GoogleMaps.GoogleStaticMaps.Marker.ColorMarker)Enum.Parse(typeof(GoogleMaps.GoogleStaticMaps.Marker.ColorMarker), comboMarkerColor.SelectedItem.ToString(), true);
                marker.Size = (GoogleMaps.GoogleStaticMaps.Marker.SizeMarker)Enum.Parse(typeof(GoogleMaps.GoogleStaticMaps.Marker.SizeMarker), comboMarkerSize.SelectedItem.ToString(), true);
                marker.Label = txtMarkerLabel.Text;

                List<GoogleMaps.GoogleStaticMaps.Marker> listMarkers = (List<GoogleMaps.GoogleStaticMaps.Marker>)listboxMarkers.DataContext;
                if (listMarkers == null) { listMarkers = new List<GoogleMaps.GoogleStaticMaps.Marker>(); }
                listMarkers.Add(marker);
                listboxMarkers.DataContext = null; listboxMarkers.ItemsSource = null;
                listboxMarkers.DataContext = listMarkers; listboxMarkers.ItemsSource = listMarkers;
            }
        }
        private void removeMarker(int marker)
        {
            if (marker >= 0)
            {
                List<GoogleMaps.GoogleStaticMaps.Marker> listMarkers = (List<GoogleMaps.GoogleStaticMaps.Marker>)listboxMarkers.DataContext;
                if (listMarkers == null) { listMarkers = new List<GoogleMaps.GoogleStaticMaps.Marker>(); }
                listMarkers.RemoveAt(marker);
                listboxMarkers.DataContext = null; listboxMarkers.ItemsSource = null;
                listboxMarkers.DataContext = listMarkers; listboxMarkers.ItemsSource = listMarkers;
            }
        }
        private void removeAllMarker()
        {
            listboxMarkers.DataContext = null; listboxMarkers.ItemsSource = null;
        }

        private void addNodeRoute(string nodeRoute)
        {
            if (nodeRoute != "")
            {
                List<string> listNodes = (List<string>)listboxNodesRoute.DataContext;
                if (listNodes == null) { listNodes = new List<string>(); }
                listNodes.Add(nodeRoute);
                listboxNodesRoute.DataContext = null; listboxNodesRoute.ItemsSource = null;
                listboxNodesRoute.DataContext = listNodes; listboxNodesRoute.ItemsSource = listNodes;
            }
        }
        private void removeNodeRoute(int node)
        {
            if (node >= 0)
            {
                List<string> listNodes = (List<string>)listboxNodesRoute.DataContext;
                if (listNodes == null) { listNodes = new List<string>(); }
                listNodes.RemoveAt(node);
                listboxNodesRoute.DataContext = null; listboxNodesRoute.ItemsSource = null;
                listboxNodesRoute.DataContext = listNodes; listboxNodesRoute.ItemsSource = listNodes;
            }
        }
        private void removeAllNodeRoute()
        {
            listboxNodesRoute.DataContext = null; listboxNodesRoute.ItemsSource = null;
        }

        private void addFeature(string feature)
        {
            if (feature != "")
            {
                List<string> listFeatures = (List<string>)listboxFeaturesSelected.DataContext;
                if (listFeatures == null) { listFeatures = new List<string>(); }
                listFeatures.Add(feature);
                listboxFeaturesSelected.DataContext = null; listboxFeaturesSelected.ItemsSource = null;
                listboxFeaturesSelected.DataContext = listFeatures; listboxFeaturesSelected.ItemsSource = listFeatures;
            }
        }
        private void removeFeature(int feature)
        {
            if (feature >= 0)
            {
                List<string> listFeatures = (List<string>)listboxFeaturesSelected.DataContext;
                if (listFeatures == null) { listFeatures = new List<string>(); }
                listFeatures.RemoveAt(feature);
                listboxFeaturesSelected.DataContext = null; listboxFeaturesSelected.ItemsSource = null;
                listboxFeaturesSelected.DataContext = listFeatures; listboxFeaturesSelected.ItemsSource = listFeatures;
            }
        }
        private void removeAllFeature()
        {
            listboxFeaturesSelected.DataContext = null; listboxFeaturesSelected.ItemsSource = null;
        }
        private void addStyle()
        {
            if(listboxFeaturesSelected.Items.Count>0)
            {
                GoogleMaps.GoogleStaticMaps.Style style;
                GoogleMaps.GoogleStaticMaps.Style.RulesStyle ruleStyle;
                List<GoogleMaps.GoogleStaticMaps.Style> listStyles = (List<GoogleMaps.GoogleStaticMaps.Style>)listboxStyles.DataContext;
                if (listStyles == null) { listStyles = new List<GoogleMaps.GoogleStaticMaps.Style>(); }

                foreach (var item in listboxFeaturesSelected.Items)
                {
                    ruleStyle = new GoogleMaps.GoogleStaticMaps.Style.RulesStyle();
                    double gamma;
                    double.TryParse(txtsliderGamma.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out gamma);
                    ruleStyle.Gamma = gamma;
                    ruleStyle.Lightness = (int)sliderLightness.Value;
                    ruleStyle.Saturation = (int)sliderSaturation.Value;
                    ruleStyle.Hue = (GoogleMaps.GoogleStaticMaps.Style.RulesStyle.ColorRule)Enum.Parse(typeof(GoogleMaps.GoogleStaticMaps.Style.RulesStyle.ColorRule), comboColorStyle.SelectedItem.ToString(), true);
                    ruleStyle.InverseLightness = (bool)checkboxInverter.IsChecked;
                    ruleStyle.Visibility = (GoogleMaps.GoogleStaticMaps.Style.RulesStyle.VisibilityRule)Enum.Parse(typeof(GoogleMaps.GoogleStaticMaps.Style.RulesStyle.VisibilityRule), comboVisibility.SelectedItem.ToString(), true);

                    style = new GoogleMaps.GoogleStaticMaps.Style();
                    style.Feature = (GoogleMaps.GoogleStaticMaps.Style.FeatureStyle)Enum.Parse(typeof(GoogleMaps.GoogleStaticMaps.Style.FeatureStyle), item.ToString(), true);
                    style.Element = (GoogleMaps.GoogleStaticMaps.Style.ElementStyle)Enum.Parse(typeof(GoogleMaps.GoogleStaticMaps.Style.ElementStyle), comboElement.SelectedItem.ToString(), true);
                    style.Rule = ruleStyle;

                    listStyles.Add(style);
                }
                listboxStyles.DataContext = null; listboxStyles.ItemsSource = null;
                listboxStyles.DataContext = listStyles; listboxStyles.ItemsSource = listStyles;
            }
        }
        private void removeStyle(int style)
        {
            if (style >= 0)
            {
                List<GoogleMaps.GoogleStaticMaps.Style> listStyles = (List<GoogleMaps.GoogleStaticMaps.Style>)listboxStyles.DataContext;
                if (listStyles == null) { listStyles = new List<GoogleMaps.GoogleStaticMaps.Style>(); }
                listStyles.RemoveAt(style);
                listboxStyles.DataContext = null; listboxStyles.ItemsSource = null;
                listboxStyles.DataContext = listStyles; listboxStyles.ItemsSource = listStyles;
            }
        }
        private void removeAllStyle()
        {
            listboxStyles.ItemsSource = null; listboxStyles.DataContext = null;
        }
      
        private void createStaticMap()
        {
            string center; int zoom;
            List<Maps.NET.GoogleMaps.GoogleStaticMaps.Marker> markers;
            Maps.NET.GoogleMaps.GoogleStaticMaps.Route routes;
            List<string> visibilityZones;
            List<GoogleMaps.GoogleStaticMaps.Style> styles;

            center = txtGeoStaticMapAdvanced.Text;
            zoom = (int)sliderZoomStaticMapBasicAdvanced.Value;
            visibilityZones = (List<string>)listboxVisibilityZones.DataContext;
            markers = (List<GoogleMaps.GoogleStaticMaps.Marker>)listboxMarkers.DataContext;
            routes = new GoogleMaps.GoogleStaticMaps.Route();
            routes.Center = (List<string>)listboxNodesRoute.DataContext;
            routes.Weight = (int)sliderWeightRoute.Value;
            routes.Color = (GoogleMaps.GoogleStaticMaps.Route.ColorRoute)Enum.Parse(typeof(GoogleMaps.GoogleStaticMaps.Route.ColorRoute), comboColorRoute.SelectedItem.ToString(), true);
            routes.FillColor = (GoogleMaps.GoogleStaticMaps.Route.ColorRoute)Enum.Parse(typeof(GoogleMaps.GoogleStaticMaps.Route.ColorRoute), comboFillColorRoute.SelectedItem.ToString(), true);
            styles = (List<GoogleMaps.GoogleStaticMaps.Style>)listboxStyles.DataContext;

            GoogleMaps.GoogleStaticMaps.StaticMap map = new GoogleMaps.GoogleStaticMaps.StaticMap();
            string urlMap = map.getMap(center, zoom, markers, routes, visibilityZones, styles);
            loadStaticMapAdvanced(urlMap);
        }
        private void loadStaticMapAdvanced(string url)
        {
            startProgress("cargando mapa");
            if (gridBrowserStaticMapAdvanced.Visibility == Visibility.Collapsed) { viewBroserStaticAdvanced(true); }
            txtUrlStaticMapAdvanced.Text = url;
            browserStaticMapAdvanced.LoadCompleted += (e1, s1) =>
            {
                bussyMessage(TimeSpan.FromSeconds(2), "mapa cargado");
            };
            browserStaticMapAdvanced.Source = new Uri(url, UriKind.Absolute);
        }
        #endregion

        private void showStaticMapRoute(string polyline,Location startPoint,Location endPoint,Size sizeMap)
        {
            GoogleMaps.GoogleStaticMaps.StaticMap map = new GoogleMaps.GoogleStaticMaps.StaticMap();
            webBrosew.Source = new Uri(map.getRouteMap(polyline, startPoint, endPoint, sizeMap), UriKind.Absolute);
            viewStaticMapRoute(true);
        }

        private void checkStaticBasicMap()
        {
            if (txtGeoStaticMap.Text != "")
            {
                GoogleMaps.GoogleStaticMaps.StaticMap map = new GoogleMaps.GoogleStaticMaps.StaticMap();
                startProgress("cargando mapa");
                if (gridStaticMapBasic.Width > 349) { viewGoWidthStaticMap(false); } else { viewGoWidthStaticMap(true); }
                browserStaticMap.LoadCompleted += (e1, s1) =>
                {
                    bussyMessage(TimeSpan.FromSeconds(2), "mapa cargado");
                };
                string url=map.getBasicMap(txtGeoStaticMap.Text, (int)sliderZoomStaticMapBasic.Value, (bool)checkBoxMarker.IsChecked);
                browserStaticMap.Source = new Uri(url, UriKind.Absolute);
                txtUrlStaticMap.Text = url;
            }
        }
       
        private void ContentPopupLayerRoute_MouseEnter(object sender, MouseEventArgs e)
        {
            ContentPopupRoute.Visibility = Visibility.Visible;
        }
        private void ContentPopupLayerRoute_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopupRoute.Visibility = Visibility.Collapsed;
        }
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var value=((Border)((StackPanel)((StackPanel)(((TextBlock)sender).Parent)).Parent).Parent).DataContext;
            if (value.GetType() == typeof(BingMaps.BingRoute.Direction))
            {
                loadPlaces("gas_station", ((BingMaps.BingRoute.Direction)value).Location);
            }
            else if(value.GetType() == typeof(GoogleMaps.GoogleRoute.Direction))
            {
           
                loadPlaces("gas_station", ((GoogleMaps.GoogleRoute.Direction)value).StartGeocoordinate);

            }
          
        }


        void pushAdd_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopupP.Visibility = Visibility.Collapsed;
            ContentPopupP.Opacity = 0.8;
        }
        void ContentPopupPlace_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopupP.Visibility = Visibility.Collapsed;
            ContentPopupP.Opacity = 0.8;
        }
        void ContentPopupPlace_MouseEnter(object sender, MouseEventArgs e)
         {
             ContentPopupP.Opacity =1;
             ContentPopupP.Visibility = Visibility.Visible;
             Canvas.SetZIndex(ContentPopupP, 10);
         }
        void push_MouseEnter(object sender, MouseEventArgs e)
        {
            Pushpin pushpin = (Pushpin)sender;
            Canvas.SetZIndex(ContentPopupPlace, 5000);
            GoogleMaps.GooglePlaces.Place place = (GoogleMaps.GooglePlaces.Place)pushpin.DataContext;
            ContentPopupPlace.DataContext = place;
            Point point = e.GetPosition(myMap);
            Location location = new Location(pushpin.Location.Latitude, pushpin.Location.Longitude);
            MapLayer.SetPosition(ContentPopupP, location);
            MapLayer.SetPositionOffset(ContentPopupP, new Point(25, -50));
            ContentName.Text = place.Name;
            ContentPopupP.Visibility = Visibility.Visible;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            detailsPlace(((GoogleMaps.GooglePlaces.Place)ContentPopupPlace.DataContext));
        }
        private void BorderRoute_MouseDown(object sender, MouseButtonEventArgs e)
        {
            getRoute(pushME.Location, ((GoogleMaps.GooglePlaces.Place)ContentPopupPlace.DataContext).GeoCoordinates);
        }
       
        private Location stringToLocation(string latitude, string longitude) 
        {
            try
            {
                double lat, lng;
                double.TryParse(latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out lat);
                double.TryParse(longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out lng);
                return new Location(lat, lng);
            }
            catch (Exception)
            {
                return null;
            }
           
        }
        private Places.TypeSearch checkTypeSearch()
        {
            Places.TypeSearch type = Places.TypeSearch.Keyword;
            if (radioName.IsChecked == true) { type = Places.TypeSearch.Name; }
            return type;
        }

        private void changeZoom(double increment)
        {
            double zoom = myMap.ZoomLevel+increment;
            if (zoom <= 1) { zoom = 1; }
            if (zoom >= 20) { zoom = 20; }

            myMap.SetView(myMap.Center, zoom);
        }
        private void zoom(Location location, double zoom)
        {
             Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                myMap.SetView(location, zoom);
            }));
        }
        private void zoomME(Location location, string tooltip, double zoom)
        {
            myMap.SetView(location, zoom);
            pushME.Location = location;
            ToolTip tool = new System.Windows.Controls.ToolTip();
            tool.Content = tooltip;
            pushME.ToolTip = tool;
            tool.Visibility = Visibility.Collapsed;
            pushME.MouseEnter += pushME_MouseEnter;
            pushME.MouseLeave+=pushME_MouseLeave;
            Canvas.SetZIndex(pushME, 500);
            bussyMessage(TimeSpan.FromSeconds(2), tooltip);
        }
        private void openWeb(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
        void pushME_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopup.Visibility = Visibility.Collapsed;
        }
        void pushME_MouseEnter(object sender, MouseEventArgs e)
        {
            Pushpin pushpin = sender as Pushpin;
            Point point = e.GetPosition(myMap);
            string resultId = pushpin.Tag as string;
            Location location = new Location(pushpin.Location.Latitude, pushpin.Location.Longitude);
            MapLayer.SetPosition(ContentPopup, location);
            MapLayer.SetPositionOffset(ContentPopup, new Point(25, -50));
            ContentPopupText.Text = ((ToolTip)pushpin.ToolTip).Content.ToString();
            ContentPopupDescription.Text = "Coordenadas: " + pushpin.Location.Latitude.ToString() + " , " + pushpin.Location.Longitude.ToString();
            ContentPopup.Visibility = Visibility.Visible;
        
        }
        private void ContentPopupLayer_MouseEnter(object sender, MouseEventArgs e)
        {
            ContentPopup.Opacity = 1;
            ContentPopup.Visibility = Visibility.Visible;
            Canvas.SetZIndex(ContentPopup, 10);
        }

        private void ContentPopupLayer_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopup.Visibility = Visibility.Collapsed;
            ContentPopup.Opacity = 0.8;
        }
        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            viewTypesPlacesView(true,"gridTypesPlaces");
        }
        private void copyPosition()
        {
            string position = ("Coordenadas (latitud, longitud, altitud): " + pushME.Location.Latitude + "\t" + pushME.Location.Longitude + "\t" + pushME.Location.Altitude +
               "\r\n" + "Dirección postal: " + ((ToolTip)pushME.ToolTip).Content);
            Clipboard.SetText(position);
        }
        private void copyAllRequest()
        {
            if (listboxRequest.DataContext != null)
            {
                string text = "";
                foreach (GoogleMaps.Request item in (List<GoogleMaps.Request>)listboxRequest.DataContext)
                {
                    text += item.Time + "\t" + item.Service + "\t" + item.Url + "\r\n";
                }
                Clipboard.SetText(text);
            }
        }
        private void copySelectedRequest(int index)
        {
            if (index >= 0)
            {
                GoogleMaps.Request request = (GoogleMaps.Request)listboxRequest.Items[index];
                Clipboard.SetText(request.Url);
            }

        }
        private void loadRequest()
        {
            listboxRequest.ItemsSource = null; listboxRequest.DataContext =null;
            listboxRequest.ItemsSource = GoogleMaps.GoogleStaticInfo.Request; listboxRequest.DataContext = GoogleMaps.GoogleStaticInfo.Request;
        }
        private void saveApiKey(string key)
        {
            GoogleMaps.GoogleStaticInfo.Key = key;
            (this.FindResource("storyMessageApiKeyON") as Storyboard).Begin();
            Properties.Settings.Default.ApiKey = key; Properties.Settings.Default.Save();
        }
        private void saveBingKey(string key)
        {
            myMap.CredentialsProvider = new ApplicationIdCredentialsProvider(key);
            (this.FindResource("storyMessageApiKeyON") as Storyboard).Begin();
            Properties.Settings.Default.BingKey = key; Properties.Settings.Default.Save();
        }


        #region "Show/Hide grids"
        private void viewAll(bool view)
        {
            if (view) { (this.FindResource("viewON") as Storyboard).Begin(); flagSeeAll = true; } else { (this.FindResource("viewOFF") as Storyboard).Begin(); flagSeeAll = false; }
        }
        private void hideAllExtraGrids()
        {
            if (gridGeocoding.Visibility == Visibility.Visible) {(this.FindResource("storyGeocodingOFF") as Storyboard).Begin(); }
            if (gridReverseGeocoding.Visibility == Visibility.Visible) { (this.FindResource("storyReverseGeocodingOFF") as Storyboard).Begin();}
            if (gridPostalCode.Visibility == Visibility.Visible) { (this.FindResource("storyPostalCodeOFF") as Storyboard).Begin(); }
            if (gridAltitude.Visibility == Visibility.Visible) { (this.FindResource("storyAltitudeOFF") as Storyboard).Begin(); }
            if (gridAltitudeCoo.Visibility == Visibility.Visible) { (this.FindResource("storyAltitudeCoorOFF") as Storyboard).Begin(); }
            if (gridStreetView.Visibility == Visibility.Visible) { (this.FindResource("storyStreetOFF") as Storyboard).Begin(); }
            if (gridTypesPlaces.Visibility == Visibility.Visible) { (this.FindResource("storytypesPlacesOFF") as Storyboard).Begin(); }
            if (gridPlaces.Visibility == Visibility.Visible) { (this.FindResource("storyPlacesOFF") as Storyboard).Begin(); }
            if (gridDetailsPlace.Visibility == Visibility.Visible) { (this.FindResource("storyDetailsPlaceOFF") as Storyboard).Begin(); }
            if (gridMaxImagePlace.Visibility == Visibility.Visible) { gridMaxImagePlace.Visibility = Visibility.Collapsed; }
            if (gridPlaceSearch.Visibility == Visibility.Visible) { (this.FindResource("storySearchPlaceOFF") as Storyboard).Begin(); }
            if (gridSettingsPlaces.Visibility == Visibility.Visible) { (this.FindResource("storySettingsPlaceOFF") as Storyboard).Begin(); }
            if (gridWarniHints.Visibility == Visibility.Visible) { gridWarniHints.Visibility=Visibility.Collapsed; }
            if (gridRoute.Visibility == Visibility.Visible && gridRoute.Height>259) { (this.FindResource("storyRouteHeightOFF") as Storyboard).Begin(); }
            if (gridRouteSelect.Visibility == Visibility.Visible) { (this.FindResource("storyRouteSelectOFF") as Storyboard).Begin(); }
            if (gridRouteGoogle.Visibility == Visibility.Visible && gridRouteGoogle.Height > 259) { (this.FindResource("storyRouteGoogleHeightOFF") as Storyboard).Begin(); }
            if (gridRouteGoogleSelect.Visibility == Visibility.Visible) { (this.FindResource("storyRouteGoogleSelectOFF") as Storyboard).Begin(); }
            if (gridAltitudeRoute.Visibility == Visibility.Visible) { (this.FindResource("storyAltitudeRouteOFF") as Storyboard).Begin(); }
            if (gridStaticMapRoute.Visibility == Visibility.Visible) { (this.FindResource("storyStaticMapRouteOFF") as Storyboard).Begin(); }
            if (gridStaticMap.Visibility == Visibility.Visible) { (this.FindResource("storyStaticMapOFF") as Storyboard).Begin(); }
            if (gridStaticMapBasic.Visibility == Visibility.Visible) { (this.FindResource("storyStaticMapBasicOFF") as Storyboard).Begin(); }
            if (gridStaticMapAdvanced.Visibility == Visibility.Visible) { (this.FindResource("storyStaticAvancedOFF") as Storyboard).Begin(); }
            if (gridBrowserStaticMapAdvanced.Visibility == Visibility.Visible) { (this.FindResource("storyStaticBrowserAvancedOFF") as Storyboard).Begin(); }
            if (gridRequest.Visibility == Visibility.Visible) { (this.FindResource("storyRequestOFF") as Storyboard).Begin(); }
            if (gridApiKey.Visibility == Visibility.Visible) { (this.FindResource("storyApiKeyOFF") as Storyboard).Begin(); }
            if (gridAbout.Visibility == Visibility.Visible) { (this.FindResource("storyAboutOFF") as Storyboard).Begin(); }
            if (gridFeedback.Visibility == Visibility.Visible) { (this.FindResource("storyFeedBackOFF") as Storyboard).Begin(); }
        }
        private void viewGeocoding(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyGeocodingON") as Storyboard).Begin(); } 
        }
        private void viewReverseGeocoding(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyReverseGeocodingON") as Storyboard).Begin(); }
        }
        private void viewPostalCode(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyPostalCodeON") as Storyboard).Begin(); } 
        }
        private void viewAltitude(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyAltitudeON") as Storyboard).Begin(); } 
        }
        private void viewAltitudeCoo(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyAltitudeCoorON") as Storyboard).Begin(); } 
        }
        private void viewAStreetView(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyStreetON") as Storyboard).Begin(); } 
        }
        private void viewTypesPlacesView(bool view, string txtReturn)
        {
            hideAllExtraGrids();
            if (view) 
            {
                viewMessageApiKey();
                if (txtReturn == "gridTypesPlaces") { (this.FindResource("storytypesPlacesON") as Storyboard).Begin(); }
                if (txtReturn == "gridPlaceSearch") { (this.FindResource("storySearchPlaceON") as Storyboard).Begin(); }
            } 
        }
        private void viewPlacesView(bool view, string txtReturn)
        {
            hideAllExtraGrids();
            if (view) 
            {
                (this.FindResource("storyPlacesON") as Storyboard).Begin();
                this.txtFlagReturnGridPlaces.Text = txtReturn;
            }
        }
        private void viewDetailsPlacesView(bool view)
        {
            //hideAllExtraGrids();
            if (view) { (this.FindResource("storyDetailsPlaceON") as Storyboard).Begin(); }
        }
        private void viewPlacesSearch(bool view)
        {
            hideAllExtraGrids();
            if (view)
            {
                (this.FindResource("storySearchPlaceON") as Storyboard).Begin(); viewMessageApiKey();
            }
        }
        private void viewSettingsPlace(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storySettingsPlaceON") as Storyboard).Begin(); } 
        }
        private void viewDetailsRoute(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyRouteHeightON") as Storyboard).Begin(); } 
        }
        private void viewRouteSelect(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyRouteSelectON") as Storyboard).Begin(); } 
        }
        private void viewDetailsRouteGoogle(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyRouteGoogleHeightON") as Storyboard).Begin(); }
        }
        private void viewRouteGoogleSelect(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyRouteGoogleSelectON") as Storyboard).Begin(); } 
        }
        private void viewAltitudeRoute(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyAltitudeRouteON") as Storyboard).Begin(); }
        }
        private void viewStaticMapRoute(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyStaticMapRouteON") as Storyboard).Begin(); }
        }
        private void viewGoWidthStaticMap(bool view)
        {
            if (view) { (this.FindResource("storyStaticMapHeightON") as Storyboard).Begin(); } else { (this.FindResource("storyStaticMapHeightOFF") as Storyboard).Begin(); }
        }
        private void viewStaticMap(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyStaticMapBasicON") as Storyboard).Begin(); } else { (this.FindResource("storyStaticMapOFF") as Storyboard).Begin(); }
        }
        private void hideBrowserStaticMap()
        {
            (this.FindResource("storyStaticMapOFF") as Storyboard).Begin();
        }
        private void viewStaticMapAdvanced(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyStaticAvancedON") as Storyboard).Begin(); }
        }
        private void viewBroserStaticAdvanced(bool view)
        {
            if (view) { (this.FindResource("storyStaticBrowserAvancedON") as Storyboard).Begin(); } else { (this.FindResource("storyStaticBrowserAvancedOFF") as Storyboard).Begin(); }
        }
        private void viewRequest(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyRequestON") as Storyboard).Begin(); loadRequest(); }
        }
        private void viewApiKey(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyApiKeyON") as Storyboard).Begin(); txtApiKey.Text = GoogleMaps.GoogleStaticInfo.Key; txtBingKey.Text = ((ApplicationIdCredentialsProvider)myMap.CredentialsProvider).ApplicationId; }
        }
        private void viewMessageApiKey()
        {
            if (GoogleMaps.GoogleStaticInfo.Key == "") { (this.FindResource("storyMessageWarningKeyON") as Storyboard).Begin(); }
        }
        private void viewAbout(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyAboutON") as Storyboard).Begin();}
        }
        private void viewFeedBack(bool view)
        {
            hideAllExtraGrids();
            if (view) { (this.FindResource("storyFeedBackON") as Storyboard).Begin(); }
        }
        #endregion

        #region "Interface changes"
        BitmapImage img;
        private BitmapImage createIcon(Uri uri)
        {
            return img = new BitmapImage(uri);
        }
        private void gridSearch_MouseEnter(object sender, MouseEventArgs e)
        {
            if (flagSeeAll) { 
                gridSearch.BeginAnimation(Grid.OpacityProperty, null);
                gridSearch.Opacity = 0.8;
                gridSearch.Height = 40;
            }
        }

        private void gridSearch_MouseLeave(object sender, MouseEventArgs e)
        {
            if (flagSeeAll)
            {
                gridSearch.BeginAnimation(Grid.OpacityProperty, null);
                gridSearch.Opacity = 0.6;
                gridSearch.Height = 30;
            }
        }
        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Search_Enter.png", UriKind.Relative));
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Search.png", UriKind.Relative));
        }

        private void ImageSave_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Save_Enter.png", UriKind.Relative));
        }

        private void ImageSave_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Save.png", UriKind.Relative));
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if(flagSeeAll)
            { 
            borderZoomMore.BeginAnimation(Border.OpacityProperty,null);
            borderZoomMore.Opacity = 0.9;
            }
        }


        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if(flagSeeAll)
            { 
            borderZoomMore.BeginAnimation(Border.OpacityProperty, null);
            borderZoomMore.Opacity = 0.6;
            }
        }
        private void borderZoomLess_MouseEnter(object sender, MouseEventArgs e)
        {
            if (flagSeeAll)
            {
                borderZoomLess.BeginAnimation(Border.OpacityProperty, null);
                borderZoomLess.Opacity = 0.9;
            }
        }

        private void borderZoomLess_MouseLeave(object sender, MouseEventArgs e)
        {
            if (flagSeeAll)
            {
                borderZoomLess.Opacity = 0.6;
            }
        }

        private void ribbonRadioAerial_Checked(object sender, RoutedEventArgs e)
        {
            if (myMap != null) { myMap.Mode = new AerialMode(false); bussyMessage(TimeSpan.FromSeconds(2), "vista aérea activada"); }
        }

        private void RibbonRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (myMap != null) { myMap.Mode = new AerialMode(true); bussyMessage(TimeSpan.FromSeconds(2), "vista aérea con etiquetas activada"); }
        }
        private void RibbonRadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            if (myMap != null) { myMap.Mode = new RoadMode(); bussyMessage(TimeSpan.FromSeconds(2), "vista carreteras activada"); }
        }
        

        private void RibbonToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (myMap != null) { viewAll(false); }
        }
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            gridGeocoding.BeginAnimation(Border.OpacityProperty, null);
            gridGeocoding.Opacity = 1;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            gridGeocoding.Opacity = 0.8;
        }
        private void Image_MouseEnter_1(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Grid/Close_Enter.png", UriKind.Relative));
        }
        private void GridR_MouseEnter(object sender, MouseEventArgs e)
        {
            gridReverseGeocoding.BeginAnimation(Border.OpacityProperty, null);
            gridReverseGeocoding.Opacity = 1;
        }

        private void GridR_MouseLeave(object sender, MouseEventArgs e)
        {
            gridReverseGeocoding.Opacity = 0.8;
        }
        private void Image_MouseLeave_1(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("Assets/Grid/Close.png", UriKind.Relative));
        }
        private void ImageMaxi_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("Assets/Grid/Maximice_Enter.png", UriKind.Relative));
        }

        private void ImageMaxi_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("Assets/Grid/Maximice.png", UriKind.Relative));
        }
        private void CursorEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void CursorLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
        private void imgGeoClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            hideAllExtraGrids();
        }
        private void imgPostalCodeClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            hideAllExtraGrids();
            refreshEventsMap();
        }
        private void GridPostal_MouseEnter(object sender, MouseEventArgs e)
        {
            gridPostalCode.BeginAnimation(Border.OpacityProperty, null);
            gridPostalCode.Opacity = 1;
        }
        private void GridPostal_MouseLeave(object sender, MouseEventArgs e)
        {
            gridPostalCode.Opacity = 0.8;
        }
        private void GridAltitude_MouseEnter(object sender, MouseEventArgs e)
        {
            gridAltitude.BeginAnimation(Border.OpacityProperty, null);
            gridAltitude.Opacity = 1;
        }

        private void GridAltitude_MouseLeave(object sender, MouseEventArgs e)
        {
            gridAltitude.Opacity = 0.8;
        }
        private void GridAltitudeCoo_MouseEnter(object sender, MouseEventArgs e)
        {
            gridAltitudeCoo.BeginAnimation(Border.OpacityProperty, null);
            gridAltitudeCoo.Opacity = 1;
        }
        private void GridAltitudeCoo_MouseLeave(object sender, MouseEventArgs e)
        {
            gridAltitudeCoo.Opacity = 0.8;

        }
        private void gridStreetView_MouseEnter(object sender, MouseEventArgs e)
        {
            gridStreetView.BeginAnimation(Border.OpacityProperty, null);
            gridStreetView.Opacity = 1;
        }
        private void gridStreetView_MouseLeave(object sender, MouseEventArgs e)
        {
            gridStreetView.Opacity = 0.8;
        }
        private void imgCloseStreet_MouseUp(object sender, MouseButtonEventArgs e)
        {
            hideAllExtraGrids();
        }
        private void imgClosePlaceSearch_MouseUp(object sender, MouseButtonEventArgs e)
        {
            hideAllExtraGrids();
            if (this.gridPlaceSearch.Height >= 370) { (this.FindResource("storySearchPlaceHeightOFF") as Storyboard).Begin(); }

        }
        private void imgCloseStreetMax_MouseUp(object sender, MouseButtonEventArgs e)
        {
            maximizeStreet(false);
        }

        private void gridGeneral_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).BeginAnimation(Border.OpacityProperty, null);
            ((Border)sender).Opacity = 1;
        }

        private void gridGeneral_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Opacity = 0.8;

        }
        private void ImageBack_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Grid/Back.png", UriKind.Relative));
        }
        private void ImageBack_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Grid/Back_Enter.png", UriKind.Relative));
        }
        private void ImageFilter_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Grid/Filter_Enter.png", UriKind.Relative));
        }

        private void ImageFilter_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Grid/Filter.png", UriKind.Relative));
        }
        private void ImageNext_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Grid/Next_Enter.png", UriKind.Relative));
        }
        private void ImageNext_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Grid/Next.png", UriKind.Relative));
        }
        private void Border_MouseEnter_1(object sender, MouseEventArgs e)
        {
            ((Border)sender).BeginAnimation(Border.OpacityProperty, null);
            ((Border)sender).Opacity = 1;
        }

        private void Border_MouseLeave_1(object sender, MouseEventArgs e)
        {
            ((Border)sender).Opacity =0.6;
        }
        private void imgDetailsPlaceClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            (this.FindResource("storyDetailsPlaceOFF") as Storyboard).Begin();
        }
        private void imgCloseImagePlace_MouseUp(object sender, MouseButtonEventArgs e)
        {
            gridMaxImagePlace.Visibility = Visibility.Collapsed;
        }

        private void ImageFilter_MouseUp(object sender, MouseButtonEventArgs e)
        {
            gridSort.Visibility = (gridSort.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        private RadialGradientBrush gradienteBorder()
        {
            RadialGradientBrush threeColors = new RadialGradientBrush();
            threeColors.GradientOrigin = new Point(0.5, 0.5);
            threeColors.Center = new Point(0.5, 0.5);

            GradientStop black = new GradientStop();
            black.Color = Colors.Black;
            black.Offset = 0.0;
            threeColors.GradientStops.Add(black);

            GradientStop gray = new GradientStop();
            gray.Color = Color.FromRgb(30, 30, 30);
            gray.Offset = 0.849;
            threeColors.GradientStops.Add(gray);

            GradientStop white = new GradientStop();
            white.Color = Colors.White;
            white.Offset = 1;
            threeColors.GradientStops.Add(white);
            return threeColors;
        }

        private void imgUpDownRoute_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).BeginAnimation(Image.SourceProperty, null);
            ((Image)sender).Source = (gridRoute.Height > 259) ? createIcon(new Uri("/Assets/Route/Down_Enter.png", UriKind.Relative)) : createIcon(new Uri("/Assets/Route/Up_Enter.png", UriKind.Relative));
        }
        private void imgUpDownRoute_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = (gridRoute.Height > 259) ? createIcon(new Uri("/Assets/Route/Down.png", UriKind.Relative)) : createIcon(new Uri("/Assets/Route/Up.png", UriKind.Relative));
        }

        private void ImagePressMap_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Route/Target_Enter.png", UriKind.Relative));
        }

        private void ImagePressMap_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Route/Target.png", UriKind.Relative));
        }
        private void Image_MouseUpGoogle_2(object sender, MouseButtonEventArgs e)
        {
            if (gridRouteGoogle.Height > 259)
            {
                (this.FindResource("storyRouteGoogleHeightOFF") as Storyboard).Begin();
            }
            else
            {
                (this.FindResource("storyRouteGoogleHeightON") as Storyboard).Begin();
            }
        }
        private void imgUpDownRouteGoogle_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).BeginAnimation(Image.SourceProperty, null);
            ((Image)sender).Source = (gridRouteGoogle.Height > 259) ? createIcon(new Uri("/Assets/Route/Down_Enter.png", UriKind.Relative)) : createIcon(new Uri("/Assets/Route/Up_Enter.png", UriKind.Relative));
        }
        private void imgUpDownRouteGoogle_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = (gridRouteGoogle.Height > 259) ? createIcon(new Uri("/Assets/Route/Down.png", UriKind.Relative)) : createIcon(new Uri("/Assets/Route/Up.png", UriKind.Relative));

        }
        private void imgRouteGoogle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Static.MapObject.removeRoute(myMap, "routeGoogle");
            bussyMessage(TimeSpan.FromSeconds(2), "ruta eliminada");
            if (gridRouteGoogle.Height > 259) { (this.FindResource("storyRouteGoogleCollapseOFF") as Storyboard).Begin(); } else { (this.FindResource("storyRouteGoogleCollapseReduceOFF") as Storyboard).Begin(); }
        }
        private void txtAddressGoogleStart_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                checkRouteGoogle();
            }else
            {
                this.txtGeoAddressGoogleStart.Text = "";
            }
        }
        private void txtAddressGoogleFinish_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                checkRouteGoogle();
            }
            else
            {
                this.txtGeoAddressGoogleFinish.Text = "";
            }
        }

        private void imgUpStaticMapBasic_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).BeginAnimation(Image.SourceProperty, null);
            ((Image)sender).Source = (gridStaticMapBasic.Width > 349) ? createIcon(new Uri("/Assets/Route/Down_Enter.png", UriKind.Relative)) : createIcon(new Uri("/Assets/Route/Up_Enter.png", UriKind.Relative));
        }
        private void imgUpStaticMapBasic_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = (gridStaticMapBasic.Width > 349) ? createIcon(new Uri("/Assets/Route/Down.png", UriKind.Relative)) : createIcon(new Uri("/Assets/Route/Up.png", UriKind.Relative));
        }


        private void Image_MouseEnter_2(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/StaticMap/Advanced/Add_Enter.png", UriKind.Relative));

        }
        private void Image_MouseLeave_2(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/StaticMap/Advanced/Add.png", UriKind.Relative));
        }
        private void Image_MouseEnter_3(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/StaticMap/Advanced/Minus_Enter.png", UriKind.Relative));
        }
        private void Image_MouseLeave_3(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/StaticMap/Advanced/Minus.png", UriKind.Relative));
        }

        private void Image_MouseEnter_CloseAbout(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/CloseBlack_Enter.png", UriKind.Relative));
        }
        private void Image_MouseLeave_CloseAbout(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/CloseBlack.png", UriKind.Relative));
        }

        private void Image_MouseEnter_4(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/StaticMap/Advanced/Delete_Enter.png", UriKind.Relative));
        }
        private void Image_MouseLeave_4(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/StaticMap/Advanced/Delete.png", UriKind.Relative));
        }

        private void Image_MouseEnter_5(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Basic/Copy_Enter.png", UriKind.Relative));
        }
        private void Image_MouseLeave_5(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Basic/Copy1.png", UriKind.Relative));
        }
        private void Image_MouseEnter_6(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Basic/CopyAll_Enter.png", UriKind.Relative));
        }
        private void Image_MouseLeave_6(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Basic/CopyAll.png", UriKind.Relative));
        }

        private void Image_MouseEnter_7(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Grid/Save_Enter.png", UriKind.Relative));
        }
        private void Image_MouseLeave_7(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = createIcon(new Uri("/Assets/Grid/Save.png", UriKind.Relative));
        }

        #endregion

        #region "events map"
        bool mapInverseGeocoding,mapAltitude;
        bool mapSelectStartRoute, mapSelectFinishRoute;
        bool mapSelectStartRouteGoogle, mapSelectFinishRouteGoogle;
        bool mapSelectStaticMap;
        bool mapSelectCenterStatic, mapSelectVisibilityZone,mapSelectMarker,mapSelectNodeRoute;

        private void refreshEventsMap()
        {
            mapInverseGeocoding = false;
            mapAltitude = false;
            mapSelectStartRoute=false;
            mapSelectFinishRoute=false;
            mapSelectStartRouteGoogle=false;
            mapSelectFinishRouteGoogle = false;
            mapSelectStaticMap = false;
            mapSelectCenterStatic = false;
            mapSelectVisibilityZone = false;
            mapSelectMarker = false;
            mapSelectNodeRoute = false;
        }
        private void imgInverseGeocodingCoordinates_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changePositionGridMessage(new Thickness(470, -160, 10, 0),System.Windows.HorizontalAlignment.Center,System.Windows.VerticalAlignment.Center);
            if (mapInverseGeocoding == false) { refreshEventsMap(); mapInverseGeocoding = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin(); 
        }
        private void imgAltitudeCoordinates_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changePositionGridMessage(new Thickness(470, -160, 10, 0), System.Windows.HorizontalAlignment.Center, System.Windows.VerticalAlignment.Center);
            if (mapAltitude == false) { refreshEventsMap(); mapAltitude = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin(); 
        }
        private void imgSelectCoordinatesStart_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mapSelectStartRoute==false) { refreshEventsMap(); mapSelectStartRoute = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin(); 

        }
        private void imgSelectCoordinatesFinish_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mapSelectFinishRoute==false) {  mapSelectFinishRoute = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin(); 

        }
        private void imgSelectCoordinatesGoogleStart_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mapSelectStartRouteGoogle == false) { mapSelectStartRouteGoogle = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin();
        }
        private void imgSelectCoordinatesGoogleFinish_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mapSelectFinishRouteGoogle == false) { mapSelectFinishRouteGoogle = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin(); 
        }
        private void imgSelectStaticMapCoordinates_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mapSelectStaticMap == false) { mapSelectStaticMap = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin(); 
        }
        private void imgSelectStaticMapAdvancedCoordinates_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changePositionGridMessage(new Thickness(560, 30, 10, 0));
            if (mapSelectCenterStatic == false) { mapSelectCenterStatic = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin();
        }
        private void imgSelectVisibilityZones_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changePositionGridMessage(new Thickness(560, 30, 10, 0));
            if (mapSelectVisibilityZone == false) { mapSelectVisibilityZone = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin();
        }
        private void imgSelectStaticMapAdvancedMarkerCoordinates_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changePositionGridMessage(new Thickness(560, 30, 10, 0));
            if (mapSelectMarker == false) { mapSelectMarker = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin();
        }
        private void imgSelectRouteNode_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changePositionGridMessage(new Thickness(560, 30, 10, 0));
            if (mapSelectNodeRoute == false) { mapSelectNodeRoute = true; }
            (this.FindResource("storyMessageLocationON") as Storyboard).Begin();
        }

        private async void changePositionGridMessage(Thickness margin,HorizontalAlignment horizontal=System.Windows.HorizontalAlignment.Left,VerticalAlignment vertical=System.Windows.VerticalAlignment.Top)
        {
            gridMessageLocation.Margin = margin;
            gridMessageLocation.HorizontalAlignment = horizontal; gridMessageLocation.VerticalAlignment = vertical;
            await Task.Delay(2000);
            gridMessageLocation.Margin = new Thickness(380, 30, 10, 0);
            gridMessageLocation.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            gridMessageLocation.VerticalAlignment = System.Windows.VerticalAlignment.Top;
        }
        private void myMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Control control = (Control)sender;
            Point relativePoint = control.TransformToAncestor(Application.Current.MainWindow)
                          .Transform(new Point(0, 0));

            Point mousePosition = e.GetPosition(this);
            mousePosition.X -= relativePoint.X;
            mousePosition.Y -= relativePoint.Y;
            Location locationMap = myMap.ViewportPointToLocation(mousePosition);
            if (mapInverseGeocoding) { txtLatReveGeo.Text = locationMap.Latitude.ToString(); txtLngReveGeo.Text = locationMap.Longitude.ToString(); }
            if (mapAltitude) { txtAltitudeLat.Text = locationMap.Latitude.ToString(); txtAltitudeLng.Text = locationMap.Longitude.ToString(); }
            if (mapSelectStartRoute) { txtGeoAddressStart.Text = locationMap.ToString(); }
            if (mapSelectFinishRoute) { txtGeoAddressFinish.Text = locationMap.ToString(); }
            if (mapSelectStartRouteGoogle) { txtGeoAddressGoogleStart.Text = locationMap.Latitude + "," +locationMap.Longitude; }
            if (mapSelectFinishRouteGoogle) { txtGeoAddressGoogleFinish.Text = locationMap.Latitude + "," + locationMap.Longitude; }
            if (mapSelectCenterStatic) { txtGeoStaticMapAdvanced.Text = locationMap.Latitude + "," + locationMap.Longitude; }
            if (mapSelectVisibilityZone) { txtVisibilityZones.Text = locationMap.Latitude + "," + locationMap.Longitude; }
            if (mapSelectMarker) { txtGeoStaticMarker.Text = locationMap.Latitude + "," + locationMap.Longitude; }
            if (mapSelectNodeRoute) { txtNodeRoute.Text = locationMap.Latitude + "," + locationMap.Longitude; }
            refreshEventsMap();
        }


        private void myMap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Control control = (Control)sender;
            Point relativePoint = control.TransformToAncestor(Application.Current.MainWindow)
                          .Transform(new Point(0, 0));

            Point mousePosition = e.GetPosition(this);
            mousePosition.X -= relativePoint.X;
            mousePosition.Y -= relativePoint.Y;
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);
            createPushPin(pinLocation, "pushAlternative");
        }
        private void createPushPin(Location pinLocation, string tag)
        {
            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;
            pin.Background = Brushes.Green;
            pin.Tag = tag;
            pin.MouseEnter += pin_MouseEnter;
            pin.MouseLeave += pin_MouseLeave;
            // Adds the pushpin to the map.
            myMap.Children.Add(pin);
        }
        void pin_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopupAlternative.Visibility = Visibility.Collapsed;
        }
        void pin_MouseEnter(object sender, MouseEventArgs e)
        {
            Pushpin pushpin = sender as Pushpin;
            Point point = e.GetPosition(myMap);
            string resultId = pushpin.Tag as string;
            Location location = new Location(pushpin.Location.Latitude, pushpin.Location.Longitude);
            MapLayer.SetPosition(ContentPopupAlternative, location);
            MapLayer.SetPositionOffset(ContentPopupAlternative, new Point(25, -50));
            ContentPopupAlternativeDescription.Text = "Coordenadas: " + pushpin.Location.Latitude.ToString("0.000000") + " , " + pushpin.Location.Longitude.ToString("0.000000");
            ContentPopupAlternative.Visibility = Visibility.Visible;
 
        }
        private void ContentPopupAlternative_MouseEnter(object sender, MouseEventArgs e)
        {
            //Show the popup if mouse is hovering over it
            ContentPopupAlternative.Opacity = 1;
            ContentPopupAlternative.Visibility = Visibility.Visible;
            Canvas.SetZIndex(ContentPopupAlternative, 10);
        }
        private void ContentPopupAlternative_MouseLeave(object sender, MouseEventArgs e)
        {
            //Hide the popup if mouse leaves it
            ContentPopupAlternative.Opacity = 0.8;

            ContentPopupAlternative.Visibility = Visibility.Collapsed;
        }
        private void ContentPopupLayerAlternative_MouseEnter(object sender, MouseEventArgs e)
        {
            ContentPopupAlternative.Opacity = 1;
            ContentPopupAlternative.Visibility = Visibility.Visible;
            Canvas.SetZIndex(ContentPopupAlternative, 10);
        }
        private void ContentPopupLayerAlternative_MouseLeave(object sender, MouseEventArgs e)
        {
            ContentPopupAlternative.Visibility = Visibility.Collapsed;
            ContentPopupAlternative.Opacity = 0.8;
        }
        private void TextBlock_MouseDown_Alternative(object sender, MouseButtonEventArgs e)
        {
            string[] coordinates = ContentPopupAlternativeDescription.Text.Split(',');
            double latitude, longitude;
            double.TryParse(coordinates[0].Replace("Coordenadas: ",""), NumberStyles.Any, CultureInfo.InvariantCulture, out latitude);
            double.TryParse(coordinates[1], NumberStyles.Any, CultureInfo.InvariantCulture, out longitude);
            Static.Place place = (Static.Place)comboPlacesAlternative.SelectedItem;
            loadPlaces(place.Name.Replace(" ","_"), new Location(latitude, longitude));
        }
        private void comboPlacesAlternative_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ContentPopupAlternativeDescription.Text!="")
            { 
                Static.Place place = (Static.Place)comboPlacesAlternative.SelectedItem;
                loadPlaces(place.Name.Replace(" ", "_"), textToLocation(ContentPopupAlternativeDescription.Text));
            }
        }
        private void TextBlock_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            zoomME(textToLocation(ContentPopupAlternativeDescription.Text), "", 15);
        }
        private void BorderRouteAlternative_MouseDown(object sender, MouseButtonEventArgs e)
        {
            getRoute(pushME.Location, textToLocation(ContentPopupAlternativeDescription.Text));
        }

        private Location textToLocation(string text)
        {
            string[] coordinates = text.Split(',');
            double latitude, longitude;
            double.TryParse(coordinates[0].Replace("Coordenadas: ", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out latitude);
            double.TryParse(coordinates[1], NumberStyles.Any, CultureInfo.InvariantCulture, out longitude);
            return new Location(latitude, longitude);
        }
        #endregion

        
        private void RibbonApplicationMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            newAdvice();
        }

        private void newAdvice()
        {
            Random rdn = new Random();
            int i=rdn.Next(100);
            txtAdvice.Text = Static.StaticAdvices.getRandomAdvice();
        }
   
    

   
   

        

      

       

    }
  
}
