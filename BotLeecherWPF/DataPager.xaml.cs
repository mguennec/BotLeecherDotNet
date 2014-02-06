using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BotLeecher.Model;
using WPFGenerics;

namespace BotLeecherWPF
{
  /// <summary>
  /// Interaction logic for DataPager.xaml
  /// </summary>
  public partial class DataPager : UserControl, INotifyPropertyChanged
  {
    public DataPager()
    {
      InitializeComponent();
      GeneratePages();
    }

    private void GeneratePages()
    {
      if (ItemsSource != null)
      {
          var enumerator = view.GetEnumerator();
          int count = 0;
          while (enumerator.MoveNext())
          {
              count++;
          }
          enumerator.Reset();
          PageCount = (int)Math.Ceiling(count / (double)ItemsPerPage);
          Pages = new RangeObservableCollection<RangeObservableCollection<Pack>>();
        for (int i = 0; i < PageCount; i++)
        {
            RangeObservableCollection<Pack> page = new RangeObservableCollection<Pack>();
          for (int j = 0; j < ItemsPerPage; j++)
          {
            enumerator.MoveNext();
            if (i * ItemsPerPage + j > count - 1) break;
            page.Add((Pack) enumerator.Current);
          }
          Pages.Add(page);
        }
        if (Pages.Count == 0)
        {
            Pages.Add(new RangeObservableCollection<Pack>());
        }
        CurrentPage = Pages[0];
        CurrentPageNumber = 1;
      }
    }

    private int PageCount;
    private int _CurrentPageNumber;
    private RangeObservableCollection<RangeObservableCollection<Pack>> Pages;
    private RangeObservableCollection<Pack> _ItemsSource;
    private RangeObservableCollection<Pack> _CurrentPage = new RangeObservableCollection<Pack>();
    private ICollectionView view;

    public RangeObservableCollection<Pack> CurrentPage
    {
      get { return _CurrentPage; }
      set
      {
          if (_CurrentPage == null)
          {
              _CurrentPage = new RangeObservableCollection<Pack>();
          }
          _CurrentPage.AddRange(value, true);
      }
    }
    public RangeObservableCollection<Pack> ItemsSource
    {
      get { return _ItemsSource; }
      set
      {
        _ItemsSource = value;
        view = CollectionViewSource.GetDefaultView(value);
        GeneratePages();
        view.CollectionChanged += UpdateSource;
      }
    }

    public int CurrentPageNumber
    {
      get { return _CurrentPageNumber; }
      set
      {
        _CurrentPageNumber = value;
        if (PropertyChanged != null)
          PropertyChanged(this, new PropertyChangedEventArgs("CurrentPageNumber"));
      }
    }

    public int ItemsPerPage
    {
      get { return (int)GetValue(ItemsPerPageProperty); }
      set { SetValue(ItemsPerPageProperty, value); }
    }
    public static readonly DependencyProperty ItemsPerPageProperty =
        DependencyProperty.Register("ItemsPerPage", typeof(int), typeof(DataPager), new UIPropertyMetadata(10));

    #region INotifyPropertyChanged Members
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion


    private void FirstPage_Click(object sender, RoutedEventArgs e)
    {
      if (Pages != null)
      {
        CurrentPage = Pages[0];
        CurrentPageNumber = 1;
      }
    }

    private void PreviousPage_Click(object sender, RoutedEventArgs e)
    {
      if (Pages != null)
      {
        CurrentPageNumber = (CurrentPageNumber - 1) < 1 ? 1 : CurrentPageNumber - 1;
        CurrentPage = Pages[CurrentPageNumber - 1];
      }
    }

    private void NextPage_Click(object sender, RoutedEventArgs e)
    {
      if (Pages != null)
      {
        CurrentPageNumber = (CurrentPageNumber + 1) > PageCount ? PageCount : CurrentPageNumber + 1;
        if (CurrentPageNumber > 0)
            CurrentPage = Pages[CurrentPageNumber - 1];
      }
    }

    private void LastPage_Click(object sender, RoutedEventArgs e)
    {
      if (Pages != null)
      {
        CurrentPage = Pages[PageCount - 1];
        CurrentPageNumber = PageCount;
      }
    }

    private void GoPage_Click(object sender, RoutedEventArgs e)
    {
      if (Pages != null)
      {
        CurrentPage = Pages[CurrentPageNumber - 1];
      }
    }

    private void UpdateSource(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        GeneratePages();
    }
  }
}
