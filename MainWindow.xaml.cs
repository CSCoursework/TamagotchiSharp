using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json;
using RestSharp;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ProgressBar = System.Windows.Controls.ProgressBar;

namespace TamagotchiSharp
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            
            InitializeComponent();
            ImageCat.Source = GetCatPic();


            var unused = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Background,
                t_Tick, Dispatcher.CurrentDispatcher) {IsEnabled = true};
            ProgressBarTiredness.Value += CalculateHoursSlept();

            var r = new Random();
            var ill = r.Next(100) > 85;
            if (ill) ProgressBarHealth.Value -= r.Next(15, 50);
        }

        private static string CauseOfDeath(IFrameworkInputElement sender)
        {
            return sender.Name switch
            {   
                "ProgressBarHunger" =>
                    "because you forgot to feed it, it ate some mouldy cheese and died.",
                "ProgressBarThirst" =>
                    "because forgot to give the cat anything to drink, it has shrivelled to the size of a garden pea.",
                "ProgressBarTiredness" => 
                    "because it got too tired, now it will never wake up again.",
                "ProgressBarHappiness" => 
                    "because it was depressed and jumped off the shed.",
                "ProgressBarHealth" => 
                    "of a brand new disease and had to taken to a lab to be studied",
                _ => "because you tried to play God."
            };
        }


        private static BitmapImage GetCatPic()
        {
            var client = new RestClient("https://api.thecatapi.com/v1/images/search?breed_ids=ragd");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-api-key", "d5defbd6-8d52-44b9-9c9d-cf5093e87514");
            var response = client.Execute(request);
            var cat = JsonConvert.DeserializeObject<List<Cat>>(response.Content);
            return new BitmapImage(cat[0].Url);
        }


        private void t_Tick(object sender, EventArgs e)
        {
            ProgressBarHunger.Value -= 0.002;
            ProgressBarThirst.Value -= 0.005;
            ProgressBarTiredness.Value -= 0.001;
            ProgressBarHappiness.Value -= 0.005;

            var progressBars = new[]
            {
                ProgressBarHunger, ProgressBarThirst, ProgressBarTiredness,
                ProgressBarHappiness, ProgressBarHealth
            };

            foreach (var progressBar in progressBars)
                if (progressBar.Value <= 0)
                {
                    TextBlockCauseOfDeath.Text =  "Your cat died " + CauseOfDeath(progressBar);
                    GridProgressBars.Visibility = Visibility.Collapsed;
                    GridDeath.Visibility = Visibility.Visible;
                }
                else if (Math.Abs(progressBar.Value - 10) < 0.0005 && Form.ActiveForm == null)
                {
                    
                }
                
        }

        private void ButtonFeed_OnClick(object sender, RoutedEventArgs e)
        {
            GridFoodButtons.Visibility = Visibility.Visible;
            //TODO animations
            GridDrinkButtons.Visibility = Visibility.Collapsed;
        }

        private void ButtonDrink_OnClick(object sender, RoutedEventArgs e)
        {
            GridFoodButtons.Visibility = Visibility.Collapsed;
            //TODO animations
            GridDrinkButtons.Visibility = Visibility.Visible;
        }

        private void ButtonPlay_OnClick(object sender, RoutedEventArgs e)
        {
            GridFoodButtons.Visibility = Visibility.Collapsed; // ButtonPlay_OnClick
            GridDrinkButtons.Visibility = Visibility.Collapsed; // ButtonPlay_OnClick
            
            //TODO Mini games (Indev)

            var r = new Random();
            var game = r.Next(0,2);
            switch (game)
            {
                case 1:
                    //TODO Typing Game
                    var words = "";
                    try
                    {
                        var currentDir = Environment.CurrentDirectory;
                        var file = File.ReadAllLines(currentDir + @"\Words.txt");
                        if (file == Array.Empty<string>()) throw new FileNotFoundException();
                        for (var i = 0 ; i < 10; i++)
                        {
                            words += file[r.Next(file.Length - 1)] + " ";
                        }
                    }
                    catch
                    {
                        words = "Error" + e;
                    }
                    
                    

                    BlockWords.Text = words;


                    GridProgressBars.Visibility = Visibility.Collapsed;
                    GridGameTyping.Visibility = Visibility.Visible;
                    
                    
                    break;
                case 2:
                    //TODO Clicking Game
                    
                    
                    
                    break;
            }
        }

        private void ButtonHeal_OnClick(object sender, RoutedEventArgs e)
        {
            ProgressBarHealth.Value += 50;
            
            GridFoodButtons.Visibility = Visibility.Collapsed; // ButtonPlay_OnClick
            GridDrinkButtons.Visibility = Visibility.Collapsed; // ButtonPlay_OnClick
        }
        
        private void ButtonFoodHealthy_OnClick(object sender, RoutedEventArgs e)
        {
            ProgressBarHunger.Value += 50;
        }

        private void ButtonFoodUnhealthy_OnClick(object sender, RoutedEventArgs e)
        {
            ProgressBarHunger.Value += 30;
            ProgressBarHappiness.Value += 10;
            ProgressBarHealth.Value -= 10;
        }

        private void ButtonDrinkHealthy_OnClick(object sender, RoutedEventArgs e)
        {
            ProgressBarThirst.Value += 50;
        }

        private void ButtonDrinkUnhealthy_OnClick(object sender, RoutedEventArgs e)
        {
            ProgressBarThirst.Value += 30;
            ProgressBarTiredness.Value += 10;
            ProgressBarHealth.Value -= 10;
            
        }
        
        private void BoxGameWord_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == (Key) Keys.Space)
            {
                
            }
        }

        private class FileNotFoundException : Exception
        {
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            //throw new NotImplementedException();

            var currentDir = Environment.CurrentDirectory;
            var fs = File.Create(currentDir + @"\Date.txt", 1024);
            try
            {
                var file = File.ReadAllText(currentDir + @"\Date.txt");
                if (file == string.Empty) throw new FileNotFoundException();
            }
            catch (System.IO.FileNotFoundException)
            {
                using (fs)
                {
                    File.Create(currentDir);
                }
            }
            finally
            {
                using (fs)
                {
                    var info = new UTF8Encoding(true).GetBytes(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    fs.Write(info, 0, info.Length);
                }
            }
        }
        

        private static int CalculateHoursSlept()
        {
            //throw new NotImplementedException();

            var currentDir = Environment.CurrentDirectory;

            try
            {
                var quitTime = DateTime.Parse(File.ReadAllText(currentDir + @"\Date.txt"));
                var startTime = DateTime.Now;

                var interval = quitTime - startTime;

                var hours = int.Parse(interval.Hours.ToString());

                return hours;
            }
            catch (System.IO.FileNotFoundException)
            {
                return 0;
            }
        }
    }
}