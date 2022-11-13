using AngouriMath;
using NumericalMethods.Controls;
using NumericalMethods.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static AngouriMath.Entity;

namespace NumericalMethods.Pages
{
    public partial class WalkthroughPage : Page
    {
        public WalkthroughPage(IInterpolator interpolator, Entity function = null)
        {
            InitializeComponent();

            m_interpolator = interpolator;
            m_function = function;

            stepper.ItemsSource = m_interpolator.Solution;

            m_content_controls = new List<ActionsView>();
            foreach (ActionCollection collection in m_interpolator.Solution)
            {
                m_content_controls.Add(new ActionsView(collection));
            }
        }

        public static string[] RequiredLocalizedResourceDictionaries = new string[]
        {
            "ActionDictionary.xaml",
        };

        private IInterpolator m_interpolator;
        private Entity m_function;
        private List<ActionsView> m_content_controls;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += Page_KeyDown;

            RenderCurrentStep();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown -= Page_KeyDown;
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Back)
            {
                navigate_prev_Click(sender, null);
            }
            else if (e.Key == Key.Right || e.Key == Key.Space || e.Key == Key.Enter)
            {
                navigate_next_Click(sender, null);
            }
        }

        private void stepper_StepChanged(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {
            RenderCurrentStep();
        }

        private void RenderCurrentStep()
        {
            step.Content = m_content_controls[stepper.StepIndex];
        }

        private void navigate_first_Click(object sender, RoutedEventArgs e)
        {
            stepper.StepIndex = 0;
        }

        private void navigate_prev_Click(object sender, RoutedEventArgs e)
        {
            if (stepper.StepIndex != 0)
            {
                stepper.Prev();
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        private void navigate_next_Click(object sender, RoutedEventArgs e)
        {
            if (stepper.StepIndex != stepper.Items.Count - 1)
            {
                stepper.Next();
            }
            else
            {
                NavigationService.Navigate(new PlotPage(m_interpolator, m_function));
            }
        }

        private void navigate_last_Click(object sender, RoutedEventArgs e)
        {
            stepper.StepIndex = stepper.Items.Count - 1;
        }
    }
}
