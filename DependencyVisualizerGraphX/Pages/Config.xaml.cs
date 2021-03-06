﻿using SolutionDependencyScanner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;

namespace DependencyVisualizerGraphX.Pages
{
  /// <summary>
  /// Interaction logic for Config.xaml
  /// </summary>
  public partial class Config : System.Windows.Controls.UserControl
  {
    private ObservableCollection<SolutionSerializableToNameAndPath> slns = new ObservableCollection<SolutionSerializableToNameAndPath>();
    private GraphFactory factory;

    public Config()
    {
      InitializeComponent();
      
      slnListView.ItemsSource = slns;
      CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(slnListView.ItemsSource);
      view.Filter = UserFilter;
    }


    private void BrowseFolder(object sender, RoutedEventArgs e)
    {
      FolderBrowserDialog dlg = new FolderBrowserDialog();
      dlg.ShowNewFolderButton = false;

      dlg.SelectedPath = RootPath.Text;

      DialogResult result = dlg.ShowDialog();
      if (result == System.Windows.Forms.DialogResult.OK)
      {
        RootPath.Text = dlg.SelectedPath;
      }

    }

    private async void scanBtn_Click(object sender, RoutedEventArgs e)
    {
      scanBtn.IsEnabled = false;
      string path = RootPath.Text;
      SolutionDependencyScanner.Scanner s = new SolutionDependencyScanner.Scanner(path);
      SolutionDependencyScanProduct sr = null;
      await Task.Factory.StartNew(async () =>
      {
          sr = s.Scan();
      });
      slns.Clear();
      List<Solution> l = new List<Solution>();
      l.AddRange(sr.Solutions);

      l.Sort((s1, s2) => s1.Name.CompareTo(s2.Name));

      foreach (Solution sln in l)
      {
        slns.Add(new SolutionSerializableToNameAndPath(sln));
      }

      factory = new GraphFactory(sr);

      scanBtn.IsEnabled = true;
     
      txtSearch.IsEnabled = true;
    }

    private bool UserFilter(object item)
    {
      if (String.IsNullOrEmpty(txtSearch.Text))
        return true;
      else
        return ((item as SolutionSerializableToNameAndPath).Solution.Name.IndexOf(txtSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private DependencyGraph CreateGraphForSolution(Solution sln)
    {
      return factory.CreateGraph(sln, GraphCreationOptions.IncludeSolutionsReferencingSelectedProjects);
    }

    private void plotBtn_Click(object sender, RoutedEventArgs e)
    {

      SolutionSerializableToNameAndPath slnser = slnListView.SelectedItem as SolutionSerializableToNameAndPath;
      Solution sln = slnser.Solution;

      BBCodeBlock bcb = new BBCodeBlock();

      GraphKeeper.Graph = CreateGraphForSolution(sln);

      try
      {
        bcb.LinkNavigator.Navigate(new Uri("/Pages/GraphPresenter.xaml", UriKind.Relative), this);
      }
      catch (Exception error)
      {
        ModernDialog.ShowMessage(error.Message, FirstFloor.ModernUI.Resources.NavigationFailed, MessageBoxButton.OK);
      }

     
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      CollectionViewSource.GetDefaultView(slnListView.ItemsSource).Refresh();
    }

    private void slnListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (slnListView.SelectedItem != null)
      {
        plotBtn.IsEnabled = true;
      }
      else
      {
        plotBtn.IsEnabled = false;
      }
    }

  }
}
