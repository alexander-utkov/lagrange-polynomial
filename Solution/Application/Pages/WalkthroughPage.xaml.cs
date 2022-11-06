using NumericalMethods.Controls;
using NumericalMethods.Core;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NumericalMethods.Pages
{
    public partial class WalkthroughPage : Page
    {
        public WalkthroughPage(IInterpolator interpolator)
        {
            InitializeComponent();

            m_interpolator = interpolator;
            stepper.ItemsSource = m_interpolator.Solution;

            m_content_controls = new List<ActionsView>();
            foreach (ActionCollection collection in m_interpolator.Solution)
            {
                m_content_controls.Add(new ActionsView(collection));
            }
        }

        private IInterpolator m_interpolator;
        private List<ActionsView> m_content_controls;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RenderCurrentStep();
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
                NavigationService.Navigate(new PlotPage(m_interpolator));
            }
        }

        private void navigate_last_Click(object sender, RoutedEventArgs e)
        {
            stepper.StepIndex = stepper.Items.Count - 1;
        }
    }
}
