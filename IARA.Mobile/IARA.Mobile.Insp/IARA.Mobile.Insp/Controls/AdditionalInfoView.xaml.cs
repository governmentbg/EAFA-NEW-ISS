using System.Collections.Generic;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ContentProperty(nameof(AdditionalFields))]
    public partial class AdditionalInfoView : SectionView
    {
        public AdditionalInfoView()
        {
            AdditionalFields = new TLCallbackCollection<View>(
                OnClear,
                OnInsert,
                OnRemoveAt,
                OnSetItem
            );

            InitializeComponent();
        }

        public IList<View> AdditionalFields { get; }

        private void OnClear()
        {
            moreStack.Children.Clear();
        }

        private void OnInsert(int index, View item)
        {
            moreStack.Children.Add(item);
        }

        private void OnRemoveAt(int index)
        {
            moreStack.Children.RemoveAt(index);
        }

        private void OnSetItem(int index, View item)
        {
            moreStack.Children[index] = item;
        }
    }
}